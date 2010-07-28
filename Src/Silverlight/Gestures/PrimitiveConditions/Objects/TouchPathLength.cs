using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace TouchToolkit.GestureProcessor.PrimitiveConditions.Objects
{
    public class TouchPathLength : IPrimitiveConditionData
    {
        private double _min = 0f;
        public double Min
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
            }
        }

        private double _max = 0f;
        public double Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
            }
        }

        public bool Equals(IPrimitiveConditionData rule)
        {
            throw new NotImplementedException();
        }


        public void Union(IPrimitiveConditionData value)
        {
            TouchPathLength touchPathLength = value as TouchPathLength;

            if (this.Min > touchPathLength.Min)
                this.Min = touchPathLength.Min;

            if (this.Max < touchPathLength.Max)
                this.Max = touchPathLength.Max;

        }


        public string ToGDL()
        {
            return string.Format("Touch path length: {0}..{1}", this.Min, this.Max);
        }
    }
}
