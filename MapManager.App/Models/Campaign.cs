using System.Collections.Generic;

namespace MapManager.App.Models
{
    public class Campaign
    {
        public string Name { get; set; } = "";
        public List<Island> Islands { get; set; } = new();
    }
}
