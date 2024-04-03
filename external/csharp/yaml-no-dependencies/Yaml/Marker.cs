using System;
using System.Collections.Generic;
using System.Text;

namespace Yaml
{
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
}
