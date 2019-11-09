using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace Batch_Rename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<FileName> _filenames = new BindingList<FileName>();
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
                    From = "google",
                    To = "youtube"
                }
            };
            _prototypes.Add(replacePrototype);
            var newcasePrototype = new NewCaseOperation()
            {
                Args = new NewCaseArgs()
                {
                    TypeNewCase = "AllUpCase"
                }
            };
            _prototypes.Add(newcasePrototype);
            var uniquenamePrototype = new UniqueNameOperation();
            _prototypes.Add(uniquenamePrototype);
            var movePrototype = new MoveOperation();
            _prototypes.Add(movePrototype);

        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            createPrototype();
            methodComboBox.ItemsSource = _prototypes;
            methodsListBox.ItemsSource = _actions;
            lvsFilename.ItemsSource = _filenames;
        }
        private void btnAddMethod_Click(object sender, RoutedEventArgs e)
        {
            var action = methodComboBox.SelectedItem as StringOperation;
            _actions.Add(action.Clone());
        }
        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                _filenames.Add(new FileName() { Filename = System.IO.Path.GetFileName(openFile.FileName), Path = openFile.FileName });
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = methodsListBox.SelectedItem as StringOperation;
            item.Config();           
        }
        private void btnBatch_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _filenames.Count(); i++)
            {
                string filename= System.IO.Path.GetFileName(_filenames[i].Path);
                string path = _filenames[i].Path;
                for (int j=0; j<_actions.Count(); j++)
                {
                    filename = _actions[j].Operate(filename);
                }
                System.IO.File.Move(_filenames[i].Path, filename);
                _filenames[i].Path = path;
                _filenames[i].New_Filename = filename;
            }
        }

        private void BtnpresetLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog loadFileDiaLog = new OpenFileDialog();
            if (loadFileDiaLog.ShowDialog()==true)
            {
              
                string preset=File.ReadAllText(loadFileDiaLog.FileName);
                string[] operation = preset.Split('\n');
                
                for (int i=0;i<operation.Count()-1;i++)
                {
                    string[] typeopera = operation[i].Split(' ');
                    if (typeopera[0]== "UniqueName")
                    {
                        var item = new UniqueNameOperation();
                        _actions.Add(item);                 
                    }
                    else if (typeopera[0]=="NewCase")
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
                    else if (typeopera[0]== "Replace")
                    {
                        var item = new ReplaceOperation()
                        {
                            Args = new ReplaceArgs()
                            {
                                From = typeopera[1],
                                To = typeopera[2]
                            }
                        };
                        _actions.Add(item);
                    }
                    else if (typeopera[0]=="Move")
                    {
                        var item = new MoveOperation();
                        _actions.Add(item);
                    }
                }
            }
        }

        private void BtnpresetSave_Click(object sender, RoutedEventArgs e)
        {
            string preset = "";
            for (int i=0;i<_actions.Count();i++)
            {
                preset += _actions[i].Preset()+"\n";
            }
            SaveFileDialog saveFileDiaLog = new SaveFileDialog();
            if (saveFileDiaLog.ShowDialog()==true)
            {
                File.WriteAllText(saveFileDiaLog.FileName, preset);
            }
        }
    }

}

//public string Split(string separator, string str)
//{
//    var tokens = str.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
//    return tokens[1];
//}
