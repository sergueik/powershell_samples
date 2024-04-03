using System;
using System.Collections.Generic;
using System.Text;

namespace Yaml
{
    public interface ParserEvent
    {
        void @Event(int i);
        void @Event(String s);
        void Content(String a, String b);
        void Property(String a, String b);
        void Error(Exception e, int line);
    }
}
