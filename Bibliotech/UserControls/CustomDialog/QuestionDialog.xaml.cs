using System.Windows;

namespace Bibliotech.UserControls.CustomDialog
{
    /// <summary>
    /// Interação lógica para QuestionDialog.xam
    /// </summary>
    public partial class QuestionDialog : Window
    {
        public QuestionDialog(string title, string description)
        {
            InitializeComponent();

            tbTitle.Text = title;
            tbDescription.Text = description;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
