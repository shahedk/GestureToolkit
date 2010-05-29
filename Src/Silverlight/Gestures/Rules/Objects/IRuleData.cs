using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TouchToolkit.GestureProcessor.Rules.Objects
{
    public interface IRuleData
    {
        /// <summary>
        /// Determines whether the specified rule data are considered equal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Equals(IRuleData value);

        /// <summary>
        /// Expands the range of validation logics to include the criteria of specified rule data
        /// </summary>
        /// <param name="value"></param>
        void Union(IRuleData value);

        /// <summary>
        /// Returns the representation of current state of rule data in gesture-definition-language. 
        /// </summary>
        /// <returns></returns>
        string ToGDL();
    }
}
