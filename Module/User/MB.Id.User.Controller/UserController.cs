using System.Web;
using System.Web.Mvc;
using MB.Common;
using MB.Common.Kendoui;
using MB.OMS.Account.Domain.Interface;
using MB.OMS.Account.Domain.Model;
using MB.OMS.Common.Domain;
using MB.OMS.Common.Repository;
using MB.Web.Core;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;


namespace MB.OMS.Account.Controller
{
    
    public class UserController : MBController
    {
        public CustomUserManager customUserManager { get; private set; }

         public UserController() : this(new CustomUserManager())
        {
        }

         public UserController(CustomUserManager cCustomUserManager)
        {
            customUserManager = cCustomUserManager;
        }

        [Dependency]
        public IUserRepository userRepository { get; set; }

        public ActionResult ListUsers()
        {
            
            return View(userRepository.GetAll());
        }

        [HttpPost]
        public JsonResult GetListUsers(DataSourceRequest dsRequest)
        {
            var data = userRepository.GetUserDatasource(dsRequest);
            return data.ToJsonDataSource();
        }

        [PermissionAuthorize(Permissions.AccessAbout,Permissions.ContactUs)]
        public ActionResult EditUser(string userId)
        {
            //var user = userRepository.GetUser(1);
            //ViewBag.UserRoles = StaticCommonRepository.GetStaticData(StaticDataKey.UserRole, "VIE");
            return View();
        }

        [HttpPost]
        public ActionResult EditUser(ApplicationUser user)
        {
            var result = userRepository.AddNewUser(user);
            if (result == 0)
                return Json(new JsonResponse { Data = result, Result = JsonMessage.Failed }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse { Result = JsonMessage.Success, Data = result }, JsonRequestBehavior.DenyGet);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await customUserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    if(user.Visible)
                    {
                        await SignInAsync(user, model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tài khoản hiện đang bị khóa, liên hệ admin để mở");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Dashboard", "Employee", new { area = "OMS" });
                //return RedirectToAction("ChooseProject", "Common", new { area = "OMS" });
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            ///Open Question- Hear it create claimIdentity. But we nothing add as such Claims but just User object.
            //public virtual Task<ClaimsIdentity> CreateIdentityAsync(TUser user, string authenticationType);

            var identity = await customUserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            //var identity = await UserManager1.CreateAsync(user);//, DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);

            //TODO: Load User Permission from database sp get permissions by user
            string csvString = "";
            var lisPermission = userRepository.GetAllPermissionForUserID(new Guid(user.Id));

            List<string> arrString = new List<string>();
            foreach(var item in lisPermission)
            {              
                arrString.Add(item.PermisionName);
                csvString = string.Join(",", arrString);
            }

            System.Web.HttpContext.Current.Session["UserPermissions"] = csvString;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "User");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }     
    }
}
