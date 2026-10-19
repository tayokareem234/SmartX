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
        private readonly CommandHistoryStore _commandHistoryStore;

        public PredictiveSuggestionService(
            SmartXDataStore dataStore,
            TelemetryHistoryService historyService,
            CommandHistoryStore commandHistoryStore)
        {
            _dataStore = dataStore;
            _historyService = historyService;
            _commandHistoryStore = commandHistoryStore;
        }

        public List<PredictiveSuggestion> GenerateSuggestions()
        {
            var suggestions = new List<PredictiveSuggestion>();
            int nextId = 1;

            // =====================================================
            // 1. RETRIEVE TELEMETRY AND COMMAND HISTORY
            // =====================================================

            List<TelemetryRecord> records =
                _historyService.GetAllRecords();

            List<SmartCommand> commands =
                _commandHistoryStore.GetAllCommands();

            DateTime now = DateTime.UtcNow;
            DateTime recentCutoff = now.AddHours(-24);

            // =====================================================
            // 2. FIND THE LATEST TELEMETRY RECORD FOR EACH DEVICE
            // =====================================================

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
                if (!record.IsAnomaly)
                {
                    continue;
                }

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
                    GeneratedAt = now,
                    IsActionable = true
                });
            }

            // =====================================================
            // 3. DETECT DISCONNECTED DEVICES
            // =====================================================

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
                    GeneratedAt = now,
                    IsActionable = true
                });
            }

            // =====================================================
            // 4. DETECT REPEATED ANOMALIES WITHIN 24 HOURS
            // =====================================================

            var recentAnomalies = records
                .Where(record =>
                    record.Timestamp >= recentCutoff &&
                    record.IsAnomaly)
                .GroupBy(
                    record => record.DeviceId,
                    StringComparer.OrdinalIgnoreCase);

            foreach (var group in recentAnomalies)
            {
                int anomalyCount = group.Count();

                if (anomalyCount < 3)
                {
                    continue;
                }

                suggestions.Add(new PredictiveSuggestion
                {
                    Id = nextId++,
                    DeviceId = group.Key,
                    Title = "Repeated anomalies detected",
                    Description =
                        $"{group.Key} has produced {anomalyCount} " +
                        "anomalous readings in the last 24 hours.",
                    SuggestedAction =
                        "Investigate the recurring issue and check whether " +
                        "the sensor requires calibration or maintenance.",
                    Severity = "Critical",
                    Source = "Repeated anomalies within 24 hours",
                    GeneratedAt = now,
                    IsActionable = true
                });
            }

            // =====================================================
            // 5. FILTER SUCCESSFULLY PROCESSED COMMANDS
            // =====================================================

            List<SmartCommand> successfulCommands = commands
                .Where(command =>
                    string.Equals(
                        command.Status,
                        "Processed",
                        StringComparison.OrdinalIgnoreCase) &&
                    command.WasSuccessful == true &&
                    !string.IsNullOrWhiteSpace(command.DeviceId) &&
                    !string.IsNullOrWhiteSpace(command.CommandType) &&
                    command.ProcessedAt.HasValue)
                .OrderBy(command => command.ProcessedAt)
                .ToList();

            // =====================================================
            // 6. DETECT REPEATED COMMAND PATTERNS
            // =====================================================

            var commandPatterns = successfulCommands
    .GroupBy(
        command => (
            DeviceId: command.DeviceId.Trim(),
            CommandType: command.CommandType.Trim()),
        new DeviceCommandPatternComparer())
    .ToList();

            foreach (var pattern in commandPatterns)
            {
                List<SmartCommand> matchingCommands =
                    pattern.OrderBy(command => command.ProcessedAt)
                        .ToList();

                if (matchingCommands.Count < 3)
                {
                    continue;
                }

                SmartCommand mostRecentCommand =
                    matchingCommands[^1];

                suggestions.Add(new PredictiveSuggestion
                {
                    Id = nextId++,
                    DeviceId = mostRecentCommand.DeviceId,
                    Title = "Recurring command pattern detected",
                    Description =
                        $"The command '{mostRecentCommand.CommandType}' " +
                        $"has been successfully processed for " +
                        $"{mostRecentCommand.DeviceId} " +
                        $"{matchingCommands.Count} times.",
                    SuggestedAction =
                        $"Review whether '{mostRecentCommand.CommandType}' " +
                        $"should be performed again for " +
                        $"{mostRecentCommand.DeviceId}.",
                    Severity = "Information",
                    Source = "Historical processed-command frequency",
                    GeneratedAt = now,
                    IsActionable = true
                });
            }

            // =====================================================
            // 7. CORRELATE ANOMALIES WITH SUBSEQUENT COMMANDS
            // =====================================================

            /*
             * For each device:
             * 1. Find anomalous readings in the last 24 hours.
             * 2. Find successful commands processed after those readings.
             * 3. Identify repeated command types associated with anomalies.
             *
             * This is a temporal association heuristic. It does not establish
             * that an anomaly caused a command or guarantee that the same
             * command will be required for a future anomaly.
             */

            var anomalyGroups = records
                .Where(record =>
                    record.Timestamp >= recentCutoff &&
                    record.IsAnomaly &&
                    !string.IsNullOrWhiteSpace(record.DeviceId))
                .GroupBy(
                    record => record.DeviceId.Trim(),
                    StringComparer.OrdinalIgnoreCase);

            foreach (var anomalyGroup in anomalyGroups)
            {
                string deviceId = anomalyGroup.Key;

                List<TelemetryRecord> deviceAnomalies =
                    anomalyGroup
                        .OrderBy(record => record.Timestamp)
                        .ToList();

                List<SmartCommand> deviceCommands =
                    successfulCommands
                        .Where(command =>
                            command.DeviceId.Equals(
                                deviceId,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();

                if (deviceAnomalies.Count < 2 ||
                    deviceCommands.Count == 0)
                {
                    continue;
                }

                var associatedCommands = new List<SmartCommand>();

                foreach (TelemetryRecord anomaly in deviceAnomalies)
                {
                    SmartCommand? subsequentCommand = deviceCommands
                        .Where(command =>
                            command.ProcessedAt!.Value >= anomaly.Timestamp &&
                            command.ProcessedAt.Value <=
                                anomaly.Timestamp.AddHours(2))
                        .OrderBy(command => command.ProcessedAt)
                        .FirstOrDefault();

                    if (subsequentCommand != null)
                    {
                        associatedCommands.Add(subsequentCommand);
                    }
                }

                if (associatedCommands.Count < 2)
                {
                    continue;
                }

                var mostCommonCommandPattern = associatedCommands
                    .GroupBy(
                        command => command.CommandType.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Select(group => new
                    {
                        CommandType = group.Key,
                        Count = group.Count(),
                        MostRecent = group
                            .OrderByDescending(command => command.ProcessedAt)
                            .First()
                    })
                    .OrderByDescending(group => group.Count)
                    .ThenByDescending(group =>
                        group.MostRecent.ProcessedAt)
                    .FirstOrDefault();

                if (mostCommonCommandPattern == null ||
                    mostCommonCommandPattern.Count < 2)
                {
                    continue;
                }

                suggestions.Add(new PredictiveSuggestion
                {
                    Id = nextId++,
                    DeviceId = deviceId,
                    Title = "Anomaly response pattern detected",
                    Description =
                        $"{deviceId} has produced {deviceAnomalies.Count} " +
                        "anomalous readings in the last 24 hours. " +
                        $"The command '{mostCommonCommandPattern.CommandType}' " +
                        "was repeatedly processed within two hours of an anomaly.",
                    SuggestedAction =
                        $"Review the recent readings for {deviceId} and consider " +
                        $"whether '{mostCommonCommandPattern.CommandType}' " +
                        "is appropriate if the anomaly recurs.",
                    Severity = "High",
                    Source = "Telemetry anomaly and command-history correlation",
                    GeneratedAt = now,
                    IsActionable = true
                });
            }

            // =====================================================
            // 8. REMOVE DUPLICATE SUGGESTIONS
            // =====================================================

            List<PredictiveSuggestion> uniqueSuggestions = suggestions
                .GroupBy(
                    suggestion =>
                        $"{suggestion.DeviceId}|" +
                        $"{suggestion.Title}|" +
                        $"{suggestion.Source}",
                    StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToList();

            // =====================================================
            // 9. ORDER SUGGESTIONS BY SEVERITY
            // =====================================================

            return uniqueSuggestions
                .OrderByDescending(suggestion =>
                    suggestion.Severity == "Critical" ? 3 :
                    suggestion.Severity == "High" ? 2 :
                    suggestion.Severity == "Medium" ? 1 : 0)
                .ThenBy(suggestion => suggestion.DeviceId)
                .ToList();
        }

        // =====================================================
        // 10. CASE-INSENSITIVE COMMAND-PATTERN COMPARER
        // =====================================================

        private sealed class DeviceCommandPatternComparer :
            IEqualityComparer<(string DeviceId, string CommandType)>
        {
            public bool Equals(
                (string DeviceId, string CommandType) x,
                (string DeviceId, string CommandType) y)
            {
                return string.Equals(
                           x.DeviceId,
                           y.DeviceId,
                           StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(
                           x.CommandType,
                           y.CommandType,
                           StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(
                (string DeviceId, string CommandType) value)
            {
                return HashCode.Combine(
                    StringComparer.OrdinalIgnoreCase.GetHashCode(
                        value.DeviceId),
                    StringComparer.OrdinalIgnoreCase.GetHashCode(
                        value.CommandType));
            }
        }
    }
}