using SmartX.Client.Forms;
using SmartX.Client.Services;
using SmartX.Shared.Models;

namespace SmartX.Client
{
    public partial class Form1 : Form
    {
        private readonly ApiClient _apiClient;

        private Label _connectionStatus = null!;
        private ListBox _sensorList = null!;

        private TextBox _macAddressTextBox = null!;
        private TextBox _uniqueIdentifierTextBox = null!;
        private TextBox _locationTextBox = null!;
        private TextBox _zoneTextBox = null!;
        private ComboBox _categoryComboBox = null!;
        private Button _registerSensorButton = null!;

        public Form1()
        {
            InitializeComponent();

            _apiClient = new ApiClient();

            BuildInterface();

            _ = LoadSensorsAsync();
        }

        private void BuildInterface()
        {
            Text = "Smart-X | IoT Telemetry Gateway";

            StartPosition =
                FormStartPosition.CenterScreen;

            Width = 1100;
            Height = 850;

            MinimumSize =
                new Size(950, 750);

            BackColor =
                Color.FromArgb(245, 247, 250);

            // =====================================================
            // HEADER
            // =====================================================

            Panel header = new Panel
            {
                Location = new Point(0, 0),
                Width = ClientSize.Width,
                Height = 105,
                BackColor =
                    Color.FromArgb(30, 41, 59),
                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right
            };

            Label title = new Label
            {
                Text = "SMART-X TELEMETRY",
                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI",
                    24,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 15)
            };

            Label subtitle = new Label
            {
                Text =
                    "Real-Time Sensor Data Ingestion and Monitoring",
                ForeColor = Color.LightGray,
                Font = new Font(
                    "Segoe UI",
                    10),
                AutoSize = true,
                Location = new Point(32, 65)
            };

            header.Controls.Add(title);
            header.Controls.Add(subtitle);

            Controls.Add(header);

            // =====================================================
            // SYSTEM ARCHITECTURE
            // =====================================================

            Label architectureTitle = new Label
            {
                Text = "System Architecture",
                Font = new Font(
                    "Segoe UI",
                    18,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(35, 125)
            };

            Controls.Add(architectureTitle);

            Label description = new Label
            {
                Text =
                    "Select an available Smart-X module to continue.",
                Font = new Font(
                    "Segoe UI",
                    10),
                AutoSize = true,
                Location = new Point(37, 160)
            };

            Controls.Add(description);

            TableLayoutPanel modules =
                new TableLayoutPanel
                {
                    Location = new Point(35, 195),
                    Width = 1030,
                    Height = 165,
                    ColumnCount = 3,
                    RowCount = 1,
                    Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Left |
                        AnchorStyles.Right
                };

            modules.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    33.33f));

            modules.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    33.33f));

            modules.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    33.34f));

            Button telemetryButton =
                CreateModuleButton(
                    "SENSOR DATA INGESTION",
                    "Telemetry monitoring, validation and analysis",
                    false);

            telemetryButton.Click +=
                TelemetryButton_Click;

            Button commandButton =
                CreateModuleButton(
                    "COMMAND STREAM",
                    "Real-time commands and history",
                    true);

            Button topologyButton =
                CreateModuleButton(
                    "NETWORK TOPOLOGY",
                    "Mesh routing and network structure",
                    true);

            modules.Controls.Add(
                telemetryButton,
                0,
                0);

            modules.Controls.Add(
                commandButton,
                1,
                0);

            modules.Controls.Add(
                topologyButton,
                2,
                0);

            Controls.Add(modules);

            // =====================================================
            // SENSOR REGISTRATION
            // =====================================================

            Label registrationTitle = new Label
            {
                Text = "Sensor Registration",
                Font = new Font(
                    "Segoe UI",
                    16,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(35, 380)
            };

            Controls.Add(registrationTitle);

            Label registrationDescription = new Label
            {
                Text =
                    "Register a Smart-X sensor using its device identity and deployment information.",
                Font = new Font(
                    "Segoe UI",
                    9),
                AutoSize = true,
                Location = new Point(37, 412)
            };

            Controls.Add(registrationDescription);

            // MAC Address

            Label macLabel = new Label
            {
                Text = "MAC Address",
                AutoSize = true,
                Location = new Point(35, 445)
            };

            _macAddressTextBox = new TextBox
            {
                Location = new Point(35, 465),
                Width = 190
            };

            // Unique Identifier

            Label identifierLabel = new Label
            {
                Text = "Unique Identifier",
                AutoSize = true,
                Location = new Point(240, 445)
            };

            _uniqueIdentifierTextBox = new TextBox
            {
                Location = new Point(240, 465),
                Width = 190
            };

            // Deployment Location

            Label locationLabel = new Label
            {
                Text = "Deployment Location",
                AutoSize = true,
                Location = new Point(445, 445)
            };

            _locationTextBox = new TextBox
            {
                Location = new Point(445, 465),
                Width = 190
            };

            // Zone

            Label zoneLabel = new Label
            {
                Text = "Zone / Node",
                AutoSize = true,
                Location = new Point(650, 445)
            };

            _zoneTextBox = new TextBox
            {
                Location = new Point(650, 465),
                Width = 150
            };

            // Category

            Label categoryLabel = new Label
            {
                Text = "Sensor Category",
                AutoSize = true,
                Location = new Point(815, 445)
            };

            _categoryComboBox = new ComboBox
            {
                Location = new Point(815, 465),
                Width = 220,
                DropDownStyle =
                    ComboBoxStyle.DropDownList
            };

            _categoryComboBox.Items.Add(
                "Environmental");

            _categoryComboBox.Items.Add(
                "Power Consumption");

            _categoryComboBox.Items.Add(
                "Actuator");

            _categoryComboBox.SelectedIndex = 0;

            // Register button

            _registerSensorButton = new Button
            {
                Text = "Register Sensor",
                Location = new Point(35, 505),
                Width = 180,
                Height = 35
            };

            _registerSensorButton.Click +=
                RegisterSensorButton_Click;

            Controls.Add(macLabel);
            Controls.Add(_macAddressTextBox);

            Controls.Add(identifierLabel);
            Controls.Add(_uniqueIdentifierTextBox);

            Controls.Add(locationLabel);
            Controls.Add(_locationTextBox);

            Controls.Add(zoneLabel);
            Controls.Add(_zoneTextBox);

            Controls.Add(categoryLabel);
            Controls.Add(_categoryComboBox);

            Controls.Add(_registerSensorButton);

            // =====================================================
            // REGISTERED SENSORS
            // =====================================================

            Label sensorTitle = new Label
            {
                Text = "Registered Sensors",
                Font = new Font(
                    "Segoe UI",
                    16,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(35, 565)
            };

            Controls.Add(sensorTitle);

            _sensorList = new ListBox
            {
                Location = new Point(35, 605),
                Width = 1030,
                Height = 125,
                Font = new Font(
                    "Segoe UI",
                    10),
                Anchor =
                    AnchorStyles.Left |
                    AnchorStyles.Right |
                    AnchorStyles.Bottom
            };

            Controls.Add(_sensorList);

            // =====================================================
            // CONNECTION STATUS
            // =====================================================

            _connectionStatus = new Label
            {
                Text =
                    "Connecting to Smart-X API...",
                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(230, 513)
            };

            Controls.Add(_connectionStatus);
        }

        private Button CreateModuleButton(
            string title,
            string description,
            bool disabled)
        {
            Button button = new Button
            {
                Dock = DockStyle.Fill,
                Text =
                    $"{title}\r\n\r\n{description}",
                Font = new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Bold),
                Margin = new Padding(10),
                Enabled = !disabled,
                UseVisualStyleBackColor = false
            };

            if (disabled)
            {
                button.Text +=
                    "\r\n\r\n[AVAILABLE IN LATER PROJECT PART]";
            }

            return button;
        }

        private async Task LoadSensorsAsync()
        {
            try
            {
                List<Sensor> sensors =
                    await _apiClient.GetSensorsAsync();

                DisplaySensors(sensors);

                _connectionStatus.Text =
                    $"API Connected | {sensors.Count} sensors loaded";

                _connectionStatus.ForeColor =
                    Color.DarkGreen;
            }
            catch (Exception ex)
            {
                _connectionStatus.Text =
                    "API Connection Failed";

                _connectionStatus.ForeColor =
                    Color.DarkRed;

                MessageBox.Show(
                    $"Unable to connect to the Smart-X API.\n\n" +
                    $"{ex.Message}",
                    "Smart-X API Connection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DisplaySensors(
            List<Sensor> sensors)
        {
            _sensorList.Items.Clear();

            foreach (Sensor sensor in sensors)
            {
                string connectionState =
                    sensor.IsConnected
                        ? "CONNECTED"
                        : "DISCONNECTED";

                _sensorList.Items.Add(
                    $"{sensor.UniqueIdentifier} | " +
                    $"{sensor.SensorCategory} | " +
                    $"{sensor.DeploymentLocation} | " +
                    $"{sensor.Zone} | " +
                    $"{connectionState}");
            }
        }

        private async void RegisterSensorButton_Click(
            object? sender,
            EventArgs e)
        {
            if (!ValidateRegistrationInput())
            {
                return;
            }

            Sensor sensor = new Sensor
            {
                MacAddress =
                    _macAddressTextBox.Text.Trim(),

                UniqueIdentifier =
                    _uniqueIdentifierTextBox.Text.Trim(),

                DeploymentLocation =
                    _locationTextBox.Text.Trim(),

                Zone =
                    _zoneTextBox.Text.Trim(),

                SensorCategory =
                    _categoryComboBox.SelectedItem?.ToString()
                    ?? string.Empty
            };

            try
            {
                _registerSensorButton.Enabled = false;

                Sensor? registeredSensor =
                    await _apiClient.RegisterSensorAsync(
                        sensor);

                if (registeredSensor == null)
                {
                    throw new InvalidOperationException(
                        "The API did not return the registered sensor.");
                }

                MessageBox.Show(
                    "Sensor registered successfully.\n\n" +
                    $"Device: {registeredSensor.UniqueIdentifier}\n" +
                    $"Location: {registeredSensor.DeploymentLocation}\n" +
                    $"Category: {registeredSensor.SensorCategory}",
                    "Smart-X Sensor Registration",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearRegistrationFields();

                await LoadSensorsAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Smart-X Sensor Registration",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"The sensor could not be registered.\n\n" +
                    $"{ex.Message}",
                    "Smart-X Sensor Registration",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _registerSensorButton.Enabled = true;
            }
        }

        private bool ValidateRegistrationInput()
        {
            if (string.IsNullOrWhiteSpace(
                _macAddressTextBox.Text))
            {
                MessageBox.Show(
                    "Please enter the sensor MAC address.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                _macAddressTextBox.Focus();

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                _uniqueIdentifierTextBox.Text))
            {
                MessageBox.Show(
                    "Please enter the sensor unique identifier.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                _uniqueIdentifierTextBox.Focus();

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                _locationTextBox.Text))
            {
                MessageBox.Show(
                    "Please enter the deployment location.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                _locationTextBox.Focus();

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                _zoneTextBox.Text))
            {
                MessageBox.Show(
                    "Please enter the zone or node.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                _zoneTextBox.Focus();

                return false;
            }

            if (_categoryComboBox.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Please select a sensor category.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                _categoryComboBox.Focus();

                return false;
            }

            return true;
        }

        private void ClearRegistrationFields()
        {
            _macAddressTextBox.Clear();
            _uniqueIdentifierTextBox.Clear();
            _locationTextBox.Clear();
            _zoneTextBox.Clear();

            _categoryComboBox.SelectedIndex = 0;
        }

        private void TelemetryButton_Click(
            object? sender,
            EventArgs e)
        {
            TelemetryForm telemetryForm =
                new TelemetryForm();

            telemetryForm.Show();
        }
    }
}