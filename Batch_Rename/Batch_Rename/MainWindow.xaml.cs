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
            private string _error;
            private string _state;
            private bool _check;

            public bool Check
            {
                get {return _check;}
                set {
                    _check = value;
                }
            }
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
            public string Error
            {
                get { return _error; }
                set
                {
                    _error = value;
                    Notify("Error");
                }
            }
            public string State
            {
                get { return _state; }
                set
                {
                    _state = value;
                    Notify("State");
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            public void Notify(string propertyName)
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
                    From = "google",
                    To = "youtube"
                }
            };
            var normalizePrototype = new NormalizeOperation();
            var newcasePototype = new NewCaseOperation()
            {
                Args = new NewCaseArgs()
                {
                    TypeNewCase = "AllUpCase"
                }
            };
            var movePrototype = new MoveOperation();
            var uniqueNamePrototype = new UniqueNameOperation();
            _prototypes.Add(replacePrototype);
            _prototypes.Add(newcasePototype);
            _prototypes.Add(normalizePrototype);
            _prototypes.Add(movePrototype);
            _prototypes.Add(uniqueNamePrototype);
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
            try
            {
                var action = methodComboBox.SelectedItem as StringOperation;
                _actions.Add(action.Clone());
            }
            catch (Exception ex)
            {
                string Error = "Can't add method without choosing method name";
                string Fix = "Please choose name of to add!";
                var errorDialog = new ErrorDialog(Error,Fix);
                errorDialog.ShowDialog();
            }
           
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
            if(_filenames.Count() != 0 )
            {
                BindingList<FileName> newFilenames = new BindingList<FileName>();
                BindingList<FileName> newFoldernames = new BindingList<FileName>();
                //Hien tren giao dien 
                if (tabcontrolAddfile.SelectedIndex == 0)
                {
                    for (int i = 0; i < _filenames.Count(); i++)
                    {
                        string final = _filenames[i].Filename;
                        for (int j = 0; j < _actions.Count(); j++)
                        {
                            final = _actions[j].Operate(final);                         
                        }
                      
                        newFilenames.Add(new FileName() { Filename = _filenames[i].Filename, New_Filename = final, Path = _filenames[i].Path.Replace(_filenames[i].Filename, final) });
                    }
                    var screen = new PreviewScreen(newFilenames);
                    if (screen.ShowDialog() == true)
                    {
                        BindingList<FileName> ReplateDuplicate = new BindingList<FileName>();
                        for (int i = 0; i < _filenames.Count(); i++)
                        {    
                                for (int j=i+1;j<_filenames.Count();j++)
                                {
                                    if (newFilenames[i].New_Filename==_filenames[j].Filename)
                                    {
                                    newFilenames[j].Path = newFilenames[j].Path.Replace(newFilenames[j].Filename, newFilenames[j].New_Filename);
                                    System.IO.File.Move(_filenames[j].Path, newFilenames[j].Path);
                                    _filenames[j].Filename = newFilenames[j].New_Filename;
                                    _filenames[j].Path = newFilenames[j].Path;
                                    _filenames[j].State = "Done";
                                    }
                                }
                                newFilenames[i].Path = newFilenames[i].Path.Replace(newFilenames[i].Filename, newFilenames[i].New_Filename);
                                System.IO.File.Move(_filenames[i].Path, newFilenames[i].Path);
                                _filenames[i].Filename = newFilenames[i].New_Filename;
                                _filenames[i].Path = newFilenames[i].Path;
                                _filenames[i].State = "Done";         
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < _foldernames.Count(); i++)
                    {
                        string final = _foldernames[i].Filename;
                        for (int j = 0; j < _actions.Count(); j++)
                        {
                            final = _actions[j].Operate(final);
                        }
                        newFoldernames.Add(new FileName() { Filename = _foldernames[i].Filename, New_Filename = final, Path = _foldernames[i].Path.Replace(_foldernames[i].Filename, final) });
                    }
                    var screen = new PreviewScreen(newFoldernames);
                    if (screen.ShowDialog() == true)
                    {
                        for (int i = 0; i < _foldernames.Count(); i++)
                        {
                            String temp = "G:\\temp";
                            newFoldernames[i].Path = newFoldernames[i].Path.Replace(newFoldernames[i].Filename, newFoldernames[i].New_Filename);
                            System.IO.Directory.Move(_foldernames[i].Path, temp);
                            System.IO.Directory.Move(temp, newFoldernames[i].Path);
                            _foldernames[i].Filename = newFoldernames[i].New_Filename;
                            _foldernames[i].Path = newFoldernames[i].Path;
                            _foldernames[i].State = "Done";
                        }
                    }
                }
            }
            else
            {
                string Error = "Can't start batch without choosing file or folder";
                string Fix = "Please choose filename or foldername!";
                var errorDialog = new ErrorDialog(Error, Fix);
                errorDialog.ShowDialog();
            }
           
        }

        private void BtnpresetLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog loadFileDiaLog = new OpenFileDialog();
            if (loadFileDiaLog.ShowDialog() == true)
            {

                string preset = File.ReadAllText(loadFileDiaLog.FileName);
                string[] operation = preset.Split('\n');
                txtPreset.Text = loadFileDiaLog.FileName;
                txtPreset.Visibility = System.Windows.Visibility.Visible;
                _actions.Clear();
                for (int i = 0; i < operation.Count() - 1; i++)
                {
                    string[] typeopera = operation[i].Split(' ');
                    if (typeopera[0] == "UniqueName")
                    {
                        var item = new UniqueNameOperation();
                        _actions.Add(item);
                    }
                    else if (typeopera[0] == "NewCase")
                    {
                        var item = new NewCaseOperation()
                        {
                            Args = new NewCaseArgs()
                            {
                                TypeNewCase = typeopera[1]
                            }
                        };
                        _actions.Add(item);
                    }
                    else if (typeopera[0] == "Replace")
                    {
                        var item = new ReplaceOperation()
                        {
                            Args = new ReplaceArgs()
                            {
                                From = typeopera[2],
                                To = typeopera[4]
                            }
                        };
                        _actions.Add(item);
                    }
                    else if (typeopera[0] == "Move")
                    {
                        var item = new MoveOperation();
                        _actions.Add(item);
                    }
                    else if(typeopera[0] == "Normalize")
                    {
                        var item = new NormalizeOperation();
                        _actions.Add(item);
                    }
                }
            }
        }
        private void BtnpresetSave_Click(object sender, RoutedEventArgs e)
        {
            if (_actions.Count() != 0)
            {
                string preset = "";
                for (int i = 0; i < _actions.Count(); i++)
                {
                    preset += _actions[i].Preset() + "\n";
                }
                SaveFileDialog saveFileDiaLog = new SaveFileDialog();
                if (saveFileDiaLog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDiaLog.FileName, preset);
                }
            }
            else
            {
                string Error = "Can't save preset without having method";
                string Fix = "Please add method!";
                var errorDialog = new ErrorDialog(Error, Fix);
                errorDialog.ShowDialog();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _filenames.Clear();
            _foldernames.Clear();
            _actions.Clear();
            _prototypes.Clear();
            txtPreset.Visibility = Visibility.Hidden;
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _actions.Clear();
            _prototypes.Clear();
            txtPreset.Visibility = Visibility.Hidden;
        }

        private void btnUpMethod_Click(object sender, RoutedEventArgs e)
        {
            var item = methodsListBox.SelectedItem as StringOperation;
            for (int i = 1; i < _actions.Count(); i++)
            {
                if (_actions[i] == item)
                {
                    var temp = _actions[i];
                    _actions[i] = _actions[i - 1];
                    _actions[i - 1] = temp;
                    _actions[i].Notify("");
                    break;
                }
            }
        }
        private void btnMaxUpMethod_Click(object sender, RoutedEventArgs e)
        {
            var item = methodsListBox.SelectedItem as StringOperation;
            for (int i = 0; i < _actions.Count(); i++)
            {
                if (_actions[i] == item)
                {
                    int j = i - 1;
                    int k = i;
                    while (j >= 0)
                    {
                        var temp = _actions[k];
                        _actions[k] = _actions[j];
                        _actions[j] = temp;
                        j--;
                        k--;
                    }
                    _actions[i].Notify("");
                    break;
                }
            }
        }
        private void btnMaxDownMethod_Click(object sender, RoutedEventArgs e)
        {
            var item = methodsListBox.SelectedItem as StringOperation;
            for (int i = 0; i < _actions.Count(); i++)
            {
                if (_actions[i] == item)
                {
                    int j = i + 1;
                    int k = i;
                    while (j < _actions.Count())
                    {
                        var temp = _actions[k];
                        _actions[k] = _actions[j];
                        _actions[j] = temp;
                        j++;
                        k++;
                    }
                    _actions[i].Notify("");
                    break;
                }
            }
        }
        private void btnDownMethod_Click(object sender, RoutedEventArgs e)
        {
            var item = methodsListBox.SelectedItem as StringOperation;
            for (int i = 0; i < _actions.Count() - 1; i++)
            {
                if (_actions[i] == item)
                {
                    var temp = _actions[i];
                    _actions[i] = _actions[i + 1];
                    _actions[i + 1] = temp;
                    _actions[i].Notify("");
                    break;
                }
            }
        }
        private void btnDeleteMethod_Click(object sender, RoutedEventArgs e)
        {
            var item = methodsListBox.SelectedItem as StringOperation;
            _actions.Remove(item);
        }
        private void methodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }

}

