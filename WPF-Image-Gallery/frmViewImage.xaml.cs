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
        private string fullPath;
        private string tbPath;
        private string name;
        private IOManager ioManager = new IOManager();

        public frmViewImage(string name, string fullPath, string tbPath)
        {
            InitializeComponent();
            this.fullPath = fullPath;
            this.tbPath = tbPath;
            this.name = name;

            try
            {
                if (File.Exists(fullPath))
                {
                    img.Source = new BitmapImage(new Uri(fullPath));
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
                //zoomFactor = (zoomFactor > 0.2) ? zoomFactor - 0.1 : 0.1;
                if(zoomFactor > 0.2)
                {
                    zoomFactor -= 0.1;
                }
                else
                {
                    zoomFactor = 0.1;
                }
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
            //zoomFactor = (zoomFactor > 0.2) ? zoomFactor - 0.1 : 0.1;  // Decrease zoom factor but avoid going too small
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

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            string rootPath = Path.GetDirectoryName(fullPath);

            //string fileName = Path.GetFileName(fullPath); //name with extension
            //string fileName = Path.GetFileNameWithoutExtension(fullPath);

            //List<string> files = ioManager.GetFilesInString(rootPath);
            var files = ioManager.Read<List<ListViewItemModel>>(System.IO.Path.GetFileName(tbPath));

            if(files==null) { return; }
            int fileIndex = files.FindIndex(f => f.Name == name);

            if(fileIndex > 0)
            {
                //can previous
                fileIndex -= 1;
                fullPath = Path.Combine(files[fileIndex].FullPath, files[fileIndex].Source + files[fileIndex].Extension);
                name = files[fileIndex].Name;
                img.Source = new BitmapImage(new Uri(fullPath));
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            string rootPath = Path.GetDirectoryName(fullPath);

            //string fileName = Path.GetFileNameWithoutExtension(fullPath);

            List<ListViewItemModel> files = ioManager.Read<List<ListViewItemModel>>(System.IO.Path.GetFileName(tbPath));

            if(files == null) { return; }
            int fileIndex = files.FindIndex(f => f.Name == name);

            if (fileIndex < files.Count - 1)
            {
                //can next
                fileIndex += 1;
                fullPath = Path.Combine(files[fileIndex].FullPath, files[fileIndex].Source + files[fileIndex].Extension);
                name = files[fileIndex].Name;
                img.Source = new BitmapImage(new Uri(fullPath));
            }
        }
    }
}