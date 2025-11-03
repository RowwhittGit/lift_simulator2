using System;
using System.Drawing;
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

        #region Form Load & Initialization

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set form background to light gray
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Style the dataGridView
            dataGridView1.BackgroundColor = Color.FromArgb(255, 255, 255);
            dataGridView1.GridColor = Color.FromArgb(220, 220, 220);
            dataGridView1.ForeColor = Color.FromArgb(60, 60, 60);
            dataGridView1.RowHeadersVisible = false;

            // Header styling
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(230, 230, 230),
                ForeColor = Color.FromArgb(50, 50, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
            };
            dataGridView1.ColumnHeadersDefaultCellStyle = headerStyle;
            dataGridView1.ColumnHeadersHeight = 30;

            // Row styling
            DataGridViewCellStyle rowStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(255, 255, 255),
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = new Font("Segoe UI", 9),
            };
            dataGridView1.DefaultCellStyle = rowStyle;

            // Alternating row colors
            DataGridViewCellStyle alternateRowStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 248, 250),
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = new Font("Segoe UI", 9),
            };
            dataGridView1.AlternatingRowsDefaultCellStyle = alternateRowStyle;

            // Update floor display label color
            label_floor_display.BackColor = Color.FromArgb(100, 150, 200);
            label_floor_display.ForeColor = Color.White;

            InitializeDataGrid();
            InitializeTimers();
            InitializeBackgroundWorker();
            LoadPastEvents();
        }
        private void InitializeDataGrid()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
        }

        private void InitializeTimers()
        {
            // Lift Animation Timer
            liftAnimationTimer = new System.Windows.Forms.Timer();
            liftAnimationTimer.Interval = 25; // milliseconds
            liftAnimationTimer.Tick += LiftAnimationTimer_Tick;

            // Door Animation Timer
            doorAnimationTimer = new System.Windows.Forms.Timer();
            doorAnimationTimer.Interval = 18; // milliseconds
            doorAnimationTimer.Tick += DoorAnimationTimer_Tick;
        }

        private void InitializeBackgroundWorker()
        {
            liftWorker.DoWork += LiftWorker_DoWork;
            liftWorker.RunWorkerCompleted += LiftWorker_RunWorkerCompleted;
        }

        #endregion

        #region Status Updates

        private void UpdateStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatus(status)));
                return;
            }

            LoadPastEvents();
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

        #endregion

        #region Lift Animation (Timer-based)

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

        #endregion

        #region Door Animation (Timer-based)

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
            int doorTargetLeftX = 30;   // How far left door should move left
            int doorTargetRightX = 194; // How far right door should move right

            if (currentLeftDoor.Left > doorTargetLeftX)
            {
                currentLeftDoor.Left -= 2;
                currentRightDoor.Left += 2;
            }
            else
            {
                currentLeftDoor.Left = doorTargetLeftX;
                currentRightDoor.Left = doorTargetRightX;
                doorAnimationTimer.Stop();
            }
        }

        private void AnimateDoorClosing()
        {
            if (_liftController.CurrentFloor == 0)
            {
                currentLeftDoor = ground_lift_left_door_btn;
                currentRightDoor = ground_lift_right_door_btn;
                doorTargetLeftX = 80;
                doorTargetRightX = 144;
            }
            else
            {
                currentLeftDoor = first_lift_left_door_btn;
                currentRightDoor = first_lift_right_door_btn;
                doorTargetLeftX = 80;
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

        #endregion

        #region Database Operations (BackgroundWorker)

        private void LoadPastEvents()
        {
            // Only load if BackgroundWorker is not already busy
            if (!liftWorker.IsBusy)
            {
                liftWorker.RunWorkerAsync();
            }
        }

        private void LiftWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                // This runs on BACKGROUND THREAD - database operations here
                var events = _db.GetAllEvents();
                e.Result = events;
            }
            catch (Exception ex)
            {
                e.Cancel = true;
            }
        }

        private void LiftWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    MessageBox.Show("Database operation was cancelled.");
                    return;
                }

                if (e.Error != null)
                {
                    MessageBox.Show($"Error loading events: {e.Error.Message}");
                    return;
                }

                var events = (System.Data.DataTable)e.Result;

                if (events != null && events.Rows.Count > 0)
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = events;
                    ResizeDataGridColumns();

                    if (dataGridView1.Rows.Count > 0)
                    {
                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating grid: {ex.Message}");
            }
        }

        private void ResizeDataGridColumns()
        {
            if (dataGridView1.Columns.Count == 0)
                return;

            if (dataGridView1.Columns.Contains("Id"))
            {
                dataGridView1.Columns["Id"].Visible = false;
            }

            int totalWidth = dataGridView1.Width;
            int eventTimeWidth = 150;

            if (dataGridView1.Columns.Contains("EventTime"))
            {
                dataGridView1.Columns["EventTime"].Width = eventTimeWidth;
            }

            int messageWidth = totalWidth - eventTimeWidth - 30;

            if (dataGridView1.Columns.Contains("Message"))
            {
                dataGridView1.Columns["Message"].Width = messageWidth;
            }
        }
        #endregion

        #region Button Click Handlers

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

        #endregion


        private void delete_logs_btn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete all logs? This cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _db.DeleteAllEvents();
                    MessageBox.Show("All logs have been deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPastEvents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}