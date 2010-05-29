using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace TouchToolkit.GestureProcessor.Rules.Objects
{
    public class IntersectTouchPath : IRuleData
    {
        bool _result;
        public bool Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
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
            throw new NotImplementedException();
        }


        public string ToGDL()
        {
            throw new NotImplementedException();
        }
    }
}
