using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WebApp
{
    public static class UtilityLocal
    {
        public static void TransformFile(string filePath, string newFilePath)
        {
            var lines = File.ReadAllLines(filePath);
            using (var writeFile = new StreamWriter(newFilePath, true))
            {
                writeFile.WriteLine(lines[0]);
                for (var i = 1; i < lines.Count() ; i +=2)
                {
                    var temp = lines[i] + lines[i + 1];
                    writeFile.WriteLine(temp);
                }
            }
        }


        public static void CleanTransformedFile(string filePath, string newFilePath)
        {
            var lines = File.ReadAllLines(filePath);
            using (var writeFile = new StreamWriter(newFilePath, true))
            {
                writeFile.WriteLine(lines[0]);
                for (var i = 1; i < lines.Count(); i ++)
                {
                    var temp = lines[i].Split('"');
                    temp[1] = temp[1].Replace(',', ' ');
                    writeFile.WriteLine(String.Join("\"", temp));
                }
            }
        }
    }
}