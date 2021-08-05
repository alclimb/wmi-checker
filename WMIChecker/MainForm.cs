namespace WMIChecker
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            this.InitializeComponent();

            // WMI取得処理
            Utils.GetWmiObj("Win32_ComputerSystemProduct", (obj) =>
            {
                System.Diagnostics.Debug.WriteLine(obj.Properties["Name"].Value.ToString());
            });
        }

    }

    public static class Utils
    {
        public static void GetWmiObj(string classPath, Action<System.Management.ManagementBaseObject> action)
        {
            // WMI取得処理
            using (var objClass = new System.Management.ManagementClass(classPath))
            using (var objCollection = objClass.GetInstances())
            {
                foreach (var obj in objCollection)
                {
                    action(obj);

                    obj.Dispose();
                }
            }
        }
    }
}
