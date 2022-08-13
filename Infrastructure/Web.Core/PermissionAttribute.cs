using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace MB.Web.Core
{
     public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _rolesSplit = new string[0];
        private readonly bool _isUseForGenericAuthorize;

        public PermissionAuthorizeAttribute()
        {
        }

        public PermissionAuthorizeAttribute(params Permissions[] permissions)
        {
            Roles = string.Join(",", permissions.Select(p => p.ToString()));
            _rolesSplit = ConvertArrayEnumToString(permissions);
        }

        public PermissionAuthorizeAttribute(bool isUseForGenericAuthorize,params Permissions[] permissions)
        {
            Roles = string.Join(",", permissions.Select(p => p.ToString()));
            _rolesSplit = ConvertArrayEnumToString(permissions);
            _isUseForGenericAuthorize = isUseForGenericAuthorize;
        }

        private string[] ConvertArrayEnumToString(Permissions[] permissions)
        {
            var rolesSplit = new string[permissions.Length];
            for (int i = 0; i < permissions.Length; i++)
            {
                rolesSplit[i] = permissions[i].ToString();
            }
            return rolesSplit;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            IPrincipal user = httpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            if ((!_isUseForGenericAuthorize || (_isUseForGenericAuthorize && httpContext.Request.HttpMethod == "POST")) && (_rolesSplit.Length > 0 && !_rolesSplit.Any(IsUserInPermission)))
            {
                return false;
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Returns HTTP 401
            filterContext.Result = new HttpUnauthorizedResult();
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
            {
                // If a child action cache block is active, we need to fail immediately, 
                // even if authorization would have succeeded. The reason is there is no way 
                // to hook a callback to rerun authorization before the fragment is served from the cache,
                // so we can't guarantee that this filter will be re-run subsequent requests.

                throw new InvalidOperationException("Authorize Attribute cannot use within Child Action Cache");
            }

            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);

            if (skipAuthorization)
            {
                return;
            }

            if (AuthorizeCore(filterContext.HttpContext))
            {
                // Force the output cache to be authorized always if the action output cache is active
                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null);
            }
            else
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        // authorize the requests to web cache
        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            bool isAuthenticated = AuthorizeCore(httpContext);

            return isAuthenticated ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
        }

        private void CacheValidateHandler(HttpContext httpContext, object data, 
            ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(httpContext));
        }
        /*
        internal static string[] SplitSring(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
        */
        bool IsUserInPermission(string permission)
        {
            if (HttpContext.Current.Session["UserPermissions"] == null)
            {
                return false;
            }

            return ((string)HttpContext.Current.Session["UserPermissions"]).ToUpper().Contains(permission.ToUpper());
        }
    }

     public enum Permissions
     {
            AccessAbout,
            ContactUs,
            XemKhachHangCuaToi,
            SuaKhachHangCuaToi,
            XemDanhSachKhachHangMoi,
            ThemKhachHangMoi,
            XoaKhachHangMoi,
            SuaKhachHangMoi,
            XuatKhachHangMoi,
            XemDanhSachCauHoi,
            ThemCauHoi,
            XoaCauHoi,
            SuaCauHoi,
            XemDanhSachDapAn,
            ThemDapAn,
            XoaDapAn,
            SuaDapAn,
            XemDanhSachDuAn,
            ThemDuAn,
            XoaDuAn,
            SuaDuAn,
            XuatDuAn,
            XemDanhSachThuocTinhKH,
            ThemThuocTinhKH,
            XoaThuocTinhKH,
            SuaThuocTinhKH,
            XuatThuocTinhKH,
            XemDanhSachTrangThaiCuocGoi,
            ThemTrangThaiCuocGoi,
            XoaTrangThaiCuocGoi,
            SuaTrangThaiCuocGoi,
            XuatTrangThaiCuocGoi,
            XemDanhSachNhanVien,
            ThemNhanVien,
            XoaNhanVien,
            SuaNhanVien,
            XuatNhanVien,
            XemDanhSachQuyen,
            ThemQuyen,
            XoaQuyen,
            SuaQuyen,
            XuatQuyen,
            XemDanhSachNguon,
            ThemNguon,
            XoaNguon,
            SuaNguon,
            XuatNguon,
            XemDanhSachChienDich,
            ThemChienDich,
            XoaChienDich,
            SuaChienDich,
            XuatChienDich,
            ChiaKHChoDTV,
            ChuyenKHChoDTV,
            XemDanhSachKHTrung,
            XemDanhSachKHLoi,
            XemThongKeBaoCaoChiTiet,
            XuatThongKeBaoCaoChiTiet,
            XemThongKeNangSuat,
            XuatThongKeNangSuat,
            XemThongKeTheoDTV,
            XuatThongKeTheoDTV,
            XemThongKeTheoNguon,
            XuatThongKeTheoNguon,
            XemCuocGoiThanhCong,
            XemCuocGoiKhongThanhCong,
            XemCuocHenGoiLai,
            XemCuocGoiSaiSo,
            XemCuocGoiTiemNang,
            XuatThongKeOverview,
            XemThongKeTheoNgay,
            XuatThongKeTheoNgay
    }
}