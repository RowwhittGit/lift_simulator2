using lift_simulator.States;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace lift_simulator
{
    public partial class Form1 : Form
    {
        private LiftContext liftContext;
        private bool isBusy = false;

        // Door animation variables
        private bool isAnimatingDoor = false;
        private bool isDoorOpening = false;
        private int doorAnimationStep = 0;
        private const int DOOR_ANIMATION_STEPS = 30;
        private int leftDoorStartX;
        private int rightDoorStartX;

        public Form1()
        {
            InitializeComponent();

            // Initialize DB and context
            var db = new DbConnection();
            liftContext = new LiftContext(db);

            // Setup background worker
            liftWorker.WorkerReportsProgress = true;
            liftWorker.WorkerSupportsCancellation = true;
            liftWorker.DoWork += liftWorker_DoWork;
            liftWorker.RunWorkerCompleted += liftWorker_RunWorkerCompleted;

            // Setup door animation timer
            doorTimer.Interval = 20; // 20ms per frame = smooth animation
            doorTimer.Tick += doorTimer_Tick;

            // Set lift to ground floor position
            lift_movable.Top = 545;

            // Store initial door positions
            leftDoorStartX = ground_lift_left_door_btn.Left;
            rightDoorStartX = ground_lift_right_door_btn.Left;

            // Initialize with closed door state
            liftContext.SetDoorState(new DoorClosedState());
            liftContext.DoorState.Enter(liftContext);

            // Wire up button clicks
            ground_call_button.Click += ground_call_button_Click;
            first_call_button.Click += first_call_button_Click;
            ground_btn.Click += ground_btn_Click;
            first_btn.Click += first_btn_Click;
            open_lift_btn.Click += open_lift_btn_Click;
            close_lift_btn.Click += close_lift_btn_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshLogs();
        }

        // Call button handlers
        private void ground_call_button_Click(object sender, EventArgs e)
        {
            if (!isBusy && liftContext.CurrentFloor != 0)
                StartLift(0);
        }

        private void first_call_button_Click(object sender, EventArgs e)
        {
            if (!isBusy && liftContext.CurrentFloor != 1)
                StartLift(1);
        }

        private void ground_btn_Click(object sender, EventArgs e)
        {
            if (!isBusy && liftContext.CurrentFloor != 0)
                StartLift(0);
        }

        private void first_btn_Click(object sender, EventArgs e)
        {
            if (!isBusy && liftContext.CurrentFloor != 1)
                StartLift(1);
        }

        // Door control handlers
        private void open_lift_btn_Click(object sender, EventArgs e)
        {
            if (!isBusy && !isAnimatingDoor && !liftContext.IsDoorOpen)
            {
                StartDoorAnimation(true); // Open doors
            }
        }

        private void close_lift_btn_Click(object sender, EventArgs e)
        {
            if (!isBusy && !isAnimatingDoor && liftContext.IsDoorOpen)
            {
                StartDoorAnimation(false); // Close doors
            }
        }

        // Start door animation
        private void StartDoorAnimation(bool opening)
        {
            isAnimatingDoor = true;
            isDoorOpening = opening;
            doorAnimationStep = 0;

            // Get the doors for current floor
            PictureBox leftDoor = liftContext.CurrentFloor == 0
                ? ground_lift_left_door_btn
                : first_lift_left_door_btn;
            PictureBox rightDoor = liftContext.CurrentFloor == 0
                ? ground_lift_right_door_btn
                : first_lift_right_door_btn;

            // Store starting positions
            leftDoorStartX = leftDoor.Left;
            rightDoorStartX = rightDoor.Left;

            // Start the timer
            doorTimer.Start();
        }

        // Timer tick event for door animation
        private void doorTimer_Tick(object sender, EventArgs e)
        {
            doorAnimationStep++;

            // Get the doors for current floor
            PictureBox leftDoor = liftContext.CurrentFloor == 0
                ? ground_lift_left_door_btn
                : first_lift_left_door_btn;
            PictureBox rightDoor = liftContext.CurrentFloor == 0
                ? ground_lift_right_door_btn
                : first_lift_right_door_btn;

            if (isDoorOpening)
            {
                // Open doors (move apart)
                leftDoor.Left = leftDoorStartX - doorAnimationStep;
                rightDoor.Left = rightDoorStartX + doorAnimationStep;
            }
            else
            {
                // Close doors (move together)
                leftDoor.Left = leftDoorStartX + doorAnimationStep;
                rightDoor.Left = rightDoorStartX - doorAnimationStep;
            }

            // Check if animation is complete
            if (doorAnimationStep >= DOOR_ANIMATION_STEPS)
            {
                doorTimer.Stop();
                isAnimatingDoor = false;

                // Update door state
                if (isDoorOpening)
                {
                    liftContext.SetDoorState(new DoorOpenState());
                    liftContext.DoorState.Enter(liftContext);
                }
                else
                {
                    liftContext.SetDoorState(new DoorClosedState());
                    liftContext.DoorState.Enter(liftContext);
                }

                RefreshLogs();
            }
        }

        private void StartLift(int targetFloor)
        {
            // Check if doors are closed before moving
            if (liftContext.IsDoorOpen)
            {
                MessageBox.Show("Please close the doors before moving the lift!");
                return;
            }

            if (!liftWorker.IsBusy)
            {
                isBusy = true;
                liftWorker.RunWorkerAsync(targetFloor);
            }
        }

        private void liftWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int targetFloor = (int)e.Argument;

            if (targetFloor > liftContext.CurrentFloor) // Moving UP
            {
                liftContext.SetState(new MovingUpState());
                liftContext.CurrentState.Enter(liftContext);

                // Animate: move from Y=545 to Y=54
                int startY = 545;
                int endY = 54;
                int steps = 50;
                int pixelsPerStep = (startY - endY) / steps;

                for (int i = 0; i < steps; i++)
                {
                    this.Invoke((Action)(() =>
                    {
                        lift_movable.Top = startY - (pixelsPerStep * i);
                    }));
                    System.Threading.Thread.Sleep(40);
                }

                this.Invoke((Action)(() =>
                {
                    lift_movable.Top = endY;
                }));

                liftContext.CurrentFloor = targetFloor;
                liftContext.CurrentState.Exit(liftContext);
            }
            else if (targetFloor < liftContext.CurrentFloor) // Moving DOWN
            {
                liftContext.SetState(new MovingDownState());
                liftContext.CurrentState.Enter(liftContext);

                // Animate: move from Y=54 to Y=545
                int startY = 54;
                int endY = 545;
                int steps = 50;
                int pixelsPerStep = (endY - startY) / steps;

                for (int i = 0; i < steps; i++)
                {
                    this.Invoke((Action)(() =>
                    {
                        lift_movable.Top = startY + (pixelsPerStep * i);
                    }));
                    System.Threading.Thread.Sleep(40);
                }

                this.Invoke((Action)(() =>
                {
                    lift_movable.Top = endY;
                }));

                liftContext.CurrentFloor = targetFloor;
                liftContext.CurrentState.Exit(liftContext);
            }
        }

        private void liftWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isBusy = false;
            liftContext.SetState(new IdleState());
            liftContext.CurrentState.Enter(liftContext);

            MessageBox.Show($"Lift reached floor {liftContext.CurrentFloor}");
            RefreshLogs();
        }

        private void RefreshLogs()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = liftContext.Db.GetAllEvents();
        }
    }
}