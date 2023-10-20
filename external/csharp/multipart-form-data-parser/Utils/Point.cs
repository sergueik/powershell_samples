using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public struct Point
    {
        double _x;
        double _y;

        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }
        public double X { get { return _x; } set { _x = value; } }

        public double Y { get { return _y; } set { _y = value; } }
}
}
