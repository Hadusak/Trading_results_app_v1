using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingAppLibrary.DataClasses;

namespace TradingAppLibrary.Manager
{
    public interface IAppManager
    {
        int CurrentAcc { get; }
        bool Logged { get; }
        List<string> TradeList { get; }
        List<string> FilteredTradeList { get; }
        double Deposited { get; set; }
        
        

        void ParseHtml(string fileName);
        void SaveData();
        List<double> GetRunningProfiInCurrency();
        int GetFilteredTradeSum();
        double GetFilteredSizeSum();
        double GetFilteredCommisionSum();
        double GetFilteredSwapSum();
        double GetFilteredProfitSum();
        double GetTotalProfitSum();
        int GetTotalTradeSum();
        string GetShortWon();
        string GetLongWon();
        double GetLargestLoss();
        double GetLargestProfit();
        double GetAverageLoss();
        double GetAverageProfit();
        string GetProfitTotal();
        string GetLossTotal();
        double GetRRR();
        double GetProfitFactor();
        List<double> GetRunningProfitInPips();
        List<double> GetRunningProfitInPercent();
        List<string> GetItemList();
        void FilterTradeList(List<string> selectedItems, DateTime from, DateTime to, string direction);
        void ChangeAcc(int id);
        void AddAcc(int id);
        void AddAcc(int accId, string accName, string accCurr);
    }
}
