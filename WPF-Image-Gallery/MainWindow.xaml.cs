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

        public MainWindow()
        {
            InitializeComponent();

            ListViewItemModels = new ObservableCollection<ListViewItemModel>();
            
            retrieveFolders();
            listView.DataContext = this;

            //set detail block to collapsed
            borderDetails.Visibility = Visibility.Collapsed;
            ((ColumnDefinition)gridRow2.ColumnDefinitions[4]).Width = new GridLength(0);
        }

        private void tbSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbSearch.Text == "Search")
            {
                tbSearch.Text = string.Empty;
                tbSearch.Foreground = Brushes.Black;
            }
        }

        private void tbSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbSearch.Text == "")
            {
                tbSearch.Text = "Search";
                tbSearch.Foreground = Brushes.Gray;
            }
        }

        private void recursiveRetrievingFolder(string mainFolderName, TreeViewItemModel item)
        {
            List<string> subFolderNames = ioManager.GetFolders(mainFolderName);
            foreach (string subFolderName in subFolderNames)
            {
                TreeViewItemModel subItemModel = new TreeViewItemModel();
                subItemModel.Icon = rootPath + "folder (2).png";
                subItemModel.Name = subFolderName;

                item.Children.Add(subItemModel);               

                string path = System.IO.Path.Combine(mainFolderName, subFolderName);
                recursiveRetrievingFolder(path, subItemModel);
            }
        }
      
        private void retrieveFolders() //retrieve starts from drive of computer
        {
            string[] drives = Environment.GetLogicalDrives();
            foreach (string drive in drives)
            {
                TreeViewItemModel treeViewItemModel = new TreeViewItemModel();
                treeViewItemModel.Icon = rootPath + "hard-drive.png";
                treeViewItemModel.Name = drive;

                itemModels.Add(treeViewItemModel);

                folderNames = ioManager.GetFolders(drive); //get folders from each drive
                for (int i = 0; i < folderNames.Count; i++)
                {
                    string mainFolderName = folderNames[i];

                    TreeViewItemModel folderItemModel = new TreeViewItemModel();
                    folderItemModel.Icon = rootPath + "folder (2).png";
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

        //single click on treeView item, shows images on listView
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

                    string folderName = System.IO.Path.GetFileName(fullPath); //last folder name

                    //read from file first
                    List<ListViewItemModel> files = ioManager.Read<List<ListViewItemModel>>(folderName);

                    if (files == null) //if files doesn't exist, 
                    {
                        List<ListViewData> filesData = ioManager.GetFiles(fullPath); //get files from computer directly
                        foreach (ListViewData file in filesData)
                        {
                            ListViewItemModels.Add(new ListViewItemModel { Icon = rootPath + "image.png", Name = getName(file), Source = getName(file), Extension = file.Extension, Size = file.Size, CreateDate = file.CreateDate, CreateTime = file.CreateTime, FullPath = fullPath });
                        }
                        ioManager.Write(folderName, ListViewItemModels); //and write to file, so next time no need to read from computer
                    }
                    else //if file exist, read from file
                    {
                        //files = files.OrderByDescending(f => f.Name).Reverse().ToList();
                        foreach (ListViewItemModel file in files)
                        {
                            ListViewItemModels.Add(file);
                        }
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

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) //double click on listView file, shows image in full screen
            {
                if (sender is Grid grid && grid.DataContext is ListViewItemModel item)
                {
                    string fullPath = System.IO.Path.Combine(item.FullPath, item.Source + item.Extension);
                    frmViewImage frmViewImage = new frmViewImage(item.Name, fullPath, tbPath.Text);
                    frmViewImage.ShowDialog();
                }
            }
            else if (e.ClickCount == 1) //single click, will show info in details block, if it's visible
            {
                if (sender is Grid grid && grid.DataContext is ListViewItemModel item)
                {
                    string root = System.IO.Path.Combine(rootPath, item.FullPath, item.Source + item.Extension);
                    photo.Source = new BitmapImage(new Uri(root));
                    tbFullPath.Text = tbPath.Text;
                    gridDetails.DataContext = item;
                }
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbSearch.Text == "Search" || string.IsNullOrEmpty(cmbSearchType.Text))
            {
                return;
            }

            string selectedSearchType = cmbSearchType.Text;
            string path = System.IO.Path.GetFileName(tbPath.Text); //last folder name 
            string searchWord = tbSearch.Text.ToLower();

            //List<ListViewData> allFiles = ioManager.GetFiles(path); //get all files name from that folder
            //List<ListViewData> filterFiles = new List<ListViewData>();

            List<ListViewItemModel> allFiles = ioManager.Read<List<ListViewItemModel>>(path);
            List<ListViewItemModel> filterFiles = new List<ListViewItemModel>();

            if (allFiles != null)
            {
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
        }

        private void updateListView(List<ListViewItemModel> filterFiles, string path)
        {
            ListViewItemModels.Clear(); // Clear existing items in the ListView

            foreach (var file in filterFiles)
            {
                ListViewItemModels.Add(file);
                //ListViewItemModels.Add(new ListViewItemModel { Icon = rootPath + "image.png", Name = getName(file), Extension = file.Extension, Size = file.Size, CreateDate = file.CreateDate, CreateTime = file.CreateTime, FullPath = path });
            }
        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            if(borderDetails.Visibility == Visibility.Collapsed)
            {
                borderDetails.Visibility = Visibility.Visible;
                ((ColumnDefinition)gridRow2.ColumnDefinitions[4]).Width = new GridLength(0.35, GridUnitType.Star); 

                if(listView.SelectedItem is ListViewItemModel item)
                {
                    borderDetails.DataContext = item;
                }
                else if(listView.SelectedItem == null)
                {
                    borderDetails.DataContext = new ListViewItemModel
                    {
                        Icon = rootPath + "folder.png",
                        FullPath = tbPath.Text,
                        Name = tbPath.Text
                    };
                }
            }
            else
            {
                borderDetails.Visibility = Visibility.Collapsed;
                ((ColumnDefinition)gridRow2.ColumnDefinitions[4]).Width = new GridLength(0);
            }
        }

        private void listView_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string[] fileExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp", ".svg" };

                for (int i=0; i<files.Length; i++)
                {
                    FileInfo file = new FileInfo(files[i]);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(files[i]);
                    string extension = file.Extension;
                    double size = Math.Ceiling(file.Length / 1024.0);
                    string fullPath = file.DirectoryName;

                    DateTime createDate = file.CreationTime;
                    string date = createDate.Date.ToShortDateString();
                    string time = createDate.ToString("HH:mm");

                    if(!fileExtensions.Contains(extension)) { return; } //if it's not image file, can't drop

                    ListViewItemModels.Add(new ListViewItemModel { Icon = rootPath + "image.png", Name = fileName, Source = fileName,
                                                                    Extension = extension, Size = size, 
                                                                    CreateDate = date, CreateTime = time, 
                                                                    FullPath = fullPath });
                }

                string folderName = System.IO.Path.GetFileName(tbPath.Text);
                ioManager.Write(folderName, ListViewItemModels);
            }
        }

        private string copiedImagePath;

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if(listView.SelectedItem is ListViewItemModel item)
            {
                copiedImagePath = System.IO.Path.Combine(item.FullPath, item.Source + item.Extension);
            }
            else if(listView.SelectedItem == null)
            {
                return;
            }
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            if(copiedImagePath != null)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(copiedImagePath);
                FileInfo file = new FileInfo(copiedImagePath);
                double size = Math.Ceiling(file.Length / 1024.0);
                string fullPath = file.DirectoryName;

                DateTime createDate = file.CreationTime;
                string date = createDate.Date.ToShortDateString();
                string time = createDate.ToString("HH:mm");

                List<ListViewItemModel> itemList = ioManager.Read<List<ListViewItemModel>>(System.IO.Path.GetFileName(tbPath.Text));

                ListViewItemModel item = itemList.Where(i => i.Name == name).FirstOrDefault();

                if(item == null)
                {
                    ListViewItemModels.Add(new ListViewItemModel { Icon = rootPath + "image.png", Name = name, Source = name, Extension = file.Extension, Size = size, CreateDate = date, CreateTime = time, FullPath = fullPath });
                }
                else
                {
                    ListViewItemModels.Add(new ListViewItemModel { Icon = rootPath + "image.png", Name = name + "-Copy", Source = name, Extension = file.Extension, Size = size, CreateDate = date, CreateTime = time, FullPath = fullPath });
                }

                //itemModel.Name = itemModel.Source + "-Copy";
                //ListViewItemModels.Add(itemModel);
                ioManager.Write(System.IO.Path.GetFileName(tbPath.Text), ListViewItemModels);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(listView.SelectedItem is ListViewItemModel item)
            {
                ListViewItemModels.Remove(item);
                ioManager.Write(System.IO.Path.GetFileName(tbPath.Text), ListViewItemModels);
            }
            else if(listView.SelectedItem == null)
            {
                return;
            }
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            if(listView.SelectedItem is ListViewItemModel item)
            {
                copiedImagePath = System.IO.Path.Combine(item.FullPath, item.Source + item.Extension);
                ListViewItemModels.Remove(item);
                ioManager.Write(System.IO.Path.GetFileName(tbPath.Text), ListViewItemModels);
            }
            else if(listView.SelectedItem == null)
            {
                return;
            }
        }

        private void btnFormat_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(cmbFormatType.Text) || listView.SelectedItem == null)
            {
                return;
            }

            if(listView.SelectedItem is ListViewItemModel item)
            {
                string selectedFormatType = cmbFormatType.Text;
                string path = System.IO.Path.Combine(item.FullPath, item.Source + item.Extension); //original path
                string outputPath = System.IO.Path.Combine(item.FullPath, item.Source + "." + selectedFormatType.ToLower()); //after format path
                //MessageBox.Show(path);

                BitmapImage bitmap = new BitmapImage(new Uri(path, UriKind.Absolute));
                BitmapEncoder encoder;

                switch(selectedFormatType)
                {
                    case "PNG": encoder = new PngBitmapEncoder(); break;
                    case "GIF": encoder = new GifBitmapEncoder(); break;
                    case "JPEG": encoder = new JpegBitmapEncoder(); break;
                    case "TIFF": encoder = new TiffBitmapEncoder(); break;
                    case "BMP": encoder = new BmpBitmapEncoder(); break;
                    case "WMP": encoder = new WmpBitmapEncoder(); break;
                    default: throw new ArgumentException("Unsupported format");
                }

                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using (var fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    encoder.Save(fileStream); //save to computer
                }

                FileInfo file = new FileInfo(outputPath);
                double size = Math.Ceiling(file.Length / 1024.0);

                DateTime createDate = file.CreationTime;
                string date = createDate.Date.ToShortDateString();
                string time = createDate.ToString("HH:mm");

                ListViewItemModels.Add(new ListViewItemModel { Icon = rootPath + "image.png", Name = item.Name, Source = item.Name, Extension = "." + selectedFormatType.ToLower(), Size = size, CreateDate = date, CreateTime = time, FullPath = item.FullPath });
                ioManager.Write(System.IO.Path.GetFileName(tbPath.Text), ListViewItemModels);
            }
        }
    }
}