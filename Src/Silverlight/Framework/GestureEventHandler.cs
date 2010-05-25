using System.Collections.Generic;

using System.Windows;
using Gestures.ReturnTypes;
using System;

namespace Framework
{
    public class GestureEventArgs : EventArgs
    {
        public List<IReturnType> Values = new List<IReturnType>();
        public Exception Error = null;
    }

    public delegate void GestureEventHandler(UIElement sender, GestureEventArgs e);
}
