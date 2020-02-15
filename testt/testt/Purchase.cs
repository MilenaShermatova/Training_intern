using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testt
{
    interface ISale
    {
        double SetMarkUp();
    }
    class WeekDays : ISale // Класс отвественный за скидку  в будни
    {
        public double SetMarkUp()
        {
            return 1.1;
        }
    }
    class Weekend : ISale // Класс отвественный за скидку  в выходные
    {
        public double SetMarkUp()
        {
            return 1.15;
        }
    }
    class EveningFrom18to20hours : ISale // Класс отвественный за скидку в любой день между 18 и 20 часами
    {
        public double SetMarkUp()
        {
            return 1.08;
        }
    }
    class PurchaseTwoThingsAndMore : ISale // Класс отвественный за скидку  в при покупке больше 2 одних и тех же товаров за раз
    {
        public double SetMarkUp()
        {
            return 1.07;
        }
    }
    class Purchase
    {
        double price; //  Цена на товар
        int quantity; //  Количество товара
        string name; //  Название товара
        double MarkUp; // Наценка
        public Purchase(List<Catalog> rows, int quantity, ISale sale, int id_product, ref double revenue)
        {
            this.name = rows[id_product].name;
            rows[id_product].quantity = rows[id_product].quantity - quantity;
            this.MarkUp = sale.SetMarkUp();
            // Если покупаемых товаров одного типа больше двух
            if (quantity > 2)
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
        // Вывод на консоль информации о покупке
        public static void ConsoleOutput(Purchase purchase)
        {
            Console.WriteLine($"Товар {purchase.name} был продан по цене {purchase.price} с наценкой {purchase.MarkUp} в количестве {purchase.quantity} штук");
        }

    }
}
