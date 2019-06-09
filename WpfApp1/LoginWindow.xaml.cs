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
    /// Interakční logika pro Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public int TextInBox { get; set; }
        public int AccId { get; set; }
        public string AccName { get; set; }
        public string AccCurr { get; set; }
        public bool NewAccPressed { get; set; }



        public Window1()
        {
            InitializeComponent();
            NewAccPressed = false;           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextInBox = int.Parse(textBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.DialogResult = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var newAccWindow = new NewAccWindow();
            if (newAccWindow.ShowDialog() == true)
            {
                AccId = int.Parse(newAccWindow.AccIdBox.Text);
                AccName = newAccWindow.NameBox.Text;
                AccCurr = newAccWindow.CurrBox.Text;
            }
            NewAccPressed = true;
            this.DialogResult = true;
        }
    }
}
