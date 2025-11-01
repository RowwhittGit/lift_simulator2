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
        private System.Windows.Forms.Timer liftAnimationTimer;
        private System.Windows.Forms.Timer doorAnimationTimer;

        private int liftAnimationFrame = 0;
        private int doorAnimationFrame = 0;
        private int targetLiftY = 0;
        private int currentLiftY = 0;
        private int liftStep = 0;

        private PictureBox currentLeftDoor;
        private PictureBox currentRightDoor;
        private int doorTargetLeftX = 0;
        private int doorTargetRightX = 0;
        private bool isDoorOpening = false;

        public Form1()
        {
            InitializeComponent();

            _db = new DbConnection();
            _liftController = new LiftController(_db);

            _liftController.OnStatusChanged += status => UpdateStatus(status);
            _liftController.OnFloorChanged += floor => UpdateLiftPosition(floor);
            _liftController.OnDoorStateChanged += isOpen => AnimateDoor(isOpen);
        }

        private void UpdateStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatus(status)));
                return;
            }

            Task.Run(() => LoadPastEvents());
        }

        private void UpdateLiftPosition(int floor)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateLiftPosition(floor)));
                return;
            }

            // Update the floor display label
            label_floor_display.Text = floor.ToString();

            if (floor == 0)
            {
                AnimateLiftMovement(540);
            }
            else if (floor == 1)
            {
                AnimateLiftMovement(55);
            }
        }

        private void AnimateLiftMovement(int targetY)
        {
            targetLiftY = targetY;
            currentLiftY = lift_movable.Top;
            liftStep = (targetY > currentLiftY) ? 7 : -5;
            liftAnimationFrame = 0;

            liftAnimationTimer.Start();
        }

        private void LiftAnimationTimer_Tick(object sender, EventArgs e)
        {
            // Check if we've reached the target
            if ((liftStep > 0 && lift_movable.Top >= targetLiftY) ||
                (liftStep < 0 && lift_movable.Top <= targetLiftY))
            {
                lift_movable.Top = targetLiftY;
                liftAnimationTimer.Stop();
                return;
            }

            // Move lift one step
            lift_movable.Top += liftStep;
            liftAnimationFrame++;
        }

        private void AnimateDoor(bool isOpen)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AnimateDoor(isOpen)));
                return;
            }

            if (isOpen)
            {
                AnimateDoorOpening();
            }
            else
            {
                AnimateDoorClosing();
            }
        }

        private void AnimateDoorOpening()
        {
            if (_liftController.CurrentFloor == 0)
            {
                currentLeftDoor = ground_lift_left_door_btn;
                currentRightDoor = ground_lift_right_door_btn;
            }
            else
            {
                currentLeftDoor = first_lift_left_door_btn;
                currentRightDoor = first_lift_right_door_btn;
            }

            doorAnimationFrame = 0;
            isDoorOpening = true;

            doorAnimationTimer.Start();
        }

        private void AnimateDoorFrame_Opening()
        {
            if (doorAnimationFrame < 18)
            {
                currentLeftDoor.Left -= 2;
                currentRightDoor.Left += 2;
                doorAnimationFrame++;
            }
            else
            {
                doorAnimationTimer.Stop();
            }
        }

        private void AnimateDoorClosing()
        {
            if (_liftController.CurrentFloor == 0)
            {
                currentLeftDoor = ground_lift_left_door_btn;
                currentRightDoor = ground_lift_right_door_btn;
                doorTargetLeftX = 79;
                doorTargetRightX = 144;
            }
            else
            {
                currentLeftDoor = first_lift_left_door_btn;
                currentRightDoor = first_lift_right_door_btn;
                doorTargetLeftX = 79;
                doorTargetRightX = 144;
            }

            doorAnimationFrame = 0;
            isDoorOpening = false;

            doorAnimationTimer.Start();
        }

        private void AnimateDoorFrame_Closing()
        {
            if (currentLeftDoor.Left < doorTargetLeftX)
            {
                currentLeftDoor.Left += 2;
                currentRightDoor.Left -= 2;
                doorAnimationFrame++;
            }
            else
            {
                currentLeftDoor.Left = doorTargetLeftX;
                currentRightDoor.Left = doorTargetRightX;
                doorAnimationTimer.Stop();
            }
        }

        private void DoorAnimationTimer_Tick(object sender, EventArgs e)
        {
            if (isDoorOpening)
            {
                AnimateDoorFrame_Opening();
            }
            else
            {
                AnimateDoorFrame_Closing();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            LoadPastEvents();

            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["EventTime"].Width = 100;
                dataGridView1.Columns["Message"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns["Id"].Visible = false;
            }

            // Initialize Lift Animation Timer
            liftAnimationTimer = new System.Windows.Forms.Timer();
            liftAnimationTimer.Interval = 25; // milliseconds
            liftAnimationTimer.Tick += LiftAnimationTimer_Tick;

            // Initialize Door Animation Timer
            doorAnimationTimer = new System.Windows.Forms.Timer();
            doorAnimationTimer.Interval = 18; // milliseconds
            doorAnimationTimer.Tick += DoorAnimationTimer_Tick;
        }

        private void LoadPastEvents()
        {
            try
            {
                var events = _db.GetAllEvents();

                Invoke(new Action(() =>
                {
                    if (events != null && events.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = events;

                        if (dataGridView1.Rows.Count > 0)
                        {
                            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
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
            _liftController.RequestDoorOpen();
        }

        private void close_lift_btn_Click(object sender, EventArgs e)
        {
            _liftController.RequestDoorClose();
        }

        private void first_btn_Click(object sender, EventArgs e)
        {
            _liftController.CallFloor(1);
        }

        private void ground_btn_Click(object sender, EventArgs e)
        {
            _liftController.CallFloor(0);
        }

        private void first_call_button_Click(object sender, EventArgs e)
        {
            _liftController.CallFloor(1);
        }

        private void ground_call_button_Click(object sender, EventArgs e)
        {
            _liftController.CallFloor(0);
        }
    }
}