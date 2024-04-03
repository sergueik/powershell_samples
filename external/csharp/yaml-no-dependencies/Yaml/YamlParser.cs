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
}
