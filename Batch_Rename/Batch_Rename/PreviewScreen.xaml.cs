using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.IO;

namespace Batch_Rename
{
    /// <summary>
    /// Interaction logic for PreviewScreen.xaml
    /// </summary>
    public partial class PreviewScreen : Window
    {
        private BindingList<Batch_Rename.MainWindow.FileName> _newFilenames;
        public PreviewScreen(BindingList<Batch_Rename.MainWindow.FileName> newFilenames)
        {
            InitializeComponent();
            _newFilenames = newFilenames;

            if (checkDuplicate() == false)
            {
                radioBtnRename.Visibility = Visibility.Hidden;
                radioBtnSame.Visibility = Visibility.Hidden;
                btnApply.Visibility = Visibility.Hidden;
                warning.Visibility = Visibility.Hidden;
            }
            lvsNewfilename.ItemsSource = _newFilenames;
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
           
            DialogResult = true;
            Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private bool checkDuplicate()
        {
            bool check = false;
            for (int i = 0; i < _newFilenames.Count() - 1; i++)
            {
                for (int j = i + 1; j < _newFilenames.Count(); j++)
                {
                    if (_newFilenames[i].New_Filename == _newFilenames[j].New_Filename)
                    {
                        check = true;
                        _newFilenames[i].Check = true;
                        _newFilenames[j].Check = true;
                    }
                }
            }

            for (int i = 0; i < _newFilenames.Count(); i++)
            {
                 for (int j = i; j < _newFilenames.Count(); j++)
                  {
                        if (_newFilenames[i].New_Filename == _newFilenames[j].New_Filename && _newFilenames[j].Filename != _newFilenames[j].New_Filename && _newFilenames[i].Check == true)
                        {
                            _newFilenames[i].Error = "Duplicate";
                        }
                   }
                }
            if (check == true)
                return true;
            else
                return false;
        }
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            for (int i = 0; i < _newFilenames.Count(); i++)
            {
                if (_newFilenames[i].Error == "Duplicate")
                {
                    if (radioBtnRename.IsChecked == true)
                    {
                        count++;
                        var tokens = _newFilenames[i].Path.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                        _newFilenames[i].Path = tokens[0] + count.ToString() + "." + tokens[1];
                        _newFilenames[i].New_Filename = System.IO.Path.GetFileName(_newFilenames[i].Path);
                        _newFilenames[i].Error = "";
                    }
                    if (radioBtnSame.IsChecked == true)
                    {
                        _newFilenames[i].Path = _newFilenames[i].Path.Replace(_newFilenames[i].New_Filename, _newFilenames[i].Filename);
                        _newFilenames[i].New_Filename = _newFilenames[i].Filename;
                        _newFilenames[i].Error = "";
                    }
                }
            }

          
        }
    }
}
