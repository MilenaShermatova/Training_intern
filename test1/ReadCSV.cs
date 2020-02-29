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
    class ReadCsv
    {
        public ReadCsv(List<Catalog> rows, string pathCsv)
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
}
