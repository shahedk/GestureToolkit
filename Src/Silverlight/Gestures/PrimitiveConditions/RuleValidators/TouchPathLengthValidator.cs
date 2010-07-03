using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using TouchToolkit.GestureProcessor.PrimitiveConditions.Objects;
using System.Collections.Generic;
using TouchToolkit.GestureProcessor.Utility;
using TouchToolkit.GestureProcessor.Objects;

namespace TouchToolkit.GestureProcessor.PrimitiveConditions.RuleValidators
{
    public class TouchPathLengthValidator : IPrimitiveConditionValidator
    {
        private TouchPathLength _data;

        public void Init(IPrimitiveConditionData ruleData)
        {
            _data = ruleData as TouchPathLength;
        }

        public bool Equals(IPrimitiveConditionValidator rule)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(List<TouchPoint2> points)
        {
            ValidSetOfPointsCollection sets = new ValidSetOfPointsCollection();
            ValidSetOfTouchPoints set = new ValidSetOfTouchPoints();

            // the length can be calculated for a single touch path. So, we check each
            // touchPoint individually

            double length = 0;
            foreach (var point in points)
            {
                length = CalculatePathLength(point);

                if (length >= _data.Min && length <= _data.Max)
                {
                    set.Add(point);
                }
            }

            if( set.Count > 0 )
                sets.Add(set);

            return sets;
        }

        private double CalculatePathLength(TouchPoint2 point)
        {
            // Paths generally contain a lot of points, we are skipping some points 
            // to improve performance. The 'step' variable decides how much we should skip
            int step = 3;
            double pathLength = 0f;

            int len = point.Stroke.StylusPoints.Count;
            if (len > step + 1)
            {
                // Initial point
                StylusPoint p1 = point.Stroke.StylusPoints[0];

                for (int i = 1; i < len; i += step)
                {
                    StylusPoint p2 = point.Stroke.StylusPoints[i - 1];

                    pathLength += TrigonometricCalculationHelper.GetDistanceBetweenPoints(p1, p2);

                    p1 = p2;
                }
            }

            return pathLength;
        }

        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            ValidSetOfPointsCollection validSets = new ValidSetOfPointsCollection();

            foreach (var item in sets)
            {
                ValidSetOfPointsCollection list = Validate(item);
                foreach (var set in list)
                {
                    validSets.Add(set);
                }
            }

            return validSets;
        }


        public IPrimitiveConditionData GenerateRuleData(List<TouchPoint2> points)
        {
            throw new NotImplementedException();
        }
    }
}
