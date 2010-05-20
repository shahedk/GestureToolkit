using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Collections.Generic;
using Gestures.Objects;
using Gestures.Rules.Objects;
using Gestures.Utility;

namespace Gestures.Rules.RuleValidators
{
    public class TouchDirectionValidator : IRuleValidator
    {
        private TouchDirection _data;

        #region IRuleValidator Members

        public void Init(IRuleData ruleData)
        {
            _data = ruleData as TouchDirection;
        }

        public bool Equals(IRuleValidator rule)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(System.Collections.Generic.List<TouchPoint2> points)
        {
            ValidSetOfPointsCollection sets = new ValidSetOfPointsCollection();
            ValidSetOfTouchPoints list = new ValidSetOfTouchPoints();

            foreach (var point in points)
            {

                int length = point.Stroke.StylusPoints.Count;
                bool result = true;
                for (int i = 0; i + 1 < length; i++)
                {
                    double slope = TrigonometricCalculationHelper.GetSlopBetweenPoints(point.Stroke.StylusPoints[i], point.Stroke.StylusPoints[i + 1]);
                    if (!SlopeToDirection(slope).Equals(_data.Values))
                    {
                        result = false;
                    }
                }
                if (result)
                {
                    list.Add(point);
                }
            }
            if (list.Count > 0)
            {
                sets.Add(list);
            }
            return sets;
        }

        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            ValidSetOfPointsCollection ret = new ValidSetOfPointsCollection();
            foreach (var item in sets)
            {
                ValidSetOfPointsCollection list = Validate(item);
                foreach (var set in list)
                {
                    ret.Add(set);
                }
            }
            return ret;
        }

        #endregion

        public IRuleData GenerateRuleData(List<TouchPoint2> points)
        {
            throw new NotImplementedException();
        }

        private double FindSlope(TouchPoint2 pointa, TouchPoint2 pointb)
        {
            double DeltaY = pointb.Position.Y - pointa.Position.Y;
            double DeltaX = pointb.Position.X - pointa.Position.X;
            return Math.Atan2(DeltaY, DeltaX);
        }

        private String SlopeToDirection(double slope)
        {
            String direction = "";

            if (slope >= 15 * Math.PI / 8 && slope <= Math.PI / 8)
            {
                direction = "Right";
            }
            else if (slope > Math.PI / 8 && slope < 3 * Math.PI / 8)
            {
                direction = "UpRight";
            }
            else if (slope >= 3 * Math.PI / 8 && slope <= 5 * Math.PI / 8)
            {
                direction = "Up";
            }
            else if (slope > 5 * Math.PI / 8 && slope < 7 * Math.PI / 8)
            {
                direction = "UpLeft";
            }
            else if (slope >= 7 * Math.PI / 8 && slope <= 9 * Math.PI / 8)
            {
                direction = "Left";
            }
            else if (slope > 9 * Math.PI / 8 && slope < 11 * Math.PI / 8)
            {
                direction = "DownLeft";
            }
            else if (slope >= 11 * Math.PI / 8 && slope <= 13 * Math.PI / 8)
            {
                direction = "Down";
            }
            else if (slope > 13 * Math.PI / 8 && slope < 15 * Math.PI / 8)
            {
                direction = "DownRight";
            }
            return direction;
        }

    }
}
