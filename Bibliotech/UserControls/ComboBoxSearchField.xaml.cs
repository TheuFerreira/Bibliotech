using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bibliotech.UserControls
{
    /// <summary>
    /// Interação lógica para ComboBoxSearchField.xam
    /// </summary>
    public partial class ComboBoxSearchField : UserControl
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(ComboBoxSearchField), new PropertyMetadata(string.Empty));

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

        public object SelectedItem
        {
            get => cb.SelectedItem;
            set => cb.SelectedItem = true;
        }

        public int SelectedIndex
        {
            get => cb.SelectedIndex;
            set => cb.SelectedIndex = value;
        }

        public IEnumerable ItemsSource
        {
            get => cb.ItemsSource;
            set => cb.ItemsSource = value;
        }

        public event RoutedEventHandler Click;

        public ComboBoxSearchField()
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

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Click?.Invoke(this, e);
        }
    }
}
