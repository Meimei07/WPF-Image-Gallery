using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using WPF_Image_Gallery.Model;

using Newtonsoft.Json;

namespace WPF_Image_Gallery
{
    public class IOManager
    {
        private string path = @".\data";
        private string extension = ".json";

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

        public List<string> GetFilesInString(string path)
        {
            List<string> fileNames = new List<string>();
            string[] fileExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp", ".svg" };

            DirectoryInfo rootFolder = new DirectoryInfo(path);
            FileInfo[] files = rootFolder.GetFiles();

            foreach (FileInfo file in files)
            {
                if (fileExtensions.Contains(file.Extension.ToLower()))
                { 
                    fileNames.Add(file.Name); 
                }
            }
            return fileNames;
        }

        public List<ListViewData> GetFiles(string path)
        {
            List<ListViewData> datas = new List<ListViewData>();
            string[] fileExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp", ".svg" };

            DirectoryInfo rootFolder = new DirectoryInfo(path);
            FileInfo[] files = rootFolder.GetFiles();

            foreach (FileInfo file in files)
            {
                if(fileExtensions.Contains(file.Extension.ToLower()))
                {
                    double fileSizeinKb = Math.Ceiling(file.Length / 1024.0);

                    DateTime createDate = file.CreationTime;
                    string date = createDate.Date.ToShortDateString();
                    string time = createDate.ToString("HH:mm");

                    datas.Add(new ListViewData(file.Name, file.Extension, fileSizeinKb, date, time));
                }
            }
            return datas;
        }

        private void CreateFolder()
        {
            Directory.CreateDirectory(path);
        }

        private bool IsPathExisted()
        {
            return File.Exists(path);
        }

        private string GetFullPath(string fileName)
        {
            return string.Format($"{path}\\{fileName}{extension}");
        }

        public void Write(string fileName, Object obj)
        {
            if (!IsPathExisted()) //if path doesn't exist
            {
                CreateFolder(); //create path
            }

            string fullPath = GetFullPath(fileName);

            StreamWriter streamWriter = new StreamWriter(fullPath);
            string content = JsonConvert.SerializeObject(obj);
            streamWriter.Write(content);
            streamWriter.Close();
        }

        public T Read<T>(string fileName)
        {
            string fullPath = GetFullPath(fileName);
            if (!File.Exists(fullPath))
            {
                return default(T);            
            }

            //string content = File.ReadAllText(fullPath);

            StreamReader streamReader = new StreamReader(fullPath);
            string content = streamReader.ReadToEnd();
            streamReader.Close();

            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}