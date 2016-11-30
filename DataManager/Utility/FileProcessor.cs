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

        public static Dictionary<string, double> GetGraphEdges(List<string> tags)
        {
            var edges = new Dictionary<string, double>();
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
                            edges.Add(key, 1.0);
                    }
                }
            }
            return edges;
        }

        public static Dictionary<string, List<double>> GetGraphEdgesHistory()
        {
            var output = new Dictionary<string, List<double>>();
            var startYear = 2008;

            for (var year = startYear; year <= 2016; year++)
            {
                var path = string.Format(@"C:\Users\ajithv\Desktop\DV-Project-Back\Top Questions\{0}.csv", year);
                var index = year - startYear;
                var yearEdges = GetGraphEdges(GetTagList(path));

                foreach(var edge in yearEdges)
                {
                    if (output.ContainsKey(edge.Key))
                    {
                        while (output[edge.Key].Count() <= index)
                            output[edge.Key].Add(0.0);
                        output[edge.Key][index] = edge.Value;
                    }
                    else
                    {
                        var value = new List<double>();
                        for (var i = 0; i <= index; i++)
                            value.Add(0.0);
                        value[index] = edge.Value;
                        output.Add(edge.Key, value);
                    }
                }
            }

            foreach(var item in output)
            {
                if (item.Value.Count < 9)
                    while (item.Value.Count() == 9)
                        item.Value.Add(0.0);
            }
            return output;
        }

        public static string GetJsonData(Dictionary<string, double> edges, int count)
        {
            var nodeJson = string.Empty;
            var edgeJson = string.Empty;
            var nodes = new Dictionary<string, double>();

            var topEdges = edges.OrderByDescending(e => e.Value).Take(count).ToList();

            //normalize Edge Value.
            var allowedMinimumEdgeValue = 2.0;
            var allowedMaximumEdgeValue = 7.0;
            var minEdge = topEdges.Min(e=>e.Value);
            var maxEdge = topEdges.Max(e=>e.Value);
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

            var allowedMinimumNodeValue = 10.0;
            var allowedMaximumNodeValue = 20.0;
            var minNode = nodes.Min(e => e.Value);
            var maxNode = nodes.Max(e => e.Value);

            foreach (var node in tempNodeList)
            {
                var normalizedNode = (((node.Value - minNode) / (maxNode - minNode)) * (allowedMaximumNodeValue - allowedMinimumNodeValue)) + allowedMinimumNodeValue;
                nodeTemp.Add("{'id': '" + node.Key + "', 'name': '" + node.Key + "', 'value': " + normalizedNode + ", 'actualValue': " + node.Value + ", 'group': " + GetGroup(node.Key) + "}");
            }
            foreach (var edge in topEdges)
            {
                var normalizedEdge = (((edge.Value - minEdge)/(maxEdge - minEdge))*(allowedMaximumEdgeValue-allowedMinimumEdgeValue)) + allowedMinimumEdgeValue;

                var nodeData = edge.Key.Split('|');
                edgeTemp.Add("{'source': " + tempNodeStringList.IndexOf(nodeData[0]) + ", 'target': " + tempNodeStringList.IndexOf(nodeData[1]) + ", 'actualValue': " + edge.Value + ", 'value': " + normalizedEdge + "}");
            }
            nodeJson = "[ " + string.Join(",", nodeTemp) + " ]";
            edgeJson = "[ " + string.Join(",", edgeTemp) + " ]";

            return "{ 'nodes': " + nodeJson + ", 'edges': " + edgeJson + "}";
        }

        private static int GetGroup(string node)
        {
            var library = GetGroupedTagLibrary();
            foreach(var group in library)
            {
                if (group.Value.Contains(node))
                    return group.Key;
            }
            return 0;
        }

        private static Dictionary<int, HashSet<string>> GetGroupedTagLibrary()
        {
            var tagLibrary = new Dictionary<int, HashSet<string>>();

            var webSet = new HashSet<string>();
            var dbSet = new HashSet<string>();
            var glSet = new HashSet<string>();
            webSet.Add("javascript");
            webSet.Add("php");
            webSet.Add("html");
            webSet.Add("css");
            webSet.Add("xml");
            webSet.Add("jquery");

            glSet.Add("java");
            glSet.Add("c#");
            glSet.Add("python");
            glSet.Add("c++");
            glSet.Add("c");
            glSet.Add("ruby-on-rails");
            glSet.Add("r");

            dbSet.Add("mysql");
            dbSet.Add("sql-server");
            dbSet.Add("postgresql");
            dbSet.Add("mongodb");
            tagLibrary.Add(1, webSet);
            tagLibrary.Add(2, dbSet);
            tagLibrary.Add(3, glSet);
            return tagLibrary;
        }

        public static void UpdateTagsWithGroupsToFile(string path)
        {
            var dictionary = new Dictionary<string, int>();
            var lines = System.IO.File.ReadAllLines(path);

            var temp = lines[0].Split(',');
            if (temp.Length == 3)
            {
                lines[0] += ",Group";
                System.IO.File.WriteAllLines(path, lines);
            }
            for (var i = 1; i < lines.Length; i++)
            {
                var lineData = lines[i].Split(',');
                if(lineData.Length == 3)
                {
                    var valid = false;
                    while (!valid)
                    {
                        Console.Write("Enter group for {0} :", lineData[0]);
                        var input = Console.ReadLine();
                        var group = 0;
                        if(int.TryParse(input, out group) && group > 0 && group < 8)
                        {
                            lines[i] += ",\"" + group + "\"";
                            System.IO.File.WriteAllLines(path, lines);
                            valid = true;
                        }
                        else if (input.Trim()== string.Empty)
                        {
                            Console.WriteLine("{0} more to go.", lines.Length - i);
                            var print = @"1 = WebTechnology
2 = Database
3 = SourceControl
4 = Language
5 = Concept
6 = Library
7 = Tool";
                            Console.WriteLine(print);
                            //valid = true;
                        }
                        else
                        {
                            Console.Write("Invalid Input.");
                        }
                    }
                }
                //dictionary.Add(lines[i].Split(',')[0], Convert.ToInt32(line.Split(',')[1]));
            }
            //var buffer = new StringBuilder();

            //return dictionary;
        }

        public static HashSet<string> GetLanguageTags()
        {
            throw new NotImplementedException();
        }
    }

    public enum TagGroup
    {
        WebTechnology = 1,
        Database = 2,
        SourceControl = 3,
        Language = 4,
        Concept = 5,
        Library = 6,
        Tool = 7
    }
}
