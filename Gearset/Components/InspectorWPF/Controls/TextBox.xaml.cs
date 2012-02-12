using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Xna.Framework;


namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class TextBox : UserControl
    {
        /// <summary>
        /// Registers a dependency property
        /// </summary>
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(String), typeof(TextBox), new PropertyMetadata(OnPlaceholderTextChanged));

        /// <summary>
        /// Registers a dependency property
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(TextBox), new PropertyMetadata(OnTextAlignmentChanged));

        /// <summary>
        /// Registers a dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(TextBox));

        private static void OnPlaceholderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((TextBox)d).placeholderText = (String)args.NewValue;
        }

        private static void OnTextAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((TextBox)d).TextBox1.TextAlignment = (TextAlignment)args.NewValue;
        }

        private bool isEmpty;
        private TextDecorationCollection savedDecorations;
        private Brush subtleBrush;
        private Brush normalBrush;

        /// <summary>
        /// What text to show as placeholder
        /// </summary>
        public String PlaceholderText { get { return (String)GetValue(PlaceholderTextProperty); } set { SetValue(PlaceholderTextProperty, value); } }
        private String placeholderText = String.Empty;

        /// <summary>
        /// Text alignment
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)TextBox1.TextAlignment; }
            set { TextBox1.TextAlignment = value; }
        }

        /// <summary>
        /// Text to show, if this value is empty, the palceholder will be shown instead.
        /// </summary>
        public String Text { get { return (String)GetValue(System.Windows.Controls.TextBox.TextProperty); } set { SetValue(System.Windows.Controls.TextBox.TextProperty, value); } }

        private bool fakeTextChanged;

        public System.Windows.Controls.TextBox ActualTextBox { get { return TextBox1; } }

        public TextBox()
        {
            placeholderText = "(empty string)";
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(TextBox_Loaded);

            TextBox1.TextChanged += new TextChangedEventHandler(TextBox1_TextChanged);
            //PlaceholderTextBlock.MouseDown += new MouseButtonEventHandler(PlaceholderTextBlock_MouseDown);
            TextBox1.LostFocus += new RoutedEventHandler(TextBox1_LostFocus);
            TextBox1.GotFocus += new RoutedEventHandler(TextBox1_GotFocus);
            
        }

        void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            subtleBrush = (Brush)FindResource("subtle1");
            normalBrush = (Brush)FindResource("normalText1");

            savedDecorations = new TextDecorationCollection();
            foreach (var decoration in TextBox1.TextDecorations)
                savedDecorations.Add(decoration);

            CheckIfEmpty();
        }

        void TextBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isEmpty)
                TextBox1.Text = String.Empty;
        }

        void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckIfEmpty();
        }

        //void PlaceholderTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    // Show the TextBox
        //    TextBox1.Visibility = System.Windows.Visibility.Visible;
        //    PlaceholderTextBlock.Visibility = System.Windows.Visibility.Hidden;
        //    //TextBox1.Focus();
        //}

        void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (fakeTextChanged) return;
            if (TextBox1.Text != String.Empty)
            {
                UseNormal();
            }
            else
            {
                if (!TextBox1.IsFocused)
                {
                    UsePlaceholder();
                }
            }
        }

        private void UseNormal()
        {
            if (TextBox1.TextDecorations.Count == 0 && savedDecorations.Count > 0)
            {
                foreach (var decoration in savedDecorations)
                    TextBox1.TextDecorations.Add(decoration.Clone());   // It won't work without the Clone call.
            }
            TextBox1.Foreground = normalBrush;
            isEmpty = false;
        }

        private void CheckIfEmpty()
        {
            if (TextBox1.Text == String.Empty)
            {
                UsePlaceholder();
            }
            else
            {
                UseNormal();
            }
            //if (TextBox1.Text == String.Empty)
            //{
            //    TextBox1.Visibility = System.Windows.Visibility.Hidden;
            //    PlaceholderTextBlock.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    TextBox1.Visibility = System.Windows.Visibility.Visible;
            //    PlaceholderTextBlock.Visibility = System.Windows.Visibility.Hidden;
            //}
        }

        private void UsePlaceholder()
        {
            TextBox1.TextDecorations.Clear();

            fakeTextChanged = true;
            TextBox1.Text = placeholderText;
            fakeTextChanged = false;
            TextBox1.Foreground = subtleBrush;
            isEmpty = true;
        }
    }
}
