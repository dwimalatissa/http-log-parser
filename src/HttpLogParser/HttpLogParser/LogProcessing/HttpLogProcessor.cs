using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HttpLogParser.Common;
using Microsoft.Extensions.Logging;

namespace HttpLogParser.LogProcessing
{
    public class HttpLogProcessor : IHttpLogProcessor
    {
        private const int GroupIndexIp = 1;
        private const int GroupIndexUri = 4;
        private const int TopResultsToShow = 3;

        private ILogger _logger;

        public HttpLogProcessor(ILogger<HttpLogProcessor> logger)
        {
            _logger = logger;
        }

        public HttpLogMetrics CalculateLogMetrics(string filePath)
        {
            _logger.LogInformation("Begin Processing File");

            var logEntries = new List<HttpLogEntry>();

            //Regex to extract IP address and text enveloped in square and round brackets
            var pattern = new Regex(@"(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}) - (-|admin) \[(.*?)\] \""(.*?)\"" (\d{3}) (\d{4}) ""-"" \""(.*?)\""");


            _logger.LogInformation("Extracting Log Data");

            foreach (var line in File.ReadLines(filePath))
            {
                var match = pattern.Match(line);
                if (match.Success)
                {
                    var logEntry = new HttpLogEntry
                    {
                        IpAddress = match.Groups[GroupIndexIp].Value,
                        Uri = match.Groups[GroupIndexUri].Value
                    };

                    logEntries.Add(logEntry);
                }
            }

            _logger.LogInformation("Calculating Metrics");

            var metrics = CalculateMetrics(logEntries);

            _logger.LogInformation($"Log Entry Count: {metrics.TotalLogEntries}");
            _logger.LogInformation($"Distinct Ips: {metrics.DistrictIpAddresses}");
            _logger.LogInformation($"Top Urls: {FormatListForPrint(metrics.TopUrls)}");
            _logger.LogInformation($"Top Ip Addresses: {FormatListForPrint(metrics.TopIpAddresses)}");

            return metrics;
        }

        public HttpLogMetrics CalculateMetrics(IList<HttpLogEntry> logEntries)
        {
            //Total Log Entries
            var entryCount = logEntries.Count;

            //Distinct Ips
            var distinctIps = logEntries.Select(le => le.IpAddress).Distinct().Count();

            //Top 3 Urls
            var topUrls = logEntries.GroupBy(le => le.Uri).Select(le => new HttpLogMetricsTopResult
            {
                Name = le.Key,
                Count = le.Count()
            }).OrderByDescending(le => le.Count).Take(TopResultsToShow).ToList();

            //Top 3 Ips
            var topIps = logEntries.GroupBy(le => le.IpAddress).Select(le => new HttpLogMetricsTopResult
            {
                Name = le.Key,
                Count = le.Count()
            }).OrderByDescending(le => le.Count).Take(TopResultsToShow).ToList();
            
            return new HttpLogMetrics
            {
                TotalLogEntries = entryCount,
                DistrictIpAddresses = distinctIps,
                TopUrls = topUrls,
                TopIpAddresses = topIps
            };
        }

        private static string FormatListForPrint(IList<HttpLogMetricsTopResult> topResults)
        {
            var formattedResult = string.Empty;

            foreach (var result in topResults)
            {
                if (!string.IsNullOrWhiteSpace(formattedResult)) formattedResult += "; ";

                formattedResult += $"Name: {result.Name} Count: {result.Count}";
            }

            return formattedResult;
        }
    }
}