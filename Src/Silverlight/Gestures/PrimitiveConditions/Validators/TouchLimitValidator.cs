using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Combinatorial;
using System.Collections.Generic;
using TouchToolkit.GestureProcessor.PrimitiveConditions.Objects;
using TouchToolkit.GestureProcessor.PrimitiveConditions.Validators;
using TouchToolkit.GestureProcessor.Objects;

namespace TouchToolkit.GestureProcessor.PrimitiveConditions
{
    public class TouchLimitValidator : IPrimitiveConditionValidator
    {
        class TouchLimitType
        {
            public const string Range = "Range";
            public const string FixedValue = "Fixed";
        }

        private TouchLimit _data;

        public void Init(IPrimitiveConditionData ruleData)
        {
            _data = ruleData as TouchLimit;
        }

        public ValidSetOfPointsCollection Validate(List<TouchPoint2> points)
        {
            ValidSetOfPointsCollection list = new ValidSetOfPointsCollection();

            if (_data.Type == TouchLimitType.FixedValue)
            {
                if (points.Count == _data.Min)
                {
                    list.Add(new ValidSetOfTouchPoints(points));
                }
                else if (points.Count > _data.Min)
                {
                    // Generate possible valid combinitions
                    Combinations c = new Combinations(points.ToArray(), _data.Min);
                    while (c.MoveNext())
                    {
                        TouchPoint2[] arr = c.Current as TouchPoint2[];
                        ValidSetOfTouchPoints set = new ValidSetOfTouchPoints(arr);
                        list.Add(set);
                    }
                }
            }
            else if (_data.Type == TouchLimitType.Range)
            {
                if (points.Count >= _data.Min && points.Count <= _data.Max)
                {
                    list.Add(new ValidSetOfTouchPoints(points));
                }
            }

            return list;
        }

        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            ValidSetOfPointsCollection list = new ValidSetOfPointsCollection();
            foreach (var item in sets)
            {
                var tlist = Validate(item);
                list.AddRange(tlist);
            }

            return list;
        }


        public bool Equals(IPrimitiveConditionValidator rule)
        {
            if (rule is TouchLimitValidator)
            {
                TouchLimitValidator r1 = rule as TouchLimitValidator;

                return (r1._data.Equals(this._data));
            }
            else
                return false;
        }




        public IPrimitiveConditionData GenerateRuleData(List<TouchPoint2> points)
        {
            throw new NotImplementedException();
        }
    }
}
