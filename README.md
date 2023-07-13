# Draw & Guess

A realtime browser-based drawing and guessing game, [skribbl.io](https://skribbl.io) clone.

## Technologies Used

This game was built using the following technologies:
- React JS: A popular JavaScript library for building user interfaces.
- SignalR : A library for building real-time web applications.
- HTML and CSS: The languages used for creating the user interface and styling the game.

## Demo Video
[![Demo Video](https://i.vimeocdn.com/video/1666954036-73f51a3b4260462290d3b61a9ceb33118ad3c7de5157e889859f0fdf87b206bb-d?mw=1200&mh=645&q=70)](https://vimeo.com/manage/videos/824841261)

## How To Play
1. Navigate to the game in your web browser by visiting `http://localhost:5000`.
2. Enter your username and choose a room then click "Join Room".
3. Once there are enough players in the lobby, the game will start.
4. If its your turn, you will be given three random secret words to choose from.
5. Use the drawing tools to create an image that represents the secret word you've just choosen.
6. When you're done drawing, the other players will have a limited amount of time to guess the word.
7. If someone guesses the word correctly will earn points accroding to the order of guessing.
8. The first player who reaches a 3000 points will take the 1st place. the 2nd and 3rd will also be announced.

## How to Run

### With Docker

2. Clone the repository: `git clone https://github.com/mohammed0xff/draw-and-guess`
3. Navigate to the project directory: `cd draw-and-guess`
4. Run the Docker Compose command to build and start the containers: `docker-compose up`
5. Open your web browser and go to `http://localhost:5000`

### Without Docker

#### Backend end 
1. Clone the repository: git clone `https://github.com/mohammed0xff/draw-and-guess`
2. Navigate to the project directory: `cd Backend/DrawAndGuess`
3. Build the project: `dotnet build`
4. Run the project with launch settings: `dotnet run`
- or just open `.sln` file and press `f5` to build and run the project -

#### front end 

5. Navigate to the project directory: `cd Frontend`
6. Install dependencies with: `npm install`
7. Start the development server with: `npm run dev` or just `vite`
8. Open your web browser and go to `http://localhost:5000`.

## Workflow

Example : user started by sending a message ..

1. In React app --> Chat Component  
```js
// user sends a message
sendMessage = async (message) => {
  try {
    await this.props.connection.invoke(InvokeMethods.SendMessage, message);
  } catch (e) {
    console.log(e);
  }
}

```

2. Chathub recieves a call on  `SendMessage` method providing a string parameter `message` 
```csharp
public class GameHub : Hub
{
    private readonly ICommandProcessor _commandProcessor;

    public void SendMessage(string message)
    {
        // creates a SendMessageCommand and pass it to command processor.
        _commandProcessor.AddCommand(
            new SendMessageCommand(Context.ConnectionId) { Message = message }
            );
    }

    // other hub methods here ..
}
```
By using the [command pattern](https://refactoring.guru/design-patterns/command), the application is more maintainable and extensible, as each command encapsulates a specific action or behavior that can be easily added or modified. The command pattern also helps to decouple the logic of executing actions from the rest of our application.

3. In `CommandProcessor` class
```csharp
public class CommandProcessor : ICommandProcessor
{
  private readonly Game _game;
  private readonly ILogger<CommandProcessor> _logger;
  private readonly TaskQueue _taskQueue;

  public void AddCommand(ICommand command)
  {
      _taskQueue.EnqueueTask(
              () => ProcessCommand(command)
          );
  }

  private async Task ProcessCommand(ICommand command)
  {
      try
      {
         await command.ExecuteAsync(_game);
      }
      catch (Exception ex)
      {
          _logger.LogError($"Error in executing command {command.GetType().Name} : {ex.Message}");
      }
  }
}
```

4. Command executes asynchronous in the task queue
```csharp
public class SendMessageCommand : ICommand
{
    public string ConnectionId { get; init; }
    public string Message { get; init; }
    
    public async Task ExecuteAsync(Game game)
    {
        var user = game.GetUserByConnectionId(ConnectionId);
        
        // some exception handling here..
        //
        
        // send message only if it doesnt contain a right guess.
        if (! await game.CheckRightGuess(user, Message))
        {
            await game.Broadcaster.SendMessage(user, Message);
        }
    }
}
```

5. `Broadcaster` Sends the message to room if user didn't make a right guess.
```csharp
public async Task SendMessage(User user, string message)
{
    try
    {
        await SendToGroup(user.Room, ClientMethods.ReceiveMessage,
            new Message
            {
                Username = user.UserName,
                Content = message
            });
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error sending message for user {user.UserName} in room {user.Room}: {ex.Message}");
    }
}
```

6. As client code listens on `ReceiveMessage`
```js
connection.on(ListeningMethods.ReceiveMessage, ({username, content}) => {
  this.setState(prevState => ({
    messages : [...prevState.messages, { username, content}] 
  }));
});
```

7. All users in the room receive the message successfully.

## Contributing
If you'd like to contribute, feel free to submit a pull request!

#### upcomming features
* Backend : 
  - [ ] sending errors to client.
  - [ ] announcing winners.
  - [ ] make some commands require certain permissions or challenges before they can be executed. 
  
* Frontend : at this point the game only features simple line drawing so in order to improve the drawing experience i will add to color paddle: 
  - [x] brush sizing.
  - [x] eraser (actually, we just turn the drawing color white 😀).
  - [x] fill tool
  - [ ] make it responsive 

## License
This game is licensed under the MIT license. See `LICENSE.md` for more information.
