import React, { Component } from 'react';
import { HUB_METHODS, DRAWING_COLORS, TOOLS, SIZES } from '../../constants';
import Select from 'react-select'
import './ColorPaddle.css'

class ColorPaddle extends Component {
  constructor(props) {
    super(props);
    this.state = {
      currentSize : 1,
      selectedTool : TOOLS[0],
      selectedColor: DRAWING_COLORS[0],
      selectedSizeOption : null,
    };

    this.selectSizeOptions = SIZES.map(
      (option) => { 
        return {
        label : (
                  <img src={`../src/assets/size-${option.size}.png`} height="30px" width="30px"/>
                ),
        size  : option.size,
        value : option.size
      }
    });
  }

  handleColorSelection = async (color) => {
    const { connection } = this.props;
    this.setState({ selectedColor: color });
    try {
      await connection.invoke(HUB_METHODS.ChangeColor, color);
    } catch (err) {
      console.error(err);
    }
  }
  
  handleToolSelection = async (idx) => {
    this.setState({selectedTool : TOOLS[idx]})
    const { connection } = this.props;
    try {
      await connection.invoke(HUB_METHODS.SelectTool, idx);
    } catch (err) {
      console.error(err);
    }
  }
  
  handleSizeSelection = async (opt) => {
    this.setState({selectedSizeOption : opt})
    const { connection } = this.props;
    try {
      await connection.invoke(HUB_METHODS.ChangeBrushSize, opt.size);
    } catch (err) {
      console.error(err);
    }
  }
  
  handleClearCanvas = async () => {
    const { connection } = this.props;
    try {
      await connection.invoke(HUB_METHODS.ClearCanvas);
    } catch (err) {
      console.error(err);
    }
  }
  
  render() {
    const customStyles = {
      control: (provided, state) => ({
        ...provided,
        border: '2px solid #fff',
        boxShadow: state.isFocused ? '0 0 0 2px #4a90e2' : 'none',
        width:'90px',
        cursor: 'pointer',
      }),
      option: (provided, state) => ({
        ...provided,
        backgroundColor: state.isSelected ? '#4a90e2' : '#fff',
        color: state.isSelected ? '#fff' : '#000',
        cursor: 'pointer',
      }),
    };

    const { selectedColor, selectedTool} = this.state;
    
    return ( 
      <div className="paddle-container">
        <div className='row' >

          <div className='col-sm tools'>
            {TOOLS.map((tool, idx) => (
              <div
              key={tool}
              className={`tool ${tool}-tool ${selectedTool === tool ? 'selected' : ''}`}
              style={{ backgroundImage: `url("../src/assets/${tool}.png")` }}
              onClick={()=>this.handleToolSelection(idx)}
              />
            ))}
          </div>
              
          <div className='col-sm'>
            <Select
              options={this.selectSizeOptions}
              defaultValue={this.selectSizeOptions[0]}
              onChange={this.handleSizeSelection}
              styles={customStyles}
              isSearchable={false}
              menuPlacement="top"
            />
          </div>

          <div className='col-sm color-paddle '>
            <input type="color" value={selectedColor} 
              className='color-input'
              onChange={(e) => {
                this.setState({ selectedColor: e.target.value });
                this.handleColorSelection(e.target.value)
              }}
            />
            
            {DRAWING_COLORS.map((color, idx) => (
              <div
                key={idx}
                className='color-btn'
                style={{
                  backgroundColor: color,
                  boxShadow:
                    selectedColor === color
                      ? '0px 0px 5px black'
                      : 'none',
                }}  
                onClick={() => this.handleColorSelection(color)}
              />
            ))}
          </div>
          <div className='col-sm '>
            <div className='clear-btn' onClick={() => this.handleClearCanvas()}></div>
          </div>
        </div>
      </div>
    );
  }
}

export default ColorPaddle;