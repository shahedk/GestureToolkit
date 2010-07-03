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
using System.Threading;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AnotoTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string ImagePath = "Pictures\\";
        public ICollectionView Pictures { get; set; }
        public List<Picture> AllImages;

        public MainWindow()
        {
            
            AllImages = new List<Picture>();
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
            GestureFramework.EventManager.AddEvent(LayoutRoot, "Line", BoxCallBack);
            // Show Recording Panel
            GestureFramework.ShowDebugPanel(GestureFramework.DebugPanels.GestureRecorder);

            // Load UI
            LoadImages();
        }

        private void LoadImages()
        {
            string[] imageNames = { "Hydrangeas.jpg", "Jellyfish.jpg", "Koala.jpg", "Lighthouse.jpg" };
            foreach (var imageName in imageNames)
            {
                AllImages.Add(new Picture(ImagePath + imageName));
            }
            Pictures = CollectionViewSource.GetDefaultView(AllImages);
            Display.Source = AllImages[Pictures.CurrentPosition].GetImage();
        }

        #region Gesture Callbacks
        private void LeftCallBack(UIElement sender, GestureEventArgs e)
        {
            ThreadStart start = delegate()
            {
                Dispatcher.Invoke(DispatcherPriority.Render,
                                  new Action(ViewPrevItem));
            };
            new Thread(start).Start();
        }

        private void RightCallBack(UIElement sender, GestureEventArgs e)
        {
            ThreadStart start = delegate()
            {
                Dispatcher.Invoke(DispatcherPriority.Render,
                                  new Action(ViewNextItem));
            };
            new Thread(start).Start();
        }

        private void BoxCallBack(UIElement sender, GestureEventArgs e)
        {
            
            ThreadStart start = delegate()
            {
                Dispatcher.Invoke(DispatcherPriority.Render,
                                  new Action(OpenDirectoryWindow));
            };
            new Thread(start).Start();
            
        }
        #endregion

        #region Navigation Methods
        private void ViewNextItem()
        {
            Pictures.MoveCurrentToNext();
            if (Pictures.IsCurrentAfterLast)
            {
                Pictures.MoveCurrentToFirst();
            }
            Display.Source = AllImages[Pictures.CurrentPosition].GetImage();
        }

        private void ViewPrevItem()
        {
            Pictures.MoveCurrentToPrevious();
            if (Pictures.IsCurrentBeforeFirst)
            {
                Pictures.MoveCurrentToLast();
            }
            Display.Source = AllImages[Pictures.CurrentPosition].GetImage();
        }
        #endregion

        private void OpenDirectoryWindow()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "Images (.jpg)|*.jpg";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string fname = dialog.FileName;
            }
        }
    }

    public class Picture
    {
        private string Image{get; set;}
        public Picture(string img)
        {
            Image = img;
        }
        public BitmapImage GetImage()
        {
            BitmapImage myBitmapImage = new BitmapImage();

            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(Image, UriKind.Relative);

            myBitmapImage.EndInit();

            return myBitmapImage;
        }
    }
}