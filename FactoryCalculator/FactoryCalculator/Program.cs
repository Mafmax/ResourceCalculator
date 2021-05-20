using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
namespace FactoryCalculator
{


    class Program
    {
        public static int Comparizon(Item X, Item Y)
        {
            return -X.ID + Y.ID;
        }

        public static string globalPath;


        static void Main(string[] args)
        {


            globalPath = Directory.GetCurrentDirectory().Replace(@"FactoryCalculator\bin\Debug\netcoreapp3.0", "");
            int key = FirstMenu();
            if (key == 1)
            {
                AddMenu();
            }
            if (key == 2)
            {
                CalculateMenu();
            }
            if(key == 3)
            {
                var list = ReadObjects();

                var json = new DataContractJsonSerializer(typeof(List<Item>));
                using(var fileStream = new FileStream(globalPath + "Objects_2.0.txt", FileMode.Create))
                {


                    json.WriteObject(fileStream, list);
                }


            }
            // WriteObjects(new List<Item>(){new Item("asdf", 0)});




            List<Item> newList = ReadObjects();

            Console.WriteLine(newList.Count);


            WriteToolTipList();


            Console.Read();
        }
        public static void CalculateMenu()
        {
            var list = ReadObjects();
            Item choiceItem;
            while (true)
            {

                WriteToolTipList();


                Console.WriteLine("Выберете элемент (Название/ID), шаги \nдля которого вы хотите расчитать: ");

                var tempName = Console.ReadLine();
                
                if (Int32.TryParse(tempName, out int result))
                {


                    choiceItem = list.Where(x => x.ID == result).FirstOrDefault();

                    if (choiceItem == null)
                    {
                        continue;
                    }

                }
                else
                {
                    choiceItem = list.Where(x => x.Name == tempName).FirstOrDefault();

                    if(choiceItem == null)
                    {
                        continue;
                    }
                }

                break;



            }

            
            WriteSteps(choiceItem);

        }
        public static void WriteSteps(Item item)
        {

            item.GetStep();


            var list =item.GetStepsChain(item);

            Console.Clear();
            Console.WriteLine("Для создания: \"" +item.Name+"\" необходимо проделать следующие шаги: " );
            Console.WriteLine(new string ('-',20));

            ShowSteps(list);



           

            Console.Read();


        }

        public static void ShowSteps(List<Detail> details)
        {
            

            CollapseSteps(ref details);
            for(int i=1; i<details.Max(x => x.item.Step);i++)
            {
                ShowStep(i, details);

            }


            Console.WriteLine(details.Count);

        }
        public static void ShowStep(int step, List<Detail> list)
        {

            

            Console.Write("Шаг №"); 
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(step);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(". Необходимые материалы:\n\n");

            int counter = 1;
            foreach(var detail in list.Where(x => x.item.Step == step).ToList())
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(counter + ". ");
                Console.ForegroundColor = ConsoleColor.White;
                counter++;

                Console.WriteLine(detail.item.Name +" в количестве " + Math.Round(detail.count,2)   + " штук\n");


            }
            Console.WriteLine(new string('-',20));

        }
        public static  void CollapseSteps(ref List<Detail> list)
        {
            var tempList = new List<Detail>();
            if (list.Count != 0 ) 
            {



                for (int i = 0; i < list.Max(x => x.item.Step); i++)
                {

                    foreach (var detail in list)
                    {
                        if (detail.item.Step == (i + 1))
                        {
                            var temp = tempList.Where(x => x.item.Name == detail.item.Name).FirstOrDefault();
                            if (temp == null)
                            {
                                tempList.Add(detail);

                            }
                            else
                            {


                                temp.count += detail.count;
                            }

                        }
                    }


                }
            }
            list = tempList;
        }

        public static void AddMenu()
        {
            while (true)
            {




                var list = ReadObjects();
                Console.Clear();
                WriteToolTipList();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Название предмета: ");
                Console.ForegroundColor = ConsoleColor.Green;
                var name = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;

                if (String.IsNullOrEmpty(name) || name == list.Where(x => x.Name == name).FirstOrDefault()?.Name)
                {
                    continue;
                }
                List<Detail> details = new List<Detail>();


                try
                {


                    for (int i = 0; i < 20; i++)
                    {
                        Console.WriteLine(new string('-', 20));
                        Console.WriteLine("\n" + (i + 1).ToString() + ". Введите название(ID) детали крафта: ");

                        Console.ForegroundColor = ConsoleColor.Green;
                        var tempName = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.White;
                        if (String.IsNullOrEmpty(tempName))
                        {
                            break;
                        }
                        Item tempItem;
                        if (Int32.TryParse(tempName, out int tempID))
                        {
                            tempItem = list.Where(x => x.ID == tempID).FirstOrDefault();

                        }
                        else
                        {
                            tempItem = list.Where(x => x.Name == tempName).FirstOrDefault();

                        }
                        if (tempItem == null)
                        {
                            throw new Exception();

                        }
                        Console.WriteLine("\nВведите количество этих предметов: ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (Single.TryParse(Console.ReadLine(), out float tempCount))
                        {
                            details.Add(new Detail(tempItem, tempCount));
                        }
                        else
                        {
                            details.Add(new Detail(tempItem, 1f));
                        }
                        Console.ForegroundColor = ConsoleColor.White;

                    }
                    AddResourse(ref list, name, details);
                }
                catch (Exception ex)
                {

                }


                WriteObjects(list);
            }

        }
        public static int FirstMenu()
        {
            Console.WriteLine("Выберите режим работы:\n1- добавление \n2- расчет шагов крафта: ");

            int result;
            while (Int32.TryParse(Console.ReadLine(), out result))
            {
                return result;
            }
            return default;


        }

        public static void AddResourse(ref List<Item> items, string name, List<Detail> details = null)
        {
            int ID;
            if (items.Count > 0)
            {

                ID = items.Max(x => x.ID) + 1;

            }
            else
            {
                ID = 1;
            }


            items.Add(new Item(name, ID, details));




        }



        public static void WriteToolTipList()
        {
            var items = ReadObjects();

            int leftBuff = Console.CursorLeft;
            int topBuff = Console.CursorTop;
            Console.Clear();
            if (items?.Count > 0)
            {

                items.Sort(Comparizon);
                Console.SetCursorPosition(40, 0);
                Console.WriteLine("Уже имеющиеся предметы: ");

                foreach (var item in items)
                {
                    Console.SetCursorPosition(40, Console.CursorTop);
                    Console.WriteLine(String.Format("ID: {0}. Name: {1}.", item.ID, item.Name));
                }

            }
            Console.SetCursorPosition(0, 0);
        }

        public static List<Item> ReadObjects()
        {

            var jsonSerializer = new DataContractJsonSerializer(typeof(List<Item>));
            using (var file = new FileStream(globalPath + "Objects.txt", FileMode.OpenOrCreate))
            {
                if (file.Length > 0)
                {
                    return jsonSerializer.ReadObject(file) as List<Item>;
                }


                return new List<Item>();
            }

        }

        public static void WriteObjects(List<Item> items)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(List<Item>));


            using (var file = new FileStream(globalPath + "Objects.txt", FileMode.Create))
            {


                if (items.Count > 0)
                {

                    jsonSerializer.WriteObject(file, items);
                }


            }
        }


    }
}
