namespace ASMControlPanel
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.StartStop_Button = new System.Windows.Forms.Button();
            this.RunningProjectLabel = new System.Windows.Forms.Label();
            this.ProjectComboBox = new System.Windows.Forms.ComboBox();
            this.RCSimulationCheckBoxesLabel = new System.Windows.Forms.Label();
            this.RC1SimulationCheckBox = new System.Windows.Forms.CheckBox();
            this.RC2SimulationCheckBox = new System.Windows.Forms.CheckBox();
            this.RC3SimulationCheckBox = new System.Windows.Forms.CheckBox();
            this.RC4SimulationCheckBox = new System.Windows.Forms.CheckBox();
            this.ToolMapComboBox = new System.Windows.Forms.ComboBox();
            this.MapToolButton = new System.Windows.Forms.Button();
            this.VncToolButton = new System.Windows.Forms.Button();
            this.VncToolListComboBox = new System.Windows.Forms.ComboBox();
            this.UnMapToolButton = new System.Windows.Forms.Button();
            this.MappingStatusLabel = new System.Windows.Forms.Label();
            this.VncStatusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StartStop_Button
            // 
            this.StartStop_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.StartStop_Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.StartStop_Button.Location = new System.Drawing.Point(12, 34);
            this.StartStop_Button.Name = "StartStop_Button";
            this.StartStop_Button.Size = new System.Drawing.Size(95, 23);
            this.StartStop_Button.TabIndex = 1;
            this.StartStop_Button.Text = "Start/Stop";
            this.StartStop_Button.UseVisualStyleBackColor = true;
            this.StartStop_Button.Click += new System.EventHandler(this.StartStop_Click);
            // 
            // RunningProjectLabel
            // 
            this.RunningProjectLabel.AutoSize = true;
            this.RunningProjectLabel.Location = new System.Drawing.Point(121, 10);
            this.RunningProjectLabel.Name = "RunningProjectLabel";
            this.RunningProjectLabel.Size = new System.Drawing.Size(117, 13);
            this.RunningProjectLabel.TabIndex = 1;
            this.RunningProjectLabel.Text = "Select Project to Run...";
            this.RunningProjectLabel.TextChanged += new System.EventHandler(this.label1_TextChanged);
            this.RunningProjectLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // ProjectComboBox
            // 
            this.ProjectComboBox.FormattingEnabled = true;
            this.ProjectComboBox.Location = new System.Drawing.Point(124, 34);
            this.ProjectComboBox.Name = "ProjectComboBox";
            this.ProjectComboBox.Size = new System.Drawing.Size(332, 21);
            this.ProjectComboBox.Sorted = true;
            this.ProjectComboBox.TabIndex = 2;
            this.ProjectComboBox.SelectedIndexChanged += new System.EventHandler(this.ProjectComboBox_SelectedIndexChanged);
            // 
            // RCSimulationCheckBoxesLabel
            // 
            this.RCSimulationCheckBoxesLabel.AutoSize = true;
            this.RCSimulationCheckBoxesLabel.Location = new System.Drawing.Point(12, 66);
            this.RCSimulationCheckBoxesLabel.Name = "RCSimulationCheckBoxesLabel";
            this.RCSimulationCheckBoxesLabel.Size = new System.Drawing.Size(192, 13);
            this.RCSimulationCheckBoxesLabel.TabIndex = 3;
            this.RCSimulationCheckBoxesLabel.Text = "Pick RC you want to simulate if needed";
            this.RCSimulationCheckBoxesLabel.Click += new System.EventHandler(this.label2_Click);
            // 
            // RC1SimulationCheckBox
            // 
            this.RC1SimulationCheckBox.AutoSize = true;
            this.RC1SimulationCheckBox.Location = new System.Drawing.Point(219, 64);
            this.RC1SimulationCheckBox.Name = "RC1SimulationCheckBox";
            this.RC1SimulationCheckBox.Size = new System.Drawing.Size(47, 17);
            this.RC1SimulationCheckBox.TabIndex = 4;
            this.RC1SimulationCheckBox.Text = "RC1";
            this.RC1SimulationCheckBox.UseVisualStyleBackColor = true;
            this.RC1SimulationCheckBox.CheckedChanged += new System.EventHandler(this.checkBoxRC1_CheckedChanged);
            // 
            // RC2SimulationCheckBox
            // 
            this.RC2SimulationCheckBox.AutoSize = true;
            this.RC2SimulationCheckBox.Location = new System.Drawing.Point(281, 64);
            this.RC2SimulationCheckBox.Name = "RC2SimulationCheckBox";
            this.RC2SimulationCheckBox.Size = new System.Drawing.Size(47, 17);
            this.RC2SimulationCheckBox.TabIndex = 5;
            this.RC2SimulationCheckBox.Text = "RC2";
            this.RC2SimulationCheckBox.UseVisualStyleBackColor = true;
            this.RC2SimulationCheckBox.CheckedChanged += new System.EventHandler(this.checkBoxRC2_CheckedChanged);
            // 
            // RC3SimulationCheckBox
            // 
            this.RC3SimulationCheckBox.AutoSize = true;
            this.RC3SimulationCheckBox.Location = new System.Drawing.Point(346, 65);
            this.RC3SimulationCheckBox.Name = "RC3SimulationCheckBox";
            this.RC3SimulationCheckBox.Size = new System.Drawing.Size(47, 17);
            this.RC3SimulationCheckBox.TabIndex = 6;
            this.RC3SimulationCheckBox.Text = "RC3";
            this.RC3SimulationCheckBox.UseVisualStyleBackColor = true;
            this.RC3SimulationCheckBox.CheckedChanged += new System.EventHandler(this.checkBoxRC3_CheckedChanged);
            // 
            // RC4SimulationCheckBox
            // 
            this.RC4SimulationCheckBox.AutoSize = true;
            this.RC4SimulationCheckBox.Location = new System.Drawing.Point(409, 64);
            this.RC4SimulationCheckBox.Name = "RC4SimulationCheckBox";
            this.RC4SimulationCheckBox.Size = new System.Drawing.Size(47, 17);
            this.RC4SimulationCheckBox.TabIndex = 7;
            this.RC4SimulationCheckBox.Text = "RC4";
            this.RC4SimulationCheckBox.UseVisualStyleBackColor = true;
            this.RC4SimulationCheckBox.CheckedChanged += new System.EventHandler(this.checkBoxRC4_CheckedChanged);
            // 
            // ToolMapComboBox
            // 
            this.ToolMapComboBox.FormattingEnabled = true;
            this.ToolMapComboBox.Location = new System.Drawing.Point(212, 93);
            this.ToolMapComboBox.Name = "ToolMapComboBox";
            this.ToolMapComboBox.Size = new System.Drawing.Size(244, 21);
            this.ToolMapComboBox.Sorted = true;
            this.ToolMapComboBox.TabIndex = 9;
            this.ToolMapComboBox.Text = "Select a Tool to Map to";
            this.ToolMapComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // MapToolButton
            // 
            this.MapToolButton.Location = new System.Drawing.Point(15, 91);
            this.MapToolButton.Name = "MapToolButton";
            this.MapToolButton.Size = new System.Drawing.Size(92, 23);
            this.MapToolButton.TabIndex = 10;
            this.MapToolButton.Text = "Map to a Tool";
            this.MapToolButton.UseVisualStyleBackColor = true;
            this.MapToolButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // VncToolButton
            // 
            this.VncToolButton.Location = new System.Drawing.Point(15, 134);
            this.VncToolButton.Name = "VncToolButton";
            this.VncToolButton.Size = new System.Drawing.Size(92, 23);
            this.VncToolButton.TabIndex = 11;
            this.VncToolButton.Text = "VNC to a Tool";
            this.VncToolButton.UseVisualStyleBackColor = true;
            this.VncToolButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // VncToolListComboBox
            // 
            this.VncToolListComboBox.FormattingEnabled = true;
            this.VncToolListComboBox.Location = new System.Drawing.Point(124, 136);
            this.VncToolListComboBox.Name = "VncToolListComboBox";
            this.VncToolListComboBox.Size = new System.Drawing.Size(332, 21);
            this.VncToolListComboBox.Sorted = true;
            this.VncToolListComboBox.TabIndex = 12;
            this.VncToolListComboBox.Text = "Select a Tool to VNC to";
            this.VncToolListComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // UnMapToolButton
            // 
            this.UnMapToolButton.Location = new System.Drawing.Point(124, 91);
            this.UnMapToolButton.Name = "UnMapToolButton";
            this.UnMapToolButton.Size = new System.Drawing.Size(75, 23);
            this.UnMapToolButton.TabIndex = 13;
            this.UnMapToolButton.Text = "UnMap";
            this.UnMapToolButton.UseVisualStyleBackColor = true;
            this.UnMapToolButton.Click += new System.EventHandler(this.button4_Click);
            // 
            // MappingStatusLabel
            // 
            this.MappingStatusLabel.AutoSize = true;
            this.MappingStatusLabel.Location = new System.Drawing.Point(209, 118);
            this.MappingStatusLabel.Name = "MappingStatusLabel";
            this.MappingStatusLabel.Size = new System.Drawing.Size(0, 13);
            this.MappingStatusLabel.TabIndex = 14;
            this.MappingStatusLabel.Click += new System.EventHandler(this.label3_Click_1);
            // 
            // VncStatusLabel
            // 
            this.VncStatusLabel.AutoSize = true;
            this.VncStatusLabel.Location = new System.Drawing.Point(209, 161);
            this.VncStatusLabel.Name = "VncStatusLabel";
            this.VncStatusLabel.Size = new System.Drawing.Size(0, 13);
            this.VncStatusLabel.TabIndex = 15;
            this.VncStatusLabel.Click += new System.EventHandler(this.label4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 178);
            this.Controls.Add(this.VncStatusLabel);
            this.Controls.Add(this.MappingStatusLabel);
            this.Controls.Add(this.UnMapToolButton);
            this.Controls.Add(this.VncToolListComboBox);
            this.Controls.Add(this.VncToolButton);
            this.Controls.Add(this.MapToolButton);
            this.Controls.Add(this.ToolMapComboBox);
            this.Controls.Add(this.RC4SimulationCheckBox);
            this.Controls.Add(this.RC3SimulationCheckBox);
            this.Controls.Add(this.RC2SimulationCheckBox);
            this.Controls.Add(this.RC1SimulationCheckBox);
            this.Controls.Add(this.RCSimulationCheckBoxesLabel);
            this.Controls.Add(this.ProjectComboBox);
            this.Controls.Add(this.RunningProjectLabel);
            this.Controls.Add(this.StartStop_Button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "ASM Control Panel";
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartStop_Button;
        private System.Windows.Forms.Label RunningProjectLabel;
        private System.Windows.Forms.ComboBox ProjectComboBox;
        private System.Windows.Forms.Label RCSimulationCheckBoxesLabel;
        private System.Windows.Forms.CheckBox RC1SimulationCheckBox;
        private System.Windows.Forms.CheckBox RC2SimulationCheckBox;
        private System.Windows.Forms.CheckBox RC3SimulationCheckBox;
        private System.Windows.Forms.CheckBox RC4SimulationCheckBox;
        private System.Windows.Forms.ComboBox ToolMapComboBox;
        private System.Windows.Forms.Button MapToolButton;
        private System.Windows.Forms.Button VncToolButton;
        private System.Windows.Forms.ComboBox VncToolListComboBox;
        private System.Windows.Forms.Button UnMapToolButton;
        private System.Windows.Forms.Label MappingStatusLabel;
        private System.Windows.Forms.Label VncStatusLabel;
    }
}

