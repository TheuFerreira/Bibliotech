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
        public static readonly DependencyProperty FieldMaskProperty = DependencyProperty.Register("Mask", typeof(FieldMask), typeof(TextField), new PropertyMetadata(FieldMask.None));
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(TextField), new PropertyMetadata("Placeholder"));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TextField), new PropertyMetadata("Title"));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextField), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(TextField), new PropertyMetadata(0));
        public static readonly DependencyProperty FieldTypeProperty = DependencyProperty.Register("FieldType", typeof(FieldType), typeof(TextField), new PropertyMetadata(FieldType.String));

        public FieldMask Mask
        {
            get => (FieldMask)GetValue(FieldMaskProperty);
            set => SetValue(FieldMaskProperty, value);
        }

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
            get
            {
                string text = (string)GetValue(TextProperty);

                if (Mask == FieldMask.Telephone)
                {
                    text = text.Replace("(", "").Replace(") ", "").Replace("-", "");
                }

                return text;
            }
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

        private bool isBackspace = false;

        public TextField()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            placeholder.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Collapsed;

            if (isBackspace)
            {
                return;
            }

            if (Mask != FieldMask.None)
            {
                textBox.CaretIndex = textBox.Text.Length;
            }

            string text = textBox.Text;

            switch (Mask)
            {
                case FieldMask.Telephone:

                    if (text.Length == 1)
                    {
                        text = text.Insert(0, "(");
                    }
                    else if (text.Length == 3)
                    {
                        text = text.Insert(3, ") ");
                    }
                    else if (text.Length == 10)
                    {
                        text = text.Insert(10, "-");
                    }

                    textBox.Text = text;

                    break;

                case FieldMask.Date:

                    if (text.Length == 2)
                    {
                        text = text.Insert(2, "/");
                    }
                    else if (text.Length == 5)
                    {
                        text = text.Insert(5, "/");
                    }

                    textBox.Text = text;

                    break;

                case FieldMask.None:
                    break;

                default:
                    break;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            switch (FieldType)
            {
                case FieldType.Number:
                    Regex regex = new Regex("[^0-9]+");
                    e.Handled = regex.IsMatch(e.Text);
                    break;
                case FieldType.String:
                    break;
                default:
                    break;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            isBackspace = e.Key == Key.Back;
        }
    }
}
