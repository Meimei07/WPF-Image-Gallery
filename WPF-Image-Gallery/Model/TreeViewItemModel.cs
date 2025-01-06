using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Image_Gallery
{
    public class TreeViewItemModel
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public ObservableCollection<TreeViewItemModel> Children { get; set; }

        public TreeViewItemModel()
        {
            Children = new ObservableCollection<TreeViewItemModel>();
        }
    }
}