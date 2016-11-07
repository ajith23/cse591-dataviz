using StackExchange.StacMan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    class Program
    {
        static void Main(string[] args)
        {
            //var data = System.IO.File.ReadAllLines(@"C:\Users\ajithv\Desktop\DV-Project-Back\Top Questions\20091.csv").ToList();

            for(var year = 2008; year <= 2016; year++)
            {
                var path = string.Format(@"C:\Users\ajithv\Desktop\DV-Project-Back\Top Questions\{0}.csv", year);
                var tagList = Utility.FileProcessor.GetTagList(path);

                var edges = Utility.FileProcessor.GetGraphEdges(tagList);
                var s = Utility.FileProcessor.GetJsonData(edges, 100);
                
                var jsonPath = string.Format(@"C:\Users\ajithv\Desktop\DV-Project-Back\Top Questions\{0}json.json", year);
                System.IO.File.WriteAllText(jsonPath, s.Replace("'", "\""));

                Console.WriteLine("-------------------- "+year+" ---------------------");
                /*var topEdges = edges.OrderByDescending(e => e.Value).ToList();
                for (var i = 0; i < 10; i++)
                    Console.WriteLine("{0} \t {1}", topEdges[i].Value, topEdges[i].Key);
                Console.WriteLine();*/
            }


            Console.ReadLine();
        }

        static void FetchData()
        {
            var client = new StacManClient();
            var task = client.Questions.GetAll("stackoverflow",
            page: 1,
            pagesize: 100,
            sort: StackExchange.StacMan.Questions.AllSort.Creation,
            order: Order.Asc,
            filter: "!*1TaJdgSr4q2jsj.d)hwNTus3p3GxTZ-OeuQzRXc1");

            task.ContinueWith(t =>
            {
                foreach (var question in t.Result.Data.Items)
                {
                    Console.WriteLine(question.Title);
                    Console.WriteLine(question.Answers[0].Body);
                    Console.WriteLine(string.Join(",", question.Tags));
                    Console.WriteLine("---------");
                }
            });


            /*var questions = Utility.RestApi.GetQuestions();
            foreach(var q in questions)
            {
                Console.WriteLine(string.Join(",", q.Tags));
            }*/
            Console.ReadLine();
        }
    }

    public class DataObject
    {
        public string Name { get; set; }
    }
}
