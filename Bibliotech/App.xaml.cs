using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Bibliotech.View.Schools;
using Bibliotech.View.Users;
using Bibliotech.View.Books;
using Bibliotech.View;
using Bibliotech.View.Lectors;

namespace Bibliotech
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainWindow();
            MainWindow.ShowDialog();
        }
    }
}
