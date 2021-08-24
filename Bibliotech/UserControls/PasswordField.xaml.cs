using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bibliotech.UserControls
{
    /// <summary>
    /// Interação lógica para PasswordField.xam
    /// </summary>
    public partial class PasswordField : UserControl
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(PasswordField), new PropertyMetadata("Placeholder"));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PasswordField), new PropertyMetadata("Title"));
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(PasswordField), new PropertyMetadata(0));

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
            get => pbField.Password;
            set => pbField.Password = value;
        }

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public PasswordField()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            placeholder.Visibility = string.IsNullOrEmpty(pbField.Password) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
