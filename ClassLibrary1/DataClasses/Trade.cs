using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingAppLibrary.DataClasses
{
    /// <summary>
    /// Class which holds data about trade.
    /// </summary> 
    class Trade
    {
        public long TradeId { get; }
        public DateTime OpenTime { get; }
        public string Type { get; }
        public double Size { get; }
        public string Item { get; }
        public double OpenPrice { get; }
        public double StopLoss { get; }
        public double ProfitTarget { get; }
        public DateTime CloseTime { get; }
        public double ClosePrice { get; }
        public double Commission { get; }
        public double Swap { get; }
        public double Profit { get; }

        public Trade(long tradeId, DateTime openTime, string type, double size, string item,
                    double openPrice, double stopLoss, double profitTarget, DateTime closeTime,
                    double closePrice, double commission, double swap, double profit)
        {
            TradeId = tradeId;
            OpenTime = openTime;
            Type = type;
            Size = size;
            Item = item;
            OpenPrice = openPrice;
            StopLoss = stopLoss;
            ProfitTarget = profitTarget;
            CloseTime = closeTime;
            ClosePrice = closePrice;
            Commission = commission;
            Swap = swap;
            Profit = profit;
        }

        public override string ToString()
        {
            return TradeId + ";" + OpenTime + ";" + Type + ";" + Size + ";" + Item
                + ";" + OpenPrice + ";" + StopLoss + ";" + ProfitTarget + ";" +
                CloseTime + ";" + ClosePrice + ";" + Commission + ";" + Swap + ";" +
                Profit;
        }
    }
}
