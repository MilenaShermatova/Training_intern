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
    class WriteCsv
    {
        public WriteCsv(List<Catalog> rows, string pathCsv)
        {
            using (StreamWriter writer = new StreamWriter(pathCsv, false, Encoding.GetEncoding(1251)))
            {
                using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Указываем ; в качестве разделителя в csv файле
                    csv.Configuration.Delimiter = ";";
                    //  Перезаписываем csv файл Data.csv
                    csv.WriteRecordsAsync(rows);
                }
            }
        }
    }
}
