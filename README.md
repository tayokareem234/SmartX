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

### 11.1 Sensors

```http
GET  /api/sensors
GET  /api/sensors/{id}
POST /api/sensors
PUT  /api/sensors/{id}
```

### 11.2 Telemetry

```http
GET  /api/telemetry
GET  /api/telemetry/{deviceId}
POST /api/telemetry/temperature
POST /api/telemetry/power
POST /api/telemetry/switch
POST /api/telemetry/validate-deployment
```

### 11.3 Attachments

```http
POST /api/sensors/{deviceId}/attachments
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
