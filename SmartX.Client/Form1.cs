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
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1100;
            Height = 700;
            MinimumSize = new Size(900, 600);

            BackColor = Color.FromArgb(245, 247, 250);
            Panel header = new Panel
            {
                Location = new Point(0, 0),
                Width = ClientSize.Width,
                Height = 105,
                BackColor = Color.FromArgb(30, 41, 59),
                Anchor = AnchorStyles.Top |
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
                Text = "Real-Time Sensor Data Ingestion and Monitoring",
                ForeColor = Color.LightGray,
                Font = new Font(
                    "Segoe UI",
                    10),
                AutoSize = true,
                Location = new Point(32, 60)
            };

            header.Controls.Add(title);
            header.Controls.Add(subtitle);

            Controls.Add(header);

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
                    Location = new Point(35, 200),
                    Width = 1030,
                    Height = 190,
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

            Label sensorTitle = new Label
            {
                Text = "Registered Sensors",
                Font = new Font(
                    "Segoe UI",
                    16,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(35, 425)
            };

            Controls.Add(sensorTitle);

            _sensorList = new ListBox
            {
                Location = new Point(35, 465),
                Width = 650,
                Height = 130,
                Font = new Font(
                    "Segoe UI",
                    10),
                Anchor =
                    AnchorStyles.Left |
                    AnchorStyles.Right |
                    AnchorStyles.Bottom
            };

            Controls.Add(_sensorList);

            _connectionStatus = new Label
            {
                Text = "Connecting to Smart-X API...",
                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(720, 470),
                Anchor =
                    AnchorStyles.Right |
                    AnchorStyles.Bottom
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
                        $"{connectionState}");
                }

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