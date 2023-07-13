namespace DrawAndGuess.Constants
{
    public static class ClientMethods
    {
        // actions
        public static readonly string UserAction = "UserAction";
        public static readonly string GameAction = "GameAction";
        public static readonly string GameStateChange = "GameStateChange";
        
        // chat
        public static readonly string UsersInRoom = "UsersInRoom";
        public static readonly string ReceiveMessage = "ReceiveMessage";
        
        // guessing 
        public static readonly string ChooseWord = "ChooseWord";
        public static readonly string RevealWord = "RevealWord";
        public static readonly string WordHint = "WordHint";
        
        // drawing 
        public static readonly string Draw = "Draw";
        public static readonly string FillColor = "FillColor";
        public static readonly string StartDrawing = "StartDrawing";
        public static readonly string ChangeColor = "ChangeColor";
        public static readonly string ClearCanvas = "ClearCanvas";
        public static readonly string NewTurn = "NewTurn";
        public static readonly string PickThePen = "PickThePen";
        public static readonly string SelectTool = "SelectTool";
        public static readonly string ChangeBrushSize = "ChangeBrushSize";
       
        // timing
        public static readonly string StopTimer = "StopTimer";
        public static readonly string StartTimer = "StartTimer";
    }
}
