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


namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class StringItem : VisualItemBase
    {
        /// <summary>
        /// Store the valur of IsUpdating when the textbox
        /// gets focus.
        /// </summary>
        private bool wasUpdating;

        public StringItem()
        {
            InitializeComponent();
            TextBox1.GotFocus += new RoutedEventHandler(TextBox1_GotFocus);
            TextBox1.LostFocus += new RoutedEventHandler(TextBox1_LostFocus);
            TextBox1.KeyDown += new KeyEventHandler(TextBox1_KeyDown);
        }

        public void TextBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            wasUpdating = TreeNode.Updating;
            TreeNode.Updating = false;
        }

        public override void UpdateUI(Object value)
        {
            if (value == null) return;
            String text = value.ToString();
            this.TextBox1.Text = text;
            this.TextBox1.ToolTip = text;
        }

        public override void UpdateVariable()
        {
            TreeNode.Property = TextBox1.Text;
        }

        public void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = false;
            System.Windows.Controls.TextBox textBox = TextBox1;
            if (e.Key == Key.Enter)
            {
                if (!Keyboard.IsKeyDown(Key.LeftShift))
                    textBox.MoveFocus(traversalRequest);
                else
                {
                    int i = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Substring(0, i) + "\n" + textBox.Text.Substring(i, textBox.Text.Length - i);
                    textBox.CaretIndex = i + 1;
                }
            }
            else if (e.Key == Key.Subtract)
            {
                System.Windows.Controls.TextBox box = (System.Windows.Controls.TextBox)sender;
                int caret = box.CaretIndex;
                box.Text = box.Text.Insert(box.CaretIndex, "-");
                box.CaretIndex = caret + 1;
                e.Handled = true;
            }
            textBox.AppendText(String.Empty);
        }

        public void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TreeNode == null)
            {
                return;
            }
            UpdateVariable();
            TreeNode.Updating = wasUpdating;
        }

    }
}
