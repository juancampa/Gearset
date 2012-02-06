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
    public partial class Texture2DMarkerItem : VisualItemBase
    {
        Texture2D currentTexture;

        public Texture2DMarkerItem()
        {
            InitializeComponent();
        }

        public override void UpdateUI(Object value)
        {
            if (value != currentTexture)
            {
                currentTexture = value as Texture2D;
                if (currentTexture == null)
                    return;

                UpdateTexture();
            }
        }

        private void UpdateTexture()
        {
            // Copy the XNA texture to a MemoryStream as png and then to the BitmapImage.
            using (MemoryStream pngImage = new MemoryStream())
            {
                int height = (int)image.Height;
                float scale = height / (float)currentTexture.Height;
                currentTexture.SaveAsPng(pngImage, (int)(currentTexture.Width * scale), height);

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.DecodePixelHeight = height;
                bi.StreamSource = pngImage;
                bi.EndInit();
                pngImage.Close();
                image.Source = bi;
            }
        }

         public void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateTexture();
        }

    }
}
