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

namespace Gestures.Rules.Objects
{
    public class TouchState : IRuleData
    {
        private List<string> _touchState = new List<string>();
        public List<string> States
        {
            get
            {
                return _touchState;
            }
            set
            {
                _touchState = value;
            }
        }

        #region IRuleData Members

        public bool Equals(IRuleData rule)
        {
            throw new NotImplementedException();
        }

        #endregion


        public void Union(IRuleData value)
        {
            if (value == null)
            {
                throw new Exception("Null Value Exception");
            }
            if (!(value is TouchState))
            {
                throw new Exception("Wrong Type Exception");
            }
            TouchState touchState = value as TouchState;

            bool matchFound = false;
            foreach (string newState in touchState.States)
            {
                foreach (string existingState in this.States)
                {
                    matchFound = string.Equals(newState, existingState);
                    if (matchFound)
                        break;
                }

                if (!matchFound)
                    this.States.Add(newState);
            }
        }

        public string ToGDL()
        {
            string uniqueStates = string.Join(" ", States);
            
            return string.Format("Touch states: {0}", uniqueStates);
        }
    }
}
