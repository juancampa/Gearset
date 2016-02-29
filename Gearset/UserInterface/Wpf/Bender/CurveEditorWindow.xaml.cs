#if WPF
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms.Integration;
    using System.Windows.Input;
    using System.Xml;
    using System.Xml.Serialization;
    using Gearset.Components;
    using Microsoft.Xna.Framework;
    #if XNA
        using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
    #endif

    namespace Gearset.UserInterface.Wpf.Bender
    {
        /// <summary>
        /// 
        /// </summary>
        public partial class CurveEditorWindow : IWindow
        {
            public bool WasHiddenByGameMinimize { get; set; }
        
            bool _isDragging;
            System.Windows.Point _downPosition;

            public CurveEditorWindow()
            {
                InitializeComponent();

                ElementHost.EnableModelessKeyboardInterop(this);

                KeyDown += CurveEditorWindow_KeyDown;
                Closing += CurveEditorWindow_Closing;
            }

            void CurveEditorWindow_KeyDown(object sender, KeyEventArgs e)
            {
                // Is Ctrl down?
                if (e.KeyboardDevice.Modifiers != ModifierKeys.Control)
                    return;

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
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }

            private void SaveCurve_Click(object sender, RoutedEventArgs e)
            {
                var leaf = ((FrameworkElement)sender).DataContext as CurveTreeLeaf;
                if (leaf == null)
                    return;

                //  Get the actual curve to save.
                var curve = leaf.Curve.Curve;
                if (curve == null)
                    return;

                // Configure save file dialog box
                var dlg = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = leaf.Curve.Name,
                    DefaultExt = ".xml",
                    Filter = "Xml files (.xml)|*.xml"
                };

                // Show save file dialog box
                var result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result != true)
                {
                    return;
                }

                // Write!
                using (var streamOut = new FileStream(dlg.FileName, FileMode.Create))
                {
                    #if XNA
                        var settings = new XmlWriterSettings {Indent = true, IndentChars = "  "};
                        var writer = XmlWriter.Create(streamOut, settings);
                        IntermediateSerializer.Serialize<Curve>(writer, curve, dlg.FileName);
                        writer.Close();
                    #elif MONOGAME || FNA
                        var formatter = new XmlSerializer(typeof(Curve));
                        formatter.Serialize(streamOut, curve);
                    #endif                    
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

            private void NewCurve_Click(object sender, RoutedEventArgs e)
            {
                GearsetResources.Console.Bender.AddCurve("New Curve", new Curve());
            }

            private void curveTree_MouseDown(object sender, MouseButtonEventArgs e)
            {
                _downPosition = Mouse.GetPosition(this);
            }

            private void curveTree_MouseMove(object sender, MouseEventArgs e)
            {
                if (curveTree.SelectedItem == null)
                    return;

                if (Mouse.LeftButton != MouseButtonState.Pressed || _isDragging)
                    return;

                if (e.OriginalSource is System.Windows.Controls.TextBox)
                    return;

                var pos = Mouse.GetPosition(this);
                if (Math.Abs(pos.X - _downPosition.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(pos.Y - _downPosition.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    StartDrag();
                }
            }

            private void StartDrag()
            {
                _isDragging = true;
                var data = new DataObject(typeof(Object), ((CurveTreeLeaf)curveTree.SelectedItem).Curve.Curve);
                DragDrop.DoDragDrop(curveTree, data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.None);
                _isDragging = false;
            }
        }
    }
#endif