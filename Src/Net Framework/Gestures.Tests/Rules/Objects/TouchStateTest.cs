using Gestures.Rules.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Gestures.Tests
{
    
    
    /// <summary>
    ///This is a test class for TouchStateTest and is intended
    ///to contain all TouchStateTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TouchStateTest
    {


        private TestContext testContextInstance;

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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region Equals tests
        /*
        [TestMethod()]
        public void EqualsTest()
        {
            //Equals method not implemented
        }*/
        #endregion

        #region toGDL tests
        [TestMethod()]
        public void TouchState_toGDL_on_new_object()
        {
            TouchState target = new TouchState();
            string expected = "Touch states: ";
            string actual;
            actual = target.ToGDL();
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void TouchState_toGDL_with_some_objects()
        {
            TouchState target = new TouchState();
            target.States.Add("State A");
            target.States.Add("State B");
            target.States.Add("State C");
            string expected = "Touch states: State A State B State C";
            string actual = target.ToGDL();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Union tests
        [TestMethod()]
        [ExpectedException(typeof(Exception), "Null Value Exception")]
        public void TouchState_Union_null_throws_exception()
        {
            TouchState target = new TouchState();
            TouchState value = null;
            target.Union(value);
        }
        [TestMethod()]
        [ExpectedException(typeof(Exception), "Wrong Type Exception")]
        public void TouchState_Union_wrong_type_throws_exception()
        {
            TouchState target = new TouchState();
            TouchPathLength input = new TouchPathLength();

            target.Union(input);
        }
        [TestMethod()]
        public void TouchState_Union_two_different_lists_gives_all_states_from_both()
        {
            TouchState target = new TouchState();
            TouchState input = new TouchState();
            target.States.Add("state1");
            target.States.Add("state2");
            target.States.Add("state3");
            input.States.Add("stateA");
            input.States.Add("stateB");
            input.States.Add("stateC");

            target.Union(input);

            string expected = "Touch states: state1 state2 state3 stateA stateB stateC";
            string actual = target.ToGDL();
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void TouchState_Union_two_lists_with_some_shared_states_does_not_duplicate()
        {
            TouchState target = new TouchState();
            TouchState input = new TouchState();
            target.States.Add("state1");
            target.States.Add("state2");
            target.States.Add("stateB");
            input.States.Add("stateA");
            input.States.Add("stateB");
            input.States.Add("stateC");

            target.Union(input);

            string expected = "Touch states: state1 state2 stateB stateA stateC";
            string actual = target.ToGDL();
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void TouchState_Union_empty_gives_target()
        {
            TouchState target = new TouchState();
            TouchState input = new TouchState();
            target.States.Add("state1");
            target.States.Add("state2");
            target.States.Add("stateB");

            target.Union(input);

            string expected = "Touch states: state1 state2 stateB";
            string actual = target.ToGDL();
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void TouchState_empty_Union_list_gives_list()
        {
            TouchState target = new TouchState();
            TouchState input = new TouchState();
            input.States.Add("state1");
            input.States.Add("state2");
            input.States.Add("stateB");

            target.Union(input);

            string expected = "Touch states: state1 state2 stateB";
            string actual = target.ToGDL();
            Assert.AreEqual(expected, actual);
        }
        #endregion 

        #region States test
        [TestMethod()]
        public void TouchState_State_initializes_with_empty_list()
        {
            TouchState target = new TouchState();
            List<string> expected = new List<string>();
            List<string> actual;
            target.States = expected;
            actual = target.States;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void TouchState_State_changes_to_list_is_reflected_by_get()
        {
            TouchState target = new TouchState();
            target.States.Add("stateA");
            string expected = "Touch states: stateA";
            string actual = target.ToGDL();
            Assert.AreEqual(expected, actual);
        }
        #endregion 
    }
}
