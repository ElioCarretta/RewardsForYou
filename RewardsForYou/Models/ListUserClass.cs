using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RewardsForYou.Models
{
    public class ListUserClass : Users
    {
        public ListUserClass()
            {
            ResultList = new List<Users>();
            }
        public List<Users> ResultList { get; set; }
        
    }
}