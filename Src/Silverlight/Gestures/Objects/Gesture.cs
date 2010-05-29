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
        public RuleCollection PreConditions = new RuleCollection();
        public RuleCollection Rules = new RuleCollection();
        public ReturnTypeInfoCollection ReturnTypes = new ReturnTypeInfoCollection();
    }
}
