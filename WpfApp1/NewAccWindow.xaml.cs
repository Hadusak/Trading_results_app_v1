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

namespace WpfApp1
{
    /// <summary>
    /// Interakční logika pro Window2.xaml
    /// </summary>
    public partial class NewAccWindow : Window
    {
        public NewAccWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AccIdBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int.Parse(AccIdBox.Text);
            }
            catch
            {
                MessageBox.Show("Account number must be a number! Use id of your trading account.");
            }

        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                
            }
            catch
            {
                
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
