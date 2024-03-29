﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;
using TradingAppLibrary.DataClasses;

namespace TradingAppLibrary.DataManipulation
{
    /// <summary>
    /// Static class which helps to load data from MT4 result file. 
    /// </summary>
    static class HtmlParser
    {
        /// <summary>
        /// static method which parse informations about trades from html. It is generated by Meta Trader 4.
        /// </summary>
        /// <param name="path"> path to file </param>
        /// <returns></returns>
        public static List<Trade> ParseTradesFromHTML(string path)
        {
            List<Trade> tradesInList = new List<Trade> { };

            var history = new HtmlDocument();
            history.Load(path);

            

            var trades = history.DocumentNode.SelectNodes("//table/tr").Where(s => Regex.IsMatch(s.ChildNodes[0].InnerText, @"\d{8}"));


            foreach (var trade in trades)
            {
                ProccesTableRows(trade, tradesInList);
            }

            return tradesInList;
        }

        /// <summary>
        /// Static method which is used to load deposit values from html file. It is generated by Meta Trader 4.
        /// </summary>
        /// <param name="path"> path to file</param>
        /// <returns></returns>
        public static double LoadDepositValues(string path)
        {
            var history = new HtmlDocument();
            history.Load(path);

            var trades = history.DocumentNode.SelectNodes("//table/tr").Where(s => Regex.IsMatch(s.InnerText, @".*deposit.*"));

            var result =  trades.Sum(s => double.Parse(s.LastChild.InnerText, CultureInfo.InvariantCulture));

            return result;


        }

        /// <summary>
        /// Check if acc from file is the same as logged acc.
        /// </summary>
        /// <param name="acc"> logged acc </param>
        /// <param name="path"> path to file </param>
        /// <returns></returns>
        public static bool CheckAcc(Account acc, string path)
        {
            var history = new HtmlDocument();
            history.Load(path);

            var accCheck = history.DocumentNode.SelectNodes("//table/tr").First();

            return (int.Parse(accCheck.ChildNodes[1].InnerText.Split(' ')[1]) == acc.AccID);
        }

        private static void ProccesTableRows(HtmlNode node, List<Trade> result)
        {
            var attributes = node.ChildNodes;

            if (attributes.Count < 14 || attributes[8].InnerText == "&nbsp;")
            {
                return;
            }

            int tradeId = int.Parse(attributes[0].InnerText);
            DateTime openTime = DateTime.Parse(attributes[1].InnerText);
            string type = attributes[2].InnerText;
            double size = double.Parse(attributes[3].InnerText, CultureInfo.InvariantCulture);
            string item = attributes[4].InnerText;
            double openPrice = double.Parse(attributes[5].InnerText, CultureInfo.InvariantCulture);
            double stopLoss = double.Parse(attributes[6].InnerText, CultureInfo.InvariantCulture);
            double profitTarget = double.Parse(attributes[7].InnerText, CultureInfo.InvariantCulture);
            DateTime closeTime = DateTime.Parse(attributes[8].InnerText);
            double closePrice = double.Parse(attributes[9].InnerText, CultureInfo.InvariantCulture);
            double commission = double.Parse(attributes[10].InnerText, CultureInfo.InvariantCulture);
            double swap = double.Parse(attributes[12].InnerText, CultureInfo.InvariantCulture);
            double profit = double.Parse(attributes[13].InnerText, CultureInfo.InvariantCulture);

            if (closeTime != null)
            {
                result.Add(
                    new Trade(tradeId, openTime, type, size, item, openPrice,
                    stopLoss, profitTarget, closeTime, closePrice, commission, swap, profit));
            }
        }

    }
}
