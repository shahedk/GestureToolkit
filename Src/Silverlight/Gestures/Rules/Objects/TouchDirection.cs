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
    public class TouchDirection : IRuleData
    {
        public enum Directions
        {
            Left = 0,
            Right,
            Up,
            Down,
            DownLeft,
            DownRight,
            UpLeft,
            UpRight

            // NOTE: We are not providing any diagonal direction matching because
            //       that would make gestures dependent on Screen Orentation/position
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
