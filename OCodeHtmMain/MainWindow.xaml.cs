using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OCodeHtmMain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string TITLE = "OCodeHTM Demo";

        public MainWindow()
        {
            InitializeComponent();

            this.Title = TITLE;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature not yet implemented!", TITLE, MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
    }
}
