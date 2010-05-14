using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.TouchInputProviders;

namespace SMARTTabletop_Application.Providers
{
    public class SMARTTabletopTouchInputProvider : TouchInputProvider
    {
        public override event TouchInputProvider.FrameChangeEventHandler FrameChanged;

        public override event TouchInputProvider.SingleTouchChangeEventHandler SingleTouchChanged;

        public override event TouchInputProvider.MultiTouchChangeEventHandler MultiTouchChanged;
    }
}
