using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Image_Gallery.Model
{
    public class ViewModel
    {
        public ObservableCollection<ListViewItemModel> Items { get; set; }

        public ViewModel()
        {
            Items = new ObservableCollection<ListViewItemModel>();

            string rootPath = "pack://application:,,,/Images/";

            List<string> folders = new IOManager().GetFolders("d");
            foreach (string folder in folders)
            {
                Items.Add(new ListViewItemModel { Icon = rootPath + "folder.png", Name = folder });
            }
        }
    }
}