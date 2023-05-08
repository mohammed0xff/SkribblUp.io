using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SignalRChat.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using DrawAndGuess.Constants;
using DrawAndGuess.Enums;
using DrawAndGuess.Utils;
using DrawAndGuess.Entities;
using DrawAndGuess.Models;
using DrawAndGuess.Invokers;

namespace DrawAndGuess
{
    /// <summary>
    /// this class holds the game logic as well as 
    /// 1 - interface for user requests hub -> game -> { with UserBroadcaster }
    /// 2 - channel for  game logic -> user(s) { with GameSender } 
    /// 3 - timer for handling waiting and intervals
    /// 
    /// private methods -> game logic
    /// public  methods -> user commands interface
    /// 
    /// preceding log message with 
    ///   [---] for game logic
    ///   [-->] for user incomming requests on hub methods 
    ///   [<--] for game notifications on client methods 
    /// </summary>
    public class Game
    {
        public readonly UserBroadcaster Broadcaster;
        
        private readonly IHubContext<GameHub> HubContext;
        private readonly ILogger Logger;
        private readonly GameTimer Timer;
        private readonly GameSender Notifier;

        private Dictionary<int, Room> Rooms;
        private Dictionary<string, User> UsersConnections;

        public Game(
            IHubContext<GameHub> hubContext,
            ILogger<Game> logger
            )
        {
            Logger = logger;
            HubContext = hubContext;
            Timer = new GameTimer();
            Notifier = new GameSender(hubContext);
            Broadcaster = new UserBroadcaster(hubContext);
            
            UsersConnections = new Dictionary<string, User>();
            
            Rooms = new Dictionary<int, Room>();
            
            // adding rooms to dict
            var room1 = new Room("Fun Room", RoomRules.MaxNumberOfUsers);
            Rooms.Add(room1.Id, room1);
                        
            var room2 = new Room("Play Room", RoomRules.MaxNumberOfUsers);
            Rooms.Add(room2.Id, room2);
                        
            var room3 = new Room("Art Room", RoomRules.MaxNumberOfUsers);
            Rooms.Add(room3.Id, room3);

        }
        private void SartGameOnRoom(Room room) =>
            Task.Run(() => GameLoop(room));

        private async void GameLoop(Room room)
        {
            room.GameState = GameState.RoundStarted;
            Logger.LogInformation($"[-*-] Room : {room.Name} - Game Started");

            while (room.NumberOfUsers >= 2 && room.IsActive)
            {
                try
                {
                    bool proceed = HandleArtistSelection(room);
                    if (!proceed) { continue; }

                    proceed = await HandleWordSelection(room);
                    if (!proceed) { continue; }

                    proceed = await HandleDrawing(room);
                    if (!proceed) { continue; }

                    await HandleRevealingWord(room);

                }catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error in {nameof(GameLoop)}: {ex.Message}");
                }
            }

            Logger.LogInformation($"[-*-] Room : {room.Name} - Game Ended");
            room.GameState = GameState.WaitingForUsersToCome;
        }

        private bool HandleArtistSelection(Room room)
        {
            room.GameState = GameState.FetchingArtist;

            Logger.LogInformation($"[---] Room : {room.Name} - Setting next artist");

            if (!TrySetNextArtistInRoom(room))
            {
                Logger.LogInformation($"[---] Room : {room.Name} - Empty Queue, Couldn't set artist rolling up.");
                room.ResetArtists(GetUsersInRoom(room.Id));

                return false;
            }

            return true;
        }

        private async Task<bool> HandleWordSelection(Room room)
        {
            room.GameState = GameState.ChoosingAWord;

            var artist = GetUserByConnectionId(room.ArtistUserId);
            if (artist == null || artist.RoomId == null)
            {
                Logger.LogInformation($"[---] CID {room.ArtistUserId} - Artist left with  from room {room.Name}");
                return false;
            }

            Logger.LogInformation($"[---] Room : {room.Name} - Artist {artist.UserName} Is choosing a word");
            
            // get word from artist 
            await Notifier.GetWordFromArtist(artist);

            // notify the artist is choosing a wrod rn.
            await Notifier.NotifyArtistChoosingWord(artist);

            // wait for artist to choose a word
            await Timer.WaitTill(
                condition: () => room.SecretWord != null,
                maxTime: WaitingTime.ChoosingAWord
            );

            // if user didnt choose a word and time's up.
            if (room.SecretWord == null)
            {
                Logger.LogInformation($"[---] Room : {room.Name} - Artist didnt choose word.");
                return false;
            }

            Logger.LogInformation($"[---] Room : {room.Name} - Artist {artist.UserName} has chosen the word {room.SecretWord}");

            return true;
        }

        private async Task<bool> HandleDrawing(Room room)
        {
            // set gamestate to drawing 
            room.GameState = GameState.ArtistIsDrawing;

            var artist = GetUserByConnectionId(room.ArtistUserId);
            
            Logger.LogInformation($"[---] Room : {room.Name} - Artist {artist.UserName} Is Drawing");
            
            await Notifier.NotifyArtistIsDrawing(artist);
            await Notifier.NotifyAristToPickThePen(artist);
            
            CancellationTokenSource cts = new();
            CancellationToken token = cts.Token;

            Timer.SetInterval(
                action: async () =>
                {
                    room.FurtherHint();
                    await Notifier.SendHint(room);
                },
                msInterval: WaitingTime.SendHint,
                maxTimes: room.HintWord!.Length - 2, // leave two characters off
                cancelationtoken: token
            );

            // start the timer on room
            await Notifier.StartTimer(room, WaitingTime.Drawing / 1000 / 60);

            Logger.LogInformation($"[---] Room : {room.Name} - waiting for drawing..");
            
            bool roundIsDone() { return IsDisconnectedUser(artist.ConnectionId) || room.AllUsersGuessedRight; }

            // wait till round is done -> user disconnects or time's up
            await Timer.WaitTill(
                condition: () => roundIsDone(),
                maxTime: WaitingTime.Drawing
            );

            Logger.LogInformation($"[---] Room : {room.Name} - round is done");

            // stop sending hints interval
            cts.Cancel();

            // stop timer on room
            await Notifier.StopTimer(room);

            return true;
        }

        private async Task HandleRevealingWord(Room room)
        {
            // setting game state
            room.GameState = GameState.WordRevealed;

            // send secret word to users in room
            await Notifier.RevealWord(room);

            // wait after revealing word
            Logger.LogInformation($"[---] Room : {room.Name} - Waiting after revealing word..");
            await Task.Delay(WaitingTime.WordRevealed);

            // send users with points updated 
            await Notifier.UpdateUsersPoints(room, GetUsersInRoom(room.Id));
            
            await Notifier.NotifyNewTurn(room);
            
            room.NewTurnSetup();
        }

        private bool TrySetNextArtistInRoom(Room room)
        {
            // while still finding a new artist in the Q
            while (room.SetNextArtist())
            {
                // check if user still online and in that room
                User? artist = GetUserByConnectionId(room.ArtistUserId);
                if (artist != null && artist.RoomId == room.Id)
                {
                    return true;
                }
            }
            return false;
        }
        private Room? GetRoomByName(string roomName)
        {
            return Rooms.Where(r => r.Value.Name == roomName)
                .FirstOrDefault().Value;
        }

        private bool IsDisconnectedUser(string connectionId)
        {
            return GetUserByConnectionId(connectionId) == null;
        }

        public async Task<bool> CheckRightGuess(User user, string message)
        {
            var room = GetRoomById(user.RoomId);
            
            if (room == null)
            {
                throw new ArgumentException("Room does not exist");
            }
            
            if (room.SecretWord == null) return false;

            if (message.Contains(room.SecretWord))
            {
                room.AwardPoints(user);
                await Notifier.NotifyRightGuess(user);
                
                return true;
            }

            return false;
        }

        public IEnumerable<User> GetUsersInRoom(int roomId)
        {
            var users = UsersConnections
                .Where(x => x.Value.RoomId == roomId)
                .Select(u => u.Value);

            return users;
        }

        public User? GetUserByConnectionId(string connectionId)
        {
            UsersConnections.TryGetValue(connectionId, out User user);

            return user;
        }

        public Room? GetRoomById(int roomId)
        {
            Rooms.TryGetValue(roomId, out Room room);

            return room;
        }

        public void UserConnected(string connectionId)
        {
            Logger.LogInformation($"[-->] CID : {connectionId} - User Connected");
            var user = new User
            {
                ConnectionId = connectionId,
            };
            UsersConnections.Add(connectionId, user);
        }

        public async Task UserDisconnected(string connectionId)
        {
            Logger.LogInformation($"[-->] CID : {connectionId} - User Disconnected ");

            // get the user who just left.
            var disconnectedUser = UsersConnections.GetValueOrDefault(connectionId);

            // if user havent joined a room just return error.
            if (disconnectedUser == null)
            {
                throw new ArgumentException("User does not exist");
            }

            // remove form users list
            UsersConnections.Remove(connectionId);
            
            // return if user didn't enter a room yet
            if (disconnectedUser.UserName == null) return;
            
            var room = GetRoomById(disconnectedUser.RoomId);
            
            // If the room doesn't exist or is full, return an error
            if (room == null)
            {
                throw new ArgumentException("Room does not exist");
            }

            // remove user from Group.
            await HubContext.Groups.RemoveFromGroupAsync(connectionId, room.Name);

            // send notification user {username} disconnected.
            await Notifier.UserDiconnected(disconnectedUser);
            
            // remove from room
            room.RemoveUser(disconnectedUser);
        }

        public async Task JoinRoom(JoinRoomForm form, string connectionId)
        {
            // Log the join room request with the user's username and connection ID
            Logger.LogInformation($"[-->] CID : {connectionId} - User : {form.Username} request to join Room {form.Room}");

            // Validate the join room form
            if (string.IsNullOrWhiteSpace(form.Username) || string.IsNullOrWhiteSpace(form.Room))
            {
                throw new ArgumentException("Invalid join room form");
            }

            // Find the room with the specified name
            var room = GetRoomByName(form.Room);

            // If the room doesn't exist or is full, return an error
            if (room == null)
            {
                throw new ArgumentException("Room does not exist");
            }
            else if (room.IsFull)
            {
                throw new ArgumentException("Room is full");
            }

            // Find the user with the specified connection ID
            var user = UsersConnections.GetValueOrDefault(connectionId);

            // If the user doesn't exist, return
            if (user == null)
            {
                throw new ArgumentException("User does not exist");
            }

            // Set the user's username and add them to the room
            lock (room) // with the pretty cool `lock` statement to ensure that
                        // the room.JoinUser method and other critical sections of code are thread-safe.
            {
                user.UserName = form.Username;
                room.JoinUser(user);
            }

            // Notify all users in the room that the user has joined
            await Notifier.UserJoinedRoom(user);

            // Add the user to the room's SignalR group
            try
            {
                await HubContext.Groups.AddToGroupAsync(connectionId, room.Name);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error adding user {user.UserName} to room {room.Name} group: {ex.Message}");
            }

            // Log that the user has joined the room
            Logger.LogInformation($"[-->] CID : {connectionId} - User : {user.UserName} Joined Room {room.Name}");

            // Send the user a list of all connected users in the room
            await Notifier.SendConnectedUsers(user, GetUsersInRoom(user.RoomId));

            // If the room has at least two users and is not active already, start the game loop
            if (room.NumberOfUsers >= 2 && !room.IsActive)
            {
                SartGameOnRoom(room);
            }
        }

        public async Task LeaveRoom(string connectionId)
        {
            var user = GetUserByConnectionId(connectionId);
            if (user == null)
            {
                throw new ArgumentException("User disconnected.");
            }

            var room = GetRoomById(user.RoomId);
            if (room == null)
            {
                throw new ArgumentException("Room does not exist");
            }

            //remove from group
            await HubContext.Groups.RemoveFromGroupAsync(connectionId, room.Name);

            // notify user left 
            await Notifier.UserLeftRoom(user);

            // remove from room
            room.RemoveUser(user);

            Logger.LogInformation($"[-->] CID : {connectionId} - User : {user.UserName} Left Room {room.Name}");
        }
    }
}
