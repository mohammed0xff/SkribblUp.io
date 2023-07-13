export const CLIENT_METHODS = {
  // drawing
  Draw : "Draw",
  FillColor : "FillColor",
  StartDrawing : "StartDrawing",
  ClearCanvas : "ClearCanvas",
  ChangeColor : "ChangeColor",
  ChangeBrushSize : "ChangeBrushSize",
  SelectTool : "SelectTool",
  PickThePen : "PickThePen",
  
  // word
  WordHint : "WordHint",
  RevealWord : "RevealWord",
  StartTimer : "StartTimer",
  StopTimer : "StopTimer",

  // user
  UserAction : "UserAction",
  ChooseWord : "ChooseWord",
  ReceiveMessage : "ReceiveMessage",
  UsersInRoom : "UsersInRoom",
  ResetUsers : "ResetUsers",
  NewTurn : "NewTurn",

  // exceptions
  Error : "Error"
};

export const HUB_METHODS = {
  // drawing 
  FillColor : "FillColor",
  SendDrawingStarted : "SendDrawingStarted",
  SendMousePos : "SendMousePos",
  ChangeColor : "ChangeColor",
  ClearCanvas : "ClearCanvas",    
  ChangeBrushSize : "ChangeBrushSize",
  SelectTool : "SelectTool",
  
  // user 
  JoinRoom : "JoinRoom",
  LeaveRoom : "LeaveRoom",
  GetUsersInRoom : "GetUsersInRoom",
  PickAWord : "PickAWord",
  SendMessage : "SendMessage",
  
}

export const USER_ACTIONS = {
  UserLeft : "UserLeft",
  UserJoined : "UserJoined",
  UserDisconnected : "UserDisconnected",
  UserGuessed : "UserGuessed",
  ChoosingWord : "ChoosingWord",
  DrawingNow : "DrawingNow",
}

export const MODS = {
  Bot : "Bot"
}

export const DRAWING_COLORS = [
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

export const TOOLS = [
  'brush', 
  'fill', 
  'eraser'
];
  
export const SIZES = [
  { size: 1, value: 2  },
  { size: 2, value: 5  },
  { size: 3, value: 10 },
  { size: 4, value: 14 },
];
  