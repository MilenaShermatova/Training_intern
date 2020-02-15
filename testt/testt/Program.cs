using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace testt
{
    enum Week
    {
        Понедельник,
        Вторник,
        Среда,
        Четверг,
        Пятница,
        Суббота,
        Воскресенье
    }
    class Catalog
    {
        public string name { get; set; }
        public double cost { get; set; }       
        public string classification { get; set; }
        public string capacity { get; set; }
        public string consist { get; set; }
        public int quantity { get; set; }

    }
    class ReadCsv
    {
        public ReadCsv(List<Catalog> rows,string pathCsv)
        {             
            try
            {
                using (StreamReader reader = new StreamReader(pathCsv, Encoding.GetEncoding(1251)))
                {
                    using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        // указываем ; в качестве разделителя 
                        csv.Configuration.Delimiter = ";";
                        // считываем название столбцов
                        csv.Read();
                        csv.ReadHeader();
                        while (csv.Read())
                        {
                            Catalog record = new Catalog
                            {
                                /// Считывание данных с файла в List по индексам где,
                                /// 0 - Название
                                /// 1 - Цена закупки(без наценки)
                                /// 2 - Классификация\группа товара
                                /// 3 - Объкм
                                /// 4 - Крепкость напитка\состав
                                /// 5 - Количество товара в магазине
                                name = csv.GetField<string>(0),
                                cost = csv.GetField<double>(1),
                                classification = csv.GetField<string>(2),
                                capacity = csv.GetField<string>(3),
                                consist = csv.GetField<string>(4),
                                quantity = csv.GetField<int>(5)
                            };
                            rows.Add(record);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка чтения файла:" + e.Message);
            }
        }
    }    
    class WriteCsv
    {
        public WriteCsv(List<Catalog> rows, string pathCsv)
        {
            using (StreamWriter writer = new StreamWriter(pathCsv,false, Encoding.GetEncoding(1251)))
            {
                using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.Delimiter = ";";
                    csv.WriteRecordsAsync(rows);
                }
            }
        }
    }
    class WriteReport
    {
        public WriteReport(List<Catalog> rows,int[,] InfoProduct, double expense, double revenue)
        {
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
    interface ISale
    {
        double SetMarkUp();                
    }
    class WeekDays : ISale
    {
        public double SetMarkUp()
        {
            return 1.1;
        }
    }
    class Weekend : ISale
    {
        public double SetMarkUp()
        {
            return 1.15;
        }
    }
    class EveningFrom18to20hours : ISale
    {
        public double SetMarkUp()
        {
            return 1.08;
        }
    }
    class PurchaseTwoThingsAndMore : ISale
    {
        public double SetMarkUp()
        {
            return 1.07;
        }
    }
    class Purchase
    {
        public double price;
        int quantity;
        string name;
        double MarkUp;
        public Purchase(List<Catalog> rows, int quantity, ISale sale, int id_product,ref double revenue)
        {
            this.name = rows[id_product].name;            
            rows[id_product].quantity = rows[id_product].quantity - quantity;
            this.MarkUp = sale.SetMarkUp();            
            if(quantity > 2)
            {
                this.price = rows[id_product].cost * MarkUp;
                revenue = revenue + this.price * 2;
                this.quantity = 2;
                Purchase.ConsoleOutput(this);
                sale = new PurchaseTwoThingsAndMore();
                this.quantity = quantity - 2;
                this.MarkUp = sale.SetMarkUp();
                this.price = rows[id_product].cost * MarkUp;
                revenue = revenue + this.price * (quantity - 2);
                Purchase.ConsoleOutput(this);
            }
            else
            {
                this.quantity = quantity;
                this.price = rows[id_product].cost * MarkUp;
                revenue = revenue + this.price * quantity;
                Purchase.ConsoleOutput(this);
            }
        }
        public static void ConsoleOutput(Purchase purchase)
        {
            Console.WriteLine($"Товар {purchase.name} был продан по цене {purchase.price} с наценкой {purchase.MarkUp} в количестве {purchase.quantity} штук");
        }

    }
    class Program
    {
        const int LastDay = 30;
       
        const int StartHour = 8;
        const int EndHour = 21;

        const int MaxCustomerInHour = 10;

        const int MaxPurchaseGoods = 10;

        const int Minimal_quantity = 10;

        static void Main(string[] args)
        {
                string pathCsv = Path.GetFullPath("Data.CSV"); // путь до Csv файла
            List<Catalog> rows = new List<Catalog>();                     
            ReadCsv read = new ReadCsv(rows,pathCsv);
            int[,] InfoProducts = new int[rows.Count,2];
            ////////
            Week day_of_week = Week.Понедельник;           
            Random rand = new Random();
            double expense = 0,
                   revenue = 0;                  
            int id_product,
                customer,               
                quantity_one_person,
                quantity_one_product,
                quantity_left;
            for (int day = 0; day <= LastDay;day ++)
            {
                for(int hour = StartHour; hour <= EndHour; hour ++)
                {
                    customer = rand.Next(1, MaxCustomerInHour);
                    while (customer > 0)
                    {
                        quantity_one_person = rand.Next(0, MaxPurchaseGoods);
                        if (quantity_one_person == 0)
                        {
                            customer--;
                            continue;
                        }
                        quantity_left = quantity_one_person;
                        while (quantity_left > 0)
                        {
                            id_product = rand.Next(0, rows.Count);
                            quantity_one_product = rand.Next(1, quantity_left);
                            if (rows[id_product].quantity < quantity_one_product)
                            {
                                quantity_one_product = rows[id_product].quantity;
                                if(rows[id_product].quantity == 0)
                                {
                                    customer--;
                                    break;
                                }
                            }
                            if (hour >= 18 && hour <= 20)
                            {
                                Purchase purchase = new Purchase(rows, quantity_one_product, new EveningFrom18to20hours(), id_product,ref revenue);                                                                
                            }
                            else if (day_of_week == Week.Суббота || day_of_week == Week.Воскресенье)
                            {                              
                                Purchase purchase = new Purchase(rows, quantity_one_product, new Weekend(), id_product,ref revenue);                                                        
                            }
                            else
                            {
                                Purchase purchase = new Purchase(rows, quantity_one_product, new WeekDays(), id_product,ref revenue);                                                              
                            }
                            InfoProducts[id_product, 0] = InfoProducts[id_product, 0] + quantity_one_product;
                            quantity_left = quantity_left - quantity_one_product;
                        }
                        customer--;
                    }
                }
                for(int Id_product = 0; Id_product < rows.Count;Id_product ++)
                {
                    if(rows[Id_product].quantity < Minimal_quantity)
                    {
                        rows[Id_product].quantity = rows[Id_product].quantity + 150;
                        expense = expense + (rows[Id_product].cost * 150);
                        InfoProducts[Id_product, 1] = InfoProducts[Id_product, 1] + 150;
                    }
                }
                day_of_week++;
            }
            WriteCsv writeCsv = new WriteCsv(rows, pathCsv);
            WriteReport writeReport = new WriteReport(rows, InfoProducts, expense, revenue);
        }
    }
}
