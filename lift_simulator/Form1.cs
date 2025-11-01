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

        // Replace your existing UpdateLiftPosition() method with this:

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

        private async void AnimateLiftMovement(int targetY)
        {
            int currentY = lift_movable.Top;
            int step = (targetY > currentY) ? 7 : -5;

            while ((step > 0 && lift_movable.Top < targetY) ||
                   (step < 0 && lift_movable.Top > targetY))
            {
                lift_movable.Top += step;
                await Task.Delay(25);
            }

            lift_movable.Top = targetY;
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

        private async void AnimateDoorOpening()
        {
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

            for (int i = 0; i < 18; i++)
            {
                leftDoor.Left -= 2;
                rightDoor.Left += 2;
                await Task.Delay(18);
            }
        }

        private async void AnimateDoorClosing()
        {
            PictureBox leftDoor, rightDoor;
            int targetLeftX, targetRightX;

            if (_liftController.CurrentFloor == 0)
            {
                leftDoor = ground_lift_left_door_btn;
                rightDoor = ground_lift_right_door_btn;
                targetLeftX = 79;
                targetRightX = 144;
            }
            else
            {
                leftDoor = first_lift_left_door_btn;
                rightDoor = first_lift_right_door_btn;
                targetLeftX = 79;
                targetRightX = 144;
            }

            while (leftDoor.Left < targetLeftX)
            {
                leftDoor.Left += 2;
                rightDoor.Left -= 2;
                await Task.Delay(30);
            }

            leftDoor.Left = targetLeftX;
            rightDoor.Left = targetRightX;
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