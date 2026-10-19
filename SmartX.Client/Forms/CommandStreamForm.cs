using SmartX.Client.Services;
using SmartX.Shared.Models;

namespace SmartX.Client.Forms
{
    public partial class CommandStreamForm : Form
    {
        private readonly CommandQueueService _commandQueueService = new();

        private readonly CommandPriorityService _commandPriorityService = new();

        private readonly CommandHistoryService _commandHistoryService = new();

        private readonly ApiClient _apiClient = new();

        // Stores commands displayed in the Command Stream interface.
        private readonly List<SmartCommand> _commandHistory = new();

        private Label _statusLabel = null!;
        private Label _queueCountLabel = null!;
        private ListBox _commandHistoryList = null!;

        private ComboBox _sensorSelector = null!;
        private ComboBox _commandTypeSelector = null!;
        private ComboBox _prioritySelector = null!;

        private TextBox _descriptionTextBox = null!;

        private Button _addCommandButton = null!;
        private Button _processCommandButton = null!;
        private Button _processPriorityButton = null!;
        private Button _undoButton = null!;
        private Button _refreshButton = null!;

        public CommandStreamForm()
        {
            InitializeComponent();

            BuildInterface();

            RefreshCommandList();
        }

        private void BuildInterface()
        {
            Text = "Smart-X | Command Stream";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1100;
            Height = 750;
            MinimumSize = new Size(900, 650);

            BackColor = Color.FromArgb(245, 247, 250);

            // =====================================================
            // HEADER
            // =====================================================

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(30, 41, 59)
            };

            Label title = new Label
            {
                Text = "COMMAND STREAM",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 15)
            };

            Label subtitle = new Label
            {
                Text = "Command processing, prioritisation and history",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(32, 62)
            };

            header.Controls.Add(title);
            header.Controls.Add(subtitle);

            Controls.Add(header);

            // =====================================================
            // STATUS
            // =====================================================

            _statusLabel = new Label
            {
                Text = "Command Stream ready.",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 115),
                ForeColor = Color.DarkGreen
            };

            Controls.Add(_statusLabel);

            // =====================================================
            // COMMAND CREATION
            // =====================================================

            Label createTitle = new Label
            {
                Text = "Create Command",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 155)
            };

            Controls.Add(createTitle);

            Label sensorLabel = new Label
            {
                Text = "Target Device",
                AutoSize = true,
                Location = new Point(30, 200)
            };

            Controls.Add(sensorLabel);

            _sensorSelector = new ComboBox
            {
                Location = new Point(30, 225),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _sensorSelector.Items.AddRange(new object[]
            {
                "TEMP-001",
                "POWER-001",
                "ACT-001"
            });

            _sensorSelector.SelectedIndex = 0;

            Controls.Add(_sensorSelector);

            Label commandTypeLabel = new Label
            {
                Text = "Command Type",
                AutoSize = true,
                Location = new Point(250, 200)
            };

            Controls.Add(commandTypeLabel);

            _commandTypeSelector = new ComboBox
            {
                Location = new Point(250, 225),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _commandTypeSelector.Items.AddRange(new object[]
            {
                "Inspect",
                "SetThreshold",
                "RequestTelemetry",
                "Reset"
            });

            _commandTypeSelector.SelectedIndex = 0;

            Controls.Add(_commandTypeSelector);

            Label priorityLabel = new Label
            {
                Text = "Priority",
                AutoSize = true,
                Location = new Point(470, 200)
            };

            Controls.Add(priorityLabel);

            _prioritySelector = new ComboBox
            {
                Location = new Point(470, 225),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _prioritySelector.Items.AddRange(new object[]
            {
                "Low",
                "Normal",
                "High",
                "Critical"
            });

            _prioritySelector.SelectedItem = "Normal";

            Controls.Add(_prioritySelector);

            Label descriptionLabel = new Label
            {
                Text = "Description",
                AutoSize = true,
                Location = new Point(640, 200)
            };

            Controls.Add(descriptionLabel);

            _descriptionTextBox = new TextBox
            {
                Location = new Point(640, 225),
                Width = 400
            };

            Controls.Add(_descriptionTextBox);

            _addCommandButton = new Button
            {
                Text = "Add Command",
                Location = new Point(30, 270),
                Width = 150,
                Height = 35
            };

            _addCommandButton.Click += AddCommandButton_Click;

            Controls.Add(_addCommandButton);

            // =====================================================
            // QUEUE STATUS
            // =====================================================

            _queueCountLabel = new Label
            {
                Text = "Pending commands: 0",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 325)
            };

            Controls.Add(_queueCountLabel);

            // =====================================================
            // COMMAND HISTORY
            // =====================================================

            Label historyTitle = new Label
            {
                Text = "Command History",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 360)
            };

            Controls.Add(historyTitle);

            _commandHistoryList = new ListBox
            {
                Location = new Point(30, 400),
                Width = 1010,
                Height = 200,
                Anchor = AnchorStyles.Top
                    | AnchorStyles.Bottom
                    | AnchorStyles.Left
                    | AnchorStyles.Right,
                Font = new Font("Consolas", 9)
            };

            Controls.Add(_commandHistoryList);

            // =====================================================
            // PROCESS FIFO COMMAND
            // =====================================================

            _processCommandButton = new Button
            {
                Text = "Process Next Command",
                Location = new Point(30, 620),
                Width = 200,
                Height = 35,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            _processCommandButton.Click += ProcessCommandButton_Click;

            Controls.Add(_processCommandButton);

            // =====================================================
            // PROCESS PRIORITY COMMAND
            // =====================================================

            _processPriorityButton = new Button
            {
                Text = "Process Priority Command",
                Location = new Point(245, 620),
                Width = 210,
                Height = 35,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            _processPriorityButton.Click += ProcessPriorityButton_Click;

            Controls.Add(_processPriorityButton);

            // =====================================================
            // UNDO
            // =====================================================

            _undoButton = new Button
            {
                Text = "Undo Last Command",
                Location = new Point(470, 620),
                Width = 180,
                Height = 35,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Enabled = false
            };

            _undoButton.Click += UndoButton_Click;

            Controls.Add(_undoButton);

            // =====================================================
            // REFRESH
            // =====================================================

            _refreshButton = new Button
            {
                Text = "Refresh",
                Location = new Point(665, 620),
                Width = 120,
                Height = 35,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            _refreshButton.Click += (_, _) =>
            {
                RefreshCommandList();

                _statusLabel.Text = "Command list refreshed.";
            };

            Controls.Add(_refreshButton);
        }

        // =========================================================
        // ADD COMMAND
        // =========================================================

        private void AddCommandButton_Click(object? sender, EventArgs e)
        {
            string deviceId =
                _sensorSelector.SelectedItem?.ToString()
                ?? string.Empty;

            string commandType =
                _commandTypeSelector.SelectedItem?.ToString()
                ?? string.Empty;

            string priority =
                _prioritySelector.SelectedItem?.ToString()
                ?? string.Empty;

            string description = _descriptionTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show(
                    "Please enter a command description.",
                    "Command Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                _descriptionTextBox.Focus();

                return;
            }

            SmartCommand command;

            if (priority is "High" or "Critical")
            {
                command = _commandPriorityService.Enqueue(
                    deviceId,
                    commandType,
                    description,
                    priority,
                    isUndoable: commandType is "SetThreshold" or "Reset");

                _statusLabel.Text =
                    $"Command #{command.Id} added to the priority queue.";
            }
            else
            {
                command = _commandQueueService.Enqueue(
                    deviceId,
                    commandType,
                    description,
                    priority,
                    isUndoable: commandType is "SetThreshold" or "Reset");

                _statusLabel.Text =
                    $"Command #{command.Id} added to the FIFO queue.";
            }

            _commandHistory.Add(command);

            _descriptionTextBox.Clear();

            RefreshCommandList();
        }

        // =========================================================
        // PROCESS FIFO COMMAND AND RECORD IT IN THE API
        // =========================================================

        private async void ProcessCommandButton_Click(
            object? sender,
            EventArgs e)
        {
            if (_processCommandButton.Enabled == false)
            {
                return;
            }

            SmartCommand? command = _commandQueueService.Dequeue();

            if (command == null)
            {
                _statusLabel.Text = "There are no pending commands.";

                RefreshCommandList();

                return;
            }

            _processCommandButton.Enabled = false;

            try
            {
                // Mark the simulated command as processed.
                command.Status = "Processed";
                command.ProcessedAt = DateTime.UtcNow;
                command.WasSuccessful = true;

                // Preserve local history and undo functionality.
                _commandHistoryService.RecordProcessedCommand(command);

                // Record the processed command on the API server.
                await _apiClient.RecordProcessedCommandAsync(command);

                _statusLabel.Text =
                    $"Command #{command.Id} processed and recorded for {command.DeviceId}.";
            }
            catch (Exception ex)
            {
                _statusLabel.Text =
                    $"Command history could not be confirmed: {ex.Message}";

                MessageBox.Show(
                    "The command was processed locally, but the API " +
                    "history could not be confirmed.\n\n" +
                    ex.Message,
                    "Command History Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                RefreshCommandList();

                _processCommandButton.Enabled =
                    _commandQueueService.HasPendingCommands;
            }
        }

        // =========================================================
        // PROCESS PRIORITY COMMAND AND RECORD IT IN THE API
        // =========================================================

        private async void ProcessPriorityButton_Click(
            object? sender,
            EventArgs e)
        {
            if (_processPriorityButton.Enabled == false)
            {
                return;
            }

            SmartCommand? command = _commandPriorityService.Dequeue();

            if (command == null)
            {
                _statusLabel.Text =
                    "There are no pending priority commands.";

                RefreshCommandList();

                return;
            }

            _processPriorityButton.Enabled = false;

            try
            {
                // Mark the simulated command as processed.
                command.Status = "Processed";
                command.ProcessedAt = DateTime.UtcNow;
                command.WasSuccessful = true;

                // Preserve local history and undo functionality.
                _commandHistoryService.RecordProcessedCommand(command);

                // Record the processed command on the API server.
                await _apiClient.RecordProcessedCommandAsync(command);

                _statusLabel.Text =
                    $"Priority command #{command.Id} processed and recorded for {command.DeviceId}.";
            }
            catch (Exception ex)
            {
                _statusLabel.Text =
                    $"Priority command history could not be confirmed: {ex.Message}";

                MessageBox.Show(
                    "The priority command was processed locally, but the API " +
                    "history could not be confirmed.\n\n" +
                    ex.Message,
                    "Command History Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                RefreshCommandList();

                _processPriorityButton.Enabled =
                    _commandPriorityService.HasPendingCommands;
            }
        }

        // =========================================================
        // UNDO LAST COMMAND
        // =========================================================

        private void UndoButton_Click(object? sender, EventArgs e)
        {
            SmartCommand? command =
                _commandHistoryService.UndoLastCommand();

            if (command == null)
            {
                _statusLabel.Text =
                    "There are no commands available to undo.";

                return;
            }

            command.Status = "Undone";

            RefreshCommandList();

            _statusLabel.Text =
                $"Command #{command.Id} has been undone.";
        }

        // =========================================================
        // REFRESH COMMAND LIST
        // =========================================================

        private void RefreshCommandList()
        {
            _commandHistoryList.Items.Clear();

            foreach (SmartCommand command in _commandHistory)
            {
                _commandHistoryList.Items.Add(
                    $"#{command.Id} | {command.DeviceId,-10} | " +
                    $"{command.CommandType,-18} | " +
                    $"{command.Priority,-8} | " +
                    $"{command.Status}");
            }

            int fifoCount = _commandQueueService.Count;

            int priorityCount = _commandPriorityService.Count;

            _queueCountLabel.Text =
                $"Pending commands: {fifoCount + priorityCount} " +
                $"(FIFO: {fifoCount}, Priority: {priorityCount})";

            _processCommandButton.Enabled =
                _commandQueueService.HasPendingCommands;

            _processPriorityButton.Enabled =
                _commandPriorityService.HasPendingCommands;

            _undoButton.Enabled =
                _commandHistoryService.CanUndo;
        }
    }
}