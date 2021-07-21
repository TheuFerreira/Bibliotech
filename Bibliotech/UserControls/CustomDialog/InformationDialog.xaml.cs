using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Bibliotech.UserControls.CustomDialog
{
    /// <summary>
    /// Interação lógica para InformationDialog.xam
    /// </summary>
    public partial class InformationDialog : Window
    {
        public InformationDialog(string title, string description, TypeDialog typeDialog)
        {
            InitializeComponent();

            tbTitle.Text = title;
            tbDescription.Text = description;

            switch (typeDialog)
            {
                case TypeDialog.Error:
                    img.Source = new BitmapImage(new Uri("pack://application:,,,/Bibliotech;component/Resources/img_error.png"));
                    break;
                case TypeDialog.Success:
                    img.Source = new BitmapImage(new Uri("pack://application:,,,/Bibliotech;component/Resources/img_ok.png"));
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
