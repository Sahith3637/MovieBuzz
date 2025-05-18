using Serilog;

namespace MovieBuzz.Services.Monitor;

public class ApiMonitoringService
{
    private readonly Dictionary<string, ApiStats> _apiStats = new();
    private readonly object _lock = new();

    public void LogApiRequest(string endpoint, string method, int statusCode, long durationMs, bool success)
    {
        lock (_lock)
        {
            var key = $"{method} {endpoint}";

            if (!_apiStats.ContainsKey(key))
            {
                _apiStats[key] = new ApiStats();
            }

            var stats = _apiStats[key];
            stats.TotalRequests++;
            stats.TotalDurationMs += durationMs;

            // Add timestamp of last execution
            stats.LastExecuted = DateTime.UtcNow;

            // Track execution history (last 10 executions)
            stats.ExecutionHistory.Enqueue(new ExecutionRecord
            {
                Timestamp = DateTime.UtcNow,
                DurationMs = durationMs,
                Success = success,
                StatusCode = statusCode
            });

            // Keep only last 10 executions
            while (stats.ExecutionHistory.Count > 10)
            {
                stats.ExecutionHistory.Dequeue();
            }

            if (success)
            {
                stats.SuccessfulRequests++;
            }
            else
            {
                stats.FailedRequests++;
            }

            Log.Information("API {Method} {Endpoint} - Status: {StatusCode} - Duration: {Duration}ms",
                method, endpoint, statusCode, durationMs);
        }
    }

    public Dictionary<string, ApiStats> GetApiStatistics()
    {
        lock (_lock)
        {
            return _apiStats.ToDictionary(
                kvp => kvp.Key,
                kvp => new ApiStats
                {
                    TotalRequests = kvp.Value.TotalRequests,
                    SuccessfulRequests = kvp.Value.SuccessfulRequests,
                    FailedRequests = kvp.Value.FailedRequests,
                    TotalDurationMs = kvp.Value.TotalDurationMs,
                    LastExecuted = kvp.Value.LastExecuted,
                    ExecutionHistory = new Queue<ExecutionRecord>(kvp.Value.ExecutionHistory)
                });
        }
    }

    public class ApiStats
    {
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public long TotalDurationMs { get; set; }
        public DateTime LastExecuted { get; set; }
        public Queue<ExecutionRecord> ExecutionHistory { get; set; } = new Queue<ExecutionRecord>();

        public double SuccessRate => TotalRequests == 0 ? 0 : (SuccessfulRequests * 100.0) / TotalRequests;
        public double AverageDurationMs => TotalRequests == 0 ? 0 : TotalDurationMs / (double)TotalRequests;
    }

    public class ExecutionRecord
    {
        public DateTime Timestamp { get; set; }
        public long DurationMs { get; set; }
        public bool Success { get; set; }
        public int StatusCode { get; set; }
    }
}