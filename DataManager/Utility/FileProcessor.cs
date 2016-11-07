using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class FileProcessor
    {
        public static List<string> GetTagList(string path)
        {
            var output = new List<string>();
            var tagIndex = 0;
            using (TextFieldParser parser = new TextFieldParser(path))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                var index = 0;
                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] fields = parser.ReadFields();
                    if (index == 0)
                    {
                        foreach (var field in fields)
                        {
                            if (field == "Tags") break;
                            tagIndex++;
                        }
                    }
                    else
                        output.Add(fields[tagIndex]);
                    index++;
                }
            }
            return output;
        }

        public static Dictionary<string, int> GetGraphEdges(List<string> tags)
        {
            var edges = new Dictionary<string, int>();
            foreach (var tagList in tags)
            {
                var temp = tagList.Trim('<').Trim('>').Replace("><", " ").Split(' ').OrderBy(n=>n).ToArray();
                for(var i =0; i< temp.Length; i++)
                {
                    for (var j = i+1; j < temp.Length; j++)
                    {
                        var key = temp[i] + "|" + temp[j];
                        if (edges.ContainsKey(key))
                            edges[key]++;
                        else
                            edges.Add(key, 1);
                    }
                }
            }
            return edges;
        }

        public static string GetJsonData(Dictionary<string, int> edges, int count)
        {
            var nodeJson = string.Empty;
            var edgeJson = string.Empty;
            var nodes = new HashSet<string>();

            var topEdges = edges.OrderByDescending(e => e.Value).Take(count).ToList();


            var edgeTemp = new List<string>();
            foreach (var edge in topEdges)
            {
                var nodeData = edge.Key.Split('|');
                nodes.Add(nodeData[0]);
                nodes.Add(nodeData[1]);
                edgeTemp.Add("{'source': '" + nodeData[0] + "', 'target': '" + nodeData[1] + "', 'value': " + edge.Value + "}");
            }

            var nodeTemp = new List<string>();
            foreach (var node in nodes)
                nodeTemp.Add("{'id': '" + node + "', 'group': " + GetGroup(node) + "}");

            nodeJson = "[ " + string.Join(",", nodeTemp) + " ]";
            edgeJson = "[ " + string.Join(",", edgeTemp) + " ]";

            return "{ 'nodes': " + nodeJson + ", 'links': " + edgeJson + "}";
        }
        private static int GetGroup(string node)
        {
            return 1;
        }

        public static HashSet<string> GetLanguageTags()
        {
            throw new NotImplementedException();
        }
    }
}
