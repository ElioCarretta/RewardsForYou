using System.Collections.Generic;

namespace RewardsForYou.Models
{
    public class RewardsForUser
    {
        public List<Rewards> Rewards { get; set; }
        public int UserID { get; set; }
        public int? UserPoints { get; internal set; }
    }
}