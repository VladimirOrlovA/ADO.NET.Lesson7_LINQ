using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET.Lesson7_LINQ
{
    class Area
    {
        public int AreaId { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public string FullName { get; set; }
        public string Ip { get; set; }
        public int PavilionId { get; set; }
        public int WorkingPeople { get; set; }
        public int Dependence { get; set; }

        public Area(int AreaId, string Name, int ParentId, string FullName, 
                    string Ip, int PavilionId, int WorkingPeople, int Dependence)
        {
            this.AreaId = AreaId;
            this.Name = Name;
            this.ParentId = ParentId;
            this.FullName = FullName;
            this.Ip = Ip;
            this.PavilionId = PavilionId;
            this.WorkingPeople = WorkingPeople;
            this.Dependence = Dependence;
        }
    }

    class Program
    {
        static List<Area> areas;
        
        static void Main(string[] args)
        {
            GetAreas();
            //ShowA();
            //ShowB();
            //ShowC();
            //ShowE();
            ShowF();

            Console.ReadKey();
        }

        /*3.	Выгрузить данные используя стандартные методы SqlDataAdapter.*/
        static void GetAreas()
        {
            string connStr = ConfigurationManager.ConnectionStrings["SqlClient"].ConnectionString;
            SqlConnection connection = new SqlConnection(connStr);
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Area", connStr);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            IEnumerable<DataRow> dataRows = dataSet.Tables[0].Rows.Cast<DataRow>();

            areas = dataRows.Select
                (s => new Area
                       (
                           (int)s["AreaId"],
                           (string)s["Name"],
                           (int)s["ParentId"],
                           (string)s["FullName"],
                           s["Ip"].ToString(),
                           //(s["Ip"] == null) ? (string)s["Ip"] : "11111",
                           (int)s["PavilionId"],
                           (int)s["WorkingPeople"],
                           (int)s["Dependence"]
                       )
                ).ToList();
        }

        //4.	Используя Linq написать следующие запросы к  данным:

        /*    a.	Использовать цепочку состоящую не менее из 3 операторов. 
            Выгрузить данные где ParentId = 47, произвести сортировку данных 
            от большего к меньшим и вывести на экран следующие данные – Name, FullName, IP
        */
        static void ShowA()
        {
            var selection = areas
                .Where(w => w.ParentId == 47)
                .OrderBy(o => o.AreaId)
                .Select(s => new { s.Name, s.FullName, s.Ip });

            foreach (var item in selection)
            {
                Console.WriteLine(item.Name + "\t" + item.FullName + "\t" + item.Ip);
            }
        }

        /*b.	Используя синтаксис облегчающий восприятия выгрузить следующие данные: 
         * для ParentId = 0, отобразить на экран следующие данные – Name, FullName, IP. 
         * При этом необходимо использовать отложенную выгрузку данных.*/
        static void ShowB()
        {
            var selection = (from a in areas
                             where a.ParentId == 0
                             select new { a.Name, a.FullName, a.Ip }
                             );

            foreach (var item in selection)
            {
                Console.WriteLine(item.Name + "\t" + item.FullName + "\t" + item.Ip);
            }
        }

        /*c.	Создать массив целых чисел «Pavilion» от 1 до 6. Произвести выгрузку 
         * данных из таблицы создав подзапрос. В подзапросе необходимо выбрать из 
         * массива «Pavilion» только 2, 4, 6. Затем вывести на экран следующие данные
         * – PavilionId, Name, FullName, IP*/

        static void ShowC()
        {
            int[] num = { 1, 2, 3, 4, 5, 6 };

            var res = num.Select(s => (s % 2 == 0) ? s : 0).Where(w => w > 0);

            foreach (var item in res)
            {
                Console.WriteLine(item);
            }

            //var selection = areas.Where(w => num.Contains(w.PavilionId))
            //                     .Select(s => new { s.PavilionId, s.Name, s.FullName, s.Ip });

            var selection = areas.Where(w => num.Contains(w.ParentId))
                     .Select(s => new { s.PavilionId, s.Name, s.FullName, s.Ip });

            //var selection = areas.Where(w => w.ParentId %2 == 0 && (w.ParentId<=6 && w.ParentId>=2));
            //var selection = areas.Where(w => w.ParentId %2 == 0 && (w.ParentId<=6 && w.ParentId>=2));

            //var selection = areas
            //     .Where(w => w.PavilionId == 2 || w.PavilionId == 4 || w.PavilionId == 6)
            //     .Select(s => new { s.Name, s.FullName, s.Ip });

            foreach (var item in selection)
            {
                Console.WriteLine(item.Name + "\t" + item.FullName + "\t" + item.Ip);
            }
        }

        static void ShowD() 
        {
            var selection = from a in areas
                            where a.ParentId==2 || a.ParentId == 4 || a.ParentId == 6
                            select a;
                            

            foreach (var item in selection)
            {
                Console.WriteLine(item.Name + "\t" + item.FullName + "\t" + item.Ip);
            }
        }

        /*e.	Создать запрос с использованием ключевого слова «let», и выгрузить
         * данные только где столбец «WorkingPeople» > 1.*/
        static void ShowE()
        {
            var selection = from a in areas
                            where a.WorkingPeople > 1
                            let wp = a
                            select wp;

        }

        /*f.	Создать запрос с использованием ключевого слова «into»,  
         * где в первом запросе должны вывестись следующие данные: 
         * ParentId, FullName, ParentId, Dependence, далее во втором 
         * запросе отобразить только те зоны у которых Dependence > 0.*/
        static void ShowF() 
        {
            var collection = from a in areas
                             select a;


            var collection1 = from a in areas
                              select a
                             
                              into a1
                              where a1.Dependence > 0
                              select a1;

            Console.WriteLine($"Кол-во записей: {collection.Count()}");
            //foreach (var item in collection)
            //{
            //    Console.WriteLine(item.ParentId +"\t" + item.FullName + "\t" + item.Dependence);
            //}

            Console.WriteLine($"Кол-во записей: {collection1.Count()}");
            foreach (var item in collection)
            {
                Console.WriteLine(item.ParentId + "\t" + item.FullName + "\t" + item.Dependence);
            }
        }

    }
}
