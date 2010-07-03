using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using TouchToolkit.GestureProcessor.Rules.Objects;
using ShapeRecognizers.ConvexHull;
using System.Collections.Generic;
using TouchToolkit.GestureProcessor.Objects;

namespace TouchToolkit.GestureProcessor.Rules.RuleValidators
{
    public class EnclosedAreaValidator : IRuleValidator
    {
        private EnclosedArea _data;
        #region IRuleValidator Members

        public void Init(IRuleData ruleData)
        {
            _data = ruleData as EnclosedArea;
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
                if (IsEnclosedAreaWithinRange(point.Stroke.StylusPoints))
                    set.Add(point);
            }

            if (set.Count > 0)
                sets.Add(set);

            return sets;
        }

        private bool IsEnclosedAreaWithinRange(StylusPointCollection stylusPointCollection)
        {
            // TODO: Move the magic number to configuration
            Point[] points = stylusPointCollection.ToFilteredPoints(5);

            double area  = ConvexHullArea.GetArea(points);

            if (area >= _data.Min && area <= _data.Max)
                return true;
            else
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
