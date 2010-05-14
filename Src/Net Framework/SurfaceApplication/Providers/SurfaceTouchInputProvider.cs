using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.TouchInputProviders;

namespace SurfaceApplication.Providers
{
    public class SurfaceTouchInputProvider : TouchInputProvider
    {
        public override event TouchInputProvider.FrameChangeEventHandler FrameChanged;

        public override event TouchInputProvider.SingleTouchChangeEventHandler SingleTouchChanged;

        public override event TouchInputProvider.MultiTouchChangeEventHandler MultiTouchChanged;

        public override void Init()
        {
            throw new NotImplementedException();
        }
    }
}
