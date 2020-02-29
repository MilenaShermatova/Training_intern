using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaShape
{
    interface IArea
    {
        double Area();
    }
    class Circle : IArea
    {
        double answer;
        const double Pi = 3.14;
        double r;     
        public Circle(double r)
        {
            this.r = r;
            if (this.r < 0)          
            {
                Console.WriteLine("Радиус круга не может быть меньше нуля\n" + "Введите новое значение радиуса круга");
                this.r = Convert.ToDouble(Console.ReadLine());
            }
        }
        public double Area()
        {
            answer = System.Math.Pow(r, 2) * Pi;
            return answer;
        }
    }
    class Triangle : IArea
    {
        double answer;
        double half_P;
        double a, b, c;
        public Triangle(double a, double b, double c)
        {
            // Вычисляем полупериметр            
            this.a = a;
            this.b = b;
            this.c = c;
            if(this.a < 0 || this.b < 0 || this.c < 0)
            {
                Console.WriteLine("Сторона треугольника не может быть меньше 0");
                if (this.a < 0)
                {
                    Console.WriteLine("Введите отриц. сторону(1) заного:");
                    this.a = Convert.ToDouble(Console.ReadLine());
                }
                if (this.b < 0)
                {
                    Console.WriteLine("Введите отриц. сторону(2) заного:");
                    this.b = Convert.ToDouble(Console.ReadLine());
                }
                if (this.c < 0)
                {
                    Console.WriteLine("Введите отриц. сторону(3) заного:");
                    this.c = Convert.ToDouble(Console.ReadLine());
                }
            }
            half_P = (this.a + this.b + this.c) / 2;
        }
        public double Area()
        {
            answer = System.Math.Sqrt(half_P * (half_P - a) * (half_P - b) * (half_P - c));
            return answer;
        }
    }
    class Figure : IArea
    {
        const int x = 0, y = 1;
        public Figure(int dimension, int[,] array)
        {
            this.array = array;            
            this.dimension = dimension;
        }
        int[,] array;
        int dimension { get; set; }
        int summ = 0;        
        double answer = 0;
        // Используется формула площади Гаусса
        public double AlgorithmSummary()
        {
            int last_index = dimension - 1;
            for (int i = 0; i < dimension - 1; i++)
            {
                summ = summ + (array[i, x] * array[i + 1,y]);  
            }
            summ = summ + (array[last_index, x] * array[0,y]);
            for (int i = 0; i < dimension - 1; i++)
            {
                summ = summ - (array[i + 1, x] * array[i, y]);
            }
            summ = summ - (array[0,x] * array[last_index,y]);
            return summ;
        }

        public double Area()
        {
            answer = (double) 0.5 * System.Math.Abs(AlgorithmSummary());
            return answer;
        }
    }
    class Shape
    {
        string Name_shape;
        public IArea Area { get; set; }
        public Shape(IArea Area,string Name_shape)
        {
            this.Name_shape = Name_shape;
            this.Area = Area;
        }
        public void ShowArea()
        {            
            Console.WriteLine($"Площадь {Name_shape} равна: " + System.Math.Round(Area.Area(),3));
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Shape shape = new Shape(new Circle(5),"круга");
            shape.ShowArea();
            ////
            shape = new Shape(new Triangle(3, 3, 3),"треугольника");
            shape.ShowArea();
            ////
            int[,] array = new int[5, 2] { { 3, 4 }, {5, 11 }, { 12, 8 }, { 9, 5 }, { 5, 6 } };
            shape = new Shape(new Figure(5, array),"произвольной фигуры");
            shape.ShowArea();
            Console.ReadKey();
        }
    }
}
