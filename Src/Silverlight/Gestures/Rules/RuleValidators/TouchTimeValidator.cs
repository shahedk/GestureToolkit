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
using Gestures.Rules.RuleValidators;

namespace Gestures.Rules
{
    /// <summary>
    /// Validates the lifetime of a touch
    /// </summary>
    public class TouchTimeValidator : IRuleValidator
    {
        TouchTime _data;
        public void Init(IRuleData ruleData)
        {
            _data = ruleData as TouchTime;
        }

        public ValidSetOfPointsCollection Validate(List<TouchPoint2> points)
        {
            ValidSetOfPointsCollection sets = new ValidSetOfPointsCollection();
            bool result = true;

            foreach (var point in points)
            {
                // TODO: We need to check the unit type (i.e. sec, min,...) and compare accordingly
                if (point.Age.TotalSeconds <= _data.Value)
                    result = false;
            }

            if (result)
                sets.Add(new ValidSetOfTouchPoints(points));

            return sets;
        }

        public bool Equals(IRuleValidator rule)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            ValidSetOfPointsCollection validSets = new ValidSetOfPointsCollection();
            foreach (var item in sets)
            {
                // Because we know it will return only one item in the list
                var results = Validate(item);
                if (results.Count > 0)
                    validSets.Add(results[0]);
            }

            return validSets;
        }

        public IRuleData GenerateRuleData(List<TouchPoint2> points)
        {
            TouchTime ruleData = new TouchTime();

            ruleData.Unit = "secs";
            ruleData.Value = GetAverageTouchPointAge(points);

            return ruleData;
        }

        private float GetAverageTouchPointAge(List<TouchPoint2> points)
        {
            double val = 0f;
            foreach (var p in points)
            {
                val += p.Age.TotalSeconds;
            }

            return (float)val / points.Count;
        }

    }
}
