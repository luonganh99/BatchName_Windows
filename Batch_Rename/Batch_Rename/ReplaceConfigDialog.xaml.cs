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
using System.Windows.Shapes;

namespace Batch_Rename
{
    /// <summary>
    /// Interaction logic for ReplaceConfigDialog.xaml
    /// </summary>
    public partial class ReplaceConfigDialog : Window
    {
        ReplaceArgs myArgs; 
        public ReplaceConfigDialog(StringArgs args)
        {
            InitializeComponent();
            myArgs = args as ReplaceArgs;
            fromTextBox.Text = myArgs.From;
            toTextBox.Text = myArgs.To;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            myArgs.From = fromTextBox.Text;
            myArgs.To = toTextBox.Text;
            DialogResult = true;
            Close();
        }
    }
}
