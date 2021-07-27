using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using ChangeInput.Core;

namespace ChangeInputTests
{
    [TestClass]
    public class CircularDictionaryTests
    {
        #region Constructor
        [TestMethod]
        public void Constructor_WhenInstantiatedWithElements_ShouldContainElements()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };

            Assert.AreEqual("First Value", circularDictionary[0]);
            Assert.AreEqual("Second Value", circularDictionary[1]);
            Assert.AreEqual("Third Value", circularDictionary[2]);
        }
        #endregion
        #region ThisGet
        [TestMethod]
        public void ThisGet_WhenCircularDictionaryContainsKey_ShouldReturnValue()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };
            string expectedValue = "Second Value";

            string actualValue = circularDictionary[1];

            Assert.AreEqual(expectedValue, actualValue);
        }

        [ExpectedException(typeof(KeyNotFoundException))]
        [TestMethod]
        public void ThisGet_WhenCircularDictionaryDoesNotContainKey_ShouldThrowKeyNotFoundException()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };

            string actualValue = circularDictionary[3];
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ThisGet_WhenKeyPassedIsNull_ShouldThrowArgumentNullException()
        {
            CircularDictionary<string, string> circularDictionary = new CircularDictionary<string, string>
            {
                { "0", "First Value" },
                { "1", "Second Value" },
                { "2", "Third Value" }
            };

            string actualValue = circularDictionary[null];
        }
        #endregion
        #region ThisSet
        [TestMethod]
        public void ThisSet_WhenCircularDictionaryContainsKeyAndDoesNotContainValue_ShouldSetValue()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };
            string expectedValue = "Something else";

            circularDictionary[1] = expectedValue;

            Assert.AreEqual(expectedValue, circularDictionary[1]);
        }

        [TestMethod]
        public void ThisSet_WhenCircularDictionaryDoesNotContainKey_ShouldAddElement()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };
            string expectedValue = "Something else";

            circularDictionary[4] = expectedValue;

            Assert.AreEqual(expectedValue, circularDictionary[4]);
        }
        
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void ThisSet_WhenCircularDictionaryContainsKeyAndContainsValue_ShouldThrowArgumentException()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };
            string expectedValue = "First Value";

            circularDictionary[1] = expectedValue;
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ThisSet_WhenKeyPassedIsNull_ShouldThrowArgumentNullException()
        {
            CircularDictionary<string, string> circularDictionary = new CircularDictionary<string, string>
            {
                { "0", "First Value" },
                { "1", "Second Value" },
                { "2", "Third Value" }
            };

            circularDictionary[null] = "Something else";
        }
        #endregion
        #region ContainsKey
        [TestMethod]
        public void ContainsKey_WhenContainsKey_ShouldReturnTrue()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };

            Assert.IsTrue(circularDictionary.ContainsKey(0));
            Assert.IsTrue(circularDictionary.ContainsKey(1));
            Assert.IsTrue(circularDictionary.ContainsKey(2));
        }

        [TestMethod]
        public void ContainsKey_WhenDoesNotContainKey_ShouldReturnFalse()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };

            Assert.IsFalse(circularDictionary.ContainsKey(3));
        }
        #endregion
        #region ContainsValue
        [TestMethod]
        public void ContainsValue_WhenContainsValue_ShouldReturnTrue()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };

            Assert.IsTrue(circularDictionary.ContainsValue("First Value"));
            Assert.IsTrue(circularDictionary.ContainsValue("Second Value"));
            Assert.IsTrue(circularDictionary.ContainsValue("Third Value"));
        }

        [TestMethod]
        public void ContainsValue_WhenDoesNotContainValue_ShouldReturnFalse()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };

            Assert.IsFalse(circularDictionary.ContainsValue("Something else"));
        }
        #endregion
        #region GetKey
        [TestMethod]
        public void GetKey_WhenContainsValue_ShouldReturnKey()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };
            int expectedValue = 1;

            int actualValue = circularDictionary.GetKey("Second Value");

            Assert.AreEqual(expectedValue, actualValue);
        }
        
        [TestMethod]
        public void GetKey_WhenDoesNotContainValue_ShouldReturnDefault()
        {
            CircularDictionary<int, string> circularDictionary = new CircularDictionary<int, string>
            {
                { 0, "First Value" },
                { 1, "Second Value" },
                { 2, "Third Value" }
            };
            int expectedValue = default;

            int actualValue = circularDictionary.GetKey("Something else");

            Assert.AreEqual(expectedValue, actualValue);
        }
        #endregion
    }
}
