using RewardsForYou.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using System.IO;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Data.Services.Client;

namespace RewardsForYou.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index(int UserID)
        {
            Users user = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                //get the user from the db
                user = db.Users
                    .Include(usr => usr.Missions.Select(mis => mis.Tasks))
                    .Include(usr => usr.UsersRewards.Select(usrrew => usrrew.Rewards))
                    .Include(usr => usr.Users2)
                    .Where(l => l.UserID == UserID).FirstOrDefault();
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult _ChooseRewards(int UserID)
        {
            //take all the rewards
            RewardsForUser rewardsForUser = new RewardsForUser();
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                rewardsForUser.UserPoints = db.Users.Where(l => l.UserID == UserID).Select(l => l.UserPoints).FirstOrDefault();
                rewardsForUser.Rewards = db.Rewards.ToList();
            }
            rewardsForUser.UserID = UserID;
            return PartialView(rewardsForUser);
        }

        public ActionResult _PartialTakeReward(int RewardsID, int UserID)
        {
            Rewards reward = null;
            Users user = null;
            UsersRewards userReward = new UsersRewards();
            Rewards availabilityReward = new Rewards();
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                Users userUpdated = db.Users.Find(UserID);
                availabilityReward = db.Rewards.Find(RewardsID);
                reward = db.Rewards.Where(l => l.RewardsID == RewardsID).FirstOrDefault();
                user = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();

                //check if the points of the user are enough for the selected reward
                if (user.UserPoints >= reward.Points)
                {
                    //sottrazione dei punti allo user
                    userUpdated.UserPoints = user.UserPoints - reward.Points;

                    //diminuzione dell'availability del reward
                    availabilityReward.Availability = availabilityReward.Availability - 1;

                    //Inserisco il nuovo reward dell'utente nel db
                    userReward.UserID = user.UserID;
                    userReward.RewardsID = reward.RewardsID;
                    userReward.Note = "";
                    userReward.RewardsDate = DateTime.Now;
                    db.UsersRewards.Add(userReward);
                    db.SaveChanges();
                    return Json(new { messaggio = $"{reward.Type} aggiunto/a con successo", flag = true });
                }

                else
                {
                    return Json(new { messaggio = $"I punti non sono sufficienti", flag = false });
                }
            }

        }

        [HttpPost]
        public async Task<JsonResult> GetAADUserImageAsync(string EMail)
        {
            JsonResult ret = null;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            try
            {
                string graphResourceID = "https://graph.windows.net/";
                UserProfileController userProfileController = new UserProfileController();

                Uri servicePointUri = new Uri(graphResourceID);
                Uri serviceRoot = new Uri(servicePointUri, tenantID);
                ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                      async () => await userProfileController.GetTokenForApplication());

                // use the token for querying the graph to get the user details

                var result1 = await activeDirectoryClient.Users.ExecuteAsync(); ;

                var result = await activeDirectoryClient.Users
                    .Where(u => u.UserPrincipalName.Equals(EMail))
                    .ExecuteAsync();

                //var result = await activeDirectoryClient.Users
                //    .Where(u => u.ObjectId.Equals(userObjectID))
                //    .ExecuteAsync();
                IUser user = result.CurrentPage.ToList().First();

                DataServiceStreamResponse photo = await user.ThumbnailPhoto.DownloadAsync();
                using (MemoryStream s = new MemoryStream())
                {
                    photo.Stream.CopyTo(s);
                    var encodedImage = Convert.ToBase64String(s.ToArray());
                    ret = Json(new {
                        Success = true,
                        Base64StringImage = String.Format("data:image/gif;base64,{0}", encodedImage)
                    });
                }
            }
            catch (AdalException e)
            {
                ret = Json(new
                {
                    Success = false,
                    Message = e.Message
                });
            }
            // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
            catch (Exception e)
            {
                ret = Json(new
                {
                    Success = false,
                    Message = e.Message
                });
            }
            return ret;
        }
    }


}