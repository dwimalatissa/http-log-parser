using HttpLogParser.Common;

namespace HttpLogParser.LogProcessing
{
    public interface IHttpLogProcessor
    {
        public HttpLogMetrics CalculateLogMetrics(string filePath);
    }
}