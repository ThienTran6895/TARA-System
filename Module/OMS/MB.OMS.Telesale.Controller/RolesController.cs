using MB.Common;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using MB.Web.Core;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MB.Common.Helpers;
using MB.Common.Kendoui;

namespace MB.OMS.Telesale.Controller
{
    public class RolesController : MBController
    {
        #region Feild

        [Dependency]
        public IRoleRepository roleRepository { get; set; }

        [Dependency]
        public IPermisionsRepository permisionsRepository { get; set; }

        [Dependency]
        public IRolePermisionRepository rolePermisionRepository { get; set; }

        #endregion

        #region List

        public ActionResult ListRole()
        {
            if (!CommonHelper.CheckPermisionExist(MB.Web.Core.Permissions.XemDanhSachQuyen.ToString()))
                return AccessDeniedView();

            var role = new Roles();
            return View(role);
        }

        [HttpPost]
        public JsonResult GetListRole(DataSourceRequest dsRequest)
        {
            var data = roleRepository.GetRoleDatasource(dsRequest: dsRequest);
            return data.ToJsonDataSource();
        }

        #endregion

        #region Add Role

        public ActionResult AddRole()
        {
            if (!CommonHelper.CheckPermisionExist(MB.Web.Core.Permissions.ThemQuyen.ToString()))
                return AccessDeniedView();

            var model = new Roles();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddRole(Roles model)
        {
            var result = roleRepository.AddNewRole(model);
            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Thêm mới quyền không thành công!",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Thêm mới quyền thành công!",
                Title = "Thêm mới thành công",
                URLList = "/OMS/Roles/ListRole",
                Id = result.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region Edit Role

        public ActionResult EditRole(int id)
        {
            if (!CommonHelper.CheckPermisionExist(MB.Web.Core.Permissions.SuaQuyen.ToString()))
                return AccessDeniedView();

            var role = roleRepository.GetRoleById(id);
            if (role == null)
                return RedirectToAction("ListRole");

            return View(role);
        }

        [HttpPost]
        public ActionResult EditRole(Roles model)
        {
            var role = roleRepository.GetRoleById(model.Id);
            if (role == null)
                return RedirectToAction("ListRole");

            var result = roleRepository.UpdateRole(model);
            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Cập nhật quyền không thành công!",
                    Title = "Cập nhật không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Cập nhật quyền thành công!",
                Title = "Cập nhật thành công",
                URLEdit = "/OMS/Roles/EditRole/" + model.Id,
                Id = model.Id.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion       

        #region Delete Role

        [HttpPost]
        public ActionResult DeleteRole(int id)
        {
            var role = roleRepository.GetRoleById(id);
            if (role == null)
                return RedirectToAction("ListRole");

            var result = roleRepository.DeleteRole(id);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Xóa quyền không thành công!",
                    Title = "Xóa không thành công"
                }, JsonRequestBehavior.DenyGet);

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa quyền thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Roles/ListRole"
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region Setting Permisions

        public ActionResult Permissions(int id)
        {
            var role = roleRepository.GetRoleById(id);
            var permissionRecords = permisionsRepository.GetAllPermisionsInRolesByParent(roleID: id).ToList();
            var rolePermision = rolePermisionRepository.GetAll(roleId: id);
            var listPermisionInRole = new List<Permisions>();
            foreach(var item in rolePermision)
            {
                listPermisionInRole.Add(permisionsRepository.GetPermisionsById(item.PermisionId));
            }

            foreach(var item in permissionRecords)
            {
                item.AvailablePermisions = listPermisionInRole.Where(i => i.Parent == item.Id).ToList();
            }           

            role.AvailablePermision = permissionRecords;

            List<string> listAction = new List<string>(new string[] { "Thêm", "Xóa", "Sửa", "Xuất" });
            for (int i = 0; i < listAction.Count; i++ )
            {
                role.AvailableAction.Add(new Roles
                {
                    Id = i + 1,
                    RoleName = listAction[i]
                });
            }
            role.Id = id;
            return View(role);
        }

        [HttpPost]
        public ActionResult PermissionsSave(FormCollection form)
        {
            var roleId = Convert.ToInt32(form["Id"]);

            List<string> listAction = new List<string>(new string[] { "Thêm", "Xóa", "Sửa", "Xuất" });
            var listRoles = new List<Roles>();
            for (int i = 0; i < listAction.Count; i++)
            {
                listRoles.Add(new Roles
                {
                    Id = i + 1,
                    RoleName = listAction[i]
                });
            }
            var permissionRecords = permisionsRepository.GetAllPermisionsInRolesByParent(roleID: roleId).ToList();

            foreach (var cr in listRoles)
            {
                string formKey = "allow_" + cr.Id;
                var permissionRecordSystemNamesToRestrict = form[formKey] != null ? form[formKey].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

                foreach (var pr in permissionRecords)
                {

                    bool allow = permissionRecordSystemNamesToRestrict.Contains(pr.PermisionName);
                    var permission = permisionsRepository.GetPermisionsByParent(parent: pr.Id).Where(p => p.ActionType == cr.Id).FirstOrDefault();
                    if(permission != null)
                    {
                        var rolePermission = rolePermisionRepository.GetAll(roleId: roleId, permisionId: permission.Id).FirstOrDefault();
                        if(allow)
                        {
                            if(rolePermission == null)
                            {
                                var data = new RolePermision();
                                data.RoleId = roleId;
                                data.PermisionId = permission.Id;
                                rolePermisionRepository.AddNewRolePermision(data);
                            }
                        }
                        else
                        {
                            if(rolePermission != null)
                            {
                                rolePermisionRepository.DeleteRolePermision(roleId: roleId, permisionId: permission.Id);
                            }
                        }
                    }                  
                }
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Sharing Page for Role

        public ActionResult SharePageForRole(int id)
        {
            var role = roleRepository.GetRoleById(id);
            return View(role);
        }

        [HttpPost]
        public JsonResult GetListPermisionsInRoles(DataSourceRequest dsRequest, RoleDTO role)
        {
            var data = permisionsRepository.GetAllPermisionsInRolesByParentDatasource(dsRequest: dsRequest, roleID: role.Id, isVisible: true);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public JsonResult GetListPermisionsNotInRoles(DataSourceRequest dsRequest, RoleDTO role)
        {
            var data = permisionsRepository.GetAllPermisionsNotInRolesByParentDatasource(dsRequest: dsRequest, roleID: role.Id, isVisible: true);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public ActionResult SharePermisionSelected(ICollection<int> selectedIds, int roleId)
        {
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    var rolePermision = new RolePermision();
                    rolePermision.PermisionId = itemt;
                    rolePermision.RoleId = roleId;
                    rolePermisionRepository.AddNewRolePermision(rolePermision);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult DestroyPermisionSelected(ICollection<int> selectedIds, int roleId)
        {
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    rolePermisionRepository.DeleteRolePermision(roleId: roleId, permisionId: itemt);
                    var listPermision = permisionsRepository.GetPermisionsByParent(itemt);
                    foreach(var delete in listPermision)
                    {
                        rolePermisionRepository.DeleteRolePermision(roleId: roleId, permisionId: delete.Id);
                    }
                }
            }

            return Json(new { Result = true });
        }
        #endregion
    }
}
