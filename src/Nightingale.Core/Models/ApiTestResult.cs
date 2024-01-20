using Nightingale.Core.Interfaces;

namespace Nightingale.Core.Models
{
    public class ApiTestResult : IDeepCloneable
    {
        public TestResult TestResult { get; set; }

        public string Name { get; set; }

        public object DeepClone()
        {
            var other = new ApiTestResult
            {
                TestResult = this.TestResult,
                Name = this.Name
            };
            return other;
        }
    }

    public enum TestResult
    {
        Unstarted,
        Pass,
        Fail,
        Error
    }
}
