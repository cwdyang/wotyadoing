
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using SignalRChatApp.Models;

namespace SignalRChatApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// for return Chat View
        /// </summary>
        public ActionResult Chat()
        {
            return View();
        }

        public ActionResult Details(string id)
        {
            var list = GetEvents();
            var model = list.Where(c => c.id == id).FirstOrDefault();
            return View(model);
        }

        public ActionResult List()
        {
            //return View(GetList());
            return View(GetEvents());
        }

        private IEnumerable<AlertModel> GetList()
        {
            var statuses = new List<string>() { "Ambulance Sent", "Arrived at Hospital", "Admitted to Hospital", "Serious", "No Action Taken" };
            var severities = new List<string>() { "Critical", "Moderate", "No Impact" };
            var contacts = new List<ContactModel>()
            {
                new ContactModel() 
                { 
                    Address = "10 Hometown Road, Home Town", Id="Contact1",
                    Cellphone = "021 123 456", Email= "homer@simpson.com",
                    Homephone="09 789 456", Name = "Homer Simpson", PreferredContact = "021 123 456", Relationship="Son"
                },
                new ContactModel() 
                { 
                    Address = "10 Hometown Road, Home Town", Id="Contact2",
                    Cellphone = "021 123 678", Email= "maggie@simpson.com",
                    Homephone="09 789 456", Name = "Maggie Simpson", PreferredContact = "021 123 678", Relationship="Daughter-in-law"
                }
            };
            var list = new List<AlertModel>
            {
                new AlertModel
                {
                    Statuses = statuses,
                    Status = "Serious",
                    Severities = severities,
                    Severity = "Moderate",
                    Contacts = contacts,
                    PrimaryContact = contacts.LastOrDefault(),
                    CareReceiver = "Flora Smith",
                    AlertType = "Fall",
                    AtHome = true,
                    HomeAddress = "210 Federal Street, Auckland, New Zealand"
                },
                new AlertModel
                {
                    Statuses = statuses,
                    Status = "UnAttended",
                    Severities = severities,
                    Severity = "Critical",
                    Contacts = contacts,
                    PrimaryContact = contacts.LastOrDefault(),
                    CareReceiver = "Bob White",
                    AlertType = "Gas",
                    AtHome = true,
                    HomeAddress = "171 Landscape Road, Mount Roskill, Auckland, New Zealand"
                }
            };
            return list;
            
        }

        #region DBHelpers

        private IEnumerable<contact> GetContacts()
        {
            using (var context = new NannyStateEntities())
            {
                return context.contacts.AsEnumerable();
            }
        }

        private IEnumerable<event_log> GetEvents()
        {
            using (var context = new NannyStateEntities())
            {
                var list =  context.event_logs.ToList();
                foreach (var item in list)
                {
                    item.Contacts = context.contacts.Where(c => c.care_receiver == item.care_receiver).ToList();
                    item.History = context.event_history.Where(c => c.event_id == item.id).ToList();
                }
                return list;
            }
        }
        

        #endregion
    }
}
