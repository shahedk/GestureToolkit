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

using TouchToolkit.GestureProcessor.ReturnTypes;
using TouchToolkit.GestureProcessor.Feedbacks.TouchFeedbacks;
using TouchToolkit.Framework;
using TouchToolkit.Framework.TouchInputProviders;
using TouchToolkit.GestureProcessor.Feedbacks.GestureFeedbacks;

namespace AnotoTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string ImagePath = "Pictures\\";
        public CollectionViewSource ImageView { get; set; }
        public List<Picture> AllImages;

        public MainWindow()
        {
            
            AllImages = new List<Picture>();
            ImageView = new CollectionViewSource();
            ImageView.Source = AllImages;
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

            //Subscribe to gesture events
            GestureFramework.EventManager.AddEvent(LayoutRoot, "right", RightCallBack);
            GestureFramework.EventManager.AddEvent(LayoutRoot, "left", LeftCallBack);
            // Show Recording Panel
            //GestureFramework.ShowDebugPanel(GestureFramework.DebugPanels.GestureRecorder);

            // Load UI
            LoadImages();
        }

        private void LoadImages()
        {
            string[] imageNames = { "Hydrangeas.jpg", "Jellyfish.jpg", "Koala.jpg", "Lighthouse.jpg" };
            foreach (var imageName in imageNames)
            {
                AllImages.Add(new Picture(ImagePath + imageName));

                PictureList.ItemsSource = AllImages;
            }
        }

        private void LeftCallBack(UIElement sender, GestureEventArgs e)
        {
            var view = ImageView.View;
            view.MoveCurrentToPrevious();
            if (view.IsCurrentBeforeFirst)
            {
                view.MoveCurrentToLast();
            }
        }

        private void RightCallBack(UIElement sender, GestureEventArgs e)
        {
            var view = ImageView.View;
            view.MoveCurrentToNext();
            if (view.IsCurrentAfterLast)
            {
                view.MoveCurrentToFirst();
            }
        }
    }

    public class Picture
    {
        public string Image{get; set;}
        public Picture(string img)
        {
            Image = img;
        }
    }
}