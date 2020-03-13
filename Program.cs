using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace TripleSearch
{

    class Triple
    {
        public string triplet;
        public int count;
    }
    class ReadFile
    {
        public ReadFile(out string Text, string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                Text = reader.ReadToEnd();
            }
        }
    }
    class Program
    {
        static void SortTriples(List<Triple> triples)
        {
            for(int i = 0; i < triples.Count; i++)
            {
                for(int j = 0; j < triples.Count; j++)
                {
                    if(triples[i].count > triples[j].count)
                    {
                        var buffer = triples[i];
                        triples[i] = triples[j];
                        triples[j] = buffer;
                    }
                }
            }
            Console.WriteLine("Количество триплетов на момент остановки - " + triples.Count);
            for (int i = 0; i < triples.Count; i++)
            {
                if (i < 3)
                {
                    Console.WriteLine(triples[i].triplet + " " + triples[i].count);
                }
                else
                    break;
            }
        }

        static async Task ProcessingTextAsync(string Text,List<Triple> triples)
        {
            DateTime dt = DateTime.Now;
            await Task.Run(() => ProcessingText(Text,triples));
            TimeSpan ts = DateTime.Now - dt;
            Console.WriteLine($"Поток {Thread.CurrentThread.ThreadState} завершился за " + ts);
            Console.WriteLine("Нажмите любую кнопку для завершения программы");
        }
        static void ProcessingText(string Text,List<Triple> triples)
        {
            // пул символов которые не учитываются в анализе включая пробел и переход на следующую строку
            char[] pool = new char[] { ' ', ',', '.', '/', '!', '(', ')', '?', '\n', '\r', ';', ':' };
            string CurrentTriplet;

            /// Переменная counter нужна чтобы отслеживать место в котором мы нашли первый триплет.
            /// Дальнейшая обработка текста будет начинаться с этого индекса.
            int counter = 0;

            /// Ищем первый триплет
            /// Посимвольно добавляем элементы пока длина не будет равна 3  
            try
            {
                Triple buff = new Triple() { triplet = "", count = 1 };
                // Пока в первый триплет не попадет три допустимых символа
                while (buff.triplet.Length < 3)
                {
                    if (!pool.Contains(Text[counter]))
                        buff.triplet += Text[counter];
                    counter++;
                }
                triples.Add(buff);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Текст не содержит триплетов");
                return;
            }

            for (; counter < Text.Length; counter++)
            {
                // Если в тексте(текущем триплете) находятся спец символы, то пропускаем этот триплет
                if (pool.Contains(Text[counter]) || pool.Contains(Text[counter - 1]) || pool.Contains(Text[counter - 2]))
                {
                    continue;
                }
                CurrentTriplet = ("" + Text[counter - 2] + Text[counter - 1] + Text[counter]);
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
                Thread.Sleep(1000);
            }
            foreach (Triple tr in triples)
            {
                Console.WriteLine(tr.triplet + " " + tr.count);
            }
        }
        static void Main(string[] args)
        {

            string path = Path.GetFullPath("text.txt");
            string Text = null;
            DateTime dt = DateTime.Now;
            new ReadFile(out Text, path);

            List<Triple> triples = new List<Triple>();
            Task task = ProcessingTextAsync(Text,triples);

            ConsoleKeyInfo cki = new ConsoleKeyInfo();

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
                            SortTriples(triples);
                        }
                        break;
                    }
                }
            }

            TimeSpan ts = DateTime.Now - dt;
            Console.WriteLine("Основной поток завершился за " + ts);
            Console.ReadKey(true);
        }
    }
}


