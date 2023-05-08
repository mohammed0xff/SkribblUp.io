using System.ComponentModel;

namespace DrawAndGuess.Enums
{
    public enum GameState
    {
        [Description("Waiting For Users To Come")]
        WaitingForUsersToCome,

        [Description("Round Started")]
        RoundStarted,

        [Description("Fetching next Artist")]
        FetchingArtist,

        [Description("Choosing a word")]
        ChoosingAWord,

        [Description("Artrist is drawing")]
        ArtistIsDrawing,

        [Description("Word is revealed")]
        WordRevealed,
    }
}
