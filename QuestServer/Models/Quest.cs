using Common;

namespace QuestServer.Models
{
    public class Quest
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Key[] Keys { get; set; }
        public Sensor[] Sensors { get; set; }
    }
}
