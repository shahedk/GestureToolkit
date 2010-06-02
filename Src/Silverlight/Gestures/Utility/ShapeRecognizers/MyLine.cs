using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchToolkit.GestureProcessor.Utility.ShapeRecognizers
{
    class MyLine
    {
        public int Theta;
        public double R;
        public int Quorum;

        public MyLine(int theta, double r)
        {
            Theta = theta;
            R = r;
            Quorum = 0;
        }
    }
}
