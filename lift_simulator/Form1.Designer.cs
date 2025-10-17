﻿namespace lift_simulator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pictureBox2 = new PictureBox();
            ground_lift_left_door_btn = new PictureBox();
            lift_movable = new PictureBox();
            ground_lift_right_door_btn = new PictureBox();
            first_lift_right_door_btn = new PictureBox();
            first_lift_left_door_btn = new PictureBox();
            pictureBox11 = new PictureBox();
            ground_call_button = new PictureBox();
            first_call_button = new PictureBox();
            dataGridView1 = new DataGridView();
            close_lift_btn = new PictureBox();
            open_lift_btn = new PictureBox();
            first_btn = new PictureBox();
            ground_btn = new PictureBox();
            liftWorker = new System.ComponentModel.BackgroundWorker();
            doorTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ground_lift_left_door_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lift_movable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ground_lift_right_door_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)first_lift_right_door_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)first_lift_left_door_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox11).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ground_call_button).BeginInit();
            ((System.ComponentModel.ISupportInitialize)first_call_button).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)close_lift_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)open_lift_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)first_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ground_btn).BeginInit();
            SuspendLayout();
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.image1;
            pictureBox2.Location = new Point(-1, 490);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(285, 256);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // ground_lift_left_door_btn
            // 
            ground_lift_left_door_btn.Image = Properties.Resources.unnamed1;
            ground_lift_left_door_btn.Location = new Point(79, 535);
            ground_lift_left_door_btn.Name = "ground_lift_left_door_btn";
            ground_lift_left_door_btn.Size = new Size(67, 188);
            ground_lift_left_door_btn.TabIndex = 4;
            ground_lift_left_door_btn.TabStop = false;
            // 
            // lift_movable
            // 
            lift_movable.Image = Properties.Resources.Screenshot_2025_10_15_1318531;
            lift_movable.Location = new Point(309, 545);
            lift_movable.Name = "lift_movable";
            lift_movable.Size = new Size(123, 201);
            lift_movable.SizeMode = PictureBoxSizeMode.StretchImage;
            lift_movable.TabIndex = 6;
            lift_movable.TabStop = false;
            // 
            // ground_lift_right_door_btn
            // 
            ground_lift_right_door_btn.Image = Properties.Resources.unnamed1;
            ground_lift_right_door_btn.Location = new Point(144, 535);
            ground_lift_right_door_btn.Name = "ground_lift_right_door_btn";
            ground_lift_right_door_btn.Size = new Size(63, 188);
            ground_lift_right_door_btn.TabIndex = 7;
            ground_lift_right_door_btn.TabStop = false;
            // 
            // first_lift_right_door_btn
            // 
            first_lift_right_door_btn.Image = Properties.Resources.unnamed1;
            first_lift_right_door_btn.Location = new Point(144, 44);
            first_lift_right_door_btn.Name = "first_lift_right_door_btn";
            first_lift_right_door_btn.Size = new Size(63, 188);
            first_lift_right_door_btn.TabIndex = 10;
            first_lift_right_door_btn.TabStop = false;
            // 
            // first_lift_left_door_btn
            // 
            first_lift_left_door_btn.Image = Properties.Resources.unnamed1;
            first_lift_left_door_btn.Location = new Point(79, 44);
            first_lift_left_door_btn.Name = "first_lift_left_door_btn";
            first_lift_left_door_btn.Size = new Size(67, 188);
            first_lift_left_door_btn.TabIndex = 9;
            first_lift_left_door_btn.TabStop = false;
            // 
            // pictureBox11
            // 
            pictureBox11.Image = Properties.Resources.image1;
            pictureBox11.Location = new Point(-1, -1);
            pictureBox11.Name = "pictureBox11";
            pictureBox11.Size = new Size(285, 256);
            pictureBox11.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox11.TabIndex = 8;
            pictureBox11.TabStop = false;
            // 
            // ground_call_button
            // 
            ground_call_button.Image = Properties.Resources.elevator_push_button;
            ground_call_button.Location = new Point(-1, 584);
            ground_call_button.Name = "ground_call_button";
            ground_call_button.Size = new Size(54, 54);
            ground_call_button.SizeMode = PictureBoxSizeMode.StretchImage;
            ground_call_button.TabIndex = 11;
            ground_call_button.TabStop = false;
            ground_call_button.Click += ground_call_button_Click;
            // 
            // first_call_button
            // 
            first_call_button.Image = Properties.Resources.elevator_push_button_rotated;
            first_call_button.Location = new Point(7, 110);
            first_call_button.Name = "first_call_button";
            first_call_button.Size = new Size(46, 46);
            first_call_button.SizeMode = PictureBoxSizeMode.StretchImage;
            first_call_button.TabIndex = 12;
            first_call_button.TabStop = false;
            first_call_button.Click += first_call_button_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(736, 21);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(614, 437);
            dataGridView1.TabIndex = 13;
            // 
            // close_lift_btn
            // 
            close_lift_btn.Image = Properties.Resources.close_button;
            close_lift_btn.Location = new Point(604, 271);
            close_lift_btn.Name = "close_lift_btn";
            close_lift_btn.Size = new Size(126, 104);
            close_lift_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            close_lift_btn.TabIndex = 14;
            close_lift_btn.TabStop = false;
            close_lift_btn.Click += close_lift_btn_Click;
            // 
            // open_lift_btn
            // 
            open_lift_btn.Image = Properties.Resources.open_button;
            open_lift_btn.Location = new Point(482, 271);
            open_lift_btn.Name = "open_lift_btn";
            open_lift_btn.Size = new Size(126, 104);
            open_lift_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            open_lift_btn.TabIndex = 15;
            open_lift_btn.TabStop = false;
            open_lift_btn.Click += open_lift_btn_Click;
            // 
            // first_btn
            // 
            first_btn.Image = Properties.Resources.first_floor_button;
            first_btn.Location = new Point(544, 161);
            first_btn.Name = "first_btn";
            first_btn.Size = new Size(126, 104);
            first_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            first_btn.TabIndex = 16;
            first_btn.TabStop = false;
            first_btn.Click += first_btn_Click;
            // 
            // ground_btn
            // 
            ground_btn.Image = Properties.Resources.ground_floor_button;
            ground_btn.Location = new Point(544, 381);
            ground_btn.Name = "ground_btn";
            ground_btn.Size = new Size(126, 104);
            ground_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            ground_btn.TabIndex = 17;
            ground_btn.TabStop = false;
            ground_btn.Click += ground_btn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1362, 750);
            Controls.Add(ground_btn);
            Controls.Add(first_btn);
            Controls.Add(open_lift_btn);
            Controls.Add(close_lift_btn);
            Controls.Add(dataGridView1);
            Controls.Add(first_call_button);
            Controls.Add(ground_call_button);
            Controls.Add(first_lift_right_door_btn);
            Controls.Add(first_lift_left_door_btn);
            Controls.Add(pictureBox11);
            Controls.Add(ground_lift_right_door_btn);
            Controls.Add(lift_movable);
            Controls.Add(ground_lift_left_door_btn);
            Controls.Add(pictureBox2);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)ground_lift_left_door_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)lift_movable).EndInit();
            ((System.ComponentModel.ISupportInitialize)ground_lift_right_door_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)first_lift_right_door_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)first_lift_left_door_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox11).EndInit();
            ((System.ComponentModel.ISupportInitialize)ground_call_button).EndInit();
            ((System.ComponentModel.ISupportInitialize)first_call_button).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)close_lift_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)open_lift_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)first_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)ground_btn).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private PictureBox pictureBox2;
        private PictureBox ground_lift_left_door_btn;
        private PictureBox lift_movable;
        private PictureBox ground_lift_right_door_btn;
        private PictureBox first_lift_right_door_btn;
        private PictureBox first_lift_left_door_btn;
        private PictureBox pictureBox11;
        private PictureBox ground_call_button;
        private PictureBox first_call_button;
        private DataGridView dataGridView1;
        private PictureBox close_lift_btn;
        private PictureBox open_lift_btn;
        private PictureBox first_btn;
        private PictureBox ground_btn;
        private System.ComponentModel.BackgroundWorker liftWorker;
        private System.Windows.Forms.Timer doorTimer;
    }
}
