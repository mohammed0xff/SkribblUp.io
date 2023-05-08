import React, { Component } from "react";
import './MessageContainer.css'

class MessageContainer extends Component {
  constructor(props) {
    super(props);
    this.messageRef = React.createRef();
  }

  componentDidUpdate(prevProps) {
    if (this.messageRef && this.messageRef.current && this.props.messages !== prevProps.messages) {
      const { scrollHeight, clientHeight } = this.messageRef.current;
      this.messageRef.current.scrollTo({ left: 0, top: scrollHeight - clientHeight, behavior: 'smooth' });
    }
  }

  render() {
    return (
      <div ref={this.messageRef} className="message-container">
        {this.props.messages.map((m, index) => (
          <div className="message" key={index}>
            <div
              className={`message-text 
                ${index % 2 === 0 ? "bg-white" : "bg-light"} 
                ${m.username === 'Bot' ? "bot-message" : ""}`}
            >
              <strong>{m.username}:</strong> {m.content}
            </div>
          </div>
        ))}
      </div>
    );
  }
}

export default MessageContainer;