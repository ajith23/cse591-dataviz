using StackExchange;
using StackExchange.StacMan;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new StacManClient();
            var response = client.Questions.GetAll("stackoverflow",
                page: 1,
                pagesize: 100,
                sort: StackExchange.StacMan.Questions.AllSort.Creation,
                order: Order.Asc,
                filter: "!))0udqvll_moMHP2HolPO1VrTZ0ZxKUzU.8a.Hk)Kz90oMtJp)4Tw").Result;

            if (response.Data != null)
            {
                if (response.Data.Items != null)
                {
                    var questions = CreateDataTable(response.Data.Items);
                    foreach (var question in response.Data.Items)
                    {
                        Console.WriteLine(question.Title);
                        Console.WriteLine(string.Join(",", question.Tags));
                        Console.WriteLine(question.Answers[0].Body);
                        Console.WriteLine("--------------");

                        if (question.Answers != null)
                        {
                            var answers = CreateDataTable(question.Answers);
                        }
                        if (question.Comments != null)
                        {
                            var comments = CreateDataTable(question.Comments);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(response.Data.ErrorMessage);
                }

            }


            Console.ReadLine();
        }

        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    /*if(properties[i].PropertyType.IsArray)
                    {
                        values[i] = string.Join(",", properties[i].GetValue(entity));
                    }
                    else*/
                        values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
