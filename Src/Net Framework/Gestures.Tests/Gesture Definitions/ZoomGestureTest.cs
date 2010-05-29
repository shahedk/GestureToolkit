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
            GestureTestFramework.Init("LiveDemo-shahed", "X1");
        }


        [TestMethod]
        public void Zoom()
        {
            bool callbackReceived = false;
            var threadHolder = new AutoResetEvent(false);

            GestureTestFramework.Validate("zoom", "resize2", (sender, e) =>
                {
                    if (e.Error == null)
                    {
                        var dis = e.Values.Get<DistanceChanged>();

                        Assert.IsNull(dis, "Failed to validate gesture");
                    }
                    else
                    {
                        Assert.Fail(e.Error.Message);
                    }

                    callbackReceived = true;
                    threadHolder.Set();
                });

            threadHolder.WaitOne();
            Assert.IsTrue(callbackReceived, "Failed to receive the callback from async method call");
        }
    }
}
