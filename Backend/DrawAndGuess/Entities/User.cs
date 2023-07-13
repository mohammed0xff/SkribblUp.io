using System.Text.Json.Serialization;

namespace DrawAndGuess.Entities
{
    public class User
    {
        [JsonIgnore]
        public string ConnectionId { get; init; }
        public string? UserName { get; set;}
        public int RoomId { get; set; } = 0;
        public string RoomName { get; set; } = null!;
        public int Points { get; set; } = 0;
    }
}
