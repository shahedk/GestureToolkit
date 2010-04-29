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
using Gestures.Rules.Objects;
using Gestures.Objects;

namespace Gestures.Rules.RuleValidators
{
    public class OnSameObjectValidator : IRuleValidator
    {

        #region IRuleValidator Members

        public void Init(IRuleData ruleData)
        {

        }

        public bool Equals(IRuleValidator rule)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(System.Collections.Generic.List<TouchPoint2> points)
        {
            ValidSetOfPointsCollection sets = new ValidSetOfPointsCollection();

#if SILVERLIGHT

            //TODO: Fix it for WPF 4.0
            // This code block only works in Silverlight. Because the WPF 4 framework does not have VisualTreeHelper.FindElementsInHostCoordinates(...)

            // TODO: Review logic
            // For simplicity, we are only comparing the first element the comes from the hit-test in visual tree
            // but for complex objects (that consists of multiple ui-elements), we may have to go deeper

            if (points.Count > 0)
            {
                bool result = true;
                var list = VisualTreeHelper.FindElementsInHostCoordinates(points[0].Position, RuleValidationHelper.LayoutRoot);
                var e1 = list.GetEnumerator();

                UIElement uiElement = null;
                if (e1.MoveNext())
                    uiElement = e1.Current;

                for (int i = 1; i < points.Count; i++)
                {
                    var uiElements = VisualTreeHelper.FindElementsInHostCoordinates(points[i].Position, RuleValidationHelper.LayoutRoot);
                    var e2 = uiElements.GetEnumerator();

                    if (e2.MoveNext())
                    {
                        UIElement firstUIElement = e2.Current;

                        if (uiElement != firstUIElement)
                        {
                            result = false;
                            break;
                        }
                    }
                }

                if (result)
                    sets.Add(new ValidSetOfTouchPoints(points));
            }
#endif

            return sets;
        }

        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            return sets.ForEachSet(Validate);

            //ValidSetOfPointsCollection validSets = new ValidSetOfPointsCollection();
            //foreach (var set in sets)
            //{
            //    var list = Validate(set);
            //    foreach (var item in list)
            //    {
            //        validSets.Add(item);
            //    }
            //}

            //return validSets;
        }

        #endregion


        public IRuleData GenerateRuleData(List<TouchPoint2> points)
        {
            throw new NotImplementedException();
        }
    }
}
