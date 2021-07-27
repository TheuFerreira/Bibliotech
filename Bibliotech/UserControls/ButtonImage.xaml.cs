using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bibliotech.UserControls
{
    /// <summary>
    /// Interação lógica para ButtonImage.xam
    /// </summary>
    public partial class ButtonImage : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ButtonImage), new PropertyMetadata("Text"));
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ButtonImage), new PropertyMetadata(null));
        public static readonly DependencyProperty HasBorderProperty = DependencyProperty.Register("HasBorder", typeof(bool), typeof(ButtonImage), new PropertyMetadata(true));

        public event RoutedEventHandler OnClick;

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public bool HasBorder
        {
            get => (bool)GetValue(HasBorderProperty);
            set => SetValue(HasBorderProperty, value);
        }

        public ButtonImage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnClick?.Invoke(sender, e);

        }
    }
}
