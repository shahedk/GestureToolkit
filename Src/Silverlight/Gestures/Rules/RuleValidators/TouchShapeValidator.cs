using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

using Gestures.Rules.Objects;
using Gestures.Objects;
using Gestures.Utility;
using Gestures.Utility.TouchHelpers;
using Framework.ShapeRecognizers;

namespace Gestures.Rules.RuleValidators
{
    public class TouchShapeValidator : IRuleValidator
    {
        private TouchShape _data;

        public void Init(Objects.IRuleData ruleData)
        {
            _data = ruleData as TouchShape;
        }

        public bool Equals(IRuleValidator rule)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(List<TouchPoint2> points)
        {
            ValidSetOfPointsCollection ret = new ValidSetOfPointsCollection();
            if (_data.Values == "Line")
            {
                foreach (var point in points)
                {
                    ValidSetOfTouchPoints tps = ValidateLine(point);
                    if (tps.Count > 0)
                    {
                        ret.Add(tps);
                    }
                }
            }
            else if (_data.Values == "Box")
            {
                throw new NotImplementedException();
            }
            else if (_data.Values == "Circle")
            {
                throw new NotImplementedException();
            }

            return ret;
        }

        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            ValidSetOfPointsCollection ret = new ValidSetOfPointsCollection();
            foreach (var set in sets)
            {
                ValidSetOfPointsCollection list = Validate(set);
                foreach (var item in list)
                {
                    ret.Add(item);
                }
            }
            return ret;
        }

        private ValidSetOfPointsCollection ValidateBox(List<TouchPoint2> points)
        {
            throw new NotImplementedException();
        }

        private ValidSetOfTouchPoints ValidateLine(TouchPoint2 points)
        {
            ValidSetOfTouchPoints ret = new ValidSetOfTouchPoints();
            HoughLine recognizer = new HoughLine(points);
            if (recognizer.IsMatch())
            {
                ret.Add(points);
            }
            return ret;
        }

        private ValidSetOfPointsCollection ValidateCircle(List<TouchPoint2> points)
        {
            throw new NotImplementedException();
        }

        public Objects.IRuleData GenerateRuleData(System.Collections.Generic.List<Gestures.Objects.TouchPoint2> points)
        {
            throw new NotImplementedException();
        }
    }
}
