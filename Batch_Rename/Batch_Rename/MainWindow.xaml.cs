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
using System.IO;
using System.ComponentModel;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;



namespace Batch_Rename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<FileName> _filenames = new BindingList<FileName>();
        BindingList<FileName> _foldernames = new BindingList<FileName>();
        BindingList<StringOperation> _actions = new BindingList<StringOperation>();
        List<StringOperation> _prototypes = new List<StringOperation>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public class FileName :INotifyPropertyChanged
        {
            private string _filename;
            private string _new_filename;
            private string _path;

            public string Filename 
            {
                get { return _filename; } 
                set 
                {
                    _filename = value;
                    Notify("Filename");
                }
            }
            public string New_Filename {
                get { return _new_filename; }
                set
                {
                    _new_filename = value;
                    Notify("New_Filename");
                }
            }
            public string Path { 
                get { return _path; }
                set
                {
                    _path = value;
                    Notify("Path");
                }
            }
            public string error { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            private void Notify(string propertyName)
            {
                if(PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        private void createPrototype()
        {
            var replacePrototype = new ReplaceOperation()
            {
                Args = new ReplaceArgs()
                {
                    From = "youtube",
                    To = "google"
                }
            };
            _prototypes.Add(replacePrototype);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            createPrototype();
            methodComboBox.ItemsSource = _prototypes;
            methodsListBox.ItemsSource = _actions;
            lvsFilename.ItemsSource = _filenames;
            lvsFoldername.ItemsSource = _foldernames;
        }
        private void btnAddMethod_Click(object sender, RoutedEventArgs e)
        {
            var action = methodComboBox.SelectedItem as StringOperation;
            _actions.Add(action.Clone());
        }
        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFolder = new CommonOpenFileDialog();
            openFolder.IsFolderPicker = true;

            if (openFolder.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string[] filePaths = Directory.GetFiles(openFolder.FileName);
                foreach(string filePath in filePaths)
                    _filenames.Add(new FileName() { Filename = System.IO.Path.GetFileName(filePath), Path = filePath });
            }
        }
        private void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFolder = new CommonOpenFileDialog();
            openFolder.IsFolderPicker = true;

            if (openFolder.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string[] filePaths = Directory.GetDirectories(openFolder.FileName);
                foreach (string filePath in filePaths)
                    _foldernames.Add(new FileName() { Filename = System.IO.Path.GetFileName(filePath), Path = filePath });
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = methodsListBox.SelectedItem as StringOperation;
            item.Config();           
        }
        private void btnBatch_Click(object sender, RoutedEventArgs e)
        {
            if (_filenames.Count() >= 0)
            {
                for (int i = 0; i < _filenames.Count(); i++)
                {
                    string final = _filenames[i].Path;
                    for (int j = 0; j < _actions.Count(); j++)
                    {
                        final = _actions[j].Operate(final);
                    }
                    System.IO.File.Move(_filenames[i].Path, final);
                    _filenames[i].Path = final;
                    _filenames[i].New_Filename = System.IO.Path.GetFileName(final);
                }
            }

            if (_foldernames.Count() >= 0)
            {
                for (int i = 0; i < _foldernames.Count(); i++)
                {
                    string final = _foldernames[i].Path;
                    for (int j = 0; j < _actions.Count(); j++)
                    {
                        final = _actions[j].Operate(final);
                    }
                    System.IO.Directory.Move(_foldernames[i].Path, final);
                    _foldernames[i].Path = final;
                    _foldernames[i].New_Filename = System.IO.Path.GetFileName(final);
                }
            }
           
        }

    }

}

//public string Split(string separator, string str)
//{
//    var tokens = str.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
//    return tokens[1];
//}
