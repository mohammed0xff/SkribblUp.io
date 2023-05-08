import React, { Component } from "react";
import { Button } from "react-bootstrap";

import { ListeningMethods, InvokeMethods } from "../../constants";
import "./WordPickerModal.css";

class WordPickerModal extends Component {
  constructor(props) {
    super(props);
    this.state = {
      words: [],
      isOpen : false
    };
  }

  componentDidMount() {
    const { connection } = this.props;
    try {
      connection.on(ListeningMethods.ChooseWord, (words) => {
        
        setTimeout(() => {
          this.setState({ words, isOpen : true })
        }, 3000);
        
        // close if time's up
        setTimeout(() => {
          this.setState({ isOpen : false });
        }, 30000);
      });
    } catch (error) {
      console.log(error);
    }
  }

  chooseWordHandler = async (word) => {
    const { connection } = this.props;
    try {
      await connection.invoke(InvokeMethods.PickAWord, word);
      this.setState({ isOpen : false });
    } catch (error) {
      console.log(error);
    }
  };

  render() {
    const { words, isOpen } = this.state;

    return (
      <>
      {isOpen && 
      <div className="modalBackground">
        <div className="modalContainer">
          <div className="titleCloseBtn">
            <div
              className="exit-button"
              onClick={(e) => {
                this.setState({ isOpen : false });
              }}
            >
              X
            </div>
          </div>
          <div className="title">
            <h1>choose a word</h1>
          </div>
          <div className="body">
            {words.map((word) => {
              return (
                <Button
                  className="word-button"
                  variant="success"
                  onClick={() => this.chooseWordHandler(word)}
                  style={{marginRight : "10px"}}
                >
                  {word}
                </Button>
              );
            })}
          </div>
        </div>
      </div>
      }
      </>
    );
  }
}

export default WordPickerModal;