import React, { Component } from 'react';
import { CLIENT_METHODS, HUB_METHODS, DRAWING_COLORS, TOOLS, SIZES } from '../../constants';
import './Canvas.css';

class Canvas extends Component {
  constructor(props) {
    super(props);
    // Create a reference to the canvas element
    this.canvasRef = React.createRef();
    this.drawing = false;
    this.selectedTool = TOOLS[0];
    this.color = DRAWING_COLORS[0];
  }

  componentDidMount() {
    const canvas = this.canvasRef.current;
    const context = canvas.getContext('2d');
    this.styleContext(context);
    
    const { connection } = this.props;
    try {
      connection.on(CLIENT_METHODS.ChangeColor, this.handleChangeColor);
      connection.on(CLIENT_METHODS.ChangeBrushSize, this.handleChangeBrushSize);
      connection.on(CLIENT_METHODS.SelectTool, this.handleSelectTool);
      connection.on(CLIENT_METHODS.StartDrawing, this.handleStartDrawing);
      connection.on(CLIENT_METHODS.Draw, this.handleDraw);
      connection.on(CLIENT_METHODS.FillColor, this.handleFillColor);
      connection.on(CLIENT_METHODS.StartTimer, this.handleStartTimer);
      connection.on(CLIENT_METHODS.ClearCanvas, this.handleClearCanvas);
    } catch (err) {
      console.error(err);
    }
    
    canvas.addEventListener('mousedown', (e) => this.onDown(e) );
    canvas.addEventListener('mousemove', (e) => this.onMove(e) ); 
    canvas.addEventListener('mouseup',   (e) => this.onUp(e)   );
    canvas.addEventListener('mouseout',  (e) => this.onOut(e)  );
  }

  componentWillUnmount() {
    const { connection } = this.props;
    try{
    connection.off(CLIENT_METHODS.ChangeColor, this.handleChangeColor);
    connection.off(CLIENT_METHODS.ChangeBrushSize, this.handleChangeBrushSize);
    connection.off(CLIENT_METHODS.SelectTool, this.handleSelectTool);
    connection.off(CLIENT_METHODS.StartDrawing, this.handleStartDrawing);
    connection.off(CLIENT_METHODS.Draw, this.handleDraw);
    connection.off(CLIENT_METHODS.FillColor, this.handleFillColor);
    connection.off(CLIENT_METHODS.StartTimer, this.handleStartTimer);
    connection.off(CLIENT_METHODS.ClearCanvas, this.handleClearCanvas);
    }catch(err){
      console.error(err);
    }
    
    const canvas = this.canvasRef.current;
    canvas.removeEventListener('mousedown', (e)=>this.onDown(e) );
    canvas.removeEventListener('mousemove', (e)=>this.onMove(e) );
    canvas.removeEventListener('mouseup',   (e)=>this.onUp(e)   );
    canvas.removeEventListener('mouseout',  (e)=>this.onOut(e)  );
  }

    
  toolsActions = {
    brush: {
      onDown:      (e) => this.startDrawing(e),
      onSelected:  (e) => this.brushSelected(e),
      onMove:      (e) => this.draw(e),
      onUp:        (e) => this.stopDrawing(e),
      onOut:       (e) => this.stopDrawing(e),
    },

    eraser: {
      onSelected: (e) => this.eraserSelected(e),
      onDown:     (e) => this.startDrawing(e),
      onMove:     (e) => this.draw(e),
      onUp:       (e) => this.stopDrawing(e),
      onOut:      (e) => this.stopDrawing(e),
    },

    fill: {
      onSelected: (e) => this.fillSelected(e),
      onDown:     (e) => this.sendfill(e),
      onMove:     (e) => {},
      onUp:       (e) => {},
      onOut:      (e) => {},
    },
  };  


  onDown(e) {
    this.toolsActions[this.selectedTool].onDown(e);
  }

  onMove(e) {
    this.toolsActions[this.selectedTool].onMove(e);
  }

  onUp(e) {
    this.toolsActions[this.selectedTool].onUp(e);
  }

  onOut(e) {
    this.toolsActions[this.selectedTool].onOut(e);
  }

  brushSelected = () =>{
    const context = this.canvasRef.current.getContext('2d');
    context.strokeStyle = this.color;
  }

  eraserSelected = () => {
    // set drawing color to white
    const context = this.canvasRef.current.getContext('2d');
    context.strokeStyle = "#FFFFFF"; 
  }

  fillSelected = () =>{
    const context = this.canvasRef.current.getContext('2d');
    context.strokeStyle = this.color;
  }

  // Start drawing when mouse is pressed down
  startDrawing = (event) => {
    if (this.props.isDrawing) {
      this.drawing = true;
      this.sendDrawingStarted(event.offsetX, event.offsetY);
    }
  };

  // Send signal that drawing has started to hub
  sendDrawingStarted = async (posx, posy) => {
    try {
      await this.props.connection.invoke(HUB_METHODS.SendDrawingStarted, { posx, posy });
    } catch (err) {
      console.error(err);
    }
  };
  
  // Draw line and send mouse position to SignalR hub when mouse is moved
  draw = (event) => {
    if (!this.drawing) return;
    if (event.buttons !== 1) return;
    this.throttle(this.sendMousePos(event.offsetX, event.offsetY), 100);
  };

  // Send mouse position to hub
  sendMousePos = async (posx, posy) => {
    try {
      await this.props.connection.invoke(HUB_METHODS.SendMousePos, { posx, posy });
    } catch (e) {
      console.error(e);
    }
  };

  // Stop drawing when mouse is released
  stopDrawing = () => {
    this.drawing = false;
  };
  
  // flood fill
  fillColor = (x, y) => {
    const context = this.canvasRef.current.getContext('2d');
    const imageData = context.getImageData(0, 0, context.canvas.width, context.canvas.height);
    const pixelStack = [[x, y]];
    // y for rows(down) and x for columns(right) 
    // basically this is how to treat a 1D array as a 2D array.
    const startPixelIndex = (y * imageData.width + x) * 4; // but why *4 ??? 
    // the * 4 is because each pixel is represented by four values in the array (rgba) 
    // and each of these values takes up one byte, so there are four bytes for each pixel.
    
    // Color of the pressed pixed.
    const startColor = {
      r: imageData.data[startPixelIndex],
      g: imageData.data[startPixelIndex + 1],
      b: imageData.data[startPixelIndex + 2],
      a: imageData.data[startPixelIndex + 3],
    };

    const targetColor = this.hexToRgb(this.color);
  
    while (pixelStack.length > 0 ) {
      const [xPos, yPos] = pixelStack.pop();
      const pixelIndex = (yPos * imageData.width + xPos) * 4;
      const pixelColor = {
        r: imageData.data[pixelIndex],
        g: imageData.data[pixelIndex + 1],
        b: imageData.data[pixelIndex + 2],
        a: imageData.data[pixelIndex + 3],
      };
  
      // Check if the pixel color is the same as the start color
      if (
        pixelColor.r === startColor.r &&
        pixelColor.g === startColor.g &&
        pixelColor.b === startColor.b &&
        pixelColor.a === startColor.a
      ) {
        // set the color of the current pixel to the target color
        imageData.data[pixelIndex] = targetColor.r;
        imageData.data[pixelIndex + 1] = targetColor.g;
        imageData.data[pixelIndex + 2] = targetColor.b;
        imageData.data[pixelIndex + 3] = targetColor.a;
  
        // Add neighboring pixels.
        if (xPos > 0) {
          pixelStack.push([xPos - 1, yPos]);
        }
        if (xPos < imageData.width - 1) {
          pixelStack.push([xPos + 1, yPos]);
        }
        if (yPos > 0) {
          pixelStack.push([xPos, yPos - 1]);
        }
        if (yPos < imageData.height - 1) {
          pixelStack.push([xPos, yPos + 1]);
        }
      }
    }
    // Put edited canvas data back to context.
    context.putImageData(imageData, 0, 0);
  }

   hexToRgb(hex) {
    const r = parseInt(hex.slice(1, 3), 16);
    const g = parseInt(hex.slice(3, 5), 16);
    const b = parseInt(hex.slice(5, 7), 16);
    return { r, g, b, a: 255 };
  }

  // Send mouse position to hub
  sendfill = async ({ offsetX: posX, offsetY: posY }) => {
    try {
      await this.props.connection.invoke(HUB_METHODS.FillColor, {posX, posY});
    } catch (e) {
      console.error(e);
    }
  };

  clearCanvas = () => {
    const canvas = this.canvasRef.current;
    const context = canvas.getContext('2d');
    context.clearRect(0, 0, canvas.width, canvas.height);
  };

  // Set the style of the canvas context
  styleContext = (context) => {
    context.strokeStyle = this.color;
    context.lineJoin = 'round'; 
    context.lineWidth = SIZES[0].value; 
  }

  resetCanvas() {
    this.selectedTool = TOOLS[0];
    this.color = DRAWING_COLORS[0];
    const context = this.canvasRef.current.getContext('2d');
    this.styleContext(context);
    this.clearCanvas();
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

  handleChangeColor = (color) => {
    this.color = color;
    if (this.selectedTool !== 'eraser') {
      const canvas = this.canvasRef.current;
      const context = canvas.getContext('2d');  
      context.strokeStyle = color;
    }
  }
  
  handleChangeBrushSize = (selectedSize) => {
    const canvas = this.canvasRef.current;
    const context = canvas.getContext('2d');
    context.lineWidth = SIZES[selectedSize - 1].value;
  }
  
  handleSelectTool = (idx) => {
    this.selectedTool = TOOLS[idx];
    this.toolsActions[this.selectedTool].onSelected();
  }
  
  handleStartDrawing = ({ posX, posY }) => {
    const canvas = this.canvasRef.current;
    const context = canvas.getContext('2d');
    context.beginPath();
    context.moveTo(posX, posY);
  }
  
  handleDraw = ({ posX, posY }) => {
    const canvas = this.canvasRef.current;
    const context = canvas.getContext('2d');
    context.lineTo(posX, posY);
    context.stroke();
  }
  
  handleFillColor = ({ posX, posY }) => {
    this.fillColor(posX, posY);
  }
  
  handleStartTimer = () => {
    this.resetCanvas();
  }
  
  handleClearCanvas = () => {
    this.clearCanvas();
  }
  
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