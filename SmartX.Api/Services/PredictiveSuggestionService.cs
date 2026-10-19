using System;
using System.Collections.Generic;
using System.Linq;
using SmartX.Shared.Models;

namespace SmartX.Api.Services
{
    public class PredictiveSuggestionService
    {
        private readonly SmartXDataStore _dataStore;
        private readonly TelemetryHistoryService _historyService;

        public PredictiveSuggestionService(
            SmartXDataStore dataStore,
            TelemetryHistoryService historyService)
        {
            _dataStore = dataStore;
            _historyService = historyService;
        }

        public List<PredictiveSuggestion> GenerateSuggestions()
        {
            var suggestions = new List<PredictiveSuggestion>();
            int nextId = 1;

            // Analyse the latest telemetry for each device.
            List<TelemetryRecord> records =
                _historyService.GetAllRecords();

            var latestByDevice = records
                .GroupBy(
                    record => record.DeviceId,
                    StringComparer.OrdinalIgnoreCase)
                .Select(group => group
                    .OrderByDescending(record => record.Timestamp)
                    .First())
                .ToList();

            foreach (TelemetryRecord record in latestByDevice)
            {
                if (record.IsAnomaly)
                {
                    suggestions.Add(new PredictiveSuggestion
                    {
                        Id = nextId++,
                        DeviceId = record.DeviceId,
                        Title = "Investigate abnormal sensor reading",
                        Description =
                            $"{record.DeviceId} has a latest reading of " +
                            $"{record.NumericValue} {record.Unit}, flagged as an anomaly.",
                        SuggestedAction =
                            "Inspect the sensor and connected equipment, " +
                            "then verify the reading.",
                        Severity = "High",
                        Source = "Latest telemetry anomaly",
                        GeneratedAt = DateTime.UtcNow,
                        IsActionable = true
                    });
                }
            }

            // Recommend checking devices currently disconnected.
            foreach (Sensor sensor in _dataStore.Sensors
                         .Where(sensor => !sensor.IsConnected))
            {
                suggestions.Add(new PredictiveSuggestion
                {
                    Id = nextId++,
                    DeviceId = sensor.UniqueIdentifier,
                    Title = "Check disconnected device",
                    Description =
                        $"{sensor.UniqueIdentifier} is currently disconnected.",
                    SuggestedAction =
                        "Check the device's power supply and network connection, " +
                        "then verify that it reconnects.",
                    Severity = "High",
                    Source = "Current sensor connection state",
                    GeneratedAt = DateTime.UtcNow,
                    IsActionable = true
                });
            }

            // Detect repeated anomalous readings in recent telemetry.
            DateTime recentCutoff = DateTime.UtcNow.AddHours(-24);

            var recentAnomalies = records
                .Where(record =>
                    record.Timestamp >= recentCutoff &&
                    record.IsAnomaly)
                .GroupBy(
                    record => record.DeviceId,
                    StringComparer.OrdinalIgnoreCase);

            foreach (var group in recentAnomalies)
            {
                if (group.Count() >= 3)
                {
                    suggestions.Add(new PredictiveSuggestion
                    {
                        Id = nextId++,
                        DeviceId = group.Key,
                        Title = "Repeated anomalies detected",
                        Description =
                            $"{group.Key} has produced {group.Count()} " +
                            "anomalous readings in the last 24 hours.",
                        SuggestedAction =
                            "Investigate the recurring issue and check " +
                            "whether the sensor requires calibration or maintenance.",
                        Severity = "Critical",
                        Source = "Repeated anomalies within 24 hours",
                        GeneratedAt = DateTime.UtcNow,
                        IsActionable = true
                    });
                }
            }

            return suggestions
                .OrderByDescending(suggestion =>
                    suggestion.Severity == "Critical" ? 3 :
                    suggestion.Severity == "High" ? 2 : 1)
                .ThenBy(suggestion => suggestion.DeviceId)
                .ToList();
        }
    }
}