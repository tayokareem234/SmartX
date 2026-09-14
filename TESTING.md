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