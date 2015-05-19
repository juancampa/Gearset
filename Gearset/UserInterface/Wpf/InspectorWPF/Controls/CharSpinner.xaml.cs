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
using Microsoft.Xna.Framework;


namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class CharSpinner : VisualItemBase
    {
        /// <summary>
        /// The real char value.
        /// </summary>
        private char charValue;

        public char CharValue
        {
            get { return charValue; }
            set
            {
                charValue = value;
                TextBox1.Text = charValue.ToString();
            }
        }

        public CharSpinner()
        {
            InitializeComponent();
            TextBox1.LostFocus += new RoutedEventHandler(TextBox1_LostFocus);
            TextBox1.KeyDown += new KeyEventHandler(TextBox1_KeyDown);
            
        }

         public void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox1.MoveFocus(traversalRequest);
            }
            else if (e.Key == Key.Subtract)
            {
                System.Windows.Controls.TextBox box = (System.Windows.Controls.TextBox)sender;
                int caret = box.CaretIndex;
                box.Text = box.Text.Insert(box.CaretIndex, "-");
                box.CaretIndex = caret + 1;
                e.Handled = true;
            }
        }


        public override void UpdateUI(Object value)
        {
            this.CharValue = (char)value;
        }

        public override void UpdateVariable()
        {
            TreeNode.Property = charValue;
        }

         public void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TreeNode == null)
            {
                return;
            }
            char newCharValue;
            if (!char.TryParse(TextBox1.Text, out newCharValue))
            {
                // TODO: Change the style here to something red
            }
            else
            {
                this.charValue = newCharValue;
                UpdateVariable();
            }
        }

    }
}
