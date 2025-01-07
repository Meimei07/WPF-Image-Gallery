using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Image_Gallery.Model
{
    public class ListViewData
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public double Size { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public ListViewData() { }
        public ListViewData(string name, string extension, double size, string createDate, string createTime)
        {
            Name = name;
            Extension = extension;
            Size = size;
            CreateDate = createDate;
            CreateTime = createTime;
        }
    }
}