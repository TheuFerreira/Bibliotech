using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            Click?.Invoke(this, e);
        }

        private void Tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Click?.Invoke(this, e);
            }
        }
    }
}
