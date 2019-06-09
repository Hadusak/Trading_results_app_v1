using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingAppLibrary.Constants;
using TradingAppLibrary.DataClasses;
using TradingAppLibrary.DataManipulation;

namespace TradingAppLibrary.Manager
{
    /// <summary>
    /// The main manager. Contains methods for communicating between frontend and backend.
    /// Methods return values for grafic interface.
    /// </summary>
    public class AppManager : IAppManager
    {


        Account currentAcc;

        public List<string> TradeList { get => currentAcc?.TradeList.Select(a=>a.ToString()).ToList(); }
        public List<string> FilteredTradeList { get => currentAcc?.FilteredTradeList.Select(a => a.ToString()).ToList(); }
        public double Deposited { get => currentAcc.Deposited; set => currentAcc.Deposited = value; }
        public int CurrentAcc => currentAcc.AccID;
        public bool Logged => currentAcc != null ? true : false;


        /// <summary>
        /// Constructor
        /// </summary>
        public AppManager()
        {
          // Is there something what i can do in constructor?
        }

        /// <summary>
        /// Method which use HtmlParser to parse a trading statement. 
        /// Data are loaded to currentAcc.tradeList.
        /// </summary>
        /// <param name="fileName"></param>
        public void ParseHtml(string fileName)
        {
            if (HtmlParser.CheckAcc(currentAcc, fileName))
            {

                foreach(var trade in Task.Run(() => HtmlParser.ParseTradesFromHTML(fileName)).Result)
                    {
                        if (!currentAcc.TradeList.Exists(t => t.TradeId.Equals(trade.TradeId)))
                        {
                            currentAcc.TradeList.Add(trade);
                        }
                    };
                Task.Run(() => currentAcc.Deposited += HtmlParser.LoadDepositValues(fileName));
            }
            else
            {
                throw new Exception("Account number from file is not same as this acc.");
            }

            currentAcc.FilteredTradeList = currentAcc.TradeList;
            

        }
        /// <summary>
        /// Saving data to .txt file
        /// </summary>
        public void SaveData()
        {
            Task.Run(() => DataManipulator.SaveData(currentAcc));
            Task.Run(() => DataManipulator.SaveAccount(currentAcc));
        }

        /// <summary>
        /// Cumulative profit for chart in currency.
        /// </summary>
        /// <returns>List of values (cumulative profit) used for chart.</returns>
        public List<double> GetRunningProfiInCurrency()
        {
            var result = new List<double> { currentAcc.Deposited };

            foreach (var trade in currentAcc.FilteredTradeList)
            {
                result.Add(result.Last() + trade.Profit);
            }

            return result;
        }

        /// <summary>
        /// return number of trades in filteredList
        /// </summary>
        /// <returns> int - number of trades </returns> 
        public int GetFilteredTradeSum()
        {
            return currentAcc.FilteredTradeList.Count;
        }

        /// <summary>
        /// it counts sum of sizes in filteredList
        /// </summary>
        /// <returns> double - sum of trades sizes</returns>
        public double GetFilteredSizeSum()
        {
            return currentAcc.FilteredTradeList.Sum(a => a.Size);
        }

        /// <summary>
        /// it counts sum of commisions in filteredList
        /// </summary>
        /// <returns></returns>
        public double GetFilteredCommisionSum()
        {
            return currentAcc.FilteredTradeList.Sum(a => a.Commission);
        }

        /// <summary>
        /// it counts sum of swaps in filteredList
        /// </summary>
        /// <returns></returns>
        public double GetFilteredSwapSum()
        {
            return currentAcc.FilteredTradeList.Sum(a => a.Swap);
        }

        /// <summary>
        /// it counts sum of profits in filteredList
        /// </summary>
        /// <returns></returns>
        public double GetFilteredProfitSum()
        {
            return currentAcc.FilteredTradeList.Sum(a => a.Profit);
        }

        /// <summary>
        /// It returns list of currencies (Every currency only once) in tradelist
        /// </summary>
        /// <returns></returns>
        public List<string> GetItemList()
        {
            return currentAcc.TradeList.Select(a => a.Item).Distinct().OrderBy(a=>a).ToList();
        }

        /// <summary>
        /// It filters tradelist by arguments (params)
        /// </summary>
        /// <param name="selectedItems"> Currency </param>
        /// <param name="from"> time - from </param>
        /// <param name="to"> time = to </param>
        /// <param name="direction"> Buy / Sell or both </param>
        public void FilterTradeList(List<string> selectedItems, DateTime from, DateTime to, string direction)
        {
            currentAcc.FilteredTradeList = currentAcc.TradeList;

            if (selectedItems.Count > 0)
            {
                currentAcc.FilteredTradeList = currentAcc.FilteredTradeList.Where(a => selectedItems.Contains(a.Item)).ToList();
            }

            currentAcc.FilteredTradeList = currentAcc.FilteredTradeList.Where(a => a.OpenTime >= from && a.OpenTime <= to).ToList();

            if (direction == "Sell" || direction == "Buy")
            {
                currentAcc.FilteredTradeList = currentAcc.FilteredTradeList.Where(a => a.Type == direction.ToLower()).ToList();
            }
            
        }

        /// <summary>
        /// It counts sum of profits in tradelist
        /// </summary>
        /// <returns></returns>
        public double GetTotalProfitSum()
        {
            return currentAcc.TradeList.Sum(a => a.Profit);
        }

        /// <summary>
        /// It counts number of trades in tradelist
        /// </summary>
        /// <returns></returns>
        public int GetTotalTradeSum()
        {
            return currentAcc.TradeList.Count;
        }

        /// <summary>
        /// It counts how many short (sell) trades are winning (in profit).
        /// </summary>
        /// <returns></returns>
        public string GetShortWon()
        {
            int shortCount = currentAcc.TradeList.Where(trade => trade.Type == "sell").Count();
            int shortWon = currentAcc.TradeList.Where(trade => trade.Type == "sell" && trade.Profit >= 0).Count();
            return shortCount == 0 ? "NaN" : $"{shortCount} ({shortWon * 100 / shortCount}%)";
        }

        /// <summary>
        /// It counts how many long (buy) trades are winning (in profit).
        /// </summary>
        /// <returns></returns>
        public string GetLongWon()
        {
            int longCount = currentAcc.TradeList.Where(trade => trade.Type == "buy").Count();
            int longWon = currentAcc.TradeList.Where(trade => trade.Type == "buy" && trade.Profit > 0).Count();
            return longCount == 0 ? "NaN" : $"{longCount} ({longWon * 100 / longCount}%)";
        }

        /// <summary>
        /// It finds largest loss in currency.
        /// </summary>
        /// <returns></returns>
        public double GetLargestLoss()
        {
            return currentAcc.TradeList.Count() == 0 ? 0 : currentAcc.TradeList.Min(trade => trade.Profit);
        }

        /// <summary>
        /// it finds largest profit in currency.
        /// </summary>
        /// <returns></returns>
        public double GetLargestProfit()
        {
            return currentAcc.TradeList.Count() == 0 ? 0 : currentAcc.TradeList.Max(trade => trade.Profit);
        }

        /// <summary>
        /// it counts averaeg loss in currency
        /// </summary>
        /// <returns></returns>
        public double GetAverageLoss()
        {
            return currentAcc.TradeList.Count() == 0 ? 0 : currentAcc.TradeList.Where(trade => trade.Profit < 0).Average(trade => trade.Profit);
        }

        /// <summary>
        /// it counts averaeg profit in currency
        /// </summary>
        /// <returns></returns>
        public double GetAverageProfit()
        {
            return currentAcc.TradeList.Count() == 0 ? 0 : currentAcc.TradeList.Where(trade => trade.Profit >= 0).Average(trade => trade.Profit);
        }

        /// <summary>
        /// it counts total profit in currency. If tradelist is empty then NaN.
        /// </summary>
        /// <returns></returns>
        public string GetProfitTotal()
        {
            int inProfit = currentAcc.TradeList.Where(trade => trade.Profit >= 0).Count();
            int total = GetTotalTradeSum();

            return total == 0 ? "NaN" : $"{inProfit} ({inProfit * 100 / total}%)";
        }

        /// <summary>
        /// it counts total loss in currency. If tradelist is empty then NaN.
        /// </summary>
        /// <returns></returns>
        public string GetLossTotal()
        {
            int inLoss = currentAcc.TradeList.Where(trade => trade.Profit < 0).Count();
            int total = GetTotalTradeSum();

            return total == 0 ? "NaN" : $"{inLoss} ({inLoss * 100 / total}%)";
        }

        /// <summary>
        /// It counts total risk reward ratio (RRR). Average profit / average loss.
        /// </summary>
        /// <returns></returns>
        public double GetRRR()
        {
            return currentAcc.TradeList.Count() == 0 ? 0 : Math.Abs(GetAverageProfit() / GetAverageLoss());
        }

        /// <summary>
        /// It counts profit factor. Sum of profits / sum of losses
        /// </summary>
        /// <returns></returns>
        public double GetProfitFactor()
        {
            double profit = currentAcc.TradeList.Where(trade => trade.Profit >= 0).Sum(trade => trade.Profit);
            double loss = currentAcc.TradeList.Where(trade => trade.Profit < 0).Sum(trade => trade.Profit);

            return currentAcc.TradeList.Count() == 0 ? 0 : Math.Abs(profit / loss);
        }

        /// <summary>
        /// Cumulative profit for chart in pips (standart unit in forex).
        /// </summary>
        /// <returns></returns>
        public List<double> GetRunningProfitInPips()
        {
            var result = new List<double> { 0 };

            foreach (var trade in currentAcc.FilteredTradeList)
            {
                double curr = 0;

                if (trade.Item.Contains("jpy"))
                {
                    curr = Math.Round(Math.Abs(trade.OpenPrice*100 - trade.ClosePrice*100),1);
                }
                else
                {
                    curr = Math.Round(Math.Abs(trade.OpenPrice * 10000 - trade.ClosePrice * 10000),1);
                }
                if (trade.Profit < 0)
                {
                    curr = -curr;
                }
                result.Add(result.Last() + curr);
            }

            return result;
        }

        /// <summary>
        /// Cumulative profit for chart in percent
        /// </summary>
        /// <returns></returns>
        public List<double> GetRunningProfitInPercent()
        {
            if (currentAcc.Deposited == 0)
            {
                throw new ArgumentException("You must set Deposit value for percentage chart.");
            }

            var result = new List<double> { 0 };

            foreach (var trade in currentAcc.FilteredTradeList)
            {
                result.Add(result.Last() + trade.Profit *100 / currentAcc.Deposited);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void ChangeAcc(int id)
        {
            if (currentAcc != null)
            {
                Task.Run(() => DataManipulator.SaveData(currentAcc));
                Task.Run(() => DataManipulator.SaveAccount(currentAcc));
            }

            currentAcc = DataManipulator.LoadAccount(id);
            Task.Run(() => currentAcc.TradeList = DataManipulator.LoadData(currentAcc));
            Task.Run(() => currentAcc.FilteredTradeList = DataManipulator.LoadData(currentAcc));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void AddAcc(int id)
        {
            Task.Run(() => DataManipulator.SaveData(currentAcc));
            Task.Run(() => DataManipulator.SaveAccount(currentAcc));
            var acc = new Account(id);
            currentAcc = acc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="accName"></param>
        /// <param name="accCurr"></param>
        public void AddAcc(int accId, string accName, string accCurr)
        {
            if (currentAcc != null)
            {
                Task.Run(() => DataManipulator.SaveData(currentAcc));
                Task.Run(() => DataManipulator.SaveAccount(currentAcc));
            }

            Enum.TryParse(accCurr, out CurrencyEnum currency);

            var acc = new Account(accId, accName, currency);
            currentAcc = acc;
        }
    }
}
