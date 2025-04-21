using System;
using System.IO;
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
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "config.dll";
            if (File.Exists(filePath)){

                byte[] bytes = File.ReadAllBytes(filePath);
                double v = BitConverter.ToDouble(bytes,0);
                DateTime t = DateTime.FromOADate(v);
                if (t < DateTime.Now) {
                    MessageBox.Show("程序已过期");
                    return;
                }
            }
            else{
               double value =  DateTime.Now.AddDays(5).ToOADate();
                byte[] bytes = BitConverter.GetBytes(value);
                ulong bits = BitConverter.ToUInt64(bytes, 0);
                string a= bits.ToString("X16");
                File.WriteAllBytes(filePath,bytes);
            }
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
