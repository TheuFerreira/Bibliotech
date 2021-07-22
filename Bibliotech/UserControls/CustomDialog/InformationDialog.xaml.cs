using System.Windows;

namespace Bibliotech.UserControls.CustomDialog
{
    /// <summary>
    /// Interação lógica para InformationDialog.xam
    /// </summary>
    public partial class InformationDialog : Window
    {
        public TypeDialog TypeDialog { get; set; }

        public InformationDialog(string title, string description, TypeDialog typeDialog)
        {
            InitializeComponent();

            tbTitle.Text = title;
            tbDescription.Text = description;
            TypeDialog = typeDialog;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
