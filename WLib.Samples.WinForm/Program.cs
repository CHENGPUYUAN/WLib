using System;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using WLib.ArcGis;

namespace WLib.Samples.WinForm
{
    static class Program
    {
        private static bool Login = false;
        private static readonly LicenseInitializer licenseInitializer = new LicenseInitializer();
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            licenseInitializer.InitializeApplication(
                new[] {
                    esriLicenseProductCode.esriLicenseProductCodeAdvanced
                },
                new[] {
                    esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork,
                    esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst,
                });

            //ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            LoginForm loginForm = new LoginForm();
            loginForm.TopMost = true;

            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new Main());
            }
            licenseInitializer.ShutdownApplication();
        }
    }
}
