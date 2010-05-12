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
    public class ClosedLoop : IRuleData
    {
        string _state = string.Empty;
        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
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
            if (value == null)
            {
                throw new Exception("Null Value Exception");
            }
            if (!(value is ClosedLoop))
            {
                throw new Exception("Invalid Type Exception");
            }
            ClosedLoop cLoop = value as ClosedLoop;

            if (this.State == "false" || cLoop.State == "false")
                this.State = "false";
            else
                this.State = "true";

        }

        public string ToGDL()
        {
            if (this.State == "true")
                return string.Format("Closed loop");
            else
                return string.Empty;
        }
    }
}