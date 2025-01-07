using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Image_Gallery.Model
{
    public class ListViewItemModel
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public double Size { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string FullPath { get; set; }
    }
}