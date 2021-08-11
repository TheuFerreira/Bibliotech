using System;
using System.Windows;
using System.Windows.Controls;

namespace Bibliotech.UserControls
{
    /// <summary>
    /// Interação lógica para CustomDatePicker.xam
    /// </summary>
    public partial class CustomDatePicker : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(CustomDatePicker), new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public DateTime? SelectedDate
        {
            get => date.SelectedDate;
            set => date.SelectedDate = value;
        }

        public DateTime? DisplayDateStart
        {
            get => date.DisplayDateStart;
            set => date.DisplayDateStart = value;
        }

        public DateTime? DisplayDateEnd
        {
            get => date.DisplayDateEnd;
            set => date.DisplayDateEnd = value;
        }

        public CustomDatePicker()
        {
            InitializeComponent();
        }
    }
}
