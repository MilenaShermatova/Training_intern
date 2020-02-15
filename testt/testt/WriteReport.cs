using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace testt
{
    class WriteReport
    {
        public WriteReport(List<Catalog> rows, int[,] InfoProduct, double expense, double revenue)
        {
            // Вычисляем прибыль магазина ( Доход - Затраты на дозакупку)
            double profit = revenue - expense; 
            string pathReport = System.IO.Path.GetFullPath(@"Report.txt");
            using (StreamWriter stWriter = new StreamWriter(pathReport, false))
            {
                for (int id_product = 1; id_product < rows.Count; id_product++)
                {
                    stWriter.WriteLineAsync($"Товар {rows[id_product].name}\nПродано {InfoProduct[id_product - 1, 0]} штук\nЗакуплено {InfoProduct[id_product, 1]} штук");
                }
                stWriter.WriteLineAsync($"Прибыль магазина  от продаж составила {Math.Round(profit, 2)} руб");
                stWriter.WriteLineAsync($"Затраченные средства на дозакупку товара {expense} руб");
            }
        }
    }
}
