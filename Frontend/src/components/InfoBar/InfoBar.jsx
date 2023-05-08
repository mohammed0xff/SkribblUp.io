import { Component } from "react";
import { ListeningMethods } from "../../constants";
import './InfoBar.css';

class InfoBar extends Component {
  constructor(props) {
    super(props);
    this.state = {
      hint: '',
      timer: null,
      minutes: 0,
      seconds: 0,
      timerOn: false
    };
    try {
      const connection = props.connection;
      connection.on(ListeningMethods.WordHint, (hint) => {
        this.setState({ hint });
      });
      connection.on(ListeningMethods.StartTimer, (minutes) => {
        this.startTimer(minutes);
      });
      connection.on(ListeningMethods.StopTimer, () => {
        this.stopTimer();
      });
      connection.on(ListeningMethods.RevealWord, (word) => {
        this.setState({hint : word})
      });
    } catch (error) {
      console.log(error);
    }
  }

  startTimer(minutes) {
    if (this.state.timerOn) {
      return;
    }
    const duration = minutes * 60;
    let timer = duration, minutesLeft, secondsLeft;
    const interval = setInterval(() => {
      minutesLeft = parseInt(timer / 60, 10);
      secondsLeft = parseInt(timer % 60, 10);

      this.setState({ minutes: minutesLeft, seconds: secondsLeft, timerOn: true });

      if (--timer < 0) {
        clearInterval(this.state.timer);
        this.setState({ timer: null, minutes: 0, seconds: 0, timerOn: false });
      }
    }, 1000);
    this.setState({ timer: interval });
  }

  stopTimer() {
    clearInterval(this.state.timer);
    this.setState({ timer: null, minutes: 0, seconds: 0, timerOn: false });
  }

  render(){
    const { minutes, seconds, timerOn } = this.state; 
    return (
      <div className="container-fluid d-flex justify-content-between align-items-center">
        <div className="room-name">
          {this.props.room}
        </div>
        <div className="word-hint">
          {this.state.hint}
        </div>
        <div className="timer">
          {timerOn && <> {minutes.toString().padStart(2, '0')}:{seconds.toString().padStart(2, '0')} </>}
        </div>
      </div>
    )
  }
}

export default InfoBar;