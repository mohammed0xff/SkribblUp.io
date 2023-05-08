import React, { Component } from 'react';
import { Form, Button } from 'react-bootstrap';
import './JoinForm.css';
import { ListeningMethods } from '../../constants';

class JoinForm extends Component {
  constructor(props) {
    super(props);
    this.state = {
      username: '',
      slectedRoom: '',
    }
  }

  handleUsernameChange = (e) => {
    this.setState({ username: e.target.value });
  }

  handleRoomChange = (room) => {
    this.setState({ slectedRoom : room });
  }

  handleSubmit = (e) => {
    e.preventDefault();
    const { username, slectedRoom } = this.state;
    this.props.joinRoom(username, slectedRoom);
    this.props.setUserData({ username : username, room : slectedRoom });
  }

  render() {
    const { username, slectedRoom } = this.state;
    return (
      <div className="form-container">
        <Form onSubmit={this.handleSubmit}>
          <div className="form-header">
            <h1>Join a Room</h1>
          </div>
          <Form.Group controlId="formBasicUsername">
            <Form.Label>Username:</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter username"
              value={username}
              onChange={this.handleUsernameChange} />
          </Form.Group>
          <Form.Group>
            <Form.Label>Select a room:</Form.Label>
            <div className="room-btns-container">
              {this.props.rooms.map((_room, index) => {
                return (
                  <Button
                    key={index}
                    variant={slectedRoom === _room ? 'secondary' : 'outline-secondary'}
                    className="room-btn"
                    onClick={() => this.handleRoomChange(_room)}>
                    {_room}
                  </Button>
                )
              })}
            </div>
          </Form.Group>
          <div className="submit-btn-container">
            <Button variant="success" type="submit" disabled={!username || !slectedRoom}>Join</Button>
          </div>
        </Form>
      </div>
    );
  }
}

export default JoinForm;