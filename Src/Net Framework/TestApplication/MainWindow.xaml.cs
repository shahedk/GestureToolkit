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
using Gestures.ReturnTypes;
using Framework.TouchInputProviders;
using Gestures.Feedbacks.TouchFeedbacks;

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

            GestureFramework.EventManager.AddEvent(LayoutRoot, "Zoom", ZoomCallback);
            GestureFramework.EventManager.AddEvent(LayoutRoot, "Pinch", ZoomCallback);
        }

        
        public void ZoomCallback(object sender, List<IReturnType> values)
        {
            var distanceChanged = values.Get<DistanceChanged>();

            messageTextBlock.Text = distanceChanged.Delta.ToString();
        }

        
    }
}
