using System.Collections.Generic;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3.Reports
{
    class TestResultList
    {
        public List<SingleTestResult> Results = new();
        public TestResultList(string section, string testName, string deviceInfo)
        {
            Section = section;
            TestName = testName;
            DeviceInfo = deviceInfo;
        }
        private string Section = "";
        private string TestName = "";
        private string DeviceInfo = "";
        public void Add(SingleTestResult.TestResult result, string comments)
        {
            var single = new SingleTestResult(Section, TestName, DeviceInfo);
            single.Update(result, comments);
            Results.Add(single);
        }
        public void AddPass(SingleTestResult.TestResult result, string comments)
        {
            if (Results.Count != 0) return; // There's already a fail in the list, so don't add a pass
            var single = new SingleTestResult(Section, TestName, DeviceInfo);
            single.Update(result, comments);
            Results.Add(single);
        }
    }
}
