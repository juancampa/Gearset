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
using Gearset.Components.Persistor;
using System.Windows.Forms.Integration;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.IO;
using Gearset.Components.CurveEditorControl;

namespace Gearset.Components
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CurveEditorWindow : Window
    {
        internal bool WasHiddenByGameMinimize { get; set; }

        public CurveEditorWindow()
        {
            InitializeComponent();

            ElementHost.EnableModelessKeyboardInterop(this);

            KeyDown += new KeyEventHandler(CurveEditorWindow_KeyDown);
            Closing += new System.ComponentModel.CancelEventHandler(CurveEditorWindow_Closing);
        }

        void CurveEditorWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Is Ctrl down?
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                // Undo!
                if (e.Key == Key.Z)
                {
                    curveEditorControl.Undo();
                }

                // Undo!
                if (e.Key == Key.Y)
                {
                    curveEditorControl.Redo();
                }
            }
        }

        public void CurveEditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.SaveLogToFile();
        }

        public void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void Close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        public void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }

        private void DisableAllButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void EnableAllButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SaveCurve_Click(object sender, RoutedEventArgs e)
        {
            CurveTreeLeaf leaf = ((FrameworkElement)sender).DataContext as CurveTreeLeaf;
            if (leaf == null)
                return;

            //  Get the actual curve to save.
            Curve curve = leaf.Curve.Curve;
            if (curve == null)
                return;

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = leaf.Curve.Name; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "Xml files (.xml)|*.xml"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result != true)
            {
                return;
            }

            // Write!
            using (var streamOut = new FileStream(dlg.FileName, FileMode.Create))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";
                XmlWriter writer = XmlWriter.Create(streamOut, settings);
                IntermediateSerializer.Serialize<Curve>(writer, curve, dlg.FileName);
                writer.Close();
            }
        }

        private void curveTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //var leaf = curveTree.SelectedItem as CurveTreeLeaf;
            //if (leaf != null)
            //    ((CurveTreeViewModel)curveTree.DataContext).SelectedCurve = leaf.Curve;
            //else
            //    ((CurveTreeViewModel)curveTree.DataContext).SelectedCurve = null;
        }

        private void RemoveCurve_Click(object sender, RoutedEventArgs e)
        {
            //HACK:
            var leaf = ((FrameworkElement)e.Source).DataContext as CurveTreeLeaf;
            if (leaf != null)
                GearsetResources.Console.RemoveCurve(leaf.Curve.Curve);
        }
    }
}
