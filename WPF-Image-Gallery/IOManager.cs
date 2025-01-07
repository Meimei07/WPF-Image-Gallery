using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using WPF_Image_Gallery.Model;

namespace WPF_Image_Gallery
{
    public class IOManager
    {
        public List<string> GetFolders(string path)
        {
            List<string> folderNames = new List<string>();

            DirectoryInfo rootFolder = new DirectoryInfo(path);
            DirectoryInfo[] folders = rootFolder.GetDirectories();

            foreach(DirectoryInfo folder in folders)
            {
                folderNames.Add(folder.Name);
            }
            return folderNames;
        }
        //public List<string> GetFiles(string path)
        //{
        //    List<string> fileNames = new List<string>();
        //    string[] fileExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp", ".svg" };

        //    DirectoryInfo rootFolder = new DirectoryInfo(path);
        //    FileInfo[] files = rootFolder.GetFiles();

        //    foreach (FileInfo file in files)
        //    {
        //        if(fileExtensions.Contains(file.Extension.ToLower())) 
        //            fileNames.Add(file.Name);
        //    }
        //    return fileNames;
        //}
        
        public List<ListViewData> GetFiles(string path)
        {
            //List<string> fileNames = new List<string>();
            List<ListViewData> datas = new List<ListViewData>();
            string[] fileExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp", ".svg" };

            DirectoryInfo rootFolder = new DirectoryInfo(path);
            FileInfo[] files = rootFolder.GetFiles();

            foreach (FileInfo file in files)
            {
                double fileSizeinKb = Math.Ceiling(file.Length / 1024.0);

                DateTime createDate = file.CreationTime;
                string date = createDate.Date.ToShortDateString();
                string time = createDate.ToString("HH:mm");

                datas.Add(new ListViewData(file.Name, file.Extension, fileSizeinKb, date, time));
            }
            return datas;
        }
    }
}