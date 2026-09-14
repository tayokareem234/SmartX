# Smart-X Testing

## 1. Testing Environment

Testing was performed using the Smart-X Windows Forms client together with the ASP.NET Core Web API.

The API was started first so that the client could communicate with the available endpoints. The Windows Forms dashboard was then used to perform the functional tests.

## 2. Testing Approach

The testing process followed a functional approach. Each major Part 1 feature was tested using valid and invalid inputs where applicable.

The main areas tested were:

1. Sensor registration and validation
2. Telemetry ingestion
3. Telemetry anomaly detection
4. Sensor disconnect detection
5. Telemetry filtering
6. File attachments
7. Recursive deployment validation

## 3. Client and API Validation

Smart-X uses validation at both application layers.

### 3.1 Client-Side Validation

The Windows Forms client checks user input before sending registration requests to the API.

Examples include:

* Required field validation
* MAC address format validation
* Unique identifier format and length validation
* Location length validation
* Zone / node length validation
* Sensor category selection

### 3.2 API Validation

The API performs its own validation after receiving requests.

Examples include:

* Required sensor information
* Duplicate sensor identifiers
* Registered device verification
* Telemetry unit validation
* Deployment hierarchy validation
* Attachment validation

This two-layer approach prevents invalid user input from being accepted solely because client-side validation was bypassed.

---

## 4. Sensor Registration Testing

The sensor registration workflow was tested through the Smart-X Windows Forms client and API.

### Test 1: Invalid MAC Address

**Input**

```text
MAC Address: AA:BB:CC
Unique Identifier: TEST-001
Deployment Location: Lab 201
Zone / Node: Zone C
Sensor Category: Environmental
```

**Expected Result**

The client-side validation rejects the MAC address before an API request is sent.

**Result**

Passed.

The user is shown a validation message and the MAC address field receives focus.

### Test 2: Valid Sensor Registration

**Input**

```text
MAC Address: AA:BB:CC:99:88:77
Unique Identifier: TEMP-002
Deployment Location: Lab 201
Zone / Node: Zone C
Sensor Category: Environmental
```

**Expected Result**

The sensor is accepted by the API, added to the sensor collection and displayed in the dashboard.

**Result**

Passed.

The registration completed successfully and the sensor list was refreshed.

### Test 3: Duplicate Sensor Identifier

**Input**

```text
MAC Address: AA:BB:CC:11:22:44
Unique Identifier: TEMP-001
Deployment Location: Lab 202
Zone / Node: Zone C
Sensor Category: Environmental
```

**Expected Result**

The API rejects the registration because the unique identifier already exists.

**Result**

Passed.

The API returned a conflict response indicating that a sensor with the unique identifier already exists.

---

## 5. Telemetry Anomaly Detection Testing

The telemetry processing workflow was tested using simulated temperature and power readings.

### Test 4: Normal Temperature Reading

**Input**

```text
Device: TEMP-001
Temperature: 25.5 °C
```

**Expected Result**

The telemetry reading is accepted and recorded with a normal status.

**Result**

Passed.

The reading is displayed as a normal telemetry record.

### Test 5: Temperature Anomaly

**Input**

```text
Device: TEMP-001
Temperature: 75.5 °C
```

**Expected Result**

The reading is accepted but identified as an anomaly because it is outside the configured temperature range.

**Result**

Passed.

The telemetry record is marked as an anomaly and the dashboard highlights the reading for troubleshooting.

### Test 6: Power Consumption Reading

**Input**

```text
Device: POWER-001
Power Consumption: 1250 W
```

**Expected Result**

The reading is accepted and processed as power consumption telemetry.

**Result**

Passed.

The reading is displayed in the recent telemetry history with the correct device, category and unit.

### Test 7: Simulate Sensor Disconnect

**Input**

```text
Device: ACT-001
Action: Simulate Disconnect
```

**Expected Result**

The selected sensor is marked as disconnected and the dashboard displays a disconnect alert.

**Result**

Passed.

The sensor status changes to disconnected and the dashboard highlights the condition for the user.

---

## 6. Telemetry Filtering Testing

### Test 9: Show All

**Input**

```text
Filter: Show All
```

**Expected Result**

All available telemetry records are displayed.

**Result**

Passed.

Normal, anomalous and disconnected telemetry conditions are displayed in the telemetry history.

### Test 10: Normal Only

**Input**

```text
Filter: Normal Only
```

**Expected Result**

Only telemetry records with a normal status are displayed.

**Result**

Passed.

Anomalous and disconnected records are excluded from the displayed results.

### Test 11: Anomalies Only

**Input**

```text
Filter: Anomalies Only
```

**Expected Result**

Only telemetry records identified as anomalies are displayed.

**Result**

Passed.

The temperature anomaly records are displayed while normal records are excluded.

### Test 12: Disconnected Only

**Input**

```text
Filter: Disconnected Only
```

**Expected Result**

Only disconnected sensor conditions are displayed.

**Result**

Passed.

The disconnected sensor condition is displayed while normal telemetry records are excluded.

---

## 7. File Attachment Testing

### Test 13: Upload Sensor Attachment

**Input**

```text
Device: TEMP-001
Attachment Type: Sensor Log
File: test.txt
```

**Expected Result**

The selected file is uploaded to the API and associated with the selected sensor.

**Result**

Passed.

The API successfully received the file and returned the attachment information.

### Test 14: Encrypted File Storage

**Expected Result**

The uploaded file is stored by the API using the encrypted file storage service.

**Result**

Passed.

The uploaded file is stored in the `UploadedFiles` directory with an encrypted file extension.

The storage service uses AES encryption with a randomly generated initialization vector for each uploaded file.

---

## 8. Recursive Deployment Validation Testing

### Test 15: Valid Deployment Hierarchy

**Input**

```text
Facility
└── Zone
    └── Sub-Zone
        └── Room
            └── Sensor
```

**Expected Result**

The complete deployment structure is accepted because each child node follows the permitted parent-child relationship.

**Result**

Passed.

The API returned a successful response indicating that the deployment structure is valid.

### Test 16: Invalid Deployment Hierarchy

**Input**

```text
Facility
└── Sensor
```

**Expected Result**

The deployment structure is rejected because a sensor cannot be placed directly under a facility.

**Result**

Passed.

The API returned a bad request response and identified the invalid deployment path.

---

## 9. Test Result

The completed functional tests produced the expected results for the implemented Part 1 features.

The testing evidence recorded in this document can be used alongside the GitHub source code and demonstration video to show the operation of the Smart-X Data Ingestion and Validation Gateway.

## Part 1 Feature Coverage

The following table summarises the main Smart-X Part 1 features and the corresponding testing completed.

| Part 1 Feature | Tested |
|---|---|
| Sensor registration | Yes |
| Client-side validation | Yes |
| API validation | Yes |
| Generic telemetry packets | Yes |
| Temperature telemetry | Yes |
| Power telemetry | Yes |
| Actuator telemetry | Yes |
| Anomaly detection | Yes |
| Sensor disconnect detection | Yes |
| Telemetry filtering | Yes |
| File attachments | Yes |
| Encrypted file storage | Yes |
| Recursive deployment validation | Yes |
| Multi-dimensional telemetry batch processing | Yes |
| Jagged sensor history | Yes |
| Operator overloading | Yes |

This testing coverage provides evidence that the main implemented Part 1 functionality has been exercised through the Smart-X client and API.
