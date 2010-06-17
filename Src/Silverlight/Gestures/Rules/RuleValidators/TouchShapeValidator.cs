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

using TouchToolkit.GestureProcessor.Rules.Objects;
using TouchToolkit.GestureProcessor.Objects;
using TouchToolkit.GestureProcessor.Utility;
using TouchToolkit.GestureProcessor.Utility.TouchHelpers;
using TouchToolkit.Framework.ShapeRecognizers;

namespace TouchToolkit.GestureProcessor.Rules.RuleValidators
{
    public class TouchShapeValidator : IRuleValidator
    {
        private TouchShape _data;
        private static int TOLERANCE = 150;


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
            ValidSetOfTouchPoints ret = new ValidSetOfTouchPoints();
            bool hasRect = false;
            RectangleParser recognizer = new RectangleParser();

            // Find 'stop points' ie. points where the velocity is zero.
            // These are usually vertices of our shapes.
            int length = points.Stroke.StylusPoints.Count;
            int step = 1;
            List<StylusPoint> stopPoints = new List<StylusPoint>();
            stopPoints.Add(points.Stroke.StylusPoints[0]);
            string finalSlope = "";
            bool advancing = false;
            int startIndex = 0;
            int endIndex = 0;
            for (int i = 0; i < length - step; i += step)
            {
                StylusPoint currentPoint = points.Stroke.StylusPoints[i];
                double stopDist = TrigonometricCalculationHelper.GetDistanceBetweenPoints(currentPoint,
                   stopPoints[stopPoints.Count - 1]);
                if (stopDist > 0)
                {
                    stopPoints.Add(currentPoint);

                    //Get slope inbetween latest stop points
                    double slope = TrigonometricCalculationHelper.GetSlopeBetweenPoints(
                        stopPoints[stopPoints.Count - 2], 
                        stopPoints[stopPoints.Count - 1]);
                    string stringSlope = TouchPointExtensions.SlopeToDirection(slope);
                    if (finalSlope.Equals(""))
                    {
                        //Throw slope through a Parser to determine if a rectangle has been made
                        if (recognizer.Advance(stringSlope))
                        {
                            if (!advancing)
                            {
                                advancing = true;
                                startIndex = i;
                            }
                            if (recognizer.IsRect)
                            {
                                hasRect = true;
                                finalSlope = stringSlope;
                            }
                        }
                        else
                        {
                            advancing = false;
                        }
                    }
                    else if (hasRect && (!finalSlope.Equals(stringSlope)))
                    {
                        endIndex = i;
                        break;
                    }
                }
            }

            //Make sure our rectangle is a closed loop
            if (hasRect)
            {
                if(endIndex == 0)
                {
                    endIndex = points.Stroke.StylusPoints.Count -1;
                }
                TouchPoint2 rectangle = points.GetRange(startIndex, endIndex);
                StylusPoint start= rectangle.Stroke.StylusPoints[0];
                StylusPoint end = rectangle.Stroke.StylusPoints[rectangle.Stroke.StylusPoints.Count - 1];
                double distance = TrigonometricCalculationHelper.GetDistanceBetweenPoints(start,end);
                bool IsClosedLoop =  distance < TOLERANCE;
                if (IsClosedLoop)
                {
                    ret.Add(rectangle);
                }
            }
            return ret;
        }

        private ValidSetOfTouchPoints ValidateLine(TouchPoint2 points)
        {
            ValidSetOfTouchPoints ret = new ValidSetOfTouchPoints();
            Correlation recognizer = new Correlation(points);
            if (Math.Abs(recognizer.RSquared) > .8)
            {
                ret.Add(points);
            }
            return ret;
        }

        private ValidSetOfTouchPoints ValidateCircle(TouchPoint2 points)
        {
            ValidSetOfTouchPoints ret = new ValidSetOfTouchPoints();
            HoughCircle recognizer = new HoughCircle(points);
            if (recognizer.IsMatch())
            {
                ret.Add(points);
            }
            return ret;
        }

        public Objects.IRuleData GenerateRuleData(System.Collections.Generic.List<TouchToolkit.GestureProcessor.Objects.TouchPoint2> points)
        {
            throw new NotImplementedException();
        }
    }
}
