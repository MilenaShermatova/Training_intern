using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TripleSearch
{

    class Triple
    {
        public string triplet { get; set; }
        public int count { get; set; }
    }
    class ProcessingText
    {
        public static async Task ProcessAsync(string path,List<Triple> triples,CancellationToken ct)
        {
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            await Task.Run(() => Process(path, triples,ct));
            sWatch.Stop();
            Console.WriteLine($"Поток {Thread.CurrentThread.ThreadState} завершился за " + sWatch.ElapsedMilliseconds.ToString() + " миллисекунд");
            Console.WriteLine("Нажмите любую кнопку для завершения программы");
        }
        public static void Process(string path,List<Triple> triples, CancellationToken ct)
        {
            // пул символов которые не учитываются в анализе включая пробел и переход на следующую строку
            char[] pool = new char[] { ' ', ',', '.', '/', '!', '(', ')', '?', ';', '\n', '\r', ':' };
            /// пул знаком окончания строки. Нужен чтобы триплет формировался из символов одной строки. 
            /// Если курсор перейдет на новую строку формирование триплета начнется заного
            char[] EndStroke = new char[] { '\n', '\r' };
            string CurrentTriplet;          
            string Text = "";
            int counter = 0;
            using (StreamReader reader = new StreamReader(path))
            {
                while(!reader.EndOfStream)
                {
                    //Если была нажата клавиша отмены
                    if(ct.IsCancellationRequested)
                    {
                        Console.WriteLine("Операция была отменена");
                        return;
                    }
                    CurrentTriplet = "";
                    // Ищем первый попавшийся триплет
                    for (; CurrentTriplet.Length < 3; counter++)
                    {
                        try
                        {
                            Text += Convert.ToChar(reader.Read());
                        }
                        // Ловим конец файла
                        catch (OverflowException)
                        {
                            SortTriples(triples);
                            return;
                        }
                        // Если конец строки формируем триплет заново
                        if(EndStroke.Contains(Text[counter]))
                        {
                            CurrentTriplet = "";
                            continue;
                        }
                        // Если считался спец символ, то пропускаем
                        if (!pool.Contains(Text[counter]))
                        {
                            CurrentTriplet += Text[counter];                    
                        }                        
                    }                  
                    // Если такого триплета еще нет, то создать
                    if (!triples.Exists(x => (x.triplet == CurrentTriplet)))
                    {
                        var buff = new Triple() { triplet = CurrentTriplet, count = 1 };
                        triples.Add(buff);
                    }
                    // Иначе найти похожий триплет и увеличить счетчик на 1
                    else
                    {
                        for (int j = 0; j < triples.Count; j++)
                        {
                            if (triples[j].triplet == CurrentTriplet)
                            {
                                triples[j].count++;
                            }
                        }
                    }
                    //  Имитация долгой работы программы (нужна для тестов)
                    //Thread.Sleep(1000);
                }                                           
            }
           
        }
        static public void SortTriples(List<Triple> triples)
        {
            for (int i = 0; i < triples.Count; i++)
            {
                for (int j = 0; j < triples.Count; j++)
                {
                    if (triples[i].count > triples[j].count)
                    {
                        var buffer = triples[i];
                        triples[i] = triples[j];
                        triples[j] = buffer;
                    }
                }
            }
            Console.WriteLine("Количество триплетов на текущий момент - " + triples.Count);
            // Вывод 10 (или меньше) самых часто встречаемых триплетов
            for (int i = 0; i < triples.Count; i++)
            {
                // Пока не выведено 10 - выводить на консоль
                if (i < 10)
                {
                    Console.WriteLine(triples[i].triplet + " " + triples[i].count);
                }
                // Если 10 триплетов уже есть закончить вывод
                else
                    break;
            }
        }
    }
    class Program
    {
               
        static void Main(string[] args)
        {
            // Текстовый файл text.txt должен лежать в папке с .exe
            string path = Path.GetFullPath("text.txt");          
            List<Triple> triples = new List<Triple>();
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();

            CancellationTokenSource cts = new CancellationTokenSource();

            Task task = ProcessingText.ProcessAsync(path, triples,cts.Token);
            
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            Console.WriteLine("Нажмите любую клавишу для прерывания обработки текста");
            // Цикл ожидания ввода пользователем
            while (true)
            {
                if (Console.KeyAvailable == true)
                {
                    cki = Console.ReadKey(true);
                    // Если cki считал нажатие и не равен пустому обьекту
                    if (!cki.Equals(new ConsoleKeyInfo()))
                    {
                        if (!task.IsCompleted)
                        {
                            ProcessingText.SortTriples(triples);
                            // Отмена операции подсчета триплетов
                            cts.Cancel();
                        }
                        break;
                    }
                }
            }
            sWatch.Stop();
            Console.WriteLine("Основной поток завершился за " + sWatch.ElapsedMilliseconds.ToString() + " миллисекунд");
            Console.ReadKey(true);
        }
    }
}


