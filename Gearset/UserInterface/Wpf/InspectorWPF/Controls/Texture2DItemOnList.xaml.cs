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
using Microsoft.Xna.Framework.Graphics;
using System.IO;


namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class Texture2DItemOnList : UserControl
    {
        /// <summary>
        /// Registers a dependency property
        /// </summary>
        //public static readonly DependencyProperty ObjectWrapperProperty =
        //    DependencyProperty.Register("ObjectWrapper", typeof(Gearset.Components.InspectorWPF.CollectionMarkerItem.ObjectWrapper), typeof(Texture2DItemOnList),
        //    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnObjectWrapperChanged));

        //private static void OnObjectWrapperChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        //{
        //    //((Texture2DItemOnList)d).ObjectWrapper = (Gearset.Components.InspectorWPF.CollectionMarkerItem.ObjectWrapper)args.NewValue;
        //    ((Texture2DItemOnList)d).UpdateUI((Gearset.Components.InspectorWPF.CollectionMarkerItem.ObjectWrapper)args.NewValue);
        //}
        ///// <summary>
        ///// What type of numeric value will this spinner handle
        ///// </summary>
        //public Gearset.Components.InspectorWPF.CollectionMarkerItem.ObjectWrapper ObjectWrapper { get { return (Gearset.Components.InspectorWPF.CollectionMarkerItem.ObjectWrapper)GetValue(ObjectWrapperProperty); } set { SetValue(ObjectWrapperProperty, value); } }

        Texture2D currentTexture;

        public Texture2DItemOnList()
        {
            InitializeComponent();
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(Texture2DItemOnList_DataContextChanged);
        }

        void Texture2DItemOnList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateUI(DataContext);
        }

        public void UpdateUI(Object value)
        {
            if (value != currentTexture)
            {
                currentTexture = value as Texture2D;
                if (currentTexture == null)
                    return;

                using (MemoryStream pngImage = new MemoryStream())
                {
                    currentTexture.SaveAsPng(pngImage, 20, 20);

                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.DecodePixelWidth = 20;
                    bi.StreamSource = pngImage;
                    bi.EndInit();
                    pngImage.Close();
                    image.Source = bi;
                }

                nameLabel.Text = String.IsNullOrEmpty(currentTexture.Name) ? "(No Name)" : currentTexture.Name;
                stringLabel.Text = "("+ currentTexture.Width + "x" + currentTexture.Height + ") " + currentTexture.Format + ". Levels:" + currentTexture.LevelCount;

            }
        }

    }
}
