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
using Framework;
using Framework.TouchInputProviders;
using TouchToolkit.GestureProcessor.Feedbacks.TouchFeedbacks;
using TouchToolkit.GestureProcessor.ReturnTypes;
using TouchToolkit.Framework;

namespace TestApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var provider = new Windows7TouchInputProvider();

            GestureFramework.Initialize(provider, LayoutRoot);
            GestureFramework.AddTouchFeedback(typeof(BubblesPath));

            GestureFramework.EventManager.AddEvent(rectangle, "Zoom", ResizeCallback);
            GestureFramework.EventManager.AddEvent(rectangle, "Pinch", ResizeCallback);
            GestureFramework.EventManager.AddEvent(rectangle, "Drag", DragCallback);

            GestureFramework.ShowDebugPanel(GestureFramework.DebugPanels.GestureRecorder);
        }

        public void DragCallback(object sender, GestureEventArgs e)
        {
            PositionChanged posChanged = e.Values.Get<PositionChanged>();

            MoveItem(sender as Rectangle, posChanged);
        }

        public void ResizeCallback(object sender, GestureEventArgs e)
        {
            var distanceChanged = e.Values.Get<DistanceChanged>();

            messageTextBlock.Text = distanceChanged.Delta.ToString();

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
