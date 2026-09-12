using SmartX.Client.Services;
using SmartX.Shared.Models;

namespace SmartX.Client.Forms
{
    public partial class TelemetryForm : Form
    {
        private readonly ApiClient _apiClient;

        private Label _connectionStatus = null!;
        private Label _temperatureValue = null!;
        private Label _powerValue = null!;
        private Label _actuatorValue = null!;
        private Label _alertLabel = null!;
        private ListBox _telemetryList = null!;

        private Button _normalTemperatureButton = null!;
        private Button _anomalyTemperatureButton = null!;
        private Button _powerButton = null!;
        private Button _switchButton = null!;
        private Button _disconnectButton = null!;

        private ComboBox _sensorSelector = null!;
        private ComboBox _attachmentTypeSelector = null!;
        private Button _attachFileButton = null!;

        public TelemetryForm()
        {
            InitializeComponent();

            _apiClient = new ApiClient();

            BuildDashboard();

            _ = LoadTelemetryAsync();
        }

        private void PopulateSensorSelector(List<Sensor> sensors)
        {
            _sensorSelector.Items.Clear();

            foreach (Sensor sensor in sensors)
            {
                _sensorSelector.Items.Add(
                    sensor.UniqueIdentifier);
            }

            if (_sensorSelector.Items.Count > 0)
            {
                _sensorSelector.SelectedIndex = 0;
            }
        }

        private void BuildDashboard()
        {
            Text = "Smart-X | Telemetry Dashboard";

            StartPosition = FormStartPosition.CenterScreen;

            Width = 1200;
            Height = 750;

            MinimumSize = new Size(1000, 650);

            BackColor = Color.FromArgb(245, 247, 250);

            // Header
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.FromArgb(30, 41, 59)
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
                Text = "Real-Time Sensor Data Ingestion and Monitoring",
                ForeColor = Color.LightGray,
                Font = new Font(
                    "Segoe UI",
                    10),
                AutoSize = true,
                Location = new Point(32, 55)
            };

            header.Controls.Add(title);
            header.Controls.Add(subtitle);

            Controls.Add(header);

            // Connection status
            _connectionStatus = new Label
            {
                Text = "Connecting to API...",
                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 110)
            };

            Controls.Add(_connectionStatus);

            // Sensor cards
            TableLayoutPanel cards =
                new TableLayoutPanel
                {
                    Location = new Point(30, 150),
                    Width = 1120,
                    Height = 150,
                    ColumnCount = 3,
                    RowCount = 1,
                    ColumnStyles =
                    {
                        new ColumnStyle(
                            SizeType.Percent,
                            33.33f),

                        new ColumnStyle(
                            SizeType.Percent,
                            33.33f),

                        new ColumnStyle(
                            SizeType.Percent,
                            33.34f)
                    },
                    Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Left |
                        AnchorStyles.Right
                };

            Panel temperatureCard =
                CreateSensorCard(
                    "TEMPERATURE",
                    "TEMP-001",
                    out _temperatureValue);

            Panel powerCard =
                CreateSensorCard(
                    "POWER CONSUMPTION",
                    "POWER-001",
                    out _powerValue);

            Panel actuatorCard =
                CreateSensorCard(
                    "ACTUATOR",
                    "ACT-001",
                    out _actuatorValue);

            cards.Controls.Add(
                temperatureCard,
                0,
                0);

            cards.Controls.Add(
                powerCard,
                1,
                0);

            cards.Controls.Add(
                actuatorCard,
                2,
                0);

            Controls.Add(cards);

            // Alert section
            _alertLabel = new Label
            {
                Text = "SYSTEM STATUS: Monitoring telemetry",
                Font = new Font(
                    "Segoe UI",
                    12,
                    FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(30, 325),
                Width = 1120,
                Height = 45,
                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right
            };

            Controls.Add(_alertLabel);

            // Telemetry simulation section
            Panel simulationPanel = new Panel
            {
                Location = new Point(30, 380),
                Width = 1120,
                Height = 80,
                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right
            };

            Label simulationTitle = new Label
            {
                Text = "Telemetry Simulation",
                Font = new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            _normalTemperatureButton = new Button
            {
                Text = "Normal Temperature",
                Location = new Point(0, 30),
                Width = 180,
                Height = 35
            };

            _anomalyTemperatureButton = new Button
            {
                Text = "Temperature Anomaly",
                Location = new Point(190, 30),
                Width = 180,
                Height = 35
            };

            _powerButton = new Button
            {
                Text = "Power Reading",
                Location = new Point(380, 30),
                Width = 150,
                Height = 35
            };

            _switchButton = new Button
            {
                Text = "Switch Trigger",
                Location = new Point(540, 30),
                Width = 150,
                Height = 35
            };

            _disconnectButton = new Button
            {
                Text = "Simulate Disconnect",
                Location = new Point(700, 30),
                Width = 170,
                Height = 35
            };

            _normalTemperatureButton.Click +=
                NormalTemperatureButton_Click;

            _anomalyTemperatureButton.Click +=
                AnomalyTemperatureButton_Click;

            _powerButton.Click +=
                PowerButton_Click;

            _switchButton.Click +=
                SwitchButton_Click;

            _disconnectButton.Click +=
                DisconnectButton_Click;

            simulationPanel.Controls.Add(simulationTitle);
            simulationPanel.Controls.Add(
                _normalTemperatureButton);

            simulationPanel.Controls.Add(
                _anomalyTemperatureButton);

            simulationPanel.Controls.Add(
                _powerButton);

            simulationPanel.Controls.Add(
                _switchButton);

            simulationPanel.Controls.Add(
                _disconnectButton);

            Controls.Add(simulationPanel);

            // File attachment section
            Panel attachmentPanel = new Panel
            {
                Location = new Point(30, 470),
                Width = 1120,
                Height = 90,
                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right
            };

            Label attachmentTitle = new Label
            {
                Text = "Sensor File Attachments",
                Font = new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            _sensorSelector = new ComboBox
            {
                Location = new Point(0, 30),
                Width = 180,
                Height = 30,
                DropDownStyle =
                    ComboBoxStyle.DropDownList
            };

            _attachmentTypeSelector = new ComboBox
            {
                Location = new Point(190, 30),
                Width = 180,
                Height = 30,
                DropDownStyle =
                    ComboBoxStyle.DropDownList
            };

            _attachmentTypeSelector.Items.AddRange(
                new object[]
                {
                    "Hardware Log",
                    "Configuration",
                    "Deployment Photo"
                });

            _attachmentTypeSelector.SelectedIndex = 0;

            _attachFileButton = new Button
            {
                Text = "Attach File",
                Location = new Point(380, 30),
                Width = 140,
                Height = 30
            };

            _attachFileButton.Click +=
                AttachFileButton_Click;

            attachmentPanel.Controls.Add(
                attachmentTitle);

            attachmentPanel.Controls.Add(
                _sensorSelector);

            attachmentPanel.Controls.Add(
                _attachmentTypeSelector);

            attachmentPanel.Controls.Add(
                _attachFileButton);

            Controls.Add(attachmentPanel);

            // Telemetry history
            Label historyTitle = new Label
            {
                Text = "Recent Telemetry",
                Font = new Font(
                    "Segoe UI",
                    16,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 520)
            };

            Controls.Add(historyTitle);

            _telemetryList = new ListBox
            {
                Location = new Point(30, 580),
                Width = 1120,
                Height = 120,
                Font = new Font(
                    "Consolas",
                    10),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 24,
                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right |
                    AnchorStyles.Bottom
            };

            _telemetryList.DrawItem += TelemetryList_DrawItem;

            Controls.Add(_telemetryList);
        }

        private Panel CreateSensorCard(
            string title,
            string sensorId,
            out Label valueLabel)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(8),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 15)
            };

            Label sensorLabel = new Label
            {
                Text = sensorId,
                Font = new Font(
                    "Segoe UI",
                    9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, 45)
            };

            valueLabel = new Label
            {
                Text = "--",
                Font = new Font(
                    "Segoe UI",
                    22,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 75)
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(sensorLabel);
            card.Controls.Add(valueLabel);

            return card;
        }

        private async Task LoadTelemetryAsync()
        {
            try
            {
                List<TelemetryRecord> records =
                    await _apiClient.GetTelemetryAsync();

                List<Sensor> sensors =
                    await _apiClient.GetSensorsAsync();

                PopulateSensorSelector(sensors);

                DisplayTelemetry(records);

                _connectionStatus.Text =
                    $"API Connected | " +
                    $"{records.Count} telemetry records loaded";

                _connectionStatus.ForeColor =
                    Color.DarkGreen;

                UpdateSensorCards(
                    records,
                    sensors);
            }
            catch (Exception ex)
            {
                _connectionStatus.Text =
                    "API Connection Failed";

                _connectionStatus.ForeColor =
                    Color.DarkRed;

                MessageBox.Show(
                    $"Unable to retrieve telemetry.\n\n" +
                    ex.Message,
                    "Smart-X API Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DisplayTelemetry(
            List<TelemetryRecord> records)
        {
            _telemetryList.Items.Clear();

            foreach (TelemetryRecord record in records)
            {
                _telemetryList.Items.Add(record);
            }

            _telemetryList.Invalidate();
        }

        private void TelemetryList_DrawItem(
            object? sender,
            DrawItemEventArgs e)
        {
            if (e.Index < 0 ||
                e.Index >= _telemetryList.Items.Count)
            {
                return;
            }

            if (_telemetryList.Items[e.Index]
                is not TelemetryRecord record)
            {
                return;
            }

            e.DrawBackground();

            Color textColor = Color.Black;
            string statusText = record.Status;

            if (record.IsAnomaly)
            {
                textColor = Color.DarkRed;
                statusText = "⚠ ANOMALY";
            }
            else if (record.IsDisconnected)
            {
                textColor = Color.DarkOrange;
                statusText = "⚠ DISCONNECTED";
            }

            string displayText =
                $"{record.Timestamp:HH:mm:ss} | " +
                $"{record.DeviceId,-10} | " +
                $"{record.NumericValue,-10} | " +
                $"{record.Unit,-8} | " +
                $"{statusText}";

            using Brush textBrush =
                new SolidBrush(textColor);

            e.Graphics.DrawString(
                displayText,
                e.Font,
                textBrush,
                e.Bounds.Left + 4,
                e.Bounds.Top + 4);

            e.DrawFocusRectangle();
        }

        private void UpdateSensorCards(
            List<TelemetryRecord> records,
            List<Sensor> sensors)
        {
            TelemetryRecord? temperature =
                records.LastOrDefault(
                    r => r.DeviceId == "TEMP-001");

            TelemetryRecord? power =
                records.LastOrDefault(
                    r => r.DeviceId == "POWER-001");

            TelemetryRecord? actuator =
                records.LastOrDefault(
                    r => r.DeviceId == "ACT-001");

            Sensor? actuatorSensor =
                sensors.FirstOrDefault(
                    s => s.UniqueIdentifier == "ACT-001");

            if (temperature != null)
            {
                _temperatureValue.Text =
                    $"{temperature.NumericValue} " +
                    $"{temperature.Unit}";
            }

            if (power != null)
            {
                _powerValue.Text =
                    $"{power.NumericValue} " +
                    $"{power.Unit}";
            }

            if (actuatorSensor != null &&
                !actuatorSensor.IsConnected)
            {
                _actuatorValue.Text =
                    "DISCONNECTED";

                _actuatorValue.ForeColor =
                    Color.DarkOrange;
            }
            else if (actuator != null)
            {
                _actuatorValue.Text =
                    actuator.NumericValue.ToString();

                _actuatorValue.ForeColor =
                    Color.Black;
            }

            CheckForAlerts(
                records,
                sensors);
        }

        private void CheckForAlerts(
            List<TelemetryRecord> records,
            List<Sensor> sensors)
        {
            bool hasAnomaly =
                records.Any(r => r.IsAnomaly);

            bool hasDisconnectedSensor =
                sensors.Any(s => !s.IsConnected);

            if (hasAnomaly &&
                hasDisconnectedSensor)
            {
                _alertLabel.Text =
                    "⚠ ALERT: Abnormal telemetry detected | " +
                    "Sensor disconnected";

                _alertLabel.ForeColor =
                    Color.DarkRed;
            }
            else if (hasAnomaly)
            {
                _alertLabel.Text =
                    "⚠ ALERT: Abnormal telemetry detected";

                _alertLabel.ForeColor =
                    Color.DarkRed;
            }
            else if (hasDisconnectedSensor)
            {
                _alertLabel.Text =
                    "⚠ ALERT: Sensor disconnected";

                _alertLabel.ForeColor =
                    Color.DarkOrange;
            }
            else
            {
                _alertLabel.Text =
                    "SYSTEM STATUS: " +
                    "All telemetry readings normal";

                _alertLabel.ForeColor =
                    Color.DarkGreen;
            }
        }

        private async void NormalTemperatureButton_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                TelemetryPacket<float> packet =
                    new TelemetryPacket<float>(
                        "TEMP-001",
                        25.5f,
                        "float",
                        "°C");

                await _apiClient.SendTemperatureAsync(
                    packet);

                await RefreshTelemetryAsync();
            }
            catch (Exception ex)
            {
                ShowTelemetryError(ex);
            }
        }

        private async void AnomalyTemperatureButton_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                TelemetryPacket<float> packet =
                    new TelemetryPacket<float>(
                        "TEMP-001",
                        75.5f,
                        "float",
                        "°C");

                await _apiClient.SendTemperatureAsync(
                    packet);

                await RefreshTelemetryAsync();
            }
            catch (Exception ex)
            {
                ShowTelemetryError(ex);
            }
        }

        private async void PowerButton_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                TelemetryPacket<int> packet =
                    new TelemetryPacket<int>(
                        "POWER-001",
                        1250,
                        "int",
                        "W");

                await _apiClient.SendPowerAsync(
                    packet);

                await RefreshTelemetryAsync();
            }
            catch (Exception ex)
            {
                ShowTelemetryError(ex);
            }
        }

        private async void SwitchButton_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                TelemetryPacket<bool> packet =
                    new TelemetryPacket<bool>(
                        "ACT-001",
                        true,
                        "bool",
                        "state");

                await _apiClient.SendSwitchAsync(
                    packet);

                await RefreshTelemetryAsync();
            }
            catch (Exception ex)
            {
                ShowTelemetryError(ex);
            }
        }

        private async void DisconnectButton_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                _disconnectButton.Enabled = false;
                _disconnectButton.Text =
                    "Disconnecting...";

                await _apiClient.SimulateDisconnectAsync(
                    "ACT-001");

                await RefreshTelemetryAsync();

                MessageBox.Show(
                    "ACT-001 has been marked as disconnected.",
                    "Smart-X Sensor Status",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to simulate sensor disconnect.\n\n" +
                    ex.Message,
                    "Smart-X Sensor Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _disconnectButton.Enabled = true;
                _disconnectButton.Text =
                    "Simulate Disconnect";
            }
        }

        private async Task RefreshTelemetryAsync()
        {
            List<TelemetryRecord> records =
                await _apiClient.GetTelemetryAsync();

            List<Sensor> sensors =
                await _apiClient.GetSensorsAsync();

            PopulateSensorSelector(sensors);

            DisplayTelemetry(records);

            _connectionStatus.Text =
                $"API Connected | " +
                $"{records.Count} telemetry records loaded";

            _connectionStatus.ForeColor =
                Color.DarkGreen;

            UpdateSensorCards(
                records,
                sensors);
        }

        private async void AttachFileButton_Click(
            object? sender,
            EventArgs e)
        {
            if (_sensorSelector.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a sensor.",
                    "Smart-X Attachment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (_attachmentTypeSelector.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select an attachment type.",
                    "Smart-X Attachment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            using OpenFileDialog dialog =
                new OpenFileDialog
                {
                    Title = "Select Sensor Attachment",
                    Filter =
                        "Supported files|*.txt;*.log;*.json;*.xml;*.csv;*.jpg;*.jpeg;*.png;*.pdf|" +
                        "All files|*.*",
                    Multiselect = false
                };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            FileInfo fileInfo =
                new FileInfo(dialog.FileName);

            const long maximumFileSize =
                50_000_000;

            if (fileInfo.Length > maximumFileSize)
            {
                MessageBox.Show(
                    "The selected file is larger than the 50 MB limit.",
                    "Smart-X Attachment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string deviceId =
                _sensorSelector.SelectedItem.ToString()!;

            string attachmentType =
                _attachmentTypeSelector.SelectedItem
                    .ToString()!;

            try
            {
                _attachFileButton.Enabled = false;
                _attachFileButton.Text =
                    "Uploading...";

                SensorAttachment? attachment =
                    await _apiClient.UploadAttachmentAsync(
                        deviceId,
                        dialog.FileName,
                        attachmentType);

                if (attachment != null)
                {
                    MessageBox.Show(
                        $"File attached successfully.\n\n" +
                        $"Sensor: {attachment.DeviceId}\n" +
                        $"File: {attachment.FileName}\n" +
                        $"Type: {attachment.AttachmentType}",
                        "Smart-X Attachment",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to upload the attachment.\n\n" +
                    ex.Message,
                    "Smart-X Attachment Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _attachFileButton.Enabled = true;
                _attachFileButton.Text =
                    "Attach File";
            }
        }

        private void ShowTelemetryError(
            Exception ex)
        {
            MessageBox.Show(
                $"Unable to send telemetry.\n\n" +
                ex.Message,
                "Smart-X Telemetry Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}