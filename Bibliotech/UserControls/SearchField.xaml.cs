using System.Windows;
using System.Windows.Controls;

namespace Bibliotech.UserControls
{
    /// <summary>
    /// Interação lógica para SearchField.xam
    /// </summary>
    public partial class SearchField : UserControl
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(SearchField), new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public string Text
        {
            get => tb.Text;
            set => tb.Text = value;
        }

        public event RoutedEventHandler Click;

        public SearchField()
        {
            InitializeComponent();
        }

        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbPlaceholder.Visibility = string.IsNullOrEmpty(tb.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(sender, e);
        }
    }
}
