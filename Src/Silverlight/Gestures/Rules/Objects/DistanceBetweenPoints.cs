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
    public class DistanceBetweenPoints : IRuleData
    {
        private string _behaviour = string.Empty;
        public string Behaviour
        {
            get
            {
                return _behaviour;
            }
            set
            {
                _behaviour = value;
            }
        }

        int _minValue = 0;
        public int Min
        {
            get
            {
                return _minValue;
            }
            set
            {
                _minValue = value;
            }
        }

        int _maxValue = 0;
        public int Max
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
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
            DistanceBetweenPoints distanceBtwPoints = value as DistanceBetweenPoints;

            if (this.Behaviour != distanceBtwPoints.Behaviour)
            {

                if (this.Behaviour == "increasing" && distanceBtwPoints.Behaviour == "decreasing")
                {
                    throw new Exception("Invalid Union Type");
                }
                else if (this.Behaviour == "decreasing" && distanceBtwPoints.Behaviour == "increasing")
                {
                    throw new Exception("Invalid Union Type");
                }
                else if (this.Behaviour == "unchanged" && distanceBtwPoints.Behaviour == "increasing")
                {
                    this.Max = distanceBtwPoints.Max;
                    this.Behaviour = "increasing";
                }
                else if (this.Behaviour == "unchanged" && distanceBtwPoints.Behaviour == "decreasing")
                {
                    this.Min = distanceBtwPoints.Min;
                    this.Behaviour = "decreasing";
                }
                else if (this.Behaviour == "unchanged" && distanceBtwPoints.Behaviour == "unchanged")
                {
                    //do nothing
                }
                else if (this.Behaviour == "increasing" && distanceBtwPoints.Behaviour == "unchanged")
                {
                    //do nothing
                }
                else if (this.Behaviour == "increasing" && distanceBtwPoints.Behaviour == "increasing")
                {
                    this.Max = distanceBtwPoints.Max;
                    this.Behaviour = "increasing";
                }
                else if (this.Behaviour == "decreasing" && distanceBtwPoints.Behaviour == "unchanged")
                {
                    //do nothing
                }
                else if (this.Behaviour == "decreasing" && distanceBtwPoints.Behaviour == "decreasing")
                {
                    this.Min = distanceBtwPoints.Min;
                    this.Behaviour = "decreasing";
                }
            }
        }


        public string ToGDL()
        {
            if (this.Behaviour == "unchanged")
                return string.Format("Distance between points : unchanged {0}%", this.Min);

            else if (this.Behaviour == "increasing")
                return string.Format("Distance between points : increasing");

            else
                return string.Format("Distance between points : decreasing");
        }
    }
}
