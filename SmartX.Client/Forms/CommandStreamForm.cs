namespace SmartX.Client.Forms
{
    public partial class CommandStreamForm : Form
    {
        private Label _statusLabel = null!;
        private ListBox _commandHistoryList = null!;
        private Button _refreshButton = null!;
        private Button _undoButton = null!;

        public CommandStreamForm()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Smart-X | Command Stream";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1100;
            Height = 750;
            MinimumSize = new Size(900, 600);

            BackColor = Color.FromArgb(245, 247, 250);

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

            _statusLabel = new Label
            {
                Text = "Command Stream is ready for implementation.",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 125),
                ForeColor = Color.DarkGreen
            };

            Controls.Add(_statusLabel);

            Label historyTitle = new Label
            {
                Text = "Command History",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 175)
            };

            Controls.Add(historyTitle);

            _commandHistoryList = new ListBox
            {
                Location = new Point(30, 220),
                Width = 1000,
                Height = 350,
                Anchor = AnchorStyles.Top
                    | AnchorStyles.Bottom
                    | AnchorStyles.Left
                    | AnchorStyles.Right,
                Font = new Font("Consolas", 10)
            };

            Controls.Add(_commandHistoryList);

            _refreshButton = new Button
            {
                Text = "Refresh",
                Location = new Point(30, 600),
                Width = 140,
                Height = 40,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            _refreshButton.Click += (_, _) =>
            {
                _statusLabel.Text =
                    "Command Stream interface refreshed.";
            };

            Controls.Add(_refreshButton);

            _undoButton = new Button
            {
                Text = "Undo Last Command",
                Location = new Point(185, 600),
                Width = 190,
                Height = 40,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Enabled = false
            };

            Controls.Add(_undoButton);
        }
    }
}