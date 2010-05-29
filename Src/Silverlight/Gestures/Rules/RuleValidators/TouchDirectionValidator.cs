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
using TouchToolkit.GestureProcessor.Objects;
using TouchToolkit.GestureProcessor.Rules.Objects;
using TouchToolkit.GestureProcessor.Utility;
using TouchToolkit.GestureProcessor.Utility.TouchHelpers;

namespace TouchToolkit.GestureProcessor.Rules.RuleValidators
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

        public ValidSetOfPointsCollection Validate(List<TouchPoint2> points)
        {
            ValidSetOfPointsCollection sets = new ValidSetOfPointsCollection();
            ValidSetOfTouchPoints list = new ValidSetOfTouchPoints();

            foreach (var point in points)
            {

                int length = point.Stroke.StylusPoints.Count;
                bool result = true;
                int step = 3;
                for (int i = 0; i + step < length - step; i = i + step)
                {
                    double slope = TrigonometricCalculationHelper.GetSlopeBetweenPoints(point.Stroke.StylusPoints[i], 
                        point.Stroke.StylusPoints[i + step]);
                    String stringSlope = TouchPointExtensions.SlopeToDirection(slope);

                    if (!stringSlope.Equals(_data.Values))
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
    }
}
