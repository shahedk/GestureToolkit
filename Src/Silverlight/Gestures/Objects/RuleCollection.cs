﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

using TouchToolkit.GestureProcessor.Rules.RuleValidators;

namespace TouchToolkit.GestureProcessor.Objects
{
    public class RuleCollection : List<IRuleValidator>
    {
        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            foreach (var rule in this)
            {
                sets = rule.Validate(sets);

                if (sets.Count == 0)
                    break;
            }

            return sets;
        }

        public ValidSetOfPointsCollection Validate(ValidSetOfTouchPoints set)
        {
            ValidSetOfPointsCollection sets = new ValidSetOfPointsCollection();

            sets.Add(set);

            return Validate(sets);
        }
    }
}
