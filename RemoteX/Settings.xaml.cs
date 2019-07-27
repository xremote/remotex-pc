using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Reflection;

namespace RemoteX
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : ModernWindow
    {
        public Settings()
        {
            InitializeComponent();
            password_box.Text = Properties.Settings.Default.Password;
            checkBox.IsChecked = Properties.Settings.Default.isstartup;
            base.Closing += this.settings_window_Closing;
        }

        private void settings_window_Closing(object sender, CancelEventArgs e)
        {
                
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Password = password_box.Text;
            Properties.Settings.Default.isstartup = (bool)checkBox.IsChecked;

            // set registry for startup

            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string app_loc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            app_loc = app_loc + "\\RemoteX.exe";
            Debug.WriteLine(app_loc);
            if ((bool)checkBox.IsChecked)
            {
                key.SetValue("Run", app_loc);
            }
            else
            {
                key.SetValue("Run", "");
            }

            Properties.Settings.Default.Save();
        }
    }
}

   

