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
    public partial class EnumItem : VisualItemBase
    {
        private bool wasUpdating;

        /// <summary>
        /// When the variable changes, the UpdateUI method
        /// will trigger a SelectionChanged event which would
        /// update the variable back (not wanted)
        /// </summary>
        private bool isEventFake;
        public EnumItem()
        {
            InitializeComponent();

            ComboBox1.SelectionChanged += new SelectionChangedEventHandler(ComboBox1_SelectionChanged);
            //ComboBox1.DropDownOpened += new EventHandler(ComboBox1_DropDownOpened);
            //ComboBox1.DropDownClosed += new EventHandler(ComboBox1_DropDownClosed);
        }

        public void ComboBox1_DropDownClosed(object sender, EventArgs e)
        {
            TreeNode.Updating = wasUpdating;
        }

        static bool a;
        public void ComboBox1_DropDownOpened(object sender, EventArgs e)
        {
            wasUpdating = TreeNode.Updating;
            TreeNode.Updating = false;

            //if (!a)
            //GearsetResources.Console.Inspect("DropDown", this);
            //a = true;
        }

        public Enum EnumValue
        {
            get 
            {
                return (Enum)Enum.Parse(TreeNode.Type, ((ComboBoxItem)ComboBox1.SelectedValue).Content.ToString());
            }
        }

         public void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isEventFake)
            {
                UpdateVariable();
            }
        }

        public override void UpdateUI(Object value)
        {
            Enum e = value as Enum;
            if (TreeNode.Type == typeof(Enum))
                return;
            if (Enum.IsDefined(TreeNode.Type, value))
            {
                isEventFake = true;
                ComboBox1.Text = value.ToString();
                isEventFake = false;
            }
        }

        public override void UpdateVariable()
        {
            TreeNode.Property = EnumValue;
        }

        /// <summary>
        /// When the treeNode is set, we populate the comboBox.
        /// </summary>
        public override void OnTreeNodeChanged()
        {
            base.OnTreeNodeChanged();
            if (TreeNode.Type == typeof(Enum))
                return;
            foreach (var value in Enum.GetNames(TreeNode.Type))
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = value;
                ComboBox1.Items.Add(item);
            }
        }
    }
}
