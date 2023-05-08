import React, { Component } from 'react';
import { InvokeMethods, ListeningMethods, DrawingColors } from '../../constants';
import './ColorPaddle.css'

class ColorPaddle extends Component {
  constructor(props) {
    super(props);
    this.state = {
      selectedColor: DrawingColors[0]
    };
  }

  handleClick = async (color) => {
    const { connection } = this.props;
    this.setState({ selectedColor: color });
    try {
      await connection.invoke(InvokeMethods.ChangeColor, color);
    } catch (error) {
      console.log(error);
    }
  }

  clearCanvas = async () => {
    const { connection } = this.props;
    try {
      await connection.invoke(InvokeMethods.ClearCanvas);
    } catch (error) {
      console.log(error);
    }
  }

  componentDidMount() {
    const { connection } = this.props;
    try {
      connection.on(ListeningMethods.ColorChanged, (color) => {
        this.setState({ selectedColor: color });
      })
      // reset color to default after user finishes drawing
      connection.on(ListeningMethods.DrawingFinished, () => {
        this.setState({ selectedColor: Colors[0] });
      })
    }
    catch (error) {
      console.log(error)  
    }
  }

  render() {
    const { selectedColor } = this.state;
    return ( 
      <div className="color-paddle ">
        <input type="color" value={selectedColor} 
          onChange={(e) => {
            this.setState({ selectedColor: e.target.value });
            this.handleClick(e.target.value)
          }}
         />
        {DrawingColors.map((color) => (
          <div
            key={color}
            className='color-btn'
            style={{
              backgroundColor: color,
              boxShadow:
                selectedColor === color
                  ? '0px 0px 5px black'
                  : 'none',
            }}
            onClick={() => this.handleClick(color)}
          />
        ))}
        <div className='clear-btn' onClick={() => this.clearCanvas()}>Clear</div>
      </div>
    );
  }
}

export default ColorPaddle;