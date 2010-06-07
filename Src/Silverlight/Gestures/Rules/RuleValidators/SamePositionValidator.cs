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
using Gestures.Base;
using System.Collections.Generic;
using Gestures.Rules.Objects;

namespace Gestures.Rules
{
    public class SamePositionValidator : IRuleValidator
    {
        private SamePosition _data;

        public void Init(IRuleData ruleData)
        {
            _data = ruleData as SamePosition;
        }

        public bool Equals(IRuleValidator rule)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(List<TouchPoint2> points)
        {
            ValidSetOfPointsCollection sets = IsValid(points);

            return sets;
        }


        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            ValidSetOfPointsCollection result = new ValidSetOfPointsCollection();
            foreach (var item in sets)
            {
                ValidSetOfPointsCollection list = Validate(item);
                foreach (var set in list)
                {
                    result.Add(set);
                }
            }

            return result;
        }

    }
}
