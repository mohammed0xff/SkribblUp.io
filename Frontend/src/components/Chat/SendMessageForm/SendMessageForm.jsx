import React, { Component } from 'react';
import { Form, Button, FormControl, InputGroup } from 'react-bootstrap';

class SendMessageForm extends Component {
  constructor(props) {
    super(props);
    this.state = {
      message: ''
    };
  }

  handleSubmit = (e) => {
    e.preventDefault();
    this.props.sendMessage(this.state.message);
    this.setState({ message: '' });
  }

  handleChange = (e) => {
    this.setState({ message: e.target.value });
  }

  render() {
    const { message } = this.state;
    return (
      <Form onSubmit={this.handleSubmit}>
        <InputGroup>
          <FormControl
            type="user"
            placeholder="message..."
            value={message}
            onChange={this.handleChange}
          />
          <Button variant="primary" type="submit" disabled={!message}>
            Send
          </Button>
        </InputGroup>
      </Form>
    );
  }
}

export default SendMessageForm;
