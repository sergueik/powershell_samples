using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Yaml;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            FileInfo file = new FileInfo(args[0]);
            TextReader reader = file.OpenText();
            ParserEvent handler = new YamlParserEvent();
            YamlParser parser = new YamlParser(reader, handler);
            parser.TryParse();
            reader.Close();
            Console.Read();
        }
    }
}
