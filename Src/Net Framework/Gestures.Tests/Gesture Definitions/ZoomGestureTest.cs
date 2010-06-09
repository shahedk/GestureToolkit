using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using Framework;
using TouchToolkit.GestureProcessor.Utility;
using TouchToolkit.GestureProcessor.ReturnTypes;
using System.Threading;
using TouchToolkit.Framework;

namespace TouchToolkit.GestureProcessor.Tests.Gesture_Definitions
{
    /// <summary>
    /// Summary description for ZoomGestureTest
    /// </summary>
    [TestClass]
    public class ZoomGestureTest
    {
        public ZoomGestureTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            GestureTestFramework.Init("TT", "Test");
        }


        [TestMethod]
        public void Zoom()
        {
            bool gestureDetected = false;
            var threadHolder = new AutoResetEvent(false);

            GestureTestFramework.Validate("Drag", "T2", 
                
                // On successful gesture detection
                (sender, e) =>
                {
                    if (e.Error == null)
                    {
                        var position = e.Values.Get<Position>();
                        Assert.IsNotNull(position, "Failed to retrieve return value: position");

                        var positionChanged = e.Values.Get<PositionChanged>();
                        Assert.IsNotNull(positionChanged, "Failed to retrieve return value: position-changed");
                    }
                    else
                    {
                        Assert.Fail(e.Error.Message);
                    }

                    gestureDetected = true;
                },

                // On playback completion
                () =>
                {
                    threadHolder.Set();
                });

            threadHolder.WaitOne();
            Assert.IsTrue(gestureDetected, "Failed to detect the gesture!");
        }
    }
}
