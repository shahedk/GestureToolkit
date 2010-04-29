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
    public class TouchArea : IRuleData
    {
        private string _type = string.Empty;
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private string _value = string.Empty;
        public string Value
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

        private int _history = 0;
        public int HistoryLevel
        {
            get
            {
                return _history;
            }
            set
            {
                _history = value;
            }
        }

        private int _historyTimeline = 0;
        public int HistoryTimeLine
        {
            get
            {
                return _historyTimeline;
            }
            set
            {
                _historyTimeline = value;
            }
        }

        public int Height
        {
            get
            {
                // TODO: refactor
                return int.Parse(Value.Split("x".ToCharArray())[0]);
            }
        }

        public int Width
        {
            get
            {
                // TODO: refactor
                return int.Parse(Value.Split("x".ToCharArray())[1]);
            }
        }

        public bool Equals(IRuleData ruleData)
        {
            var data = ruleData as TouchArea;

            return (data.Type == this.Type && data.Value == this.Value);
        }


        public void Union(IRuleData value)
        {
            throw new NotImplementedException();
        }


        public string ToGDL()
        {
            throw new NotImplementedException();
        }
    }
}
