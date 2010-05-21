using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using libSMARTMultiTouch.Table;
using libSMARTMultiTouch.Input;
using SMARTTabletop_Application.Providers;
using Framework;
using Gestures.Feedbacks.TouchFeedbacks;
using Gestures.ReturnTypes;


namespace SMARTTabletop_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        

        public MainWindow()
        {
            InitializeComponent();
            TableManager.Initialize(this, LayoutRoot);
            //TableManager.IsFullScreen = true;

            //Need background color otherwise SDK considers table as blank/nothing
            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);

            this.Loaded+= new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var provider = new SMARTTabletopTouchInputProvider(this);
            GestureFramework.Initialize(provider, LayoutRoot);
            GestureFramework.AddTouchFeedback(typeof(BubblesPath));

            GestureFramework.EventManager.AddEvent(moveRect, "Drag", DragCallback);
            GestureFramework.EventManager.AddEvent(moveRect, "Pinch", PinchCallback);
            GestureFramework.EventManager.AddEvent(moveRect, "Rotate", RotateCallback);

            provider.SingleTouchChanged += new Framework.TouchInputProviders.TouchInputProvider.SingleTouchChangeEventHandler(provider_SingleTouchChanged);
        }

        void provider_SingleTouchChanged(object sender, Framework.TouchInputProviders.SingleTouchEventArgs e)
        {
            Console.WriteLine(e.TouchPoint.Action.ToString());   
        }

        #region CallBacks

        private void DragCallback(UIElement sender, List<IReturnType> values)
        {
            var posChanged = values.Get<PositionChanged>();
            if (posChanged != null)
            {
                MoveItem(sender, posChanged);
            }
        }

        private void ZoomCallback(UIElement sender, List<IReturnType> values)
        {
            var dis = values.Get<DistanceChanged>();

            if (dis != null)
                Resize(sender as Rectangle, dis.Delta);
        }

        private void PinchCallback(UIElement sender, List<IReturnType> values)
        {
            var dis = values.Get<DistanceChanged>();
            if (dis != null)
                Resize(sender as Rectangle, dis.Delta);
        }

        private void RotateCallback(UIElement sender, List<IReturnType> values)
        {
            var slopeChanged = values.Get<SlopeChanged>();
            if (slopeChanged != null)
            {
                var img = sender as Rectangle;
                if (img != null)
                    Rotate(img, Math.Round(slopeChanged.Delta, 1));
            }
        }
        #endregion

        #region Helper Functions

        bool rotateInProgress = false;
        private void Rotate(Rectangle img, double delta)
        {
            if (!rotateInProgress & delta != 0)
            {
                rotateInProgress = true;
                RotateTransform rt = img.RenderTransform as RotateTransform;

                if (rt == null)
                    rt = new RotateTransform();

                rt.Angle += delta;
                rt.CenterX = img.Width / 2;
                rt.CenterY = img.Height / 2;

                img.RenderTransform = rt;

                rotateInProgress = false;
            }
        }

        private void Resize(Rectangle image, double delta)
        {
            if (image.Height + delta > 0)
                image.Height += delta;

            if (image.Width + delta > 0)
                image.Width += delta;
        }

        private void MoveItem(UIElement sender, PositionChanged posChanged)
        {
            Rectangle item = sender as Rectangle;
            double x = (double)item.GetValue(Canvas.LeftProperty);
            double y = (double)item.GetValue(Canvas.TopProperty);

            item.SetValue(Canvas.LeftProperty, x + posChanged.X);
            item.SetValue(Canvas.TopProperty, y + posChanged.Y);
        }

        #endregion

    }
}
