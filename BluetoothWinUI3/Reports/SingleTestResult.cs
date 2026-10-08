using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.ApplicationSettings;

namespace BluetoothWinUI3.Reports
{
    class SingleTestResult
    {
        /// <summary>
        /// Importance of the test failure plus pass, not tested etc.
        /// </summary>
        public enum TestResult { NotTested, CannotTest, Pass, FailShould, FailShall, FailUnofficial };

        /// <summary>
        /// Sets up the constant parts of a test result
        /// </summary>
        public SingleTestResult(string section, string testName, string deviceInfo)
        {
            Section = section;
            TestName = testName;
            DeviceInfo = deviceInfo;
        }

        /// <summary>
        /// Update a test result with the result including pass/fail and a comment about why the test failed.
        /// </summary>
        public SingleTestResult Update(SingleTestResult.TestResult result, string comments)
        {
            Result = result;
            Comments = comments;
            return this;
        }

        public static SingleTestResult MakeCannotTest(string section, string testName, string comments="")
        {
            if (string.IsNullOrEmpty(comments)) comments = "No automated checks are available for these guidelines";
            var retval = new SingleTestResult(section, testName, "");
            retval.Update(TestResult.CannotTest, comments);
            return retval;
        }

        /// <summary>
        /// Section in the Microsoft Bluetooth Accessory Guidelines that are the failure, plus a 
        /// suffix for sections with multiple requirements
        /// </summary>
        public string Section = "";
        /// <summary>
        /// User friendly description of the test in general.
        /// </summary>
        public string TestName = "";
        /// <summary>
        /// Information from the device (like Manufacturer Name) that relevant to the test.
        /// </summary>
        public string DeviceInfo = "";
        /// <summary>
        /// Reason for the test failure
        /// </summary>
        public string Comments = "";
        public TestResult Result = TestResult.NotTested;
        public string ResultAsString
        {
            get
            {
                switch (Result)
                {
                    case TestResult.NotTested:
                        return "Not tested";
                    case TestResult.CannotTest:
                        return "Cannot test this guideline";
                    case TestResult.Pass:
                        return "Pass";
                    case TestResult.FailShould:
                        return "A SHOULD guideline is not satisfied";
                    case TestResult.FailShall:
                        return "A SHALL guideline is not satisfied";
                    case TestResult.FailUnofficial:
                        return "The accessory has an issue that's not part of the official guidelines";
                    default:
                        return "Unknown Result";
                }
            }
        }
    }
}
