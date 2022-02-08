using System.IO;
using HttpLogParser.LogProcessing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttpLogParser.Test.LogProcessing
{
    [TestClass]
    public class HttpLogProcessorTests
    {
        private IHttpLogProcessor _httpLogProcessor;

        [TestInitialize]
        public void TestInitialize()
        {
            var logger = Mock.Of<ILogger<HttpLogProcessor>>();
            _httpLogProcessor = new HttpLogProcessor(logger);
        }

        [TestMethod]
        public void CalculateMetrics_ValidLog_ReturnsMetrics()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "example-log-data.log");

            // Act
            var result = _httpLogProcessor.CalculateLogMetrics(filePath);

            // Assert
            Assert.AreEqual(23, result.TotalLogEntries, "Total log entry counnts are not equal");
            Assert.AreEqual(11, result.DistrictIpAddresses, "Distinct IP addresses are not equal");
            Assert.AreEqual("168.41.191.40", result.TopIpAddresses[0].Name, "Top IP addresses are not equal");
            Assert.AreEqual(2, result.TopUrls[0].Count, "Top Urls counts are not equal");
        }
    }
}