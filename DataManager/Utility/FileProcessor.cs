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
            var nodes = new Dictionary<string, int>();

            var topEdges = edges.OrderByDescending(e => e.Value).Take(count).ToList();

            //normalize Edge Value.
            //var minEdge = topEdges.Min(e=>e.Value);
            //var maxEdge = topEdges.Max(e=>e.Value);

            //2 - 7
            

            var edgeTemp = new List<string>();
            foreach (var edge in topEdges)
            {
                var nodeData = edge.Key.Split('|');
                if (nodes.ContainsKey(nodeData[0])) nodes[nodeData[0]]++; else nodes.Add(nodeData[0], 1);
                if (nodes.ContainsKey(nodeData[1])) nodes[nodeData[1]]++; else nodes.Add(nodeData[1], 1);
                //edgeTemp.Add("{'source': '" + nodeData[0] + "', 'target': '" + nodeData[1] + "', 'value': " + edge.Value + "}");
            }

            var nodeTemp = new List<string>();
            var tempNodeList = nodes.ToList();
            var tempNodeStringList = nodes.Select(n=>n.Key).ToList();
            foreach (var node in tempNodeList)
                nodeTemp.Add("{'id': '" + node.Key + "', 'name': '" + node.Key + "', 'value': " + node.Value + ", 'group': " + GetGroup(node.Key) + "}");
            foreach (var edge in topEdges)
            {
                var nodeData = edge.Key.Split('|');
                edgeTemp.Add("{'source': " + tempNodeStringList.IndexOf(nodeData[0]) + ", 'target': " + tempNodeStringList.IndexOf(nodeData[1]) + ", 'value': " + edge.Value + "}");
            }
            nodeJson = "[ " + string.Join(",", nodeTemp) + " ]";
            edgeJson = "[ " + string.Join(",", edgeTemp) + " ]";

            return "{ 'nodes': " + nodeJson + ", 'edges': " + edgeJson + "}";
        }

        static int t = 0;
        private static int GetGroup(string node)
        {
            return t++ % 3;
        }

        public static HashSet<string> GetLanguageTags()
        {
            throw new NotImplementedException();
        }
    }
}
