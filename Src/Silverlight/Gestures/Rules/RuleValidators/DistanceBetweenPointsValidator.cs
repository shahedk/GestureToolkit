using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Gestures.Rules.Objects;
using Combinatorial;
using Gestures.Exceptions;
using Gestures.Utility;
using System.Collections.Generic;
using Gestures.Objects;

namespace Gestures.Rules.RuleValidators
{
    public class DistanceBetweenPointsValidator : IRuleValidator
    {
        class Behaviour
        {
            public const string Increasing = "increasing";
            public const string Decreasing = "decreasing";
            public const string Range = "range";
            public const string UnChanged = "unchanged";
        }

        DistanceBetweenPoints _data;

        #region IRuleValidator Members

        public void Init(IRuleData ruleData)
        {
            _data = ruleData as DistanceBetweenPoints;
        }

        public bool Equals(IRuleValidator rule)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(List<TouchPoint2> points)
        {

            ValidSetOfPointsCollection sets = new ValidSetOfPointsCollection();

            bool result = false;
            if (points.Count > 2)
            {
                // Assumption: If there are more that 2 points, then each point should 
                // match the condition with another point in at least one condition

                Dictionary<TouchPoint2, bool> resultSet = new Dictionary<TouchPoint2, bool>(points.Count);

                Combinations combinationGen = new Combinations(points.ToArray(), 2);
                while (combinationGen.MoveNext())
                {
                    TouchPoint2[] arr = combinationGen.Current as TouchPoint2[];
                    if (IsValid(arr))
                    {
                        // First item of the combinition set
                        if (!resultSet.ContainsKey(arr[0]))
                            resultSet.Add(arr[0], true);

                        // Second item of the combinition set
                        if (!resultSet.ContainsKey(arr[1]))
                            resultSet.Add(arr[1], true);

                        if (resultSet.Count == points.Count)
                        {
                            // All points have been validated at least once
                            result = true;
                            break;
                        }
                    }
                }
            }
            else if (points.Count == 2)
            {
                result = IsValid(points.ToArray());
            }

            if (result)
            {
                ValidSetOfTouchPoints set = new ValidSetOfTouchPoints(points);
                sets.Add(set);
            }

            return sets;
        }

        private bool IsValid(TouchPoint2[] arr)
        {
            bool result = false;
            double distanceN = 0f, distanceN_1 = 0f;

            if (arr.Length != 2)
                throw new UnexpectedDataFormatException("Distance can be measured between two points only");

            int firstStrokeLen = arr[0].Stroke.StylusPoints.Count;
            int secondStrokeLen = arr[1].Stroke.StylusPoints.Count;

            //Sometimes we get to much precious data that captures unwanted behaviours... 
            //try few times to match the pattern
            for (int n = 1; n < 4; n++)
            {
                if (firstStrokeLen - n < 0 || secondStrokeLen - n < 0)
                    break;

                // Get the point of two gestures at nth position
                StylusPoint p1 = arr[0].Stroke.StylusPoints[firstStrokeLen - n];
                StylusPoint p2 = arr[1].Stroke.StylusPoints[secondStrokeLen - n];
                distanceN = TrigonometricCalculationHelper.GetDistanceBetweenPoints(p1, p2);

                if (_data.Behaviour.ToLower() == Behaviour.Increasing
                    || _data.Behaviour.ToLower() == Behaviour.Decreasing
                    || _data.Behaviour.ToLower() == Behaviour.UnChanged)
                {
                    if (firstStrokeLen - n - 1 < 0 || secondStrokeLen - n - 1 < 0)
                        break;

                    // Get the point of two gestures at (n-1)th position
                    p1 = arr[0].Stroke.StylusPoints[firstStrokeLen - n - 1];
                    p2 = arr[1].Stroke.StylusPoints[secondStrokeLen - n - 1];
                    distanceN_1 = TrigonometricCalculationHelper.GetDistanceBetweenPoints(p1, p2);
                }

                // Validate expected behaviour pattern
                if (_data.Behaviour.ToLower() == Behaviour.Increasing)
                {
                    if (distanceN > distanceN_1)
                    {
                        result = true;
                        break;
                    }
                }
                else if (_data.Behaviour.ToLower() == Behaviour.Decreasing)
                {
                    if (distanceN < distanceN_1)
                    {
                        result = true;
                        break;
                    }
                }
                else if (_data.Behaviour.ToLower() == Behaviour.Range)
                {
                    if (distanceN >= _data.Min && distanceN <= _data.Max)
                    {
                        result = true;
                        break;
                    }
                }
                else if (_data.Behaviour.ToLower() == Behaviour.UnChanged)
                {
                    // Note: Check if the change is within acceptable range
                    // For example: unchanged 10% means if the change is 
                    // below 10% of the distance than consider it acceptable

                    double diff = Math.Abs(distanceN - distanceN_1);
                    if (diff < distanceN / _data.Min) 
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            ValidSetOfPointsCollection validSets = new ValidSetOfPointsCollection();

            foreach (var set in sets)
            {
                ValidSetOfPointsCollection list = Validate(set);
                foreach (var item in list)
                {
                    validSets.Add(item);
                }
            }

            return validSets;
        }

        #endregion


        public IRuleData GenerateRuleData(List<TouchPoint2> points)
        {
            throw new NotImplementedException();
        }
    }
}
