using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization.Configuration;

using System.IO;
using System.Collections.ObjectModel;
using WPF_Image_Gallery.Model;

namespace WPF_Image_Gallery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IOManager ioManager = new IOManager();
        public List<string> folderNames = new List<string>();
        //private string rootPath = "D:\\";
        private string rootPath = "pack://application:,,,/Images/";

        public ObservableCollection<TreeViewItemModel> itemModels = new ObservableCollection<TreeViewItemModel>();
        public ObservableCollection<ListViewItemModel> ListViewItemModels { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            ListViewItemModels = new ObservableCollection<ListViewItemModel>();
            
            retrieveFolders();
            listView.DataContext = this;
        }

        private void recursiveRetrievingFolder(string mainFolderName, TreeViewItemModel item)
        {
            List<string> subFolderNames = ioManager.GetFolders(mainFolderName);
            foreach (string subFolderName in subFolderNames)
            {
                TreeViewItemModel subItemModel = new TreeViewItemModel();
                subItemModel.Icon = rootPath + "folder.png";
                subItemModel.Name = subFolderName;

                //TreeViewItem subItem = new TreeViewItem();
                //subItem.Header = subItemModel;
                item.Children.Add(subItemModel);               

                string path = System.IO.Path.Combine(mainFolderName, subFolderName);
                recursiveRetrievingFolder(path, subItemModel);
            }
        }

        private void retrieveFolders()
        {
            string[] drives = Environment.GetLogicalDrives();
            foreach (string drive in drives)
            {
                TreeViewItemModel treeViewItemModel = new TreeViewItemModel();
                treeViewItemModel.Icon = rootPath + "folder.png";
                treeViewItemModel.Name = drive;

                //TreeViewItem driveItem = new TreeViewItem();
                //driveItem.Header = treeViewItemModel;
                itemModels.Add(treeViewItemModel);

                folderNames = ioManager.GetFolders(drive);
                for (int i = 0; i < folderNames.Count; i++)
                {
                    string mainFolderName = folderNames[i];

                    TreeViewItemModel folderItemModel = new TreeViewItemModel();
                    folderItemModel.Icon = rootPath + "folder.png";
                    folderItemModel.Name = mainFolderName;

                    //TreeViewItem item = new TreeViewItem();
                    //item.Header = folderItemModel;
                    treeViewItemModel.Children.Add(folderItemModel);

                    try
                    {
                        //retrieveFolders(drive + mainFolderName, item);
                        recursiveRetrievingFolder(drive + mainFolderName, folderItemModel);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Handle access denial by skipping this folder and continuing
                        Console.WriteLine($"Access to folder {mainFolderName} is denied.");
                        continue; // Skip this folder and move to the next
                    }

                    //driveItem.Items.Add(item);
                }
                //treeView.Items.Add(driveItem);
            }
            treeView.ItemsSource = itemModels;
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Search")
            {
                txtSearch.Text = "";
            }
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "")
            {
                txtSearch.Text = "Search";
            }
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //clear the old list, if another left mouse is clicked on tree view item 
            //if not clear, the new subfolders will not overwrite, and will appear more and more on list view
            ListViewItemModels.Clear(); 

            if(e.ClickCount == 1)
            {
                if (sender is StackPanel stackPanel && stackPanel.DataContext is TreeViewItemModel item)
                {
                    //MessageBox.Show(item.Name);
                    string fullPath = "";

                    //VisualTreeHelper.GetParent() method help to retrieve/find the parent of the clicked stackPanel
                    DependencyObject parent = VisualTreeHelper.GetParent(stackPanel);

                    while (parent != null) //loop until the top level of tree, to get full path of the clicked stackPanel
                    {
                        if (parent is TreeViewItem treeViewItem && treeViewItem.DataContext is TreeViewItemModel parentItem)
                        {
                            if (string.IsNullOrEmpty(fullPath))
                            {
                                fullPath = parentItem.Name;
                            }
                            else
                            {
                                fullPath = System.IO.Path.Combine(parentItem.Name, fullPath);
                            }
                        }

                        parent = VisualTreeHelper.GetParent(parent);
                    }

                    //MessageBox.Show($"fullpath: {fullPath}");

                    string rootPath = "pack://application:,,,/Images/";

                    //List<string> folders = new IOManager().GetFiles(fullPath);
                    List<ListViewData> files = new IOManager().GetFiles(fullPath);
                    foreach (ListViewData file in files)
                    {
                        ListViewItemModels.Add(new ListViewItemModel { Icon = rootPath + "file.png", Name = getName(file), Extension = file.Extension, Size = file.Size, CreateDate = file.CreateDate, CreateTime = file.CreateTime, FullPath = fullPath });
                    }
                }
            }
        }

        private string getName(ListViewData file)
        {
            string nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(file.Name);
            return nameWithoutExtension;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (sender is Grid grid && grid.DataContext is ListViewItemModel item)
                {
                    //MessageBox.Show($"file name: {item.FullPath}");

                    string fullPath = System.IO.Path.Combine(item.FullPath, item.Name+item.Extension);
                    //List<ListViewData> files = new IOManager().GetFiles(fullPath);

                    frmViewImage frmViewImage = new frmViewImage(fullPath);
                    frmViewImage.ShowDialog();
                }
            }
        }
    }
}