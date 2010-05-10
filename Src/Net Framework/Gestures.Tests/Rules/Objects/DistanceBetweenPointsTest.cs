using Gestures.Rules.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BehaviourTypes = Gestures.Rules.Objects.DistanceBetweenPoints_Accessor.BehaviourTypes;

namespace Gestures.Tests.Rules.Objects
{


    /// <summary>
    ///This is a test class for DistanceBetweenPointsTest and is intended
    ///to contain all DistanceBetweenPointsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DistanceBetweenPointsTest
    {
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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }

        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        #endregion
        [TestMethod()]
        public void EqualsTest_Null_Check()
        {
            // The type we are testing
            DistanceBetweenPoints target = new DistanceBetweenPoints()
            {
                Behaviour = BehaviourTypes.Increasing,
                Max = -1,
                Min = -1
            };

            bool expected = false;
            bool actual = target.Equals(null);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void EqualsTest_Different_Types_of_RuleData()
        {
            // The type we are testing
            DistanceBetweenPoints target = new DistanceBetweenPoints()
            {
                Behaviour = BehaviourTypes.Increasing,
                Max = -1,
                Min = -1
            };

            // Any random type - just not the type that we are testing
            IRuleData anotherRuleData = new ClosedLoop();

            // Since the ruleData are of different type, it should not be equal
            bool expected = false;
            bool actual = target.Equals(anotherRuleData);

            Assert.AreEqual(expected, actual, "Different types of rules should not match!");
        }

        [TestMethod()]
        public void EqualsTest_Same_Type_of_RuleData()
        {
            // The type we are testing
            DistanceBetweenPoints target = new DistanceBetweenPoints()
            {
                Behaviour = BehaviourTypes.Increasing,
                Max = -1,
                Min = -1
            };

            // Another instance of same type of ruleData
            IRuleData anotherRuleData = new DistanceBetweenPoints()
            {
                Behaviour = BehaviourTypes.Decreasing,
                Max = -1,
                Min = -1
            };

            // Since the values are different, it should not be equal
            bool expected = false;
            bool actual = target.Equals(anotherRuleData);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void EqualsTest_Same_Type_of_RuleData_with_same_value()
        {
            // The type we are testing
            DistanceBetweenPoints target = new DistanceBetweenPoints()
            {
                Behaviour = BehaviourTypes.Increasing,
                Max = -1,
                Min = -1
            };

            // Another instance of same type of ruleData
            IRuleData anotherRuleData = new DistanceBetweenPoints()
            {
                Behaviour = BehaviourTypes.Increasing,
                Max = -1,
                Min = -1
            };

            // Since the values are same, it should be equal
            bool expected = true;
            bool actual = target.Equals(anotherRuleData);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void toGDL_Unchanged_test()
        {
            // The type we are testing
            DistanceBetweenPoints target = new DistanceBetweenPoints()
            {
                Behaviour = BehaviourTypes.UnChanged,
                Max = 2,
                Min = 0
            };


            // Since the ruleData are of different type, it should not be equal
            bool expected = true;
            bool actual = target.ToGDL().Equals("Distance between points: 0..2");

            Assert.AreEqual(expected, actual, "Behavior for type should be unchanged");
        }
    }
}
