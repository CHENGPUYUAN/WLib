/*---------------------------------------------------------------- 
// auth�� ArcGIS.com
// date�� None
// desc�� ArcGIS License Initializer
// mdfy:  Windragon
//----------------------------------------------------------------*/

using System;
using System.Windows.Forms;
using ESRI.ArcGIS;

namespace WLib.ArcGis
{
    /** LicenseInitializerʹ��˵����
     * private static LicenseInitializer licenseInitializer = new LicenseInitializer();
     *   //Main Method��
     *   licenseInitializer.InitializeApplication(
     *   new[]
     *   {
     *       // �˴���Advanced��Ҫ�ٰ�����Ȩ�ޣ� �����esriLicenseProductCodeEngineȨ����ЩGP�����޷�ʹ��
     *       // ArcGIS10.1���ϰ汾ΪesriLicenseProductCodeAdvanced��ArcGIS10.0Ӧ��ΪesriLicenseProductCodeDesktop
     *       esriLicenseProductCode.esriLicenseProductCodeAdvanced
     *   },
     *   new[]
     *   {
     *       esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst,
     *       esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork
     *   });
     *   Application.Run(new MainForm());
     *   licenseInitializer.ShutdownApplication();
     */

    /// <summary>
    /// ��ʼ��ARCGIS���л�����ɣ�
    /// <para>
    /// private static LicenseInitializer licenseInitializer = new LicenseInitializer();
    /// 
    /// //Main Method��
    /// licenseInitializer.InitializeApplication(
    ///     new[]
    ///     {
    ///         /* �˴���Advanced/Desktop��Ҫ�ٰ�����Ȩ�ޣ�����EngineȨ����ЩGP�����޷�ʹ�� */
    ///         esriLicenseProductCode.esriLicenseProductCodeAdvanced
    ///     },
    ///     new[]
    ///     {
    ///         esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst,
    ///         esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork
    ///     });
    /// </para>
    /// </summary>
    public partial class LicenseInitializer
    {
        /// <summary>
        /// ��ʼ��ARCGIS���
        /// </summary>
        public LicenseInitializer()
        {
            ResolveBindingEvent += BindingArcGISRuntime;
        }

        public void InitializeApplication(object[] p1, object[] p2)
        {
            throw new NotImplementedException();
        }

        private void BindingArcGISRuntime(object sender, EventArgs e)
        {
            ProductCode[] supportedRuntimes = { ProductCode.Engine, ProductCode.Desktop };
            foreach (ProductCode productCode in supportedRuntimes)
            {
                if (RuntimeManager.Bind(productCode))
                    return;
            }
            MessageBox.Show("ArcGIS�����ɴ���", "��ʾ", MessageBoxButtons.OK);
            Environment.Exit(0);
        }
    }
}