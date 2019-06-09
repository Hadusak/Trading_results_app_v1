using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingAppLibrary.Constants;

namespace TradingAppLibrary.DataClasses
{
    /// <summary>
    /// class which holds data about accounts
    /// </summary>
    class Account
    {
        public int AccID { get; }
        public string Name { get; }
        public CurrencyEnum Currency { get; }
        public DateTime LastUpdate { get; set; }
        public double Deposited { get; set; }
        public List<Trade> TradeList { get; set; }
        public List<Trade> FilteredTradeList { get; set; }

        public Account(int accId, string name, CurrencyEnum curr, DateTime upd, double deposit)
        {
            AccID = accId;
            Name = name;
            Currency = curr;
            LastUpdate = upd;
            Deposited = deposit;
            TradeList = new List<Trade> { };
            FilteredTradeList = new List<Trade> { };
        }

        public Account(int accId, string name, CurrencyEnum curr)
        {
            AccID = accId;
            Name = name;
            Currency = curr;
            LastUpdate = DateTime.Now;
            Deposited = 0;
            TradeList = new List<Trade> { };
            FilteredTradeList = new List<Trade> { };
        }

        public Account(int accId)
        {
            AccID = accId;
            Name = "default";
            Currency = CurrencyEnum.EUR;
            LastUpdate = DateTime.Now;
            Deposited = 0;
            TradeList = new List<Trade> { };
            FilteredTradeList = new List<Trade> { };
        }

    }

}
