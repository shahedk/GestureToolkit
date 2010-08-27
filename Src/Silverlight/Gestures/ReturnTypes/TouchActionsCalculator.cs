using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchToolkit.GestureProcessor.ReturnTypes
{
    public class TouchActionsCalculator : IReturnTypeCalculator
    {
        public IReturnType Calculate(Objects.ValidSetOfTouchPoints set)
        {
            TouchActions actions = new TouchActions();

            foreach (var item in set)
                actions.Add(item.TouchDeviceId, item.Action);

            return actions;
        }
    }
}
