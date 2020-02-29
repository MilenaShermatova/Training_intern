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
        // Создаем перечисление которое хранит дни недели
        Понедельник,
        Вторник,
        Среда,
        Четверг,
        Пятница,
        Суббота,
        Воскресенье
    }      
   
    class Program
    {
        //
        // Последний день после которого сформируется отчет в Report.txt и программа завершит работу
        //
        const int LastDay = 30;
        //
        // Час открытия\закрытия магазина
        //
        const int StartHour = 8;
        const int EndHour = 21;
        //
        // Максимальное количество посетителей за один час
        //
        const int MaxCustomerInHour = 10;
        //
        //Максимальное количество покупок одним клиентом
        //
        const int MaxPurchaseGoods = 10;
        //
        //Минимальное количество товаров для осуществления закупки
        //
        const int Minimal_quantity = 10;

        static void Main(string[] args)
        {
                string pathCsv = Path.GetFullPath("Data.CSV"); // путь до Csv файла
            // Создаем лист который хранит в себе товары загруженные из csv файла
            List<Catalog> rows = new List<Catalog>();
            // Считываем csv файл Data.csv 
            ReadCsv read = new ReadCsv(rows,pathCsv);
            // массив в котором будет хранится информация о покупках\закупках товаров
            // 1 столбец - количество покупок клиентами товара
            // 2 столбец - количество закупок товара
            int[,] InfoProducts = new int[rows.Count,2];
            // Создаем обьект перечисления и присваем ему значение Понедельник
            Week day_of_week = Week.Понедельник;           
            Random rand = new Random();
            double expense = 0, // Издержки - траты на закупку товаров в конце рабочего дня
                   revenue = 0; // Доход - общее количество заработанных денег за все товары                 
            int id_product, // Идентификатор продукта(см. Класс ReadCSV) 
                customer, // Посетители               
                quantity_one_person, // Количество товаров которое может купить один человек
                quantity_one_product, // Количество определенного товара которое человек покупает
                quantity_left; // сколько товаров осталось купить посетителю
            for (int day = 0; day <= LastDay;day ++)
            {
                for(int hour = StartHour; hour <= EndHour; hour ++)
                {
                    customer = rand.Next(1, MaxCustomerInHour);
                    while (customer > 0)
                    {
                        quantity_one_person = rand.Next(0, MaxPurchaseGoods);
                        // посетитель зашел но ничего не купил
                        if (quantity_one_person == 0)
                        {
                            customer--;
                            continue;
                        }
                        quantity_left = quantity_one_person;
                        // пока клиент не купит нужное ему количество
                        while (quantity_left > 0)
                        {
                            id_product = rand.Next(0, rows.Count);
                            quantity_one_product = rand.Next(1, quantity_left);
                            // Если количество которое хочет купить клиент больше остатка товара
                            // Продается то что осталось
                            if (rows[id_product].quantity < quantity_one_product)
                            {
                                quantity_one_product = rows[id_product].quantity;
                                // Если данного товара не осталось
                                // Клиент уходит
                                if (rows[id_product].quantity == 0)
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
                // Цикл отвественный за дозакупку товара
                for (int Id_product = 0; Id_product < rows.Count;Id_product ++)
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
            // записываем в csv файл актуальную информацию о товарах
            WriteCsv writeCsv = new WriteCsv(rows, pathCsv);
            // записываем отчет о проданных товарах и прибыли
            WriteReport writeReport = new WriteReport(rows, InfoProducts, expense, revenue);
        }
    }
}
