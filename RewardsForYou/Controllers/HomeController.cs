using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RewardsForYou.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ActionResult ret = null;
            Session["UserID"] = null;
            Session["ManagerUserID"] = null;

            string EMail = ((System.Security.Claims.ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity).Name;

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                Users user = db.Users.Where(l => l.EMail == EMail).FirstOrDefault();
                if (user != null)
                {
                    Session["UserID"] = user.UserID;

                    switch (user.RoleID)
                    {
                        case 1:
                            //ret = new AdministratorController().Index();
                            ret = RedirectToAction("Index", "Administrator");
                            break;
                        case 2:
                            Session["ManagerUserID"] = user.UserID;
                            //ret = new ManagerController().ListaUsers();
                            ret = RedirectToAction("Index", "Manager", new { UserID = user.UserID });
                            break;
                        case 3:
                            ret = RedirectToAction("Index", "Employee", new { UserID = user.UserID });
                            break;
                    }
                }
                else
                {
                    ret = View("UserNotRegistered");
                }
            }

            return ret;
        }

        //public ActionResult Index()
        //{
        //    int userId = 0;
        //    int roleId = 0;
        //    string EMail = ((System.Security.Claims.ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity).Name;
        //    using (RewardsForYouEntities db = new RewardsForYouEntities())
        //    {
        //        var d = db.Users.Where(l => l.EMail == EMail).FirstOrDefault();
        //        if (d != null)
        //        {
        //            Session["UserID"] = d.UserID;
        //            roleId = d.RoleID;
        //            userId = d.UserID;
        //        }

        //    }
        //    ViewData["Employee"] = false;
        //    ViewData["Manager"] = false;
        //    ViewData["Amministrator"] = false;
        //    if (roleId == 1)
        //    {
        //        ViewData["Employee"] = true;
        //        ViewData["Manager"] = true;

        //    }
        //    else if (roleId == 2)
        //    {
        //        ViewData["Employee"] = true;
        //        ViewData["Amministrator"] = true;
        //    }
        //    else if (roleId == 3)
        //    {
        //        ViewData["Manager"] = true;
        //        ViewData["Amministrator"] = true;
        //    }


        //    return View();
        //}

    }
}



