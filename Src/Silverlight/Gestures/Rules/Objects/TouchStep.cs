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
    public class TouchStep : IRuleData
    {
        private int _touchCount = 0;
        public int TouchCount
        {
            get
            {
                return _touchCount;
            }
            set
            {
                _touchCount = value;
            }
        }

        private double _timeLimit = 0;
        public double TimeLimit
        {
            get
            {
                return _timeLimit;
            }
            set
            {
                _timeLimit = value;
            }
        }

        private string _unit = string.Empty;
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

        #region IRuleData Members

        public bool Equals(IRuleData rule)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Union(IRuleData value)
        {
            TouchStep touchStep = value as TouchStep;

            // TouchCount
            if (this.TouchCount < touchStep.TouchCount)
                this.TouchCount = touchStep.TouchCount;

            // TimeLimit
            if (this.Unit == touchStep.Unit)
            {
                if (this.TimeLimit < touchStep.TimeLimit)
                    this.TimeLimit = touchStep.TimeLimit;
            }
            else
            {
                // TODO: We need to consider different units while considering expending the range
                throw new NotImplementedException();
            }
        }

        public string ToGDL()
        {
            throw new NotImplementedException();
        }
    }
}
