using System.Web;
using System.Web.Optimization;

namespace Busilac
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
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/site-css").Include(
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/gentella_css").Include(
                      "~/vendors/bootstrap/dist/css/bootstrap.min.css",
                      "~/vendors/font-awesome/css/font-awesome.min.css",
                      "~/vendors/nprogress/nprogress.css",
                      "~/vendors/iCheck/skins/flat/green.css",
                      "~/vendors/bootstrap-progressbar/css/bootstrap-progressbar-3.3.4.min.css",
                      "~/vendors/jqvmap/dist/jqvmap.min.css",
                      "~/vendors/bootstrap-daterangepicker/daterangepicker.css",
                      "~/Content/morris.css",
                      "~/Content/custom.css",
                      "~/Content/custom-project.css"));

            bundles.Add(new ScriptBundle("~/bundles/gentella_js").Include(
                      "~/vendors/jquery/dist/jquery.min.js",
                      "~/vendors/bootstrap/dist/js/bootstrap.min.js",
                      "~/vendors/fastclick/lib/fastclick.js",
                      "~/vendors/nprogress/nprogress.js",
                      "~/vendors/Chart.js/dist/Chart.min.js",
                      "~/vendors/gauge.js/dist/gauge.min.js",
                      "~/vendors/bootstrap-progressbar/bootstrap-progressbar.min.js",
                      "~/vendors/iCheck/icheck.min.js",
                      "~/vendors/skycons/skycons.js",
                      "~/vendors/Flot/jquery.flot.js",
                      "~/vendors/Flot/jquery.flot.pie.js",
                      "~/vendors/Flot/jquery.flot.time.js",
                      "~/vendors/Flot/jquery.flot.stack.js",
                      "~/vendors/Flot/jquery.flot.resize.js",
                      "~/vendors/flot.orderbars/js/jquery.flot.orderBars.js",
                      "~/vendors/flot-spline/js/jquery.flot.spline.min.js",
                      "~/vendors/flot.curvedlines/curvedLines.js",
                      "~/vendors/DateJS/build/date.js",
                      "~/vendors/jqvmap/dist/jquery.vmap.js",
                      "~/vendors/jqvmap/dist/maps/jquery.vmap.world.js",
                      "~/vendors/jqvmap/examples/js/jquery.vmap.sampledata.js",
                      "~/vendors/moment/min/moment.min.js",
                      "~/vendors/bootstrap-daterangepicker/daterangepicker.js",
                       "~/Scripts/raphael.js",
                      "~/Scripts/morris.min.js",
                      "~/Scripts/custom.js"));
        }
    }
}
