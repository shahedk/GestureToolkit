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
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using SurfaceApplication.Providers;
using Gestures.Objects;
using Framework;
using Gestures.Feedbacks.TouchFeedbacks;
using Gestures.ReturnTypes;

namespace SurfaceApplication
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(SurfaceWindow1_Loaded);

            // Add handlers for Application activation events
            AddActivationHandlers();
        }

        #region ...
        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for Application activation events
            RemoveActivationHandlers();
        }

        /// <summary>
        /// Adds handlers for Application activation events.
        /// </summary>
        private void AddActivationHandlers()
        {
            // Subscribe to surface application activation events
            ApplicationLauncher.ApplicationActivated += OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed += OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated += OnApplicationDeactivated;
        }

        /// <summary>
        /// Removes handlers for Application activation events.
        /// </summary>
        private void RemoveActivationHandlers()
        {
            // Unsubscribe from surface application activation events
            ApplicationLauncher.ApplicationActivated -= OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed -= OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated -= OnApplicationDeactivated;
        }

        /// <summary>
        /// This is called when application has been activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationActivated(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when application is in preview mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationPreviewed(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        ///  This is called when application has been deactivated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationDeactivated(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }
        #endregion

        void SurfaceWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize Gesture Framework
            var provider = new SurfaceTouchInputProvider(this);
            GestureFramework.Initialize(provider, LayoutRoot);
            GestureFramework.AddTouchFeedback(typeof(BubblesPath));

            GestureFramework.EventManager.AddEvent(rectangle, "Zoom", ResizeCallback);
            GestureFramework.EventManager.AddEvent(rectangle, "Pinch", ResizeCallback);
            GestureFramework.EventManager.AddEvent(rectangle, "Drag", DragCallback);
        }

        public void DragCallback(object sender, List<IReturnType> values)
        {
            PositionChanged posChanged = values.Get<PositionChanged>();

            MoveItem(sender as Rectangle, posChanged);
        }

        public void ResizeCallback(object sender, List<IReturnType> values)
        {
            var distanceChanged = values.Get<DistanceChanged>();
            Resize(sender as Rectangle, distanceChanged.Delta);
        }

        private void MoveItem(Rectangle sender, PositionChanged posChanged)
        {
            double x = (double)sender.GetValue(Canvas.LeftProperty);
            double y = (double)sender.GetValue(Canvas.TopProperty);

            sender.SetValue(Canvas.LeftProperty, x + posChanged.X);
            sender.SetValue(Canvas.TopProperty, y + posChanged.Y);
        }

        private void Resize(Rectangle item, double delta)
        {
            if (item.Height + delta > 0)
                item.Height += delta;

            if (item.Width + delta > 0)
                item.Width += delta;
        }
    }
}