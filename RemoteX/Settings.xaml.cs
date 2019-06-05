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
            Debug.WriteLine(checkBox.IsChecked);
            Properties.Settings.Default.settingwindows = 1;
            base.Closing += this.settings_window_Closing;
        }

        private void settings_window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.settingwindows = 0;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Password = password_box.Text;
            Properties.Settings.Default.isstartup = (bool)checkBox.IsChecked;
            Properties.Settings.Default.Save();
        }
    }
}

   

