using System;
using System.Collections.Generic;
using System.Text;

namespace Yaml
{
    public class ParserException : FormatException
    {

        public ParserException()
            : base()
        {
        }

        public ParserException(string message)
            : base(message)
        {

        }

        public ParserException(string message, int line)
            :base(message)
        {
            _lineNumber = line;
        }

        private int _lineNumber;

        public int LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }
	
    }
}
