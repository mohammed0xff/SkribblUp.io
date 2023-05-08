import { useState, useEffect, Component } from 'react';
import React from 'react';
import SendMessageForm from './SendMessageForm/SendMessageForm';
import MessageContainer from './MessageContainer/MessageContainer';

import "./Chat.css"

import { InvokeMethods, ListeningMethods, Mods, RoomUserActions } from '../../constants';

class Chat extends Component {
  constructor(props) {
    super(props);
    this.state = {
      messages : []
    }
    this.setupCallbacks();
  }

  setupCallbacks(){
    const { connection } = this.props;
    
    try{
      connection.on(ListeningMethods.ReceiveMessage, ({username, content}) => {
          this.setState(prevState => ({
            messages : [...prevState.messages, { username, content}] 
          }));
      });

      connection.on(ListeningMethods.UserAction, ({user, actionType})=>{
        let message = ""; 
          
        switch (actionType) {
          case RoomUserActions.UserJoined:
            message = `${user.userName} Joined the room.`;
            break;
            
          case RoomUserActions.UserLeft:
            message = `${user.userName} Left the room.`;
            break;
    
          case RoomUserActions.UserDisconnected:
            message = `${user.userName} Disconnected.`;
            break;
    
          case RoomUserActions.UserGuessed:
            message = `${user.userName} Guessed the word!`;
            break;

          case RoomUserActions.ChoosingWord:
            message = `${user.userName} Choosing a word.`;
            break;
          case RoomUserActions.DrawingNow:
            message = `${user.userName} Got the pen.`;
            break;
            
          default:
            message = `${user.userName} Done something but idk what it is.`;
            break;
        }
        this.addBotMessage(message);
      })

      connection.on(ListeningMethods.RevealWord, (word) => {
        let message = `Word was ${word}!`
        this.addBotMessage(message);
      });

    } catch (error) {
      console.log(error);
    }
  }
  
  addBotMessage(message){
    this.setState(prevState => ({
      messages : [...prevState.messages, { username : Mods.Bot, content : message}] 
    }));
  }

  sendMessage = async (message) => {
    try {
      await this.props.connection.invoke(InvokeMethods.SendMessage, message);
    } catch (e) {
      console.log(e);
    }
  }

  render() {
    return(
      <div className='chat'>
          <MessageContainer messages={this.state.messages} />
          <SendMessageForm sendMessage={this.sendMessage} />
      </div>
      )
  }
}

export default Chat;