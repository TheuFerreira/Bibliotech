using System.Windows;

namespace Bibliotech.UserControls.CustomDialog
{
    /// <summary>
    /// Interação lógica para TextFieldDialog.xam
    /// </summary>
    public partial class TextFieldDialog : Window
    {
        public TypeTextFieldDialog TypeTextFieldDialog { get; set; }
        public string Text { get; set; }
        public TextFieldDialog(string textFieldTitle, string description, TypeTextFieldDialog typeTextFieldDialog)
        {
            InitializeComponent();

            tbPassword.Title = textFieldTitle;
            tbTitle.Text = description;
            TypeTextFieldDialog = typeTextFieldDialog;
        }

        public void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            Text = tbPassword.Text;

            DialogResult = true;
            Close();
        }
    }
}
