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
using TouchToolkit.Framework.TouchInputProviders;
using TouchToolkit.Framework;
using TouchToolkit.GestureProcessor.Feedbacks.TouchFeedbacks;

namespace TestApplication
{
    /// <summary>
    /// Interaction logic for TestControl2.xaml
    /// </summary>
    public partial class TestControl2 : UserControl
    {
        private TouchInputProvider provider;
        
        public TestControl2(TouchInputProvider provider)
        {
            InitializeComponent();
            this.provider = provider;
            this.Loaded += new RoutedEventHandler(TestControl_Loaded);
        }

        void TestControl_Loaded(object sender, RoutedEventArgs e)
        {
            GestureFramework.Initialize(provider, LayoutRoot);
            GestureFramework.ShowDebugPanel(GestureFramework.DebugPanels.GestureRecorder);
            GestureFramework.AddTouchFeedback(typeof(BubblesPath));
        }
    }
}
