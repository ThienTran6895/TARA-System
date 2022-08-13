using MB.Common;
using MB.Common.Helpers;
using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using MB.Web.Core;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Controller
{
    public class UserController : MBController
    {
        #region Fields

        [Dependency]
        public IUserRepository userRepository { get; set; }

        [Dependency]
        public ICampaignsRepository campaignsRepository { get; set; }

        [Dependency]
        public IProjectsRepository projectsRepository { get; set; }

        [Dependency]
        public IProjectUsersRepository projectUserRepository { get; set; }

        [Dependency]
        public IRoleRepository roleRepository { get; set; }

        [Dependency]
        public IUserRoleRepository userRoleRepository { get; set; }

        #endregion

        #region List User

        public ActionResult ListUser()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemDanhSachNhanVien.ToString()))
                return AccessDeniedView();

            var user = new UserDTO();

            return View(user);
        }

        [HttpPost]
        public JsonResult GetListUser(DataSourceRequest dsRequest, MB.Common.Kendoui.Filter filter = null)
        {
            string userName = null;
            string fullName = null;
            string email = null;
            string phone = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("UserName")).Any())
                    userName = ((string[])arrayFilter.Where(f => f.Field == "UserName").FirstOrDefault().Value)[0];

                if (arrayFilter.Where(f => f.Field.Contains("FullName")).Any())
                    fullName = ((string[])arrayFilter.Where(f => f.Field == "FullName").FirstOrDefault().Value)[0];

                if (arrayFilter.Where(f => f.Field.Contains("Email")).Any())
                    email = ((string[])arrayFilter.Where(f => f.Field == "Email").FirstOrDefault().Value)[0];

                if (arrayFilter.Where(f => f.Field.Contains("Phone")).Any())
                    phone = ((string[])arrayFilter.Where(f => f.Field == "Phone").FirstOrDefault().Value)[0];
            }
            var data = userRepository.GetUserDatasource(dsRequest: dsRequest, userName: userName, fullName: fullName, email: email, phone: phone);
            return data.ToJsonDataSource();
        }

        #endregion      

        #region Add User
        public ActionResult AddUser()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.ThemNhanVien.ToString()))
                return AccessDeniedView();

            var model = new UserDTO();
            model.AvailableGenders.Add(new SelectListItem()
            {
                Text = "Nam",
                Value = "0",
                Selected = true
            });
            model.AvailableGenders.Add(new SelectListItem()
            {
                Text = "Nữ",
                Value = "1"
            });

            model.AvailableRoles = roleRepository.GetAll(isVisible: true).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult AddUser(UserDTO model)
        {
            if (model.SelectedRoleIds == null)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Role không được để trống",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);

            var allRoles = roleRepository.GetAll(isVisible: true);
            var newRoles = new List<Roles>();
            foreach (var role in allRoles)
                if (model.SelectedRoleIds != null && model.SelectedRoleIds.Contains(role.Id))
                    newRoles.Add(role);
            var oldSystemPasswordHasher = new OldSystemPasswordHasher();
            model.Password = oldSystemPasswordHasher.HashPassword(model.Password);
            model.IsDelete = false;
            var check = userRepository.CheckUserByUsername(model.UserName);

            var result = Guid.Empty;
            if (check == null || check.Count() == 0)
            {
                result = userRepository.AddNewUser(model);
            }
            else  // User name alreader exists
            {
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Username đã có trong hệ thống!",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);
            }

            if (result != Guid.Empty && newRoles != null && newRoles.Count > 0)
            {
                foreach (var role in newRoles)
                {
                    var userRole = new UserRole();
                    userRole.UserId = result;
                    userRole.RoleId = role.Id;
                    userRoleRepository.AddNewUserRole(userRole);
                }
            }

            if (result == Guid.Empty)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Thêm mới nhân viên không thành công!",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Thêm mới nhân viên thành công!",
                Title = "Thêm mới thành công",
                URLList = "/OMS/User/ListUser",
                Id = result.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region Edit User

        public ActionResult EditUser(Guid id)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.SuaNhanVien.ToString()))
                return AccessDeniedView();

            var user = userRepository.GetUserById(id);
            if (user == null)
                return RedirectToAction("ListUser");
            if (user.BirthDay != null)
                user.BirthDay = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(user.BirthDay));
            user.AvailableGenders.Add(new SelectListItem()
            {
                Text = "Nam",
                Value = "0",
                Selected = true
            });
            user.AvailableGenders.Add(new SelectListItem()
            {
                Text = "Nữ",
                Value = "1"
            });

            user.SelectedRoleIds = userRoleRepository.GetAll(userIds: id).Select(cr => cr.RoleId).ToArray();
            user.AvailableRoles = roleRepository.GetAll(isVisible: true).ToList();

            user.SelectedProjectIds = userRepository.GetProjectByUser(usersId: id).Select(cr => cr.ProjectID).ToArray();
            user.AvailableProjects = projectsRepository.GetAll().ToList();

            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(UserDTO user)
        {
            if (user.SelectedRoleIds == null)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Role không được để trống",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);

            var model = userRepository.GetUserById(user.Id);
            if (model == null)
                return RedirectToAction("ListUser");
            var result = userRepository.UpdateUser(user);


            // Update roles
            var allRoles = roleRepository.GetAll(isVisible: true);
            foreach (var role in allRoles)
            {
                var userRole = userRoleRepository.GetAll(userIds: user.Id).Count(cr => cr.RoleId == role.Id);
                if (user.SelectedRoleIds != null && user.SelectedRoleIds.Contains(role.Id))
                {
                    if (userRole == 0)
                    {
                        var data = new UserRole();
                        data.UserId = user.Id;
                        data.RoleId = role.Id;
                        userRoleRepository.AddNewUserRole(data);
                    }
                }
                else
                {
                    if (userRole > 0)
                    {
                        userRoleRepository.DeleteUserRole(userIds: user.Id, roleId: role.Id);
                    }
                }
            }

            // Update projects
            var allProjects = projectsRepository.GetAll();
            foreach (var proj in allProjects)
            {
                var userProj = userRepository.GetProjectByUser(usersId: user.Id).Count(cr => cr.ProjectID == proj.ProjectID);
                if (user.SelectedProjectIds != null && user.SelectedProjectIds.Contains(proj.ProjectID))
                {
                    if (userProj == 0)
                    {
                        var data = new ProjectUsers();
                        data.UserId = user.Id;
                        data.ProjectID = proj.ProjectID;
                        projectUserRepository.AddNewProjectUsers(data);
                    }
                }
                else
                {
                    if (userProj > 0)
                    {
                        projectUserRepository.DeleteProjectUsers(projectID: proj.ProjectID, usersId: user.Id);
                    }
                }
            }
            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Cập nhật nhân viên không thành công!",
                    Title = "Cập nhật không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Cập nhật nhân viên thành công!",
                Title = "Cập nhật thành công",
                URLEdit = "/OMS/User/EditUser/" + user.Id,
                Id = user.Id.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion        

        #region Delete User

        [HttpPost]
        public ActionResult DeleteUser(Guid id)
        {
            var user = userRepository.GetUserById(id);
            if (user == null)
                return RedirectToAction("ListUser");

            var result = userRepository.DeleteUser(id);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Xóa nhân viên không thành công!",
                    Title = "Xóa không thành công"
                }, JsonRequestBehavior.DenyGet);

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa nhân viên thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/User/ListUser"
            }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<Guid> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    userRepository.DeleteUser(item);
                }
            }
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa nhân viên thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/User/ListUser"
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region ResetPassword
        public ActionResult Resetpassword(Guid id)
        {
            var model = userRepository.GetUserById(id);
            if (model == null)
                return RedirectToAction("ListUser");
            else
            {
                var oldSystemPasswordHasher = new OldSystemPasswordHasher();
                model.Password = oldSystemPasswordHasher.HashPassword("123456");
                var result = userRepository.UpdateUser(model);
                return RedirectToAction("ListUser");
            }

        }

        #endregion
    }
}
