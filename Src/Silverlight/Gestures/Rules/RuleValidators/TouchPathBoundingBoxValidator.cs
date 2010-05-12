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
using System.Collections.Generic;
using Gestures.Objects;

namespace Gestures.Rules.RuleValidators
{
    public class TouchPathBoundingBoxValidator : IRuleValidator
    {
        TouchPathBoundingBox _data;

        #region IRuleValidator Members

        public void Init(IRuleData ruleData)
        {
            _data = ruleData as TouchPathBoundingBox;
        }

        public bool Equals(IRuleValidator rule)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(System.Collections.Generic.List<TouchPoint2> points)
        {
            ValidSetOfPointsCollection sets = new ValidSetOfPointsCollection();

            if (points.Count > 0)
            {
                Rect parent = points[0].Stroke.GetBounds();
                for (int i = 1; i < points.Count; i++)
                {
                    Rect rect = points[i].Stroke.GetBounds();
                    parent.Union(rect);
                }

                if (parent.Height >= _data.MinHeight && parent.Height <= _data.MaxHeight
                    && parent.Width >= _data.MinWidth && parent.Width <= _data.MaxWidth)
                {
                    ValidSetOfTouchPoints set = new ValidSetOfTouchPoints(points);
                    sets.Add(set);
                }
            }
            return sets;
        }

        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            //TODO: move this code block to base class or extension methods. its common in many validators
            ValidSetOfPointsCollection validSets = new ValidSetOfPointsCollection();
            foreach (var set in sets)
            {
                var list = Validate(set);
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
