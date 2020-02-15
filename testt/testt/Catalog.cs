using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testt
{
    class Catalog
    {
        //  Название товара
        public string name { get; set; }
        //  Цена на дозакупку
        public double cost { get; set; }
        // Классификация товара
        public string classification { get; set; }
        //  Объем товара
        public string capacity { get; set; }
        // Состав товара
        public string consist { get; set; }
        // Количество товара в магазине
        public int quantity { get; set; }

    }
}
