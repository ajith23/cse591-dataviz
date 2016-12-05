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
        public static HashSet<string> NotFoundSet = new HashSet<string>();
        private static int GetGroup(string node)
        {
            var library = GetTagGroupDictionary();
            if (library.ContainsKey(node))
                return library[node];
            else
            {
                NotFoundSet.Add(string.Format("tagDictionary.Add(\"{0}\", (int)TagGroup);", node));
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
                        if(int.TryParse(input, out group) && group >= 0 && group < 8)
                        {
                            lines[i] += ",\"" + group + "\"";
                            System.IO.File.WriteAllLines(path, lines);
                            valid = true;
                        }
                        else if (input.Trim()== string.Empty)
                        {
                            Console.WriteLine("{0} more to go.", lines.Length - i);
                            var print = @"0 = Other
1 = WebTechnology
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

        public static Dictionary<string, int> GetTagGroupDictionary()
        {
            var tagDictionary = new Dictionary<string, int>();
            tagDictionary.Add("javascript", (int)TagGroup.WebTechnology);
            tagDictionary.Add("jquery", (int)TagGroup.Library);
            tagDictionary.Add("css", (int)TagGroup.WebTechnology);
            tagDictionary.Add("html", (int)TagGroup.WebTechnology);
            tagDictionary.Add("ios", (int)TagGroup.Library);
            tagDictionary.Add("swift", (int)TagGroup.Language);
            tagDictionary.Add("c++", (int)TagGroup.Language);
            tagDictionary.Add("c++11", (int)TagGroup.Language);
            tagDictionary.Add("android", (int)TagGroup.Library);
            tagDictionary.Add("java", (int)TagGroup.Language);
            tagDictionary.Add(".net", (int)TagGroup.Library);
            tagDictionary.Add("c#", (int)TagGroup.Language);
            tagDictionary.Add("angularjs", (int)TagGroup.Library);
            tagDictionary.Add("objective-c", (int)TagGroup.Language);
            tagDictionary.Add("mysql", (int)TagGroup.Database);
            tagDictionary.Add("php", (int)TagGroup.WebTechnology);
            tagDictionary.Add("android-studio", (int)TagGroup.Tool);
            tagDictionary.Add("python", (int)TagGroup.Language);
            tagDictionary.Add("python-3.x", (int)TagGroup.Language);
            tagDictionary.Add("java-8", (int)TagGroup.Language);
            tagDictionary.Add("c", (int)TagGroup.Language);
            tagDictionary.Add("python-2.7", (int)TagGroup.Language);
            tagDictionary.Add("node.js", (int)TagGroup.Library);
            tagDictionary.Add("xcode", (int)TagGroup.Tool);
            tagDictionary.Add("asp.net", (int)TagGroup.WebTechnology);
            tagDictionary.Add("css3", (int)TagGroup.WebTechnology);
            tagDictionary.Add("numpy", (int)TagGroup.Library);
            tagDictionary.Add("c++14", (int)TagGroup.Language);
            tagDictionary.Add("sql", (int)TagGroup.Database);
            tagDictionary.Add("sql-server", (int)TagGroup.Database);
            tagDictionary.Add("templates", (int)TagGroup.Concept);
            tagDictionary.Add("pandas", (int)TagGroup.Library);
            tagDictionary.Add("pointers", (int)TagGroup.Concept);
            tagDictionary.Add("angular2", (int)TagGroup.Library);
            tagDictionary.Add("typescript", (int)TagGroup.Library);
            tagDictionary.Add("list", (int)TagGroup.Concept);
            tagDictionary.Add("language-lawyer", (int)TagGroup.Concept);
            tagDictionary.Add("arrays", (int)TagGroup.Concept);
            tagDictionary.Add("string", (int)TagGroup.Concept);
            tagDictionary.Add("linq", (int)TagGroup.Database);
            tagDictionary.Add("reactjs", (int)TagGroup.Library);
            tagDictionary.Add("android-layout", (int)TagGroup.Library);
            tagDictionary.Add("iphone", (int)TagGroup.Tool);
            tagDictionary.Add("spring", (int)TagGroup.WebTechnology);
            tagDictionary.Add("ecmascript-6", (int)TagGroup.Library);
            tagDictionary.Add("django", (int)TagGroup.Library);
            tagDictionary.Add("java-stream", (int)TagGroup.Library);
            tagDictionary.Add("ruby", (int)TagGroup.Language);
            tagDictionary.Add("ruby-on-rails", (int)TagGroup.WebTechnology);

            tagDictionary.Add("asp.net-mvc", (int)TagGroup.WebTechnology);
            tagDictionary.Add("asp.net-mvc-4", (int)TagGroup.WebTechnology);
            tagDictionary.Add("ios9", (int)TagGroup.Library);
            tagDictionary.Add("ios8", (int)TagGroup.Library);
            tagDictionary.Add("ios7", (int)TagGroup.Library);
            tagDictionary.Add("laravel", (int)TagGroup.Library);
            tagDictionary.Add("laravel-4", (int)TagGroup.Library);
            tagDictionary.Add("swift2", (int)TagGroup.Language);
            
            tagDictionary.Add("xcode6", (int)TagGroup.Tool);
            tagDictionary.Add("entity-framework", (int)TagGroup.Concept);
            tagDictionary.Add("android-5.0-lollipop", (int)TagGroup.Library);
            tagDictionary.Add("ruby-on-rails-4", (int)TagGroup.WebTechnology);
            tagDictionary.Add("twitter-bootstrap", (int)TagGroup.Library);
            tagDictionary.Add("html5", (int)TagGroup.WebTechnology);
            tagDictionary.Add("eclipse", (int)TagGroup.Tool);
            tagDictionary.Add("cocoa-touch", (int)TagGroup.Library);
            tagDictionary.Add("r", (int)TagGroup.Language);
            tagDictionary.Add("ggplot2", (int)TagGroup.Library);

            tagDictionary.Add("winforms", (int)TagGroup.Library);
            tagDictionary.Add("tsql", (int)TagGroup.Database);
            tagDictionary.Add("database", (int)TagGroup.Concept);
            tagDictionary.Add("sql-server-2005", (int)TagGroup.Database);
            tagDictionary.Add("vb.net", (int)TagGroup.Library);
            tagDictionary.Add("ajax", (int)TagGroup.Concept);
            tagDictionary.Add("windows", (int)TagGroup.Tool);
            tagDictionary.Add("swing", (int)TagGroup.Library);
            tagDictionary.Add("generics", (int)TagGroup.Concept);
            tagDictionary.Add("svn", (int)TagGroup.SourceControl);
            tagDictionary.Add("version-control", (int)TagGroup.SourceControl);
            tagDictionary.Add("visual-studio", (int)TagGroup.Tool);
            tagDictionary.Add("visual-studio-2008", (int)TagGroup.Tool);
            tagDictionary.Add("stl", (int)TagGroup.Other);
            tagDictionary.Add("oracle", (int)TagGroup.Database);
            tagDictionary.Add("cocoa", (int)TagGroup.Library);
            tagDictionary.Add("xml", (int)TagGroup.WebTechnology);
            tagDictionary.Add("wpf", (int)TagGroup.Library);
            tagDictionary.Add("linq-to-sql", (int)TagGroup.Database);
            tagDictionary.Add("winapi", (int)TagGroup.Other);
            tagDictionary.Add("reflection", (int)TagGroup.Concept);
            tagDictionary.Add("multithreading", (int)TagGroup.Concept);
            tagDictionary.Add("xaml", (int)TagGroup.Language);
            tagDictionary.Add("hibernate", (int)TagGroup.Library);
            tagDictionary.Add("dom", (int)TagGroup.WebTechnology);
            tagDictionary.Add("git", (int)TagGroup.SourceControl);
            tagDictionary.Add("linux", (int)TagGroup.Tool);
            tagDictionary.Add("ruby-on-rails-3", (int)TagGroup.WebTechnology);
            tagDictionary.Add("ipad", (int)TagGroup.Tool);
            tagDictionary.Add("visual-studio-2010", (int)TagGroup.Tool);
            tagDictionary.Add("jquery-ui", (int)TagGroup.Library);
            tagDictionary.Add("jpa", (int)TagGroup.Library);
            tagDictionary.Add("uitableview", (int)TagGroup.Other);
            tagDictionary.Add("bash", (int)TagGroup.Other);
            tagDictionary.Add("shell", (int)TagGroup.Other);
            tagDictionary.Add("boost", (int)TagGroup.Other);
            tagDictionary.Add("asp.net-mvc-3", (int)TagGroup.WebTechnology);
            tagDictionary.Add("github", (int)TagGroup.SourceControl);
            tagDictionary.Add("razor", (int)TagGroup.WebTechnology);
            tagDictionary.Add("backbone.js", (int)TagGroup.Library);
            tagDictionary.Add("express", (int)TagGroup.Other);
            tagDictionary.Add("android-fragments", (int)TagGroup.Other);
            tagDictionary.Add("matplotlib", (int)TagGroup.Library);
            tagDictionary.Add("ios6", (int)TagGroup.Library);
            tagDictionary.Add("maven", (int)TagGroup.Tool);
            tagDictionary.Add("angularjs-directive", (int)TagGroup.WebTechnology);
            tagDictionary.Add("twitter-bootstrap-3", (int)TagGroup.Library);
            tagDictionary.Add("gradle", (int)TagGroup.Other);
            tagDictionary.Add("angular-ui-router", (int)TagGroup.Library);
            tagDictionary.Add("recyclerview", (int)TagGroup.Other);
            tagDictionary.Add("android-gradle", (int)TagGroup.Other);
            tagDictionary.Add("laravel-5", (int)TagGroup.Library);
            tagDictionary.Add("material-design", (int)TagGroup.Concept);


            return tagDictionary;
        }
    }

    public enum TagGroup
    {
        Other = 0,
        WebTechnology = 1,
        Database = 2,
        SourceControl = 3,
        Language = 4,
        Concept = 5,
        Library = 6,
        Tool = 7
    }
}
