export const ListeningMethods = {
    // word
    StartTimer : "StartTimer",
    StopTimer : "StopTimer",
    RevealWord : "RevealWord",
    WordHint : "WordHint",
    // user
    UserAction : "UserAction",
    ChooseWord : "ChooseWord",
    ReceiveMessage : "ReceiveMessage",
    UsersInRoom : "UsersInRoom",
    ResetUsers : "ResetUsers",
    NewTurn : "NewTurn",
    // drawing
    Draw : "Draw",
    StartDrawing : "StartDrawing",
    ColorChanged : "ColorChanged",
    ClearCanvas : "ClearCanvas",
    PickThePen : "PickThePen",
    // exceptions
    error : "error"
};

export const InvokeMethods = {
    JoinRoom : "JoinRoom",
    LeaveRoom : "LeaveRoom",
    SendMessage : "SendMessage",
    SendDrawingStarted : "SendDrawingStarted",
    SendMousePos : "SendMousePos",
    ChangeColor : "ChangeColor",
    ClearCanvas : "ClearCanvas",    
    PickAWord : "PickAWord",
    GetUsersInRoom : "GetUsersInRoom",
}


export const RoomUserActions = {
    UserLeft : "UserLeft",
    UserJoined : "UserJoined",
    UserDisconnected : "UserDisconnected",
    UserGuessed : "UserGuessed",
    ChoosingWord : "ChoosingWord",
    DrawingNow : "DrawingNow",
}

export const Mods = {
    Bot : "Bot"
}

export const DrawingColors = [
    '#4c6ef5',
    '#6f42c1',
    '#007bff',
    '#17a2b8',
    '#20c997',
    '#28a745',
    '#ffc107',
    '#dc3545',
    '#6c757d',
    '#343a40',
  ];