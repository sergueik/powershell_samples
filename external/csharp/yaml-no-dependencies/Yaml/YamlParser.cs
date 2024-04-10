using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Yaml
{
    public class YamlParser
    {

        public const int LIST_OPEN = '[';
        public const int LIST_CLOSE = ']';
        public const int MAP_OPEN = '{';
        public const int MAP_CLOSE = '}';
        public const int LIST_NO_OPEN = 'n';
        public const int MAP_NO_OPEN = 'N';
        public const int DOCUMENT_HEADER = 'H';
        public const int MAP_SEPARATOR = ':';
        public const int LIST_ENTRY = '-';   

        protected ParserReader _reader;
        protected int _line;

        private ParserEvent _event;
        private Dictionary<String, String> _props;
        private char _pendingEvent;



        public YamlParser(TextReader reader,ParserEvent @event)
        {
            this._reader = new ParserReader( reader);
            this._event = @event;
            this._props = new Dictionary<string, string>();
        }

        protected string ReaderString()
        {
            return this._reader.GetString();
        }

        private void ClearEvents()
        {
            this._props.Clear();
        }

        #region events

        protected void OnEvent(int i) 
        {
            if (_event != null)
                _event.Event(i);
        }
        protected void OnEvent(String s)
        {
            if (_event != null)
                _event.Event(s);
        }

        protected void OnContent(String a, String b)
        {
            if (_event != null)
                _event.Content(a, b);
        }
        protected void OnProperty(String a, String b)
        {
            if (_event != null)
                _event.Property(a, b);
        }
        protected void OnError(Exception e, int line)
        {
            if (_event != null)
                _event.Error(e, line);
        }

        #endregion
        private void SendEvents()
        {
            if (_pendingEvent == '[')
                OnEvent(LIST_OPEN);
            else if (_pendingEvent == '{')
                OnEvent(MAP_OPEN);

            _pendingEvent = Char.MinValue;

            if (_props.ContainsKey("anchor"))
                OnProperty("anchor", _props["anchor"]);

            if (_props.ContainsKey("transfer"))
                OnProperty("transfer", _props["transfer"]);

            if (_props.ContainsKey("alias"))
                OnProperty("alias", _props["alias"]);

            if (_props.ContainsKey("string"))
                OnProperty("string", _props["string"]);

            _props.Clear();
        }

        /// <summary>
        /// Returns the length of the Indent without consuming it.
        /// </summary>
        /// <returns></returns>
        public int ParseIndent()
        {
            using (Marker mark = _reader.GetMarker())
            {
                int i = this._reader.ReadWhile(delegate(int c) { return YamlCharacter.Is(c, YamlCharacterType.Indent); });
                mark.Reset();
                return i;
            }           
        }
        /// <summary>
        /// Parses a string of the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>A string of the parsed characters or null if no matching characters are available for parsing</returns>
        public string ParseArray(YamlCharacterType type)
        {
            using (Marker mark = _reader.GetMarker())
            {
                int i = this._reader.ReadWhile(delegate(int c) { return YamlCharacter.Is(c, type); });

                if (i != 0)                                    
                    return mark.GetString();
                else                
                    return null;
            }
        }

        /// <summary>
        /// Parses a string of spaces
        /// </summary>
        /// <returns>Returns a string of spaces or null</returns>
        public bool ParseSpace()
        {
            return ParseArray(YamlCharacterType.Space) != null;
        }

        public bool ParseLine()
        {
            return ParseArray(YamlCharacterType.Line) != null;
        }

        public bool ParseLineSp()
        {
            return ParseArray(YamlCharacterType.LineSP) != null;
        }

        public bool ParseWord()
        {
            return ParseArray(YamlCharacterType.Word) != null;
        }

        public bool ParseNumber()
        {
            return ParseArray(YamlCharacterType.Digit) != null;
        }

        /// <summary>
        /// Consumes an indent with a length of 'n'.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool ParseIndent(int n)
        {
            using (Marker marker = _reader.GetMarker())
            {
                int i = this._reader.ReadWhile(delegate(int c) { return YamlCharacter.Is(c, YamlCharacterType.Indent); });
                if (i != n)
                {
                    marker.Reset();
                    return false;
                }
                else
                {
                    return true;
                }
            }               
        }

        public bool ParseNewline() 
        {
            _line++;
            using (Marker mark = _reader.GetMarker())
            {
                int c = _reader.Read();
                if (c == -1)
                    return true;
                else if(c == 13 && _reader.Peek() == 10)                
                {
                    _reader.Read(); //read in the 10 since we only peeked it before
                    return true;
                }
                else if (YamlCharacter.Is(c, YamlCharacterType.LineBreak))
                {                    
                    return true;
                }

                mark.Reset();
                _line--;
                return false;
            }
        }

        public bool ParseEnd() 
        {
            using (Marker mark = _reader.GetMarker())
            {

                ParseSpace();

                if (!ParseNewline())
                {
                    mark.Reset();
                    return false;
                }

                while (ParseComment(-1, false)) ;
                
                return true;
            }
        }

        private bool ParseStringSimple()  
        {
            char ch;
            int c;

            int i = 0;

            while ( true ) {
                c = _reader.Read();
                if (c == -1) break;

                ch = (char) c;
                if (i == 0 && (YamlCharacter.IsSpaceChar(ch) || YamlCharacter.IsIndicatorNonSpace(ch) || YamlCharacter.IsIndicatorSpace(ch) ) )
                    break;

                if ( ! YamlCharacter.IsLineSpChar(ch) ||
                     (YamlCharacter.IsIndicatorSimple(ch) && _reader.GetPrevious() != '\\' ) )
                    break;
                i++;
            }

            _reader.Unread();

            if (i != 0)
                return true;

            return false;
        }

        
        private bool ParseStringQ1() 
        {
            if ( _reader.GetCurrent() != '\'')
                return false;

            _reader.Read();
            int c=0;
            int i = 0;

            while ( YamlCharacter.Is( c=_reader.Read(), YamlCharacterType.LineSP ) ) {
                if (c == '\''  && _reader.GetPrevious() != '\\' )
                    break;
                i++;
            }

            if (c != '\'')
                throw new ParserException("unterminated string",_line);

            if (i != 0)
                return true;

            return false;
        }

        private bool ParseStringQ2() 
        {

            if ( _reader.GetCurrent() != '"')
                return false;

            _reader.Read();
            int c=0;
            int i = 0;

            while (YamlCharacter.Is( c=_reader.Read(), YamlCharacterType.LineSP ) ) {
                if ( c == '"'  && _reader.GetPrevious() != '\\' )
                    break;
                i++;
            }

            if ( c != '"' )
                throw new ParserException("unterminated string", _line);

            if (i != 0)
                return true;

            return false;
        }

        public bool ParseString()
        {
            Mark();

            if (ParseStringQ1() || ParseStringQ2() || ParseStringSimple())
            {
                _props.Add("string", _reader.GetString().Trim() );
                Unmark();
                return true;
            }

            Reset();
            return false;
        }

        public bool ParseAlias() 
        {
            using (Marker marker = _reader.GetMarker())
            {

                if (!_reader.ReadIf('*'))
                {
                    return false;
                }

                if (!ParseWord())
                {
                    marker.Reset();
                    return false;
                }

                
                _props.Add("alias", marker.GetString());
                return true;
            }
        }
        /// <summary>
        /// anchor := '&' word
        /// </summary>
        /// <returns></returns>
        public bool ParseAnchor() 
        {
            using (Marker mark = _reader.GetMarker())
            {
                if (!_reader.ReadIf('&'))                
                    return false;
                
                if (!ParseWord())
                {
                    mark.Reset();//reset the anchor tag
                    return false;
                }
                else
                {
                    _props.Add("anchor", mark.GetString());
                    return true;
                }
            }
        }

        public bool ParseComment(int n, bool @explicit) 
        {
            Mark();

            if (n != -1 && ParseIndent() >= n) {
                Reset();
                return false;
            }

            ParseSpace();

            int c;
            if ((c=_reader.Read()) == '#')
                ParseLineSp();
            else {
                if (c == -1) {
                    Unmark();
                    return false;
                }

                if (@explicit) {
                    Reset();
                    return false;
                }
                else
                    _reader.Unread();
            }            

            if (!ParseNewline())
            {
                Reset();
                return false;
            }

            Unmark();
            return true;
        }

        public bool ParseHeader() 
        {
            Mark();

            int c = _reader.Read();
            int c2 = _reader.Read();
            int c3 = _reader.Read();

            if (c != '-' || c2 != '-' || c3 != '-')
            {
                Reset();
                return false;
            }

            while ( ParseSpace()  && ParseDirective() );

            Unmark();
            OnEvent(DOCUMENT_HEADER);
            return true;
        }

        public bool ParseDirective() 
        {
            Mark();

            if ( _reader.Read() != '#' ) {
                _reader.Unread();
                Unmark();
                return false;
            }

            if ( ! ParseWord() ) {
                Reset();
                return false;
            }

            if ( _reader.Read() != ':') {
                Reset();
                return false;
            }

            if (! ParseLine() ) {
                Reset();
                return false;
            }

            OnContent("directive", _reader.GetString() );
            Unmark();
            return true;
        }

        public bool ParseTransfer()
        {
            Mark();

            if ( _reader.Read() != '!' ) {
                _reader.Unread();
                Unmark();
                return false;
            }

            if (! ParseLine() ) {
                Reset();
                return false;
            }

            Unmark();
            _props.Add("transfer", _reader.GetString() );
            return true;
        }

        public bool ParseProperties() 
        {
            Mark();

            if (ParseTransfer())
            {
                ParseSpace();
                ParseAnchor();
                Unmark();
                return true;
            }

            if (ParseAnchor())
            {
                ParseSpace();
                ParseTransfer();
                Unmark();
                return true;
            }

            Reset();
            return false;
        }

        public bool ParseKey(int n) 
        {
            if (_reader.GetCurrent() == '?') {
                _reader.Read();
                if (!ParseNestedValue(n + 1)) throw new ParserException("'?' key indicator without a nested value", _line);
                if (!ParseIndent(n)) throw new ParserException("Incorrect indentations after nested key", _line);
                return true;
            }

            if (!ParseInlineValue())
                return false;

            ParseSpace();
            return true;
        }

        public bool ParseValue(int n) 
        {
            if (ParseNestedValue(n) || ParseBlockValue(n))
                return true;

            if (!ParseInlineValue()) return false;

            if (! ParseEnd() )
                throw new ParserException("Unterminated inline value",_line);

            return true;
        }

        public bool ParseNaValue(int n)
        {
            if (ParseNestedValue(n) || ParseBlockValue(n))
                return true;

            if (!ParseInlineNaValue()) return false;

            if (! ParseEnd() )
                throw new ParserException("Unterminated inline value",_line);

            return true;
        }

        public bool ParseInlineValue()
        {
            Mark();

            if (ParseProperties())
                ParseSpace();

            if (ParseAlias() || ParseString()) {
                SendEvents();
                Unmark();
                return true;
            }

            if (ParseList() || ParseMap()) {
                Unmark();
                return true;
            }

            ClearEvents();
            Reset();
            return false;
        }

        public bool ParseInlineNaValue()
        {
            Mark();

            if (ParseProperties())
                ParseSpace();

            if (ParseString()) {
                SendEvents();
                Unmark();
                return true;
            }

            if (ParseList() || ParseMap())
            {
                Unmark();
                return true;
            }

            ClearEvents();
            Reset();
            return false;
        }

        public bool ParseNestedValue(int n)
        {
            Mark();
    // System.out.println("----------------------- 0");
            if (ParseProperties())
                ParseSpace();
    // System.out.println("----------------------- 1");
            if (!ParseEnd())
            {
                ClearEvents();
                Reset();
                return false;
            }
    // System.out.println("----------------------- 2");
            SendEvents();

            while (ParseComment(n, false)) ;
    // System.out.println("----------------------- 3");
            if (ParseNestedList(n) || ParseNestedMap(n))
            {
                Unmark();
                return true;
            }

            Reset();
            return false;
        }

        public bool ParseBlockValue(int n)
        {
            Mark();

            if (ParseProperties())
                ParseSpace();

            if (!ParseBlock(n))
            {
                ClearEvents();
                Reset();
                return false;
            }

            SendEvents();

            while (ParseComment(n, false)) ;

            Unmark();
            return true;
        }

        public bool ParseNestedMap(int n) 
        {
            Mark();
        // System.out.println("----------------------- 10");
            int indent = ParseIndent();

            if (n == -1)
                n = indent;
            else
                if (indent > n)
                    n = indent;

            _pendingEvent = '{';

            int i = 0;
            while (true) {
                if (! ParseIndent(n)) break;
                if (! ParseNestedMapEntry(n) ) break;
                i++;
            }
        // System.out.println("----------------------- 11");
            
          if (i>0) {
                OnEvent(MAP_CLOSE);
                Unmark();
                return true;
            }
        // System.out.println("----------------------- 12");
            _pendingEvent = Char.MinValue;
            Reset();
            return false;
        }

        public bool ParseNestedMapEntry(int n) 
        {
            if (!ParseKey(n)) return false;
            if ( _reader.GetCurrent() != ':') return false;
            _reader.Read();

            OnEvent(MAP_SEPARATOR);
            ParseSpace();    /* enforce this space? */

            if (! ParseValue(n+1)) throw new ParserException("no value after ':'",_line);

            return true;
        }

        public bool ParseNestedList(int n) 
        {
            Mark();

            int indent = ParseIndent();

            if (n == -1)
                n = indent;
            else
                if (indent > n)
                    n = indent;

            _pendingEvent = '[';

            int i=0;
            while (true) {
                if (! ParseIndent(n)) break;
                if (!ParseNestedListEntry(n)) break;
                i++;
            }

            if (i>0) {
                OnEvent(LIST_CLOSE);
                Unmark();
                return true;
            }

            _pendingEvent = char.MinValue;
            Reset();
            return false;
        }

        public bool ParseNestedMapInlist(int n) 
        {
            Mark();

            if (! ParseString()) {
                Reset();
                return false;
            }

            ParseSpace();

            if (_reader.Read() != ':') {
                 Reset();
                 return false;
            }
            if (! ParseSpace()) {
                Reset();
                return false;
            }
            if (! ParseValue(n+1))
                throw new ParserException("No value after ':' in map_in_list",_line);

            ParseNestedMap(n+1);

            Unmark();
            return true;
        }

        public bool ParseNestedListEntry(int n) 
        {
            if (_reader.GetCurrent() != '-') return false;
            _reader.Read();

    // System.out.println("nlist_entry");

            if (!ParseSpace())
                throw new ParserException("No space after nested list entry",_line);

            if (ParseNestedMapInlist(n+1) || ParseValue(n+1) ) {
                return true;
            }

            throw new ParserException("bad nlist",_line);
        }

        public bool ParseBlock(int n) 
        {
            int c = _reader.GetCurrent();
            if (c != '|' && c != ']') return false;

            _reader.Read();
            if (_reader.GetCurrent() == '\\') _reader.Read();

            ParseSpace();
            if (ParseNumber())
                ParseSpace();

            if (!ParseNewline()) throw new ParserException("No newline after block definition",_line);

            StringBuilder sb = new StringBuilder();

            while ( ParseBlockLine(n,sb, (char) c) ) ;

            OnContent("string",sb.ToString());

            return true;
        }

        public bool ParseBlockLine(int n, StringBuilder sb, char ch)
        {
            int indent = ParseIndent();

            if (indent < n)
                return false;

            n = indent;

            ParseIndent(n);

            Mark();

            ParseLineSp();
            sb.Append ( _reader.GetString() );

            Unmark();

            if (ch == '|')
                sb.Append('\n');
            else
                sb.Append(' ');

            ParseNewline();
            return true;
        }

        public bool ParseList()  
        {
            if (_reader.GetCurrent() != '[') return false;
            _reader.Read();

            OnEvent(LIST_OPEN);

            while (ParseListEntry())
            {
                int c = _reader.GetCurrent();
                if (c == ']')
                {
                    _reader.Read();
                    OnEvent(LIST_CLOSE);
                    return true;
                }
                if (c != ',')
                    throw new ParserException("inline list error: expecting ','",_line);
                _reader.Read();
            }
            throw new ParserException("inline list error",_line);
        }

        public bool ParseListEntry() 
        {
            ParseSpace();

            if (!ParseInlineValue())
                return false;

            ParseSpace();
            return true;
        }

        public bool ParseMap() 
        {
            if ( _reader.GetCurrent() != '{') return false;
            _reader.Read();

            OnEvent(MAP_OPEN);

            while (ParseMapEntry())
            {
                int c = _reader.GetCurrent();
                if (c == '}')
                {
                    _reader.Read();
                    OnEvent(MAP_CLOSE);
                    return true;
                }
                if (c != ',')
                    throw new ParserException("inline map error: expecting ','",_line);
                _reader.Read();
            }
            throw new ParserException("inline map error",_line);
        }

        

        public bool ParseMapEntry()  
        {
            ParseSpace();

            if (!ParseInlineValue())
                return false;

            ParseSpace();

            if (_reader.GetCurrent() != ':')
                return false;

            _reader.Read();

            OnEvent(MAP_SEPARATOR);

            if (!ParseSpace())
                throw new ParserException("No space after ':'",_line);

            if (!ParseInlineValue())
                throw new ParserException("No value after ':'",_line);

            ParseSpace();
            return true;
        }

        public bool ParseFirstDocument()  
        {
            bool b = ParseNestedList(-1) || ParseNestedMap(-1);
            if ( ! b ) throw new ParserException("first document is not a nested list or map",_line);
            return true;
        }

        /** document_next ::= header node_non_alias(-1) */

        public bool ParseNextDocument() 
        {
            if (!ParseHeader()) return false;
            if (!ParseNaValue(-1)) return false;
            return true;
        }

        public bool TryParse()
        {
            try
            {
                Parse();
                return true;
            }
            catch (ParserException)
            {
                return false;
            }
        }

        public void Parse()
        {
            try {
                while (ParseComment(-1,false));
                
                if (!ParseHeader())
                   ParseFirstDocument();
                else
                    ParseNaValue(-1);

                while (ParseNextDocument());
            }
            catch (ParserException e)
            {
                OnError(e, e.LineNumber);
                throw;
            }
        }

        private void Mark()
        {
            this._reader.Mark();
        }

        private void Reset()
        {
            this._reader.Reset();
        }

        private void Unmark()
        {
            this._reader.Unmark();
        }


    }
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
    public class Marker : IDisposable
    {
        private ParserReader _reader;
        private bool _resetCalled;

        public Marker(ParserReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            _reader = reader;
            _reader.Mark();
        }

        public string GetString()
        {
            return _reader.GetString();
        }

        public void Reset()
        {            
            _reader.Reset();
            _resetCalled = true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if(!_resetCalled)
                _reader.Unmark();            
        }

        #endregion
    }
    public interface ParserEvent
    {
        void @Event(int i);
        void @Event(String s);
        void Content(String a, String b);
        void Property(String a, String b);
        void Error(Exception e, int line);
    }

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
    public class ParserReader
    {
        private TextReader _reader;
        private char[] _buffer;        
        private int _bufferPosition;
        private int _readPosition;
        private int _eofPosition;
        private int _markLevel;        
        private int[] _marks;
        private int _bufferSize ;
        
        
        public const int DEFAULT_BUFFER_LEN = 4096;

        public ParserReader(TextReader reader)
            : this(reader,DEFAULT_BUFFER_LEN)
        {

        }

        public ParserReader(TextReader reader, int bufferSize)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (bufferSize < 0) throw new ArgumentOutOfRangeException("bufferSize");           

            this._bufferSize = bufferSize;
            this._reader = reader;            
            this._buffer = new char[_bufferSize];
            this._buffer[0] = Char.MinValue;
            this._bufferPosition = 0;
            this._readPosition = 0;
            this._eofPosition = -1;
            this._markLevel = 0;            
            this._marks = new int[32];            
        }

        public char CurrentChar
        {
            get {
                if (_bufferPosition == 0)
                    throw new InvalidOperationException("A Read operation must occur prior to accessing the CurrentChar field.");
                return this._buffer[this._bufferPosition - 1]; 
            }
        }
	

        /// <summary>
        /// Gets a string from the last Mark() to the current character
        /// </summary>
        /// <returns></returns>
        public string GetString() 
        {
            int begin = this._marks[this._markLevel - 1];
            int end = this._bufferPosition;

            return new String(this._buffer, begin, end - begin);
        }

        /// <summary>
        /// Returns the current character then
        /// reads the next character in the stream.
        /// 
        /// 
        /// </summary>
        /// <returns>-1 if no more chars are available or the next character</returns>
        public int Read()
        {               
            _bufferPosition++;
            if (PastEndOfBuffer())
                if (!FillBuffer())
                {
                    //_bufferPosition = _eofPosition;
                    return -1;
                }
            
            return CurrentChar;
        }

        public int Peek()
        {
            try
            {
                return this.Read();
            }
            finally
            {
                this.Unread();
            }
        }

        public bool ReadIf(int c)
        {
            if (this.Peek() == c)
            {
                this.Read();
                return true;
            }
            else
            {
                return false;
            }
        }

        public int ReadIf(Predicate<int> match)
        {
            if (match(this.Peek()))
            {
                return this.Read();
            }
            else 
            {
                return -1;
            }
        }
        /// <summary>
        /// Checks the next character for a match, if it matches then it reads in the character
        /// </summary>
        /// <param name="match">delegate to match with in the form of bool(int)</param>
        /// <returns>Returns the number of matched characters</returns>
        public int ReadWhile(Predicate<int> match) 
        {
            int i = 0;
            while (match(this.Peek()))
            {
                this.Read();
                i++;
            }
            return i;
        }


        /// <summary>
        /// Checks the next character for a match, if it matches then it reads in the character
        /// </summary>
        /// <param name="match">delegate to match with in the form of bool(int)</param>
        /// <returns>Returns the number of matched characters</returns>
        public int ReadWhile(int maxReads, Predicate<int> match)
        {
            int i = 0;
            while (match(this.Peek()) && maxReads-- > 0)
            {
                this.Read();
                i++;
            }
            return i;
        }

        /// <summary>
        /// Returns a value indicating is the bufferPosition is more then the number of characters read into the buffer.
        /// </summary>
        /// <returns></returns>
        private bool PastEndOfBuffer()
        {
            return _bufferPosition > _readPosition;
        }

        /// <summary>
        /// Private function to fill the character buffer. Doubles the buffer sizeif it is not big enough.
        /// </summary>
        private bool FillBuffer()
        {
            if (_bufferPosition >= _eofPosition && _eofPosition != -1)
                return false;

            if (_bufferPosition > _buffer.Length) //double buffer
            {
                char[] charBuffer = new char[_buffer.Length * 2];
                Buffer.BlockCopy(_buffer, 0, charBuffer, 0, _readPosition * 2);
                _buffer = charBuffer;                
            }
            
            int charsRead = _reader.Read(_buffer,_readPosition,_buffer.Length - _readPosition);
            if (charsRead == 0)
            {
                _eofPosition = _readPosition + 1;
                return false;
            }
            
            _readPosition += charsRead;
            return true;
        }

        /// <summary>
        /// Gets the current character
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use CurrentChar field")]
        public int GetCurrent()
        {
            return this.Peek();
             
        }

        /// <summary>
        /// Gets the character prior to the last character. If there is no previous character it returns -1;
        /// </summary>
        /// <returns></returns>
        public int GetPrevious()
        {            
            if (this._bufferPosition < 2)
                return -1;
            else 
                return this._buffer[this._bufferPosition - 2];
        }

        /// <summary>
        /// Marks a location in the stream. This can be used for marking undo points to go back in the event of incorrect parsing or a failure.
        /// </summary>        
        public void Mark()
        {
            if (_markLevel == 32) throw new InvalidOperationException("Cannot create more then 32 marks deep.");
            this._marks[this._markLevel] = this._bufferPosition;
            this._markLevel++;
        }

        /// <summary>
        /// Unmarks the last location in the stream that was set.
        /// </summary>        
        public void Unmark()
        {
            if (this._markLevel < 1)
                throw new InvalidOperationException("No more marks to unmark");

            this._markLevel--;
        }

        public Marker GetMarker()
        {
            return new Marker(this);
        }

        /// <summary>
        /// Reverts reading back to the previous mark. Performs an Unmark.
        /// </summary>
        public void Reset()
        {
            Unmark();
            this._bufferPosition = this._marks[this._markLevel];
        }

        [Obsolete("This method is obsolete there should be no reason to go backwords when marking is available.")]
        public void Unread() {            
            if (_bufferPosition == 0) 
                throw new InvalidOperationException("Cannot unread past beginning of buffer.");
            _bufferPosition--;
        }
    }

    public class YamlCharacter
    {

        private const string INDICATORS = "-:[]{},?*&!|#@%^'\"";
        private const string INDICATORS_SP = "-:";
        private const string INDICATORS_INLINE = "[]{},";
        private const string INDICATORS_SIMPLE = ":[]{},";
        private const string INDICATORS_NONSP = "?*&!]|#@%^\"'";
        public static bool @Is(char c, YamlCharacterType type)
        {
            switch (type)
            {
                case YamlCharacterType.Printable: return IsPrintableChar(c);
                case YamlCharacterType.Word: return IsWordChar(c);
                case YamlCharacterType.Line: return IsLineChar(c);
                case YamlCharacterType.LineSP: return IsLineSpChar(c);
                case YamlCharacterType.Space: return IsSpaceChar(c);
                case YamlCharacterType.LineBreak: return IsLineBreakChar(c);
                case YamlCharacterType.Digit: return Char.IsDigit(c);
                case YamlCharacterType.Indent: return (c == ' ');                
                default: return false;

            }

        }

        public static bool IsLineBreakChar(char c)
        {
            if (c == 10 || c == 13 || c == 0x85 || c == 0x2028 || c == 0x2029) return true;
            return false;
        }

        public static bool IsSpaceChar(char c)
        {
            if (c == 9 || c == 0x20) return true;
            return false;
        }

        public static bool IsLineSpChar(char c)
        {
            if (c == 10 || c == 13 || c == 0x85) return false;
            return IsPrintableChar(c);
        }

        public static bool IsWordChar(char c)
        {
            if (c >= 0x41 && c <= 0x5a) return true;
            if (c >= 0x61 && c <= 0x7a) return true;
            if (c >= 0x30 && c <= 0x39) return true;
            if (c == '-') return true;
            return false;
        }

        public static bool IsPrintableChar(char c)
        {
            if (c >= 0x20 && c <= 0x7e) return true;
            if (c == 9 || c == 10 || c == 13 || c == 0x85) return true;
            if (c >= 0xa0 && c <= 0xd7ff) return true;
            if (c >= 0xe000 && c <= 0xfffd) return true;                        
            return false;
        }

        public static bool IsLineChar(char c)
        {
            if (c == 0x20 || c == 9 || c == 10 || c == 13 || c == 0x85) return false;
            return IsPrintableChar(c);
        }

        public static bool @Is(int c, YamlCharacterType type)
        {
            if (c == -1) return false;
            char ch = Convert.ToChar(c);
            return Is(ch, type);
        }

        public static bool IsIndicator(char c)
        {            
            return (INDICATORS.IndexOf(c) != -1);
        }

        public static bool IsIndicatorSpace(char c)
        {
            return (INDICATORS_SP.IndexOf(c) != -1);
        }

        public static bool IsIndicatorInline(char c)
        {
            return (INDICATORS_INLINE.IndexOf(c) != -1);
        }
        
        public static bool IsIndicatorNonSpace(char c)
        {
            return (INDICATORS_NONSP.IndexOf(c) != -1);
        }

        public static bool IsIndicatorSimple(char c)
        {
            return (INDICATORS_SIMPLE.IndexOf(c) != -1);
        }

    }
    public enum YamlCharacterType
    {
        Printable,
        Word,
        Line,
        LineSP,
        Space ,
        LineBreak,
        Digit,
        Indent,
    }

}
