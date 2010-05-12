using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Gestures.Utility;
using Gestures.Rules.Objects;
using System.Collections.Generic;
using Gestures.Objects;

namespace Gestures.Rules.RuleValidators
{
    public class ClosedLoopValidator : IRuleValidator
    {
        private ClosedLoop _data;
        int threshHold = 100; // 100 pixel

        #region IRuleValidator Members

        public void Init(IRuleData ruleData)
        {
            _data = ruleData as ClosedLoop;
        }

        public bool Equals(IRuleValidator rule)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(System.Collections.Generic.List<TouchPoint2> points)
        {
            ValidSetOfPointsCollection sets = new ValidSetOfPointsCollection();
            ValidSetOfTouchPoints set = new ValidSetOfTouchPoints();

            foreach (var point in points)
            {
                if (IsClosedLoop(point))
                    set.Add(point);
            }

            if (set.Count > 0)
                sets.Add(set);

            return sets;
        }

        private bool IsClosedLoop(TouchPoint2 point)
        {
            // Check the distance between start and end point
            if (point.Stroke.StylusPoints.Count > 1)
            {
                StylusPoint firstPoint = point.Stroke.StylusPoints[0];
                StylusPoint lastPoint = point.Stroke.StylusPoints[point.Stroke.StylusPoints.Count - 1];

                double distance = TrigonometricCalculationHelper.GetDistanceBetweenPoints(firstPoint, lastPoint);

                if (distance < threshHold)
                    return true;
            }
            return false;
        }

        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            return sets.ForEachSet(Validate);
        }

        #endregion


        public IRuleData GenerateRuleData(List<TouchPoint2> points)
        {
            throw new NotImplementedException();
        }
    }
}
