import React, { Component } from 'react';
import { Button } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import { HubConnectionBuilder, LogLevel, HubConnectionState } from '@microsoft/signalr';

import JoinForm from '../src/components/JoinForm/JoinForm';
import Canvas from '../src/components/Canvas/Canvas';
import ColorPaddle from '../src/components/ColorPaddle/ColorPaddle';
import Chat from '../src/components/Chat/Chat';
import UserList from '../src/components/UserList/UserList';
import WordPickerModal from './components/ChooseWordForm/WordPickerModal';
import InfoBar from './components/InfoBar/InfoBar';

import { CLIENT_METHODS, HUB_METHODS } from './constants';

import './App.css';

class App extends Component {
  constructor(props) {
    super(props);

    this.state = {
      connection: null,
      isDrawing: false,
      inLobby : true,
      userData : {
        username : '',
        room : ''
      },
      rooms: [
        'Fun Room', 
        'Art Room', 
        'Play Room', 
      ],
    }
  }

  async componentDidMount() {
    await this.makeConnection();
  }

  async makeConnection(){
    const { connection } = this.state;
    if( connection !== null && connection.state === HubConnectionState.Connected){
      // already connected.
      return;
    }
    const connectionBuilder = new HubConnectionBuilder()
    .withUrl("http://localhost:3000/game")
    .configureLogging(LogLevel.Information)
    .build();
  
    connectionBuilder.on(CLIENT_METHODS.AvailableRooms, (rooms) => {
      this.setState({ rooms })
    });
    
    connectionBuilder.on(CLIENT_METHODS.PickThePen, () => {
      this.setState({ isDrawing: true });
    })
    
    connectionBuilder.on(CLIENT_METHODS.NewTurn, () => {
      this.setState({ isDrawing: false });
    })
    connectionBuilder.on(CLIENT_METHODS.Error, (error) => {
      console.error(error)
    })
    connectionBuilder.onclose(e => {
        this.setState({ connection: null, inLobby : true });
    });

    try {
      await connectionBuilder.start();
      this.setState({ connection: connectionBuilder });
    } catch (error) {
      console.error("Error starting connection:", error);
    }
  }

  setUserData = (userData) => {
    this.setState({userData})
  }

  joinRoom = async (username, room) => {
    try {
      await this.makeConnection();
      if(this.state.connection == null){
        alert("Can't connect to server!");
        return;
      }
      await this.state.connection.invoke(HUB_METHODS.JoinRoom, { username, room });
      this.setState({ inLobby : false })
    } catch (e) {
      console.log(e);
    }
  }

  leaveRoom = async () => {
    try {
      await this.state.connection.invoke(HUB_METHODS.LeaveRoom);
      this.setState({ inLobby : true })
    } catch (e) {
      console.log(e);
    }
  }

  closeConnection = async () => {
    try {
      await this.state.connection.stop();
    } catch (e) {
      console.log(e);
    }
  }

  render() {
    const { connection, isDrawing, userData, inLobby, rooms } = this.state;

    return (
      <>
        <div className='App'>
          {inLobby === true ? (
            <JoinForm 
              rooms = {rooms} 
              connection = {connection} 
              joinRoom = {this.joinRoom} 
              setUserData = {this.setUserData}
            />
          ) : (
            <>
              <WordPickerModal connection={connection} />

              <nav className="navbar navbar-light"  >
                <InfoBar room={userData.room} connection={connection} />
              </nav>

              <div className="container-fluid text-center background">
                <div className="row content">
                  <div className="col-lg-2 sidenav ">
                    <UserList connection={connection} leaveRoom={this.leaveRoom} roomName={userData.room} />
                    <Button 
                      variant="danger" 
                      className='leave-room-btn' 
                      onClick={()=>this.leaveRoom()}
                      >
                      Leave Room
                    </Button>
                  </div>

                  <div className="col-lg-7 d-flex flex-wrap" style={{ flexDirection: "row" }}>
                    <Canvas connection={connection} isDrawing={isDrawing} />
                    {isDrawing && <ColorPaddle connection={connection} />}
                  </div>

                  <div className="col-lg-3 sidenav">
                    <Chat connection={connection} />
                  </div>
                </div>
              </div>
            </>
          )}
        </div>
      </>
    );
  }
}

export default App;