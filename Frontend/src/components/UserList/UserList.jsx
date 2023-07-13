import React, { Component } from 'react';
import { HUB_METHODS, CLIENT_METHODS, USER_ACTIONS } from '../../constants';
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
      connection.on(CLIENT_METHODS.UsersInRoom, (users) => {
        this.setState({ userList: users });
      });

      connection.on(CLIENT_METHODS.NewTurn, () => {
        this.resetUsers();
      });
      connection.on(CLIENT_METHODS.UserAction, ({ user, actionType }) => {
        this.handleUserActions(user, actionType);
      });
      connection.onclose((e) => {
        this.setState({ userList: [] });
      });
    } catch (err) {
      console.error(err);
    }
  }

  componentDidMount(){
    this.getUsersInRoom();
  }

  getUsersInRoom = async () => {
    try {
      await this.props.connection.invoke(HUB_METHODS.GetUsersInRoom, this.props.roomName);
    } catch (err) {
      console.error(err);
    }
  }

  handleUserActions = (user, action) => {
    switch (action) {
      case USER_ACTIONS.UserJoined:
        this.addUser(user);
        break;

      case USER_ACTIONS.UserDisconnected || USER_ACTIONS.UserLeft:
        this.removeUser(user);
        break;

      case USER_ACTIONS.UserGuessed:
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