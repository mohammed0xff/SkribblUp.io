import React, { Component } from 'react';
import { ListeningMethods, InvokeMethods, DrawingColors } from '../../constants';
import './Canvas.css';

class Canvas extends Component {
  constructor(props) {
    super(props);
    // Create a reference to the canvas element
    this.canvasRef = React.createRef();
    this.state = {
      context: null,
      drawing: false,
      color: '#4c6ef5'
    };
    // Flag used to ensure that event listeners are only added once
    this.flag = true;
  }

  componentDidMount() {
    const canvas = this.canvasRef.current;
    const context = canvas.getContext('2d');
    this.styleContext(context);
    this.setState({ context });

    if (this.flag) {
      const { connection } = this.props;
      try {
        connection.on(ListeningMethods.StartDrawing, ({ posX, posY }) => {
          if (context === null) {
            console.log('nullll');
          }
          context.beginPath();
          context.moveTo(posX, posY);
        });
        
        connection.on(ListeningMethods.Draw, ({ posX, posY }) => {
          context.lineTo(posX, posY);
          context.stroke();
        });
        
        connection.on(ListeningMethods.StartTimer, () => {
          this.clearCanvas();
          this.setState({ color: DrawingColors[0] });
        });

        connection.on(ListeningMethods.ColorChanged, (color) => {
          this.setState({ color : color });
          context.strokeStyle = color;
        });

        connection.on(ListeningMethods.ClearCanvas, () => {
          this.clearCanvas();
        });
      } catch (error) {
        console.log(error);
      }
      this.flag = false;
    }

    canvas.addEventListener('mousedown', this.startDrawing);
    canvas.addEventListener('mousemove', this.draw);
    canvas.addEventListener('mouseup', this.stopDrawing);
    canvas.addEventListener('mouseout', this.stopDrawing);
  }

  componentWillUnmount() {
    const canvas = this.canvasRef.current;
    canvas.removeEventListener('mousedown', this.startDrawing);
    canvas.removeEventListener('mousemove', this.draw);
    canvas.removeEventListener('mouseup', this.stopDrawing);
    canvas.removeEventListener('mouseout', this.stopDrawing);
  }

  // Send mouse position to SignalR hub
  sendMousePos = async (posx, posy) => {
    try {
      await this.props.connection.invoke(InvokeMethods.SendMousePos, { posx, posy });
    } catch (e) {
      console.log(e);
    }
  };

  // Send signal that drawing has started to hub
  sendDrawingStarted = async (posx, posy) => {
    try {
      await this.props.connection.invoke(InvokeMethods.SendDrawingStarted, { posx, posy });
    } catch (e) {
      console.log(e);
      alert(e);
    }
  };

  // Start drawing when mouse is pressed down
  startDrawing = (event) => {
    if (this.props.isDrawing) {
      this.setState({ drawing: true });
      this.sendDrawingStarted(event.offsetX, event.offsetY);
    }
  };

  // Draw line and send mouse position to SignalR hub when mouse is moved
  draw = (event) => {
    const { drawing } = this.state;
    if (!drawing) return;
    if (event.buttons !== 1) return;
    this.throttle(this.sendMousePos(event.offsetX, event.offsetY), 50);
  };

  // Stop drawing when mouse is released
  stopDrawing = () => {
    this.setState({ drawing: false });
  };

  clearCanvas = () => {
    const canvas = this.canvasRef.current;
    const context = canvas.getContext('2d');
    context.clearRect(0, 0, canvas.width, canvas.height);
  };

  // Set the style of the canvas context
  styleContext = (context) => {
    context.strokeStyle = this.state.color;
    context.lineJoin = 'round'; 
    context.lineWidth = 3; 
  }

  // Limits number of calls per second
  throttle = (callback, delay) => {
    let previousCall = new Date().getTime();
    return function () {
      let time = new Date().getTime();

      if (time - previousCall >= delay) {
        previousCall = time;
        callback.apply(null, arguments);
      }
    };
  };

  render() {
    return (
      <div className="container">
        <div className="canvas-container column">
          <canvas
            ref={this.canvasRef}
            className="canvas"
            width="820"
            height="600"
          />
        </div>
      </div>
    );
  }
}

export default Canvas;