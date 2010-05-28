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
using System.Threading;

using Framework;
using Framework.HardwareListeners;
using Framework.TouchInputProviders;
using Gestures;
using Gestures.Feedbacks.GestureFeedbacks;
using Gestures.Feedbacks.TouchFeedbacks;
using Gestures.ReturnTypes;
using AnotoApplication.Providers;

namespace AnotoApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize Gesture Framework
            TouchInputProvider inputProvider = new AnotoInputProvider(LayoutRoot);
            GestureFramework.Initialize(inputProvider, LayoutRoot);

            // Add touch feedbacks
            GestureFramework.AddTouchFeedback(typeof(BubblesPath));

            // Show Recording Panel
            //GestureFramework.ShowDebugPanel(GestureFramework.DebugPanels.GestureRecorder);

            // Load UI
            LoadImages(false);
        }


        private void LoadImages(bool randomPosition)
        {
            // Load images from embedded resource
            var bitmaps = GetImages();
            int count = 0;
            foreach (var bitmap in bitmaps)
            {
                Image img = new Image();

                // Load the image in UI
                img.Source = bitmap;

                // TODO: For debugging
                img.Tag = count++;

                SetImageLocation(img, randomPosition);
                LayoutRoot.Children.Add(img);

                // Subscribe to gesture events for image
                GestureFramework.EventManager.AddEvent(img, "zoom", ZoomCallback);
                GestureFramework.EventManager.AddEvent(img, "pinch", PinchCallback);
                GestureFramework.EventManager.AddEvent(img, "drag", DragCallback);
                GestureFramework.EventManager.AddEvent(img, "rotate", RotateCallback);
                GestureFramework.EventManager.AddEvent(LayoutRoot, "left", LeftCallBack);
                GestureFramework.EventManager.AddEvent(LayoutRoot, "right", RightCallBack);
                GestureFramework.EventManager.AddEvent(LayoutRoot, "line", LineCallBack);
            }
        }

        #region Setting image properties
        Random randomNumberGenerator = new Random();
        double lastImageLeftPost = 50;
        private void SetImageLocation(Image img, bool randomPosition)
        {
            double topPosition, leftPosition;
            if (randomPosition)
            {
                topPosition = randomNumberGenerator.Next(100, 700);
                leftPosition = randomNumberGenerator.Next(100, 1200);
            }
            else
            {
                topPosition = 100;
                leftPosition = lastImageLeftPost;
            }

            img.SetValue(Canvas.TopProperty, topPosition);
            img.SetValue(Canvas.LeftProperty, leftPosition);
            img.Width = 120;
            img.Height = 90;

            lastImageLeftPost = leftPosition + 130;
        }

        /// <summary>
        /// Returns the images from assebly embedded resource
        /// </summary>
        /// <returns></returns>
        private List<BitmapImage> GetImages()
        {
            string[] imageNames = { "Hydrangeas.jpg", "Jellyfish.jpg", "Koala.jpg", "Lighthouse.jpg" };
            List<BitmapImage> images = new List<BitmapImage>();
            foreach (string imgName in imageNames)
            {
                BitmapImage img = Framework.Utility.ContentHelper.GetEmbeddedImage(this, imgName);
                images.Add(img);
            }

            return images;
        }
        #endregion

        #region Gesture Events
        private void LineCallBack(UIElement sender, GestureEventArgs e)
        {
            if (e.Values.Get<TouchPoints>().Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Line");
            }
        }
        private void RightCallBack(UIElement sender, GestureEventArgs e)
        {
            Thread t = new Thread(Scatter);
            t.Start();
        }
        private void LeftCallBack(UIElement sender, GestureEventArgs e)
        {
            Thread t = new Thread(Revert);
            t.Start();
        }

        private void ZoomCallback(UIElement sender, GestureEventArgs e)
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

        private void PinchCallback(UIElement sender, GestureEventArgs e)
        {
            var dis = e.Values.Get<DistanceChanged>();
            if (dis != null)
                Resize(sender as Image, dis.Delta);
        }

        private void DragCallback(UIElement sender, GestureEventArgs e)
        {
            var posChanged = e.Values.Get<PositionChanged>();
            if (posChanged != null)
            {
                MoveItem(sender, posChanged);
            }
        }

        private void MoveItem(UIElement sender, PositionChanged posChanged)
        {
            Image item = sender as Image;
            double x = (double)item.GetValue(Canvas.LeftProperty);
            double y = (double)item.GetValue(Canvas.TopProperty);

            item.SetValue(Canvas.LeftProperty, x + posChanged.X);
            item.SetValue(Canvas.TopProperty, y + posChanged.Y);
        }

        private void Resize(Image image, double delta)
        {
            if (image.Height + delta > 0)
                image.Height += delta;

            if (image.Width + delta > 0)
                image.Width += delta;
        }

        #endregion

        #region UI Helper Methods
        private void DrawLine(Point p1, Point p2)
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();

            line.X1 = p1.X;
            line.X2 = p2.X;
            line.Y1 = p1.Y;
            line.Y2 = p2.Y;
            line.Stroke = new SolidColorBrush(Colors.Red);

            LayoutRoot.Children.Add(line);
        }

        Polygon CreatePolygon(TouchPoints points)
        {
            Polygon p = new Polygon();

            foreach (var point in points)
            {
                p.Points.Add(point);
            }

            return p;
        }

        private void Scatter()
        {
            Thread.Sleep(105);
            Action action = () =>
            {
                foreach (var element in LayoutRoot.Children)
                {
                    Image img = element as Image;
                    lastImageLeftPost = 50;
                    if (img != null)
                    {
                        SetImageLocation(img, true);
                    }
                }
            };
            Dispatcher.BeginInvoke(action);
        }
        private void Revert()
        {
            Thread.Sleep(105);
            Action action = () =>
            {
                double x = 50;
                double y = 100;
                foreach (var element in LayoutRoot.Children)
                {
                    Image img = element as Image;
                    if (img != null)
                    {
                        img.SetValue(Canvas.TopProperty, y);
                        img.SetValue(Canvas.LeftProperty, x);
                        x += 130;
                    }
                }
            };
            Dispatcher.BeginInvoke(action);
        }
        #endregion

    }
}
