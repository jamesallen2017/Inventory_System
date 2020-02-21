using System.Web;
using System.Web.Optimization;

namespace GMCINVENTORYSYSTEM
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/assets/plugins/lib/jquery-2.2.4.min.js",
                        "~/assets/plugins/lib/jquery-ui.min.js",
                        "~/assets/plugins/bootstrap/bootstrap.min.js",
                        "~/assets/plugins/lib/plugins.js",
                        "~/Scripts/jquery.signalR-2.4.1.min.js",
                        "~/Scripts/jstree.min.js",
                        "~/Scripts/bootbox.all.min.js",
                        "~/Scripts/bootbox.locales.min.js",
                        "~/Scripts/bootbox.min.js",
                        "~/assets/plugins/date-picker/js/bootstrap-datepicker.min.js",
                        "~/assets/plugins/dateTime-picker/js/bootstrap-datetimepicker.min.js",
                        //"~/assets/plugins/fullcalendar/moment.min.js",
                        "~/assets/plugins/toast/jquery.toast.min.js",
                        "~/assets/plugins/select2/js/select2.min.js",
                        "~/assets/plugins/jasny-bootstrap/js/jasny-bootstrap.min.js",
                        //"~/assets/plugins/libs/moment.min.js",
                        "~/assets/plugins/datatable/jquery.dataTables.min.js",
                        "~/assets/plugins/datatable/dataTables.bootstrap.min.js",
                        //"~/assets/plugins/libs/datetime-moment.js",
                        "~/assets/plugins/datatable/dataTables.select.min.js",
                        "~/assets/plugins/flot/excanvas.min.js",
                        "~/assets/plugins/flot/jquery.flot.min.js",
                        "~/assets/plugins/flot/jquery.flot.tooltip.js",
                        "~/assets/plugins/flot/jquery.flot.resize.min.js",
                        "~/assets/plugins/flot/jquery.flot.crosshair.min.js",
                        "~/assets/plugins/flot/jquery.flot.pie.min.js",
                        "~/assets/plugins/lib/sparklines.js",
                        "~/assets/plugins/lib/jquery.knob.min.js",
                        "~/assets/plugins/monthly/js/monthly.js",
                        "~/assets/plugins/emojionearea/emojionearea.min.js",
                        "~/assets/plugins/validationEngine/jquery.validationEngine.js",
                        "~/assets/plugins/validationEngine/languages/jquery.validationEngine-en.js",
                        "~/assets/plugins/validation/jquery.validate.min.js",
                        "~/assets/js/app.base.js",
                        "~/assets/js/cmp-todo.js",
                        "~/assets/js/page-table.js",
                        "~/assets/js/page-validation.js",
                        "~/assets/js/cmp-alerts.js"
                        ));

            bundles.Add(new ScriptBundle("~/loginbundles/jquery").Include(
                "~/assets/plugins/lib/modernizr.js",
                "~/assets/plugins/lib/jquery-2.2.4.min.js",
                "~/assets/plugins/lib/jquery-ui.min.js",
                "~/assets/plugins/bootstrap/bootstrap.min.js",
                "~/ssets/plugins/lib/plugins.js",
                "~/assets/plugins/toast/jquery.toast.min.js",
                "~/assets/plugins/jasny-bootstrap/js/jasny-bootstrap.min.js",
                "~/assets/plugins/validationEngine/jquery.validationEngine.js",
                "~/assets/plugins/validationEngine/languages/jquery.validationEngine-en.js",
                "~/assets/plugins/validation/jquery.validate.min.js",
                "~/assets/plugins/validation/additional-methods.min.js",
                "~/assets/js/page-validation.js",
                "~/assets/js/cmp-alerts.js"

                ));
            bundles.Add(new StyleBundle("~/loginbundles/Content").Include(
                "~/assets/plugins/bootstrap/bootstrap.css",
                "~/assets/plugins/validationEngine/validationEngine.jquery.css",
                "~/assets/plugins/toast/jquery.toast.min.css",
                "~/assets/plugins/jasny-bootstrap/css/jasny-bootstrap.min.css",
                "~/assets/css/bs-overide/bootstrap.buttons.css",
                      "~/assets/css/bs-overide/bootstrap.formcontrol.css",
                      "~/assets/css/bs-overide/bootstrap.wizard.css",
                      "~/assets/css/bs-overide/temp.css",
                "~/assets/css/main.css",
                "~/assets/css/style-default.css"
                ));


            bundles.Add(new ScriptBundle("~/ContentJS/Modernizr").Include(
            "~/assets/plugins/lib/modernizr.js"
            ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/assets/plugins/bootstrap/bootstrap.css",
                      "~/assets/plugins/date-picker/css/bootstrap-datepicker3.min.css",
                      "~/assets/plugins/dateTime-picker/css/bootstrap-datetimepicker.min.css",
                      "~/assets/plugins/monthly/css/monthly.css",
                      "~/assets/plugins/datatable/dataTables.bootstrap.min.css",
                      "~/assets/plugins/datatable/dataTables.select.min.css",
                      "~/assets/plugins/validationEngine/validationEngine.jquery.css",
                      "~/assets/plugins/toast/jquery.toast.min.css",
                      "~/assets/plugins/select2/css/select2.min.css",
                      "~/assets/plugins/jasny-bootstrap/css/jasny-bootstrap.min.css",
                      "~/assets/css/bs-overide/bootstrap.buttons.css",
                      "~/assets/css/bs-overide/bootstrap.formcontrol.css",
                      "~/assets/css/bs-overide/bootstrap.wizard.css",
                      "~/assets/css/bs-overide/temp.css",
                      "~/Content/themes/default/style.min.css",
                      "~/assets/css/main.css",
                      "~/assets/css/style-default.css"));
        }
    }
}
