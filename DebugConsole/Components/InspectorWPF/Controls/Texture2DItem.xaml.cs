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
    public partial class Texture2DItem : VisualItemBase
    {
        Texture2D currentTexture;

        public Texture2DItem()
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

            }
        }

    }
}
