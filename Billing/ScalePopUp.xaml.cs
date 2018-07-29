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

namespace Billing
{
    /// <summary>
    /// Interaction logic for ScalePopUp.xaml
    /// </summary>
    public partial class ScalePopUp : Window
    {
        public ScalePopUp()
        {
            InitializeComponent();
        }

        private void btn_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox.Text = string.Empty;
        }

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
