using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MailBox.Properties;

namespace MailBox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (Settings.Default.isAuthenticated)
            {
              //  Settings.Default.isSaved = false;
             //   Settings.Default.Save();
                var cl = Client.GetInstance();
                cl.Email = Settings.Default.mail;
                cl.Password = Settings.Default.pass;
                cl.Host = Settings.Default.host;
                MainWindow mw = new MainWindow();
                mw.Show();
            }          
            else
            {
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
            }  
        }
    }
}
