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
using static System.Net.WebRequestMethods;

namespace WPF_Image_Gallery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IOManager ioManager = new IOManager();
        public List<string> folderNames = new List<string>();
        private string rootPath = "pack://application:,,,/Images/";

        public ObservableCollection<TreeViewItemModel> itemModels = new ObservableCollection<TreeViewItemModel>();
        public ObservableCollection<ListViewItemModel> ListViewItemModels { get; set; }
        public ListViewItemModel listViewItem { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            ListViewItemModels = new ObservableCollection<ListViewItemModel>();
            listViewItem = new ListViewItemModel();
            
            retrieveFolders();
            listView.DataContext = this;
            //contentControlDetails.DataContext = this;
        }

        private void tbSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbSearch.Text == "Search")
            {
                tbSearch.Text = "";
            }
        }

        private void tbSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbSearch.Text == "")
            {
                tbSearch.Text = "Search";
            }
        }

        private void recursiveRetrievingFolder(string mainFolderName, TreeViewItemModel item)
        {
            List<string> subFolderNames = ioManager.GetFolders(mainFolderName);
            foreach (string subFolderName in subFolderNames)
            {
                TreeViewItemModel subItemModel = new TreeViewItemModel();
                subItemModel.Icon = rootPath + "folder.png";
                subItemModel.Name = subFolderName;

                item.Children.Add(subItemModel);               

                string path = System.IO.Path.Combine(mainFolderName, subFolderName);
                recursiveRetrievingFolder(path, subItemModel);
            }
        }

        //retrieve starts from drive of computer
        private void retrieveFolders()
        {
            string[] drives = Environment.GetLogicalDrives();
            foreach (string drive in drives)
            {
                TreeViewItemModel treeViewItemModel = new TreeViewItemModel();
                treeViewItemModel.Icon = rootPath + "folder.png";
                treeViewItemModel.Name = drive;

                itemModels.Add(treeViewItemModel);

                folderNames = ioManager.GetFolders(drive); //get folders from each drive
                for (int i = 0; i < folderNames.Count; i++)
                {
                    string mainFolderName = folderNames[i];

                    TreeViewItemModel folderItemModel = new TreeViewItemModel();
                    folderItemModel.Icon = rootPath + "folder.png";
                    folderItemModel.Name = mainFolderName;

                    treeViewItemModel.Children.Add(folderItemModel);

                    try
                    {
                        recursiveRetrievingFolder(drive + mainFolderName, folderItemModel);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Handle access denial by skipping this folder and continuing
                        Console.WriteLine($"Access to folder {mainFolderName} is denied.");
                        continue; // Skip this folder and move to the next
                    }
                }
            }
            treeView.ItemsSource = itemModels;
        }

        //single click on treeView, shows images on listView
        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //clear the old list, if another left mouse is clicked on tree view item 
            //if not clear, the new subfolders will not overwrite, and will appear more and more on list view
            ListViewItemModels.Clear(); 

            if(e.ClickCount == 1)
            {
                if (sender is StackPanel stackPanel && stackPanel.DataContext is TreeViewItemModel item)
                {
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

                    string rootPath = "pack://application:,,,/Images/";

                    List<ListViewData> files = new IOManager().GetFiles(fullPath);
                    foreach (ListViewData file in files)
                    {
                        ListViewItemModels.Add(new ListViewItemModel { Icon = rootPath + "file.png", Name = getName(file), Extension = file.Extension, Size = file.Size, CreateDate = file.CreateDate, CreateTime = file.CreateTime, FullPath = fullPath });
                    }

                    tbPath.Text = fullPath;
                }
            }
        }

        private string getName(ListViewData file)
        {
            string nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(file.Name);
            return nameWithoutExtension;
        }

        //double click on listView file, shows image in full screen
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (sender is Grid grid && grid.DataContext is ListViewItemModel item)
                {
                    string fullPath = System.IO.Path.Combine(item.FullPath, item.Name + item.Extension);

                    frmViewImage frmViewImage = new frmViewImage(fullPath);
                    frmViewImage.ShowDialog();
                }
            }
            else if(e.ClickCount == 1)
            {
                if (sender is Grid grid && grid.DataContext is ListViewItemModel item)
                {
                    item.Icon = System.IO.Path.Combine(rootPath, item.FullPath, item.Name+item.Extension);
                    gridDetails.DataContext = item;
                }
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbSearchType.Text))
            {
                return;
            }

            string selectedSearchType = cmbSearchType.Text;
            string path = tbPath.Text;
            string searchWord = tbSearch.Text.ToLower();

            //MessageBox.Show($"type:{selectedSearchType}, path:{path}");

            List<ListViewData> allFiles = ioManager.GetFiles(path);
            List<ListViewData> filterFiles = new List<ListViewData>();

            if (selectedSearchType == "Name")
            {
                filterFiles = allFiles.Where(f => f.Name.ToLower().Contains(searchWord)).ToList();
                updateListView(filterFiles, path);
            }
            else if (selectedSearchType == "Type")
            {
                filterFiles = allFiles.Where(f => f.Extension.ToLower().Contains(searchWord)).ToList();
                updateListView(filterFiles, path);
            }
            else if (selectedSearchType == "Size")
            {
                filterFiles = allFiles.Where(f => f.Size.ToString().Contains(searchWord)).ToList();
                updateListView(filterFiles, path);
            }
            else if (selectedSearchType == "Create Date")
            {
                filterFiles = allFiles.Where(f => f.CreateDate.Contains(searchWord)).ToList();
                updateListView(filterFiles, path);
            }
        }

        private void updateListView(List<ListViewData> filterFiles, string path)
        {
            ListViewItemModels.Clear(); // Clear existing items in the ListView

            foreach (ListViewData file in filterFiles)
            {
                ListViewItemModels.Add(new ListViewItemModel { Icon = rootPath + "file.png", Name = getName(file), Extension = file.Extension, Size = file.Size, CreateDate = file.CreateDate, CreateTime = file.CreateTime, FullPath = path });
            }
        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(listView.SelectedItems.Count.ToString());
            if(listView.SelectedItems.Count == 0)
            {

            }
            else if(listView.SelectedItems.Count > 0)
            {
                //MessageBox.Show(listView.SelectedItem.ToString());
                var selectedListViewItem = listView.SelectedItem as ListViewItemModel;
                //MessageBox.Show(selectListViewItem.Name);
                listViewItem = new ListViewItemModel
                {
                    Name = selectedListViewItem.Name,
                    Extension = selectedListViewItem.Extension,
                    Size = selectedListViewItem.Size,
                    FullPath = selectedListViewItem.FullPath,
                    CreateDate = selectedListViewItem.CreateDate
                };
            }
        }
    }
}