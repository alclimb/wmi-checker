namespace WMIChecker
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            this.InitializeComponent();

            this.wmiClassNameTextBox.Text = "Win32_OperatingSystem";
        }

        private async void WmiClassNameTextBox_TextChanged(object sender, EventArgs e)
        {
            // UI操作: プロパティグリッドを設定
            this.propertyGrid.SelectedObject = new Object();

            // UI操作: プロパティグリッドを無効化
            this.propertyGrid.Enabled = false;

            // UI操作: プログレスバーをマーキースタイルに変更
            this.toolStripProgressBar1.Style = ProgressBarStyle.Marquee;

            // UI操作: 情報取得
            var className = this.wmiClassNameTextBox.Text;
            var scope = this.scopeTextBox.Text;

            try
            {
                var info = new CustomObjectType();

                await Task.Run(() =>
                {
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
                });

                // UI操作: プロパティグリッドを設定
                this.propertyGrid.SelectedObject = info;
            }
            catch (Exception)
            {
            }
            finally
            {
                // UI操作: プログレスバーを元に戻す
                this.toolStripProgressBar1.Style = ProgressBarStyle.Continuous;

                // UI操作: プロパティグリッドを有効化
                this.propertyGrid.Enabled = true;
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
