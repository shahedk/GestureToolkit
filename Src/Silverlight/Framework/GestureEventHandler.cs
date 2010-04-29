using System.Collections.Generic;

using System.Windows;
using Gestures.ReturnTypes;

namespace Framework
{
    public delegate void GestureEventHandler(UIElement sender, List<IReturnType> values);
}
