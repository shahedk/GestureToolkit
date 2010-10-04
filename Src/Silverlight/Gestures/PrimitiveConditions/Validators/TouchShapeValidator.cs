using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

using TouchToolkit.GestureProcessor.PrimitiveConditions.Objects;
using TouchToolkit.GestureProcessor.Objects;
using TouchToolkit.GestureProcessor.Utility;
using TouchToolkit.GestureProcessor.Utility.TouchHelpers;
using TouchToolkit.Framework.ShapeRecognizers;

namespace TouchToolkit.GestureProcessor.PrimitiveConditions.Validators
{
    public class TouchShapeValidator : IPrimitiveConditionValidator
    {
        private TouchShape _data;
        private static int TOLERANCE = 150;


        public void Init(Objects.IPrimitiveConditionData ruleData)
        {
            _data = ruleData as TouchShape;
        }

        public bool Equals(IPrimitiveConditionValidator rule)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(List<TouchPoint2> points)
        {
            ValidSetOfPointsCollection ret = new ValidSetOfPointsCollection();
                foreach (var point in points)
                {
                    ValidSetOfTouchPoints tps = null;
                    if(_data.Values.Equals("Line"))
                    {
                        tps = ValidateLine(point);
                    }
                    else if(_data.Values.Equals("Box"))
                    {
                        tps = ValidateBox(point);
                    }
                    else if (_data.Values.Equals("Circle"))
                    {
                        tps = ValidateCircle(point);
                    }
                    if (tps.Count > 0)
                    {
                        ret.Add(tps);
                    }
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

        private ValidSetOfTouchPoints ValidateBox(TouchPoint2 points)
        {
            ValidSetOfTouchPoints output = new ValidSetOfTouchPoints();
            int length = points.Stroke.StylusPoints.Count;
            if(length < 1)
            {
                return output;
            }

            List<string> slopes = new List<string>();
            TouchPoint2 newPoints = points.GetEmptyCopy();
            
            for (int i = 0; i < length - 1; i++ )
            {
                var point1 = points.Stroke.StylusPoints[i];
                var point2 = points.Stroke.StylusPoints[i + 1];
                double slope = TrigonometricCalculationHelper.GetSlopeBetweenPoints(point1, point2);
                double distance = TrigonometricCalculationHelper.GetDistanceBetweenPoints(point1, point2);
                string stringSlope = TouchPointExtensions.SlopeToDirection(slope);
                if (distance > 0)
                {
                    newPoints.Stroke.StylusPoints.Add(point1);
                    Correlation recognizer = new Correlation(newPoints);
                    if(Math.Abs(recognizer.RSquared) < 0.85)
                    {
                        int linelength = newPoints.Stroke.StylusPoints.Count;
                        double lineSlope = TrigonometricCalculationHelper.GetSlopeBetweenPoints(newPoints.Stroke.StylusPoints[1],
                            newPoints.Stroke.StylusPoints[linelength - 1]);
                        string lineStringSlope = TouchPointExtensions.SlopeToDirection(lineSlope);
                        slopes.Add(lineStringSlope);
                        newPoints = newPoints.GetEmptyCopy();
                    }
                }
            }
            RectangleParser parser = new RectangleParser();
            bool hasRect = parser.Advance(slopes);
            if (hasRect)
            {
                output.Add(points);
            }
            return output;
        }

        private ValidSetOfTouchPoints ValidateLine(TouchPoint2 points)
        {
            ValidSetOfTouchPoints ret = new ValidSetOfTouchPoints();
            Correlation recognizer = new Correlation(points);
            if (Math.Abs(recognizer.RSquared) > .975)
            {
                ret.Add(points);
            }
            return ret;
        }

        private ValidSetOfTouchPoints ValidateCircle(TouchPoint2 points)
        {
            ValidSetOfTouchPoints ret = new ValidSetOfTouchPoints();
            CircleRecognizer recognizer = new CircleRecognizer(points);
            if (recognizer.R > .975)
            {
                ret.Add(points);
            }

            return ret;
        }

        public Objects.IPrimitiveConditionData GenerateRuleData(System.Collections.Generic.List<TouchToolkit.GestureProcessor.Objects.TouchPoint2> points)
        {
            throw new NotImplementedException();
        }
    }
}
