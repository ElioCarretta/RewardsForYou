using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace RewardsForYou.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {

        public object Serial { get; private set; }
        public int MissionID { get; private set; }
        public int TaskID { get; private set; }

        // GET: Manager
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaUsers()
        {
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                int userID = (int)Session["UserID"];
                List<Users> users = new List<Users>();
                users = db.Users.Where(x => x.ManagerUserID == userID).ToList();

                return View(users);
            }
        }

        public ActionResult ViewTask(int? UserID = null)
        {

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                List<Tasks> task = new List<Tasks>();
                List<Missions> t = null;

                //task = db.Tasks.Where(l => l.TaskID == UserID).ToList();

                //t = db.Missions.Include(m => m.Tasks).Where(l => l.UserID == UserID).ToList();

                t = db.Missions.Include(m => m.Tasks).Where(l => l.UserID == UserID).ToList();

                foreach (Missions m in t)
                {
                    task.Add(m.Tasks);
                }

                return View(task);
            }

        }

        public ActionResult AssegnaTask(int UserID)
        {

            TaskUserView tasksUsers = new TaskUserView();



            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                tasksUsers.task = db.Tasks.ToList();



                // tasksUsers.task= db.Tasks.ToList();

            }
            tasksUsers.UsersID = UserID;

            return View(tasksUsers);

        }
                
        public ActionResult _DoAddTaskJson(Tasks DatiTask, int TaskID, int UserID)
        {

            TaskUserView tasksUsers = new TaskUserView();

            Tasks task = null;
            Users user = null;



            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                task = db.Tasks.Where(l => l.TaskID == TaskID).FirstOrDefault();
                user = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();

                Missions mission = new Missions
                {

                    Tasks = task,
                    Users = user,
                    UserID = user.UserID,
                    TaskID = task.TaskID,
                    StartDate = DateTime.Now,
                    EndDate = task.ExpiryDate,
                    Note = "",
                    Status = 0
                };

                db.Missions.Add(mission);
                db.SaveChanges();
            }
            return Json(new { messaggio = $"Task : {DatiTask.TaskID} assegnato con successo" });
        }
    }
}


//public ActionResult DoAddTask(Tasks DatiTask)
//{

//    if (DatiTask.TaskID != 0 && !string.IsNullOrEmpty(DatiTask.Type) && !string.IsNullOrEmpty(DatiTask.Description) && DatiTask.Points != 0)
//    {
//        using (RewardsForYouEntities db = new RewardsForYouEntities())
//        {
//            db.Tasks.Add(DatiTask);

//            db.SaveChanges();


//        }
//    }

//    return View("Index");
//}




//public ActionResult DetailEmployee()
//{
//    using (RewardsForYouEntities db = new RewardsForYouEntities())
//    {

//        return View();
//    }
//}




//public ActionResult _PartialTaskDetails(int UserID)
//{
//    ViewModel viewModel = new ViewModel();
//    Missions x = null;
//    Users u = null;
//    IQueryable<Tasks> t = null;

//    using (RewardsForYouEntities db = new RewardsForYouEntities())
//    {
//        //get tasks of the user
//        u= db.Users.Where(l => l.EMail == EMail).FirstOrDefault();
//        x = db.Missions.Where(l => l.UserID == UserID).FirstOrDefault();
//        t = db.Tasks.Where(l => l.TaskID == x.TaskID);

//        viewModel.User = u;
//        viewModel.Mission = t.ToList();



//    }

//    return PartialView(viewModel);
//}



/*
 * @using (Html.BeginForm("ListaUsers", "Manager", null, FormMethod.Post))
 * 
 
     <a class="btn btn-primary" onclick="ShowMissionModal ('@item.UserID', '@item.Name', '@item.Surname', '@item.EMail','@item.ManagerUserID')">Details Employee</a>
                @*<a class="btn btn-primary" onclick="ShowAddlModal ('@item.TaskID', '@item.Type','@item.Description', '@item.ExpiryDate', '@item.Points', '@item.Finished')">+</a>*@
            </td>
        </tr>
    }

</table>

<script type="text/javascript">
    function ShowMissionModal(UserID, Name, Surname, EMail, ManagerUserID) {

        $('#showModal .modal-body').load(Router.action('Manager', '_PartialMission', { UserID: UserID, Name: Name, Surname: Surname, EMail: EMail, ManagerUserID: ManagerUserID }));
        $('#showModal').modal("show");


    }

    function ShowAddModal(TaskID, Type, Description, ExpiryDate, Points, Finished) {

        $('#showModal .modal-body').load(Router.action('Manager', 'DoAddTask', { TaskID: TaskID, Type: Type, Description: Description, ExpiryDate: ExpiryDate, Points: Points, Finished: Finished}));
        $('#showModal').modal("show");


    }



</script>
     
     
     */

