import { useState, useEffect, Component } from 'react';
import React from 'react';
import SendMessageForm from './SendMessageForm/SendMessageForm';
import MessageContainer from './MessageContainer/MessageContainer';

import "./Chat.css"

import { HUB_METHODS, CLIENT_METHODS, MODS, USER_ACTIONS } from '../../constants';

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
      connection.on(CLIENT_METHODS.ReceiveMessage, ({username, content}) => {
          this.setState(prevState => ({
            messages : [...prevState.messages, { username, content}] 
          }));
      });

      connection.on(CLIENT_METHODS.UserAction, ({user, actionType})=>{
        let message = ""; 
          
        switch (actionType) {
          case USER_ACTIONS.UserJoined:
            message = `${user.userName} Joined the room.`;
            break;
            
          case USER_ACTIONS.UserLeft:
            message = `${user.userName} Left the room.`;
            break;
    
          case USER_ACTIONS.UserDisconnected:
            message = `${user.userName} Disconnected.`;
            break;
    
          case USER_ACTIONS.UserGuessed:
            message = `${user.userName} Guessed the word!`;
            break;

          case USER_ACTIONS.ChoosingWord:
            message = `${user.userName} Choosing a word.`;
            break;
          case USER_ACTIONS.DrawingNow:
            message = `${user.userName} Got the pen.`;
            break;
            
          default:
            message = `${user.userName} Done something but idk what it is.`;
            break;
        }
        this.addBotMessage(message);
      })

      connection.on(CLIENT_METHODS.RevealWord, (word) => {
        let message = `Word was ${word}!`
        this.addBotMessage(message);
      });

    } catch (error) {
      console.log(error);
    }
  }
  
  addBotMessage(message){
    this.setState(prevState => ({
      messages : [...prevState.messages, { username : MODS.Bot, content : message}] 
    }));
  }

  sendMessage = async (message) => {
    try {
      await this.props.connection.invoke(HUB_METHODS.SendMessage, message);
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