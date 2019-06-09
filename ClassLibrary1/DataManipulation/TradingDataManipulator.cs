using TradingAppLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingAppLibrary.Constants;

namespace TradingAppLibrary.DataManipulation
{
    /// <summary>
    /// Static class which helps to load/save data (acc or trades). Data are saved in txt file (for now).  
    /// </summary>
    static class DataManipulator
    {
        /// <summary>
        /// Save trades data to txt file, one row for one trade. Parameters are writen like "first;second;..."
        /// </summary>
        /// <param name="acc"></param>
        public static void SaveData(Account acc)
        {
            using (StreamWriter writer = new StreamWriter(@"..\..\Data\" + acc.AccID + ".txt"))
            {
                foreach (var trade in acc.TradeList)
                {
                    writer.WriteLine(trade.ToString());
                }
            }
        }

        /// <summary>
        /// Load data from txt file to app memory.
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static List<Trade> LoadData(Account acc)
        {
            List<Trade> resultList = new List<Trade> { };
            var file = @"..\..\Data\" + acc.AccID + ".txt";

            if (!File.Exists(file) || new FileInfo(file).Length == 0)
            {
                return new List<Trade> { };
            }

            using (StreamReader reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(';');

                    resultList.Add(new Trade(
                        int.Parse(line[0]),
                        DateTime.Parse(line[1]),
                        line[2],
                        double.Parse(line[3]),
                        line[4],
                        double.Parse(line[5]),
                        double.Parse(line[6]),
                        double.Parse(line[7]),
                        DateTime.Parse(line[8]),
                        double.Parse(line[9]),
                        double.Parse(line[10]),
                        double.Parse(line[11]),
                        double.Parse(line[12])
                    ));
                }
            }

            return resultList;
        }

        /// <summary>
        /// Load acc information from txt file.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Account LoadAccount(int id)
        {
            var file = @"..\..\Data\" + id + "_Acc.txt";

            if (!File.Exists(file) || new FileInfo(file).Length == 0)
            {
                throw new Exception("Account doesnt exist");
            }

            using (StreamReader reader = new StreamReader(file))
            {
                
                var line = reader.ReadLine().Split(';');

                Enum.TryParse(line[1], out CurrencyEnum currency);

                return new Account(
                    int.Parse(line[0]),
                    line[2],
                    currency,
                    DateTime.Now,
                    double.Parse(line[4])
                );                
            }
   
        }

        /// <summary>
        /// Save acc information to txt file
        /// </summary>
        /// <param name="acc"></param>
        public static void SaveAccount(Account acc)
        {
            using (StreamWriter writer = new StreamWriter(@"..\..\Data\" + acc.AccID + "_Acc.txt"))
            {
                string toWrite = acc.AccID.ToString() + ";" + acc.Currency.ToString() + ";" + acc.Name 
                    + ";" + acc.LastUpdate.ToString() + ";" + acc.Deposited.ToString();

                writer.WriteLine(toWrite);
                
            }
        }
    }
}
