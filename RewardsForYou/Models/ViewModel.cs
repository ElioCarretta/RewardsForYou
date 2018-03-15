using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RewardsForYou.Models
{
    public class ViewModel : RewardsForYouEntities
    {
        public Users User { get; set; }
        public List<Tasks> Mission { get; set; }
        public List<Rewards> Reward { get; set; }
        
    }
}