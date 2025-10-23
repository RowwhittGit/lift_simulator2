using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using lift_simulator.Controllers;
using lift_simulator.Database;

namespace lift_simulator
{
    public partial class Form1 : Form
    {
        private readonly LiftController _liftController;
        private readonly DbConnection _db;

        public Form1()
        {
            InitializeComponent();

            // Initialize DB and controller
            _db = new DbConnection();
            _liftController = new LiftController(_db);

            // Subscribe to controller events for UI feedback
            _liftController.OnStatusChanged += status => UpdateStatus(status);
            _liftController.OnFloorChanged += floor => UpdateLiftPosition(floor);
            _liftController.OnDoorStateChanged += state => AnimateDoor(state);
        }

        // === UI Updates ===

        private void UpdateStatus(string status)
        {
            // Ensure we're on the UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatus(status)));
                return;
            }

            // Refresh DataGridView asynchronously so it doesn't freeze
            Task.Run(() => LoadPastEvents());
        }

        private void UpdateLiftPosition(int floor)
        {
            // Ensure we're on the UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateLiftPosition(floor)));
                return;
            }

            // Animate lift moving visually (0 = Ground, 1 = First)
            if (floor == 0)
            {
                AnimateLiftMovement(545); // Ground floor position
            }
            else if (floor == 1)
            {
                AnimateLiftMovement(120); // First floor position
            }
        }

        private async void AnimateLiftMovement(int targetY)
        {
            // Smooth animation
            int currentY = lift_movable.Top;
            int step = (targetY > currentY) ? 5 : -5;

            while ((step > 0 && lift_movable.Top < targetY) ||
                   (step < 0 && lift_movable.Top > targetY))
            {
                lift_movable.Top += step;
                await Task.Delay(20); // Smooth animation delay
            }

            lift_movable.Top = targetY; // Ensure exact position
        }

        private void AnimateDoor(string state)
        {
            // Ensure we're on the UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => AnimateDoor(state)));
                return;
            }

            if (state == "Open")
            {
                AnimateDoorOpening();
            }
            else if (state == "Closed")
            {
                AnimateDoorClosing();
            }
        }

        private async void AnimateDoorOpening()
        {
            // Determine which floor's doors to animate based on lift position
            PictureBox leftDoor, rightDoor;

            if (_liftController.CurrentFloor == 0)
            {
                leftDoor = ground_lift_left_door_btn;
                rightDoor = ground_lift_right_door_btn;
            }
            else
            {
                leftDoor = first_lift_left_door_btn;
                rightDoor = first_lift_right_door_btn;
            }

            // Save original positions
            int originalLeftX = leftDoor.Left;
            int originalRightX = rightDoor.Left;

            // Animate doors opening
            for (int i = 0; i < 30; i++)
            {
                leftDoor.Left -= 2;
                rightDoor.Left += 2;
                await Task.Delay(30);
            }
        }

        private async void AnimateDoorClosing()
        {
            // Determine which floor's doors to animate
            PictureBox leftDoor, rightDoor;
            int targetLeftX, targetRightX;

            if (_liftController.CurrentFloor == 0)
            {
                leftDoor = ground_lift_left_door_btn;
                rightDoor = ground_lift_right_door_btn;
                targetLeftX = 79;  // Original position from designer
                targetRightX = 144; // Original position from designer
            }
            else
            {
                leftDoor = first_lift_left_door_btn;
                rightDoor = first_lift_right_door_btn;
                targetLeftX = 79;
                targetRightX = 144;
            }

            // Animate doors closing back to original position
            while (leftDoor.Left < targetLeftX)
            {
                leftDoor.Left += 2;
                rightDoor.Left -= 2;
                await Task.Delay(30);
            }

            // Ensure exact position
            leftDoor.Left = targetLeftX;
            rightDoor.Left = targetRightX;
        }

        // === Event Handlers ===

        private void Form1_Load(object sender, EventArgs e)
        {
            // Setup DataGridView
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;


            // Load past events from database
            LoadPastEvents();
        }

        private void LoadPastEvents()
        {
            try
            {
                // Get all events as DataTable (on background thread)
                var events = _db.GetAllEvents();

                // Always use Invoke to switch back to UI thread
                Invoke(new Action(() =>
                {
                    if (events != null && events.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = events;

                        // Auto-scroll to latest
                        if (dataGridView1.Rows.Count > 0)
                        {
                            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
                            dataGridView1.Columns["EventTime"].Width = 100;  // Fixed width for time
                            dataGridView1.Columns["Message"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;  // Fill remaining space
                            dataGridView1.Columns["Id"].Visible = false;  // Hide Id column if you don't need it
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => MessageBox.Show($"Error loading past events: {ex.Message}")));
            }
        }

        private void open_lift_btn_Click(object sender, EventArgs e)
        {
            _liftController.OpenDoor();
        }

        private void close_lift_btn_Click(object sender, EventArgs e)
        {
            _liftController.CloseDoor();
        }

        private void first_btn_Click(object sender, EventArgs e)
        {
            // Move to first floor from inside the lift
            _liftController.MoveToFloor(1);
        }

        private void ground_btn_Click(object sender, EventArgs e)
        {
            // Move to ground floor from inside the lift
            _liftController.MoveToFloor(0);
        }

        private void first_call_button_Click(object sender, EventArgs e)
        {
            // External call from first floor - works like real lift
            _liftController.MoveToFloor(1);
        }

        private void ground_call_button_Click(object sender, EventArgs e)
        {
            // External call from ground floor - works like real lift
            _liftController.MoveToFloor(0);
        }

    }
}