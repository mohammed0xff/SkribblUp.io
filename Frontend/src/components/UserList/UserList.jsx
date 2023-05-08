import React, { Component } from 'react';
import { InvokeMethods, ListeningMethods, RoomUserActions } from '../../constants';
import './UserList.css';

class UserList extends Component {
  constructor(props) {
    super(props);
    this.users = [
    ] 
    this.state = {
      userList: this.users
    };
    this.userRefs = [];
    this.setupCallbacks();
  }

  setupCallbacks() {
    const { connection} = this.props;
    try {
      connection.on(ListeningMethods.UsersInRoom, (users) => {
        this.setState({ userList: users });
      });

      connection.on(ListeningMethods.NewTurn, () => {
        this.resetUsers();
      });
      connection.on(ListeningMethods.UserAction, ({ user, actionType }) => {
        this.handleUserActions(user, actionType);
      });
      connection.onclose((e) => {
        this.setState({ userList: [] });
      });
    } catch (e) {
      console.log(e);
    }
  }

  componentDidMount(){
    this.getUsersInRoom();
  }

  getUsersInRoom = async () => {
    try {
      await this.props.connection.invoke(InvokeMethods.GetUsersInRoom, this.props.roomName);
    } catch (error) {
      console.log(error);
    }
  }

  handleUserActions = (user, action) => {
    switch (action) {
      case RoomUserActions.UserJoined:
        this.addUser(user);
        break;

      case RoomUserActions.UserDisconnected || RoomUserActions.UserLeft:
        this.removeUser(user);
        break;

      case RoomUserActions.UserGuessed:
        this.markUserAsGuessed(user);
        break;

      default:
        break;
    }
  };

  addUser = (user) => {
    const { userList } = this.state;
    const newUserList = [...userList];
    newUserList.push(user);
    this.setState({ userList: newUserList });
  };

  removeUser = (user) => {
    const { userList } = this.state;
    const newUserList = [...userList];
    newUserList.pop(user);
    this.userRefs.pop(user);
    this.setState({ userList: newUserList });
  };

  markUserAsGuessed = ({userName}) => {
    const { userList } = this.state;
    const userIndex = userList.findIndex((user) => user.userName === userName);
    const userDiv = this.userRefs[userIndex];
    userDiv.classList.add('guessed');
  };

  resetUsers = () => {
    this.userRefs.forEach((element) => {
      element.classList.remove('guessed');
    });
  };

  render() {
    const { userList } = this.state;

    return (
      <div className="user-list-container">
        <div className="user-list">
          <div>Users in Room</div>
          <div id="users">
            {userList.map((user, index) => (
              <div
                key={user.userName}
                className="user"
                ref={(el) => (this.userRefs[index] = el)}
              >
                <div className="user-number">{index + 1}.</div>
                <div className="user-name">
                  {user.userName}  
                </div>
                <div className='points'> {user.points} pts </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  }
}

export default UserList;