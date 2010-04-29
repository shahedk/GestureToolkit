using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.TouchInputProviders
{
    public class Windows7TouchInputProvider : TouchInputProvider
    {
        public override event TouchInputProvider.FrameChangeEventHandler FrameChanged;
        public override event TouchInputProvider.SingleTouchChangeEventHandler SingleTouchChanged;
        public override event TouchInputProvider.MultiTouchChangeEventHandler MultiTouchChanged;

        public Windows7TouchInputProvider()
        {

        }
    }
}
