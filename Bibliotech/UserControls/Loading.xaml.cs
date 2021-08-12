using System.Windows;
using System.Windows.Controls;

namespace Bibliotech.UserControls
{
    /// <summary>
    /// Interação lógica para Loading.xam
    /// </summary>
    public partial class Loading : UserControl
    {
        public static readonly DependencyProperty AwaitingProperty = DependencyProperty.Register("Awaiting", typeof(bool), typeof(Loading), new PropertyMetadata(false));

        public bool Awaiting
        {
            get => (bool)GetValue(AwaitingProperty);
            set
            {
                SetValue(AwaitingProperty, value);
                Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Loading()
        {
            InitializeComponent();
        }
    }
}
