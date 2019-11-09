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

namespace Batch_Rename
{
    /// <summary>
    /// Interaction logic for NewCaseConfigDialog.xaml
    /// </summary>
    public partial class NewCaseConfigDialog : Window
    {
        NewCaseArgs myArgs;
        BindingList<StringArgs> newCase = new BindingList<StringArgs>();
        public NewCaseConfigDialog(StringArgs args)
        {
            
            InitializeComponent();
            myArgs = args as NewCaseArgs;



        }
        private void createCase()
        {

            var item1 = new NewCaseArgs
            {

                TypeNewCase = "AllUpCase"


            };
           
            newCase.Add(item1);

            var item2 = new NewCaseArgs
            {

                TypeNewCase = "AllLowCase"

            };
            
            newCase.Add(item2);
            var item3 = new NewCaseArgs()
            {


                TypeNewCase = "FistWordUpCase"


            };
            newCase.Add(item3);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            createCase();
            newcaseComboBox.ItemsSource = newCase;
           
        }

        private void NewcasebtnOk_Click(object sender, RoutedEventArgs e)
        {
            myArgs.TypeNewCase = (newcaseComboBox.SelectedItem as NewCaseArgs).TypeNewCase;
            DialogResult = true;
            Close();
        }

        private void NewcasebtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
