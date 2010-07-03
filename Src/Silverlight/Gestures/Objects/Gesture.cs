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

namespace TouchToolkit.GestureProcessor.Objects
{
    public class Gesture
    {
        public string Name { get; set; }
        public PrimitiveConditionCollection PreConditions = new PrimitiveConditionCollection();
        public PrimitiveConditionCollection Rules = new PrimitiveConditionCollection();
        public ReturnTypeInfoCollection ReturnTypes = new ReturnTypeInfoCollection();
    }
}
