using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Bibliotech.UserControls
{
    /// <summary>
    /// Interação lógica para ComboBox.xam
    /// </summary>
    public partial class CustomComboBox : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(CustomComboBox), new PropertyMetadata("Title"));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public object SelectedItem
        {
            get => cb.SelectedItem;
            set => cb.SelectedItem = value;
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

        public event SelectionChangedEventHandler SelectionChanged;

        public CustomComboBox()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
