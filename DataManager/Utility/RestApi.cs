using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.StacMan;

namespace Utility
{
    public static class RestApi
    {
        public static List<Question> GetQuestions()
        {
            var questions = new List<Question>();
            var client = new StacManClient();
            var response = client.Questions.GetAll("stackoverflow").Result;
            foreach(var item in response.Data.Items)
            {
                questions.Add(item);
            }
            return questions;
        }
    }
}
