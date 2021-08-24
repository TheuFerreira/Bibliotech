using Bibliotech.UserControls.CustomEnums;
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

        public TextFieldDialog(string textFieldTitle, string description, TypeTextFieldDialog typeTextFieldDialog, FieldType fieldType, string placeholder)
        {
            InitializeComponent();

            tbField.FieldType = fieldType;
            tbField.Title = textFieldTitle;
            tbField.Placeholder = placeholder;
            tbTitle.Text = description;
            TypeTextFieldDialog = typeTextFieldDialog;
            tfPassword.Title = textFieldTitle;
            tfPassword.Placeholder = placeholder;

            tfPassword.Visibility = typeTextFieldDialog == TypeTextFieldDialog.Password ? Visibility.Visible : Visibility.Collapsed;
            tbField.Visibility = typeTextFieldDialog != TypeTextFieldDialog.Password ? Visibility.Visible : Visibility.Collapsed;
        }

        public void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            switch (TypeTextFieldDialog)
            {
                case TypeTextFieldDialog.Add:
                    Text = tbField.Text;
                    break;
                case TypeTextFieldDialog.Password:
                    Text = tfPassword.Text;
                    break;
                case TypeTextFieldDialog.Save:
                    Text = tbField.Text;
                    break;
                default:
                    break;
            }


            DialogResult = true;
            Close();
        }
    }
}
