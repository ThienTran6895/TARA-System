using System.Web;
using System.Web.Optimization;

namespace OMSTeleSale
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //bundles.Add(new StyleBundle("~/Content/css/adminlte").Include(
            //            "~/Content/AdminLTE/bootstrap/css/bootstrap.min.css",
            //            "~/Content/AdminLTE/css/AdminLTE.min.css",
            //            "~/Content/AdminLTE/css/skins/_all-skins.min.css",
            //            "~/Content/AdminLTE/plugins/iCheck/flat/blue.css",
            //            "~/Content/AdminLTE/plugins/morris/morris.css",
            //            "~/Content/AdminLTE/plugins/jvectormap/jquery-jvectormap-1.2.2.css",
            //            "~/Content/AdminLTE/plugins/datepicker/datepicker3.css",
            //            "~/Content/AdminLTE/plugins/datetimepicker/bootstrap-datetimepicker.min.css",
            //            "~/Content/AdminLTE/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css",
            //            "~/Content/AdminLTE/plugins/select2/select2.css",
            //            "~/Content/AdminLTE/plugins/iCheck/all.css",
            //            "~/Styles/sweetalert.css",
            //            "~/Content/Site.css",
            //            "~/Scripts/uploadify/uploadify.css"));

            //bundles.Add(new ScriptBundle("~/bundles/js/adminlte").Include(
            //           "~/Content/AdminLTE/plugins/jQuery/jquery-2.2.3.min.js",
            //           "~/Content/AdminLTE/plugins/jQuery/jquery-migrate-1.2.1.js",
            //           "~/Content/AdminLTE/plugins/jQueryUI/jquery-ui.min.js",
            //           "~/Content/AdminLTE/bootstrap/js/bootstrap.min.js",
            //           "~/Content/AdminLTE/js/app.min.js",
            //           "~/Content/AdminLTE/plugins/morris/morris.min.js",
            //           "~/Content/AdminLTE/plugins/sparkline/jquery.sparkline.min.js",
            //           "~/Content/AdminLTE/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
            //           "~/Content/AdminLTE/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
            //           "~/Content/AdminLTE/plugins/daterangepicker/moment.min.js",
            //           "~/Content/AdminLTE/plugins/datepicker/bootstrap-datepicker.js",
            //           "~/Content/AdminLTE/plugins/datetimepicker/bootstrap-datetimepicker.min.js",
            //           "~/Content/AdminLTE/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
            //           "~/Content/AdminLTE/plugins/slimScroll/jquery.slimscroll.min.js",
            //           "~/Content/AdminLTE/plugins/fastclick/fastclick.min.js",
            //           "~/Content/AdminLTE/plugins/select2/select2.full.min.js",
            //           "~/Content/AdminLTE/plugins/iCheck/icheck.js"
            //           ));

            //bundles.Add(new ScriptBundle("~/bundles/js/common").Include(
            //    "~/Scripts/Common/common.js",
            //    "~/Scripts/sweetalert.min.js",
            //    "~/Scripts/uploadify/jquery.uploadify.min.js"
            //    ));

            //bundles.Add(new ScriptBundle("~/bundles/js/kendoui").Include(
            //    "~/Content/kendoui/js/kendo.all.min.js",
            //    "~/Content/kendoui/js/kendo.aspnetmvc.min.js",
            //    "~/Content/kendoui/js/kendo.timezones.min.js"
            //    ));

            //bundles.Add(new ScriptBundle("~/bundles/css/kendoui").Include(
            //    "~/Content/kendoui/styles/kendo.common-material.min.css",
            //    "~/Content/kendoui/styles/kendo.rtl.min.css",
            //    "~/Content/kendoui/styles/kendo.material.min.css",
            //    "~/Content/kendoui/styles/kendo.material.mobile.min.css"
            //    ));
        }
    }
}
