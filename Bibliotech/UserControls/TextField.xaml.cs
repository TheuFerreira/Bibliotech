using Bibliotech.UserControls.CustomEnums;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bibliotech.UserControls
{
    /// <summary>
    /// Interação lógica para TextField.xam
    /// </summary>
    public partial class TextField : UserControl
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(TextField), new PropertyMetadata("Placeholder"));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TextField), new PropertyMetadata("Title"));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextField), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(TextField), new PropertyMetadata(0));
        public static readonly DependencyProperty FieldTypeProperty = DependencyProperty.Register("FieldType", typeof(FieldType), typeof(TextField), new PropertyMetadata(FieldType.String));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public FieldType FieldType
        {
            get => (FieldType)GetValue(FieldTypeProperty);
            set => SetValue(FieldTypeProperty, value);
        }

        public TextField()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            placeholder.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            switch (FieldType)
            {
                case FieldType.Number:
                    Regex regex = new Regex("[^0-9]+");
                    e.Handled = regex.IsMatch(e.Text);
                    break;
            }
        }
    }
}
