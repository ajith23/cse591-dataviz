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
            //CreateJsonForGraph();
            string path = @"C:\Users\avimalch\git\cse591-dataviz\DataManager\Utility\forumSupportData.csv";
            Utility.FileProcessor.UpdateTagsWithGroupsToFile(path);
        }

        static void CreateJsonForGraph()
        {
            for (var year = 2008; year <= 2016; year++)
            {
                Console.WriteLine("Processing " + year + "...");
                //var path = string.Format(@"C:\Users\ajithv\Desktop\DV-Project-Back\Top Questions\{0}.csv", year);
                var path = string.Format(@"C:\Users\avimalch\Downloads\{0}.csv", year);

                var tagList = Utility.FileProcessor.GetTagList(path);

                var edges = Utility.FileProcessor.GetGraphEdges(tagList);
                var s = Utility.FileProcessor.GetJsonData(edges, 50);

                //var jsonPath = string.Format(@"C:\Users\ajithv\Desktop\DV-Project-Back\Top Questions\{0}json.json", year);
                var jsonPath = string.Format(@"C:\Users\avimalch\git\cse591-dataviz\dv-web-app\WebApp\data\{0}json.json", year);
                System.IO.File.WriteAllText(jsonPath, s.Replace("'", "\""));
            }
            System.IO.File.WriteAllLines(@"C:\Users\avimalch\git\cse591-dataviz\dv-web-app\WebApp\data\duh.txt", Utility.FileProcessor.NotFoundSet.ToArray());
            Console.WriteLine("Completed.");
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
