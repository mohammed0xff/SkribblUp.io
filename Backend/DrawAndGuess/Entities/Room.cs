using System;
using System.Collections.Generic;
using DrawAndGuess.Enums;

namespace DrawAndGuess.Entities
{
    public class Room
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public int MaxNumberOfUsers { get; init; }
        public string? ArtistUserId { get; private set; } = null!;
        public string? SecretWord { get; private set; } = null!;
        public string? HintWord { get; private set; } = null!;
        public int NumberOfUsers { get; private set; } = 0;
        public int NumberOfRightGuessers { get; private set; } = 0;
        public GameState GameState { get; set; } = GameState.WaitingForUsersToCome;
        public bool IsActive => GameState != GameState.WaitingForUsersToCome;
        public bool IsFull => NumberOfUsers >= MaxNumberOfUsers;
        public bool AllUsersGuessedRight => NumberOfRightGuessers >= NumberOfUsers - 1;
        
        public Queue<string> ArtistsQueue { get; } = new();
        private readonly Random _random = new Random();
        
        private static int _nextRoomId = 1;
        public Room(string name, int maxNumberOfUsers)
        {
            Name = name; 
            MaxNumberOfUsers = maxNumberOfUsers;
            Id = _nextRoomId++;
        }
        public void JoinUser(User user)
        {
            user.RoomName = Name;
            user.RoomId = Id;
            NumberOfUsers++;
            ArtistsQueue.Enqueue(user.ConnectionId);
        }

        public void RemoveUser(User user)
        {
            if (user.RoomName == Name) { 
                user.UserName = null; 
                user.RoomId = 0; 
            }
            if (NumberOfUsers > 0) NumberOfUsers--;
        }

        public void ResetArtists(IEnumerable<User> users)
        {
            ArtistsQueue.Clear();
            foreach (var user in users)
            {
                ArtistsQueue.Enqueue(user.ConnectionId);
            }
        }

        public bool SetNextArtist()
        {
            ArtistUserId = null;
            if(ArtistsQueue.Count > 0)
                ArtistUserId = ArtistsQueue.Dequeue();
            
            return ArtistUserId != null;
        }

        public void SetSecretWord(string connectionId, string word)
        {
            if (connectionId != ArtistUserId)
                throw new Exception("Not the current artist of the room.");
            
            if(SecretWord != null)
                throw new Exception("Secret word has been set once already.");
            
            SecretWord = word;
            HintWord = new string('_', SecretWord.Length);
        }

        public void NewTurnSetup()
        {
            SecretWord = null;
            HintWord = null;
            ArtistUserId = null;
            NumberOfRightGuessers = 0;
        }

        public void AwardPoints(User user)
        {
            user.Points += NumberOfRightGuessers++
                switch
                {
                    0 => 500,
                    1 => 250,
                    2 => 150,
                    3 => 100,
                    _ => 50,
                };
        }

        public void FurtherHint()
        {
            if (SecretWord == null || HintWord == null) return;
            
            // Check if there are any underscores left in HintWord
            if (!HintWord.Contains("_")) return;
            
            int index;
            do
            {
                index = _random.Next(SecretWord.Length);
            } while (HintWord[index] != '_');
            
            HintWord = HintWord.Remove(index, 1)
                .Insert(index, SecretWord[index].ToString()
                );
        }
    }
}
