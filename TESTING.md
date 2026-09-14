# Smart-X Testing

## Sensor Registration Testing

The sensor registration workflow was tested through the Smart-X Windows Forms client and API.

### Test 1: Invalid MAC Address

**Input**

Test 1
MAC Address: AA:BB:CC
Unique Identifier: TEST-001
Deployment Location: Lab 201
Zone / Node: Zone C
Sensor Category: Environmental

Expected Result

The client-side validation rejects the MAC address before an API request is sent.

Result

Passed.

The user is shown a validation message and the MAC address field receives focus.


Test 2
MAC Address: AA:BB:CC:99:88:77
Unique Identifier: TEMP-002
Deployment Location: Lab 201
Zone / Node: Zone C
Sensor Category: Environmental

Expected Result

The sensor is accepted by the API, added to the sensor collection and displayed in the dashboard.

Result

Passed.

The registration completed successfully and the sensor list was refreshed.


Test 3
MAC Address: AA:BB:CC:11:22:44
Unique Identifier: TEMP-001
Deployment Location: Lab 202
Zone / Node: Zone C
Sensor Category: Environmental

Expected Result

The API rejects the registration because the unique identifier already exists.

Result

Passed.

The API returned a conflict response indicating that a sensor with the unique identifier already exists.

## Telemetry Anomaly Detection Testing

The telemetry processing workflow was tested using simulated temperature and power readings.

### Test 4: Normal Temperature Reading

**Input**

```text
Device: TEMP-001
Temperature: 25.5 °C

Expected Result

The telemetry reading is accepted and recorded with a normal status.

Result

Passed.

The reading is displayed as a normal telemetry record.

Test 5: Temperature Anomaly

Input

Device: TEMP-001
Temperature: 75.5 °C

Expected Result

The reading is accepted but identified as an anomaly because it is outside the configured temperature range.

Result

Passed.

The telemetry record is marked as an anomaly and the dashboard highlights the reading for troubleshooting.

Test 6: Power Consumption Reading

Input

Device: POWER-001
Power Consumption: 1250 W

Expected Result

The reading is accepted and processed as power consumption telemetry.

Result

Passed.

The reading is displayed in the recent telemetry history with the correct device, category and unit.

### Test 7: Simulate Sensor Disconnect

**Input**

```text
Device: ACT-001
Action: Simulate Disconnect

Expected Result

The selected sensor is marked as disconnected and the dashboard displays a disconnect alert.

Result

Passed.

The sensor status changes to disconnected and the dashboard highlights the condition for the user.

### Test 9: Show All

**Input**

```text
Filter: Show All

Expected Result

All available telemetry records are displayed.

Result

Passed.

Normal, anomalous and disconnected telemetry conditions are displayed in the telemetry history.

Test 10: Normal Only

Input

Filter: Normal Only

Expected Result

Only telemetry records with a normal status are displayed.

Result

Passed.

Anomalous and disconnected records are excluded from the displayed results.

Test 11: Anomalies Only

Input

Filter: Anomalies Only

Expected Result

Only telemetry records identified as anomalies are displayed.

Result

Passed.

The temperature anomaly records are displayed while normal records are excluded.

Test 12: Disconnected Only

Input

Filter: Disconnected Only

Expected Result

Only disconnected sensor conditions are displayed.

Result

Passed.

The disconnected sensor condition is displayed while normal telemetry records are excluded.