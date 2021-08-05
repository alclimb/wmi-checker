﻿namespace WMIChecker
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

            try
            {
                var info = new CustomObjectType();

                // WMI取得処理
                Utils.GetWmiObj(className, (obj) =>
                {
                    foreach (var property in obj.Properties)
                    {
                        info.Properties.Add(new CustomProperty { Name = property.Name, Type = typeof(String), DefaultValue = property.Value?.ToString() });
                    }
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
