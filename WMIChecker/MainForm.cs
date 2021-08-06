namespace WMIChecker
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            this.InitializeComponent();

            this.wmiClassNameTextBox.Text = "Win32_OperatingSystem";
        }

        private void WmiClassNameTextBox_TextChanged(object sender, EventArgs e)
        {
            var className = this.wmiClassNameTextBox.Text;
            var scope = this.scopeTextBox.Text;

            try
            {
                var info = new CustomObjectType();
                var i = 0;

                // WMI取得処理
                Utils.GetWmiObj(scope, className, (obj) =>
                {
                    foreach (var property in obj.Properties)
                    {
                        info.Properties.Add(new CustomProperty()
                        {
                            Name = property.Name,
                            Type = typeof(String),
                            DefaultValue = property.Value?.ToString(),
                            Category = string.Format("{0}", i),
                            Desc = string.Format("Type: \"{0}\"", property.Type),
                        });
                    }

                    i++;
                });

                this.propertyGrid.SelectedObject = info;
            }
            catch (Exception)
            {
                this.propertyGrid.SelectedObject = new Object();
            }
        }
    }

    public static class Utils
    {
        public static void GetWmiObj(string scope, string classPath, Action<System.Management.ManagementBaseObject> action)
        {
            // WMI取得処理
            using (var objClass = new System.Management.ManagementClass(scope, classPath, new System.Management.ObjectGetOptions()))
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
