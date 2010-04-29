using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace Gestures.Rules.Objects
{
    public class TouchTime : IRuleData
    {
        private float _value = 0f;
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        private string _unit = "sec";
        public string Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
            }
        }

        public bool Equals(IRuleData rule)
        {
            throw new NotImplementedException();
        }

        public void Union(IRuleData value)
        {
            TouchTime touchTime = value as TouchTime;

            if (this.Unit == touchTime.Unit)
            {
                if (this.Value < touchTime.Value)
                    this.Value = touchTime.Value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public string ToGDL()
        {
            return string.Format("Touch time: {0} {1}", this.Value, this.Unit);
        }
    }
}
