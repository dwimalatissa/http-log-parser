using System.Collections.Generic;

namespace HttpLogParser.Common
{
    public class HttpLogMetrics
    {
        public int TotalLogEntries { get; set; }

        public int DistrictIpAddresses { get; set; }

        public IList<HttpLogMetricsTopResult> TopUrls { get; set; }

        public IList<HttpLogMetricsTopResult> TopIpAddresses { get; set; }
    }
}