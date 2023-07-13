using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using DrawAndGuess.Models;
using DrawAndGuess.Commands;
using DrawAndGuess.CommandProcessor;


namespace SignalRChat.Hubs
{
    public class GameHub : Hub
    {
        private readonly ICommandProcessor _commandProcessor;

        public GameHub(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        public override async Task OnConnectedAsync()
        {
            _commandProcessor.AddCommand(new ConnnectCommand(Context.ConnectionId));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _commandProcessor.AddCommand(new DisconnectCommand(Context.ConnectionId));
            await base.OnDisconnectedAsync(exception);
        }

        public void JoinRoom(JoinRoomForm form)
        {
            _commandProcessor.AddCommand(
                new JoinRoomCommand(Context.ConnectionId) { Form = form }
                );
        }

        public void LeaveRoom()
        {
            _commandProcessor.AddCommand(new LeaveRoomCommand(Context.ConnectionId));
        }

        public void SendMessage(string message)
        {
            _commandProcessor.AddCommand(
                new SendMessageCommand(Context.ConnectionId) { Message = message }
                );
        }

        public void SendDrawingStarted(MousePos mousePos)
        {
            _commandProcessor.AddCommand(
                new StartDrawingCommand(Context.ConnectionId) { mousePos = mousePos }
                );
        }

        public void SendMousePos(MousePos mousePos)
        {
            _commandProcessor.AddCommand(
                new DrawCommand(Context.ConnectionId) { MousePos = mousePos }
                );
        }

        public void FillColor(MousePos mousePos)
        {
            _commandProcessor.AddCommand(
                new FillColorCommand(Context.ConnectionId) { mousePos = mousePos }
                );
        }

        public void ChangeColor(string color)
        {
            _commandProcessor.AddCommand(
                new ChangeColorCommand(Context.ConnectionId) { NewColor = color }
                );
        }
        public void SelectTool(int toolIdx)
        {
            _commandProcessor.AddCommand(
                new SelectToolCommand(Context.ConnectionId) { ToolIndex = toolIdx }
                );
        }
        public void ChangeBrushSize(int size)
        {
            _commandProcessor.AddCommand(
                new ChangeBrushSizeCommand(Context.ConnectionId) { Size = size }
                );
        }

        public void ClearCanvas()
        {
            _commandProcessor.AddCommand(
                new ClearCanvasCommand(Context.ConnectionId)
                );
        }

        public void PickAWord(string word)
        {
            _commandProcessor.AddCommand(
                new PickWordCommand(Context.ConnectionId) { Word = word }
                );
        }

        public void GetUsersInRoom(string roomName)
        {
            _commandProcessor.AddCommand(
                new GetUserInRoomCommand(Context.ConnectionId) { RoomName = roomName }
                );
        }
    }
}