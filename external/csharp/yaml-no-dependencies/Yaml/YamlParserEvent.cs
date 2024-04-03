using System;
using System.Collections.Generic;
using System.Text;

namespace Yaml
{
    public class YamlParserEvent : ParserEvent 
    {

        private int _level = 0;
        #region ParserEvent Members

        public void Event(int i)
        {
            switch (i)
            {
                case YamlParser.MAP_CLOSE:
                case YamlParser.LIST_CLOSE:
                case YamlParser.MAP_NO_OPEN:
                case YamlParser.LIST_NO_OPEN:
                    this._level--;
                    break;
            }

            Console.WriteLine(sp() + (char) i);

            switch (i)
            {
                case YamlParser.LIST_OPEN:
                case YamlParser.MAP_OPEN:
                    this._level++;
                    break;
            }
        }

        public void Event(string s)
        {
            
        }

        public void Content(string a, string b)
        {
            Console.WriteLine(sp() + a + " : <" + b + ">");
        }

        public void Property(string a, string b)
        {
            Console.WriteLine(sp() + "( " + a + " : <" + b + "> )");
        }

        public void Error(Exception e, int line)
        {
            Console.WriteLine("Error near line " + line + ": " + e.ToString());
        }

        #endregion

        private String sp()
        {
            if (_level < 0) return "";            
            return new String(' ',_level);
        }
    }
}
