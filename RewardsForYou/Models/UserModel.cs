using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RewardsForYou.Models
{
    public class UserModel : Users
    {
        List<Users> ListaUser { get; set; }
    }
}