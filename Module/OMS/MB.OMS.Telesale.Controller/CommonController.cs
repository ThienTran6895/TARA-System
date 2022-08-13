using MB.OMS.Telesale.Domain.Interface;
using MB.Web.Core;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MB.OMS.Telesale.Domain.Model;
using System.Web;
using MB.Common.Helpers;
using MB.Common;

namespace MB.OMS.Telesale.Controller
{
    public class CommonController : MBController
    {
        #region Feild
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CommonController");

        [Dependency]
        public ICallLogRepository callLogRepository { get; set; }

        [Dependency]
        public ICampaignsRepository campaignsRepository { get; set; }

        [Dependency]
        public IProjectsRepository projectsRepository { get; set; }

        [Dependency]
        public IUserRepository userRepository { get; set; }

        [Dependency]
        public IUserRoleRepository userRoleRepository { get; set; }

        [Dependency]
        public IRoleRepository roleRepository { get; set; }

        [Dependency]
        public IProjectUsersRepository projectUsersRepository { get; set; }

        #endregion

        public ActionResult ListReCall ()
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity.GetUserId()))
                    return null;
                var recall = callLogRepository.GetAllReCall(userID: new Guid(User.Identity.GetUserId()), projectID: CommonHelper.CurrentProject())
                    .Where(r => r.RecallDate != null && r.RecallDate.Value.Date <= DateTime.Now.Date && r.RecallDate.Value.Hour == DateTime.Now.Hour);

                return PartialView(recall);
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: ListReCall, Detail:" + ex);
                return null;
            }
        }

        public ActionResult DisplayProjectName()
        {
            var project = new Projects();
            project.Name = projectsRepository.GetProjectsById(CommonHelper.CurrentProject()).Name;
            return PartialView(project);
        }

        public ActionResult HeaderLinks()
        {  
            //var isAuthentiCated = User.Identity.IsAuthenticated;
            var user = userRepository.GetUserById(new Guid(User.Identity.GetUserId()));
            user.FullName = user.LastName + " " + user.FirstName;

            return PartialView(user);
        }

        public ActionResult DisplayInformation()
        {
            //var isAuthentiCated = User.Identity.IsAuthenticated;
            var user = userRepository.GetUserById(new Guid(User.Identity.GetUserId()));
            user.FullName = user.LastName + " " + user.FirstName;

            return PartialView(user);
        }

        public ActionResult ChooseProject()
        {
            var model = new ChooseProjectDTO();

            model.AvailableProject.Add(new SelectListItem()
            {
                Text = "--Chọn dự án--",
                Value = "0",
                Selected = true
            });
            var userRole = userRoleRepository.GetAll(userIds: new Guid(User.Identity.GetUserId())).FirstOrDefault();
            var role = roleRepository.GetRoleById(userRole.RoleId);
            if (role.RoleName == Role.Administrator.ToString())
            {
                model.AvailableProject.AddRange(projectsRepository.GetAll(visiable: true).Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.ProjectID.ToString()
                }));
            }
            else
            {
                var projectUser = projectUsersRepository.GetAll(usersId: new Guid(User.Identity.GetUserId()));
                var listProject = new List<Projects>();
                foreach(var item in projectUser)
                {
                    listProject.Add(projectsRepository.GetProjectsById(item.ProjectID));
                }
                model.AvailableProject.AddRange(listProject.Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.ProjectID.ToString()
                }));
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ChooseProject(ChooseProjectDTO model)
        {
            if(model.ProjectID > 0)
            {
                CookieStore.SetCookie("ProjectID", model.ProjectID.ToString());
                var userRole = userRoleRepository.GetAll(userIds: new Guid(User.Identity.GetUserId())).FirstOrDefault();
                var role = roleRepository.GetRoleById(userRole.RoleId);
                if (role.RoleName == Role.Administrator.ToString())
                    return RedirectToAction("Overview", "Report");
                else if(role.RoleName == Role.Manager.ToString())
                    return RedirectToAction("Overview", "Report");
                else if (role.RoleName == Role.User.ToString())
                    return RedirectToAction("ListCustomerInitial", "Customer");
                else
                    return RedirectToAction("ListCustomerInitial", "Customer");
            }
            else
            {
                model.AvailableProject.Add(new SelectListItem()
                {
                    Text = "--Chọn dự án--",
                    Value = "0",
                    Selected = true
                });
                model.AvailableProject.AddRange(projectsRepository.GetAll(visiable: true).Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.ProjectID.ToString()
                }));
                ModelState.AddModelError("", "Chưa chọn dự án!");
            }
            return View(model);       
        }

        public ActionResult ChangePassword()
        {
            var model = new ChangePassWord();
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassWord model)
        {
            if(string.Compare(model.NewPassWord, model.ReplayPassWord) != 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Mật khẩu nhập lại không giống với mật khẩu mới",
                    Title = "Đổi mật khẩu không thành công"
                }, JsonRequestBehavior.DenyGet);

            var user = userRepository.GetUserById(new Guid(User.Identity.GetUserId()));

            var oldSystemPasswordHasher = new OldSystemPasswordHasher();
            string newPassword = oldSystemPasswordHasher.HashPassword(model.NewPassWord);

            PasswordVerificationResult result = oldSystemPasswordHasher.VerifyHashedPassword(user.Password, model.OldPassWord);
            if (result == PasswordVerificationResult.Success)
            {
                user.Password = newPassword;
                userRepository.UpdateUser(user);
            }
            else
            {
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Mật khẩu không chính xác, vui lòng nhập lại!",
                    Title = "Đổi mật khẩu không thành công"
                }, JsonRequestBehavior.DenyGet);
            }
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Đổi mật khẩu thành công!",
                Title = "Đổi mật khẩu thành công",
                URLEdit = "/OMS/Common/ChangePassword"
            }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
