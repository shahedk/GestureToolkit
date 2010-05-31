﻿using System;
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
using TouchToolkit.Framework;
using TouchToolkit.GestureProcessor.Feedbacks.GestureFeedbacks;
using TouchToolkit.GestureProcessor.ReturnTypes;
using TouchToolkit.GestureProcessor.Feedbacks.TouchFeedbacks;

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

            SetImages(false);

            provider.SingleTouchChanged += new TouchToolkit.Framework.TouchInputProviders.TouchInputProvider.SingleTouchChangeEventHandler(provider_SingleTouchChanged);

        }

        private void SetImages(bool randomPosition)
        {
            foreach (var bitmap in LayoutRoot.Children)
            {
                GestureFramework.EventManager.AddEvent(bitmap as Image, "Zoom", ZoomCallback);
                GestureFramework.EventManager.AddEvent(bitmap as Image, "Pinch", PinchCallback);
                GestureFramework.EventManager.AddEvent(bitmap as Image, "Drag", DragCallback);
                GestureFramework.EventManager.AddEvent(bitmap as Image, "Rotate", RotateCallback);
            }

            //Uncomment here to add lasso functionality
            //GestureFramework.EventManager.AddEvent(LayoutRoot, "Lasso", LassoCallback);
        }

        void provider_SingleTouchChanged(object sender, TouchToolkit.Framework.TouchInputProviders.SingleTouchEventArgs e)
        {
            Console.WriteLine(e.TouchPoint.Action.ToString());   
        }

        #region CallBacks

        private void DragCallback(UIElement sender, GestureEventArgs e)
        {
            var pos = e.Values.Get<Position>();
            if (pos != null)
            {
                sender.SetValue(Canvas.TopProperty, pos.Y);
                sender.SetValue(Canvas.LeftProperty, pos.X);

                Console.WriteLine(pos);
            }
        }

        private void ZoomCallback(UIElement sender, GestureEventArgs e)
        {
            var dis = e.Values.Get<DistanceChanged>();

            if (dis != null)
                Resize(sender as Image, dis.Delta);
        }

        private void PinchCallback(UIElement sender, GestureEventArgs e)
        {
            var dis = e.Values.Get<DistanceChanged>();
            if (dis != null)
                Resize(sender as Image, dis.Delta);
        }

        private void RotateCallback(UIElement sender, GestureEventArgs e)
        {
            var slopeChanged = e.Values.Get<SlopeChanged>();
            if (slopeChanged != null)
            {
                var img = sender as Image;
                if (img != null)
                    Rotate(img, Math.Round(slopeChanged.Delta, 1));
            }
        }
        #endregion

        #region Helper Functions

        bool rotateInProgress = false;
        private void Rotate(Image img, double delta)
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

        private void Resize(Image image, double delta)
        {
            if (image.Height + delta > 0)
                image.Height += delta;

            if (image.Width + delta > 0)
                image.Width += delta;
        }

        private void MoveItem(UIElement sender, PositionChanged posChanged)
        {
            Image item = sender as Image;
            double x = (double)item.GetValue(Canvas.LeftProperty);
            double y = (double)item.GetValue(Canvas.TopProperty);

            item.SetValue(Canvas.LeftProperty, x + posChanged.X);
            item.SetValue(Canvas.TopProperty, y + posChanged.Y);
        }

        #endregion

    }
}
