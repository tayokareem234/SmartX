# Smart-X Data Ingestion and Validation Gateway

Smart-X is a **.NET-based IoT data ingestion and monitoring application** developed for Part 1 of the Programming 3B / Advanced Application Development project.

The application receives simulated sensor telemetry, validates and processes the data, and displays the results through a Windows Forms dashboard.

---

## 1. Project Structure

```text
SmartX
├── SmartX.Api       # ASP.NET Core Web API
├── SmartX.Client    # Windows Forms client
└── SmartX.Shared    # Shared models
```

---

## 2. Main Features

* Sensor registration and validation
* ASP.NET Core Web API integration
* Temperature, power and actuator telemetry
* Generic `TelemetryPacket<T>`
* Operator overloading
* Multi-dimensional and jagged arrays
* `List<T>` collections
* Recursive deployment validation
* Anomaly and disconnect detection
* Real-time telemetry dashboard
* Telemetry filtering
* Sensor file and log attachments
* Encrypted file storage

---

## 3. Architecture

```text
SmartX.Client
      │
      │ HTTP / JSON
      ▼
SmartX.Api
      │
      ▼
SmartX.Shared
```

The client communicates asynchronously with the API using `HttpClient`.

---

## 4. Requirements

* Visual Studio
* .NET 10 SDK
* Windows

Open `SmartX.sln` in Visual Studio to run the project.

---

## 5. Running the Application

1. Start `SmartX.Api`.
2. Start `SmartX.Client`.
3. The startup screen displays the available Smart-X modules.
4. Select **Sensor Data Ingestion** to open the telemetry dashboard.

> **Note:** The other modules are disabled because they belong to later project parts.

---

## 6. Sensor Registration

Sensors can be registered using the following information:

* MAC Address
* Unique Identifier
* Deployment Location
* Zone / Node
* Sensor Category

Available sensor categories:

```text
Environmental
Power Consumption
Actuator
```

Client-side validation is performed before the request is sent to the API. The API then performs its own validation, including duplicate identifier checking.

---

## 7. Telemetry

Smart-X uses the generic `TelemetryPacket<T>` class to support different telemetry types.

```text
TelemetryPacket<float>  → Temperature
TelemetryPacket<int>    → Power Consumption
TelemetryPacket<bool>   → Actuator
```

The dashboard can simulate:

* Normal temperature readings
* Temperature anomalies
* Power readings
* Switch triggers
* Sensor disconnects

---

## 8. Advanced Programming Features

### 8.1 Operator Overloading

`SensorValue` overloads the following operators:

```text
+
-
>
<
```

The `-` operator is used to calculate the difference between the current and previous telemetry readings.

### 8.2 Arrays and Collections

A multi-dimensional `double[50, 3]` array temporarily stores raw telemetry batches.

A jagged `double[][]` array stores variable-length sensor histories.

Processed batch data is transferred into a `List<TelemetryRecord>`.

### 8.3 Recursive Validation

Deployment structures use `DeploymentNode` objects with child nodes.

Example:

```text
Facility
└── Zone
    └── Sub-Zone
        └── Room
            └── Sensor
```

The recursive validator checks each node and its parent-child relationship.

**Endpoint:**

```http
POST /api/telemetry/validate-deployment
```

---

## 9. Dashboard Engagement

The selected engagement strategy is:

**Real-Time Visual Telemetry Feedback with Proactive Anomaly Highlighting**

Normal readings are displayed in the telemetry history, while anomalies and disconnected sensors receive visual alerts so that problems can be identified quickly.

Telemetry can also be filtered using:

```text
Show All
Normal Only
Anomalies Only
Disconnected Only
```

---

## 10. File Attachments

Files can be attached to individual sensors through the dashboard.

The API receives files using multipart upload and stores them using the encrypted file storage service.

---

## 11. API Endpoints

Smart-X exposes REST API endpoints for sensor management, telemetry ingestion, deployment validation and file attachments.

### 11.1 Sensors

| Method | Endpoint            | Purpose                        |
| ------ | ------------------- | ------------------------------ |
| GET    | `/api/sensors`      | Returns all registered sensors |
| GET    | `/api/sensors/{id}` | Returns a sensor by ID         |
| POST   | `/api/sensors`      | Registers a new sensor         |
| PUT    | `/api/sensors/{id}` | Updates an existing sensor     |

### 11.2 Telemetry

| Method | Endpoint                             | Purpose                                 |
| ------ | ------------------------------------ | --------------------------------------- |
| GET    | `/api/telemetry`                     | Returns all telemetry records           |
| GET    | `/api/telemetry/{deviceId}`          | Returns telemetry for a specific device |
| POST   | `/api/telemetry/temperature`         | Receives temperature telemetry          |
| POST   | `/api/telemetry/power`               | Receives power consumption telemetry    |
| POST   | `/api/telemetry/switch`              | Receives actuator switch telemetry      |
| POST   | `/api/telemetry/validate-deployment` | Validates a deployment hierarchy        |

### 11.3 Attachments

| Method | Endpoint                              | Purpose                                |
| ------ | ------------------------------------- | -------------------------------------- |
| POST   | `/api/sensors/{deviceId}/attachments` | Uploads a file attachment for a sensor |

### 11.4 Telemetry Request Types

The telemetry endpoints use the generic `TelemetryPacket<T>` model:

```text
TelemetryPacket<float>  → Temperature
TelemetryPacket<int>    → Power Consumption
TelemetryPacket<bool>   → Actuator
```

---

## 12. Testing

The application has been tested through the Windows Forms client and API `.http` requests.

Testing includes:

* Generic `float`, `int` and `bool` telemetry
* Normal and anomalous readings
* Sensor disconnects
* Valid and invalid deployment hierarchies
* Invalid MAC addresses
* Successful sensor registration
* Duplicate sensor identifiers
* Telemetry filtering
* File attachments

---

## 13. Part 1 Scope

This repository focuses on the **Sensor Data Ingestion and Telemetry** functionality required for Part 1.

```text
Sensor Data Ingestion
        │
        ▼
      Part 1

Command Stream
        │
        ▼
    Later Part

Network Topology
        │
        ▼
    Final POE
```

---

## 14. Part 2: Command Stream and Predictive Insights

Part 2 extends Smart-X with command processing, command prioritisation, historical command tracking and predictive suggestions.

The Command Stream allows operators to create commands for registered devices, process commands according to their priority, and review command history. The predictive suggestion engine analyses telemetry and historical command data to identify recurring issues and recommend appropriate actions.

### 14.1 Command Stream

The Command Stream provides a dedicated interface for creating, prioritising and processing commands.

Main features:

* Command creation for registered devices
* FIFO command processing
* Priority-based command processing
* Command history tracking
* Stack-based undo functionality
* Shared command ID generation
* Processing status and timestamps
* Historical command frequency analysis

Commands contain the following information:

```text
ID
Device ID
Command Type
Description
Priority
Created At
Status
Processed At
Was Successful
```

The command status and processing timestamp are updated when a command is processed.

> **Note:** Command processing is simulated within the application. It records the local processing outcome and does not directly execute commands on physical IoT hardware.

---

### 14.2 Advanced Collections

Smart-X uses different data structures to manage commands and historical information.

#### FIFO Queue

The FIFO queue processes commands in the order they were added.

```text
First In → First Out
```

This provides a predictable processing order for standard commands.

#### Priority Queue

The priority queue allows higher-priority commands to be processed before lower-priority commands.

The application supports the following priority levels:

```text
Critical
High
Normal
Low
```

This allows important commands to be handled before less urgent commands.

#### Stack-Based Undo

A stack is used to maintain undoable command history.

```text
Last In → First Out
```

The undo operation retrieves the most recently recorded undoable command and updates its local status.

> **Note:** Undo changes the application's command state. It does not reverse an operation on a physical device.

---

### 14.3 Dictionary and SortedDictionary

Smart-X uses dictionary-based collections to support efficient lookup and chronological history management.

#### Dictionary

A `Dictionary<string, Sensor>` stores sensors using their unique identifiers as keys.

The dictionary uses case-insensitive identifier comparison to prevent duplicate identifiers that differ only in letter casing.

This supports:

* Sensor lookup by unique identifier
* Duplicate identifier detection
* Sensor identifier updates
* Consistent sensor identification

#### SortedDictionary

A `SortedDictionary<DateTime, List<TelemetryRecord>>` stores telemetry records according to their timestamps.

Multiple records can be stored under the same timestamp.

This supports:

* Chronological telemetry history
* Retrieval of all historical telemetry records
* Retrieval of telemetry for a specific device
* Historical analysis of sensor readings

---

### 14.4 HashSet Tracking

Smart-X uses `HashSet<string>` collections to track active anomalies and disconnected devices.

The collections use case-insensitive identifier comparison to prevent duplicate device entries.

Tracked information includes:

* Devices with active telemetry anomalies
* Devices currently marked as disconnected

When a device returns to normal operation, the relevant tracking entry is updated or removed.

This allows the application to maintain a collection of currently active issues rather than repeatedly adding the same device.

---

### 14.5 Predictive Suggestion Engine

The predictive suggestion engine analyses telemetry records, sensor connection states and historical processed commands.

It generates suggestions to help operators identify problems and decide what to investigate.

The engine currently supports the following insights:

| Insight | Description |
| ------- | ----------- |
| Abnormal telemetry | Identifies devices whose latest telemetry reading is flagged as anomalous. |
| Repeated anomalies | Detects devices with at least three anomalous readings within the last 24 hours. |
| Disconnected devices | Recommends checking the power supply and network connection of disconnected devices. |
| Recurring command patterns | Identifies commands that have been successfully processed for the same device at least three times. |
| Telemetry and command correlation | Uses recent telemetry anomalies and command history to provide additional investigation suggestions. |

Suggestions contain the following information:

```text
ID
Device ID
Title
Description
Suggested Action
Severity
Source
Generated At
Is Actionable
```

Suggestions are ordered by severity so that critical and high-priority issues appear before lower-priority insights.

The dashboard displays these results in the **Suggested Actions | Automated Insights** section.

The engine uses rule-based analysis of historical application data. Its suggestions support operator decisions and do not automatically execute physical device operations.

---

### 14.6 Command History API

The API stores successfully processed commands so that the predictive suggestion engine can analyse historical command patterns.

| Method | Endpoint | Purpose |
| ------ | -------- | ------- |
| GET | `/api/commands/history` | Returns all recorded commands. |
| GET | `/api/commands/history/device/{deviceId}` | Returns recorded commands for a specific device. |
| POST | `/api/commands/history` | Records a successfully processed command. |

The API validates incoming command history records before storing them.

A command must have a valid ID, a device identifier, a processed status and a processing timestamp. Duplicate command IDs are rejected.

---

### 14.7 Predictive Suggestions API

The predictive suggestion engine exposes an endpoint for retrieving generated suggestions.

| Method | Endpoint | Purpose |
| ------ | -------- | ------- |
| GET | `/api/suggestions` | Returns generated predictive suggestions based on available telemetry and command history. |

The endpoint can return suggestions for abnormal telemetry, repeated anomalies, disconnected devices and recurring command patterns.

---

### 14.8 Telemetry Tracking Endpoints

Additional endpoints expose the current anomaly and disconnected-device tracking collections.

| Method | Endpoint | Purpose |
| ------ | -------- | ------- |
| GET | `/api/telemetry/tracking/anomalies` | Returns the identifiers of devices with active anomalies. |
| GET | `/api/telemetry/tracking/disconnected` | Returns the identifiers of disconnected devices. |
| GET | `/api/telemetry/history/sorted` | Returns telemetry history ordered by timestamp. |
| POST | `/api/sensors/{deviceId}/disconnect` | Marks a sensor as disconnected. |

These endpoints support the monitoring and tracking functionality used by the Smart-X application.

---

### 14.9 Dashboard Integration

The telemetry dashboard presents predictive insights alongside current sensor readings and telemetry alerts.

The dashboard includes:

* Temperature, power consumption and actuator status cards
* Visual highlighting of abnormal readings
* Disconnected-device alerts
* Suggested actions and automated insights
* Telemetry simulation controls
* Recent telemetry history
* Telemetry filtering
* Sensor file attachments

This integration allows operators to review current telemetry and recommended actions from one interface.

---

## 15. Part 2 Testing

Part 2 functionality has been manually tested through the Windows Forms client and API requests.

Testing includes:

* FIFO command queue processing
* Priority-based command processing
* Command history recording
* Stack-based undo functionality
* Shared command ID generation
* Unique sensor identifier enforcement
* Chronological telemetry history
* Active anomaly tracking
* Disconnected-device tracking
* Clearing tracking entries when device states change
* Predictive suggestions for abnormal telemetry
* Detection of repeated anomalous readings
* Historical command frequency analysis
* Dashboard presentation of suggested actions
* API retrieval of command history and predictive suggestions

The application has also been built using the .NET CLI.

Automated test execution should be verified separately before submission.

---

## 16. Current Implementation Scope

The current implementation includes the following modules:

```text
Sensor Data Ingestion
        |
        v
   Part 1 Features

Command Stream
        |
        v
   Part 2 Features

Predictive Insights
        |
        v
   Part 2 Features

Network Topology
        |
        v
   Future Feature
```

Network Topology remains unavailable through the main menu and is reserved for a future project stage.
