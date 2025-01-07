using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WPF_Image_Gallery.Model;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;

namespace WPF_Image_Gallery
{
    /// <summary>
    /// Interaction logic for frmViewImage.xaml
    /// </summary>
    public partial class frmViewImage : Window
    {
        private double zoomFactor = 1.0;
        public frmViewImage(string fullPath)
        {
            InitializeComponent();

            try
            {
                if (File.Exists(fullPath))
                {
                    //MessageBox.Show(fullPath);
                    img.Source = new BitmapImage(new Uri(fullPath));

                    //BitmapImage bitmap = new BitmapImage(new Uri(fullPath));

                    //// Create a WriteableBitmap from the BitmapImage
                    //WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);

                    //// Set the Image source to the WriteableBitmap
                    //img.Source = writeableBitmap;

                    //// Optionally, adjust the Stretch property for scaling
                    //img.Stretch = Stretch.Uniform;
                }
                else
                {
                    MessageBox.Show("File does not exist: " + fullPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }

        private void img_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0) // Scroll Up -> Zoom In
            {
                zoomFactor += 0.1;
            }
            else // Scroll Down -> Zoom Out
            {
                zoomFactor = (zoomFactor > 0.2) ? zoomFactor - 0.1 : 0.1;
            }

            applyZoom();
        }

        private void applyZoom()
        {
            ImageScale.ScaleX = zoomFactor;
            ImageScale.ScaleY = zoomFactor;
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            zoomFactor += 0.1;  // Increase zoom factor
            applyZoom();
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            zoomFactor = (zoomFactor > 0.2) ? zoomFactor - 0.1 : 0.1;  // Decrease zoom factor but avoid going too small
            if(zoomFactor > 0.2)
            {
                zoomFactor -= 0.1;
            }
            else
            {
                zoomFactor = 0.1;
            }
            applyZoom();
        }
    }
}