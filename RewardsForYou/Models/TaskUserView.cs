using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RewardsForYou.Models
{
    public class TaskUserView : RewardsForYouEntities
    {
        public List<Tasks> task { get; set; }
        public int UsersID { get; set; }

    }
}

