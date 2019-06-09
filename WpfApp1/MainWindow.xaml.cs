using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TradingAppLibrary.Manager;

namespace WpfApp1
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //List<Account> AccList { get; set; }


        IAppManager manager;

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> ItemList { get; set; }
        private int defaultValue = 0;


        public MainWindow()
        {
            InitializeComponent();

            manager = new AppManager();

            if (Login())
            {
                this.DataContext = this;

                itemsListBox.ItemsSource = manager.GetItemList();

                SeriesCollection = new SeriesCollection
                {
                new LineSeries ( new ChartValues<double> (manager.GetRunningProfiInCurrency()))
                };

                resultChart.Series = SeriesCollection;
                UpdateAll();

            }
            else
            {
                this.Close();
            }               
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Web pages (*.htm)|*.htm|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;

                try
                {
                   manager.ParseHtml(selectedFileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    UpdateAll();
                }
            }

        }

        private bool Login()
        {
            Window1 window1 = new Window1();
            if (window1.ShowDialog() == true)
            {
                if (window1.NewAccPressed)
                {
                    manager.AddAcc(window1.AccId, window1.AccName, window1.AccCurr);
                }
                else
                {
                    var acc = window1.TextInBox;
                    try
                    {
                        manager.ChangeAcc(acc);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return Login();
                    }
                }
            }
            return window1.DialogResult.Value;
        }

        private void TradeGridFill()
        {
            TradeGrid.Items.Clear();

            foreach (var trade in manager.FilteredTradeList)
            {
                var tradeArray = trade.Split(';');
                TradeGrid.Items.Add(new
                {
                    TradeId = tradeArray[0],
                    OpenTime = tradeArray[1],
                    Type = tradeArray[2],
                    Size = tradeArray[3],
                    Item = tradeArray[4],
                    OpenPrice = tradeArray[5],
                    StopLoss = tradeArray[6],
                    ProfitTarget = tradeArray[7],
                    CloseTime = tradeArray[8],
                    ClosePrice = tradeArray[9],
                    Commission = tradeArray[10],
                    Swap = tradeArray[11],
                    Profit = tradeArray[12]
                });
            }
        }

        private void StackPanelFill()
        {
            tradesTotal.Text = manager.GetFilteredTradeSum().ToString();
            sizeTotal.Text = manager.GetFilteredSizeSum().ToString();
            commisionTotal.Text = manager.GetFilteredCommisionSum().ToString();
            swapTotal.Text = manager.GetFilteredSwapSum().ToString();
            profitTotal.Text = manager.GetFilteredProfitSum().ToString();          
        }

        private void StatisticsFill()
        {
            statBalance.Text = (manager.GetTotalProfitSum() + defaultValue).ToString("0.##");
            statTotTrades.Text = manager.GetTotalTradeSum().ToString("0.##");
            statShortWon.Text = manager.GetShortWon();
            statLongWon.Text = manager.GetLongWon();
            statLargestProfit.Text = manager.GetLargestProfit().ToString("0.##");
            statLargestLoss.Text = manager.GetLargestLoss().ToString("0.##");
            statProfitTotal.Text = manager.GetProfitTotal();
            statLossTotal.Text = manager.GetLossTotal();
            statAverageLoss.Text = manager.GetAverageLoss().ToString("0.##");
            statAverageProfit.Text = manager.GetAverageProfit().ToString("0.##");
            statRRR.Text = manager.GetRRR().ToString("0.##");
            statProfitFactor.Text = manager.GetProfitFactor().ToString("0.##");
        }

        private void UpdateAll()
        {
            AccountIdTextBlock.Text = manager.CurrentAcc.ToString();
            depositTextBlock.Text = manager.Deposited.ToString();

            TradeGridFill();
            itemsListBox.ItemsSource = manager.GetItemList();
            SeriesCollection[0].Values = new ChartValues<double>(manager.GetRunningProfiInCurrency());
            StackPanelFill();
            StatisticsFill();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (manager.Logged)
            {
                manager.SaveData();
            }
        }

        

        private void TgLayoutUpdated(object sender, EventArgs e)
        {
            Thickness t = tradesTotal.Margin;
            t.Left = 0;

            tradesTotal.Margin = t;
            tradesTotal.Width = TradeGrid.Columns[0].ActualWidth;
       
            sizeTotal.Width = TradeGrid.Columns[1].ActualWidth 
                + TradeGrid.Columns[2].ActualWidth 
                + TradeGrid.Columns[3].ActualWidth;

            commisionTotal.Width = TradeGrid.Columns[4].ActualWidth
                + TradeGrid.Columns[5].ActualWidth
                + TradeGrid.Columns[6].ActualWidth
                + TradeGrid.Columns[7].ActualWidth
                + TradeGrid.Columns[8].ActualWidth
                + TradeGrid.Columns[9].ActualWidth
                + TradeGrid.Columns[10].ActualWidth;

            swapTotal.Width = TradeGrid.Columns[11].ActualWidth;

            profitTotal.Width = TradeGrid.Columns[12].ActualWidth;

        }

        private void FilterButtonClick(object sender, RoutedEventArgs e)
        {
            DateTime from, to;
            var selectedItems = new List<string> { };
            
            if (itemsListBox.SelectedItems.Count > 0)
            {
                foreach (var item in itemsListBox.SelectedItems)
                {
                    selectedItems.Add(item as string);
                }
            }


            if (fromDate.SelectedDate.HasValue)
            {
                from = fromDate.SelectedDate.Value;
            }
            else
            {
                from = DateTime.MinValue;
            }

            if (toDate.SelectedDate.HasValue)
            {
                to = toDate.SelectedDate.Value;

            }
            else
            {
                to = DateTime.Now;
            }

            manager.FilterTradeList(selectedItems, from, to, dirComboBox.SelectionBoxItem.ToString());
            UpdateAll();
        }

        private void ResultChart_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            
        }

        private void MouseDoubleClickOpenTradeInfo(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void CurrencyChart(object sender, RoutedEventArgs e)
        {
            SeriesCollection[0].Values = new ChartValues<double>(manager.GetRunningProfiInCurrency());
        }

        private void PipsChart(object sender, RoutedEventArgs e)
        {
            SeriesCollection[0].Values = new ChartValues<double>(manager.GetRunningProfitInPips());
        }

        private void PercentageChart(object sender, RoutedEventArgs e)
        {
            try
            {
                SeriesCollection[0].Values = new ChartValues<double>(manager.GetRunningProfitInPercent());
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("There is no deposit.");
            }
        }
        

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            if (window1.ShowDialog() == true)
            {
                if (window1.NewAccPressed)
                {
                    manager.AddAcc(window1.AccId, window1.AccName, window1.AccCurr);
                }
                else
                {
                    var acc = window1.TextInBox;
                    try
                    {
                        manager.ChangeAcc(acc);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            UpdateAll();
        }
    }
}
