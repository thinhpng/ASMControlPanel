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
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxRC1 = new System.Windows.Forms.CheckBox();
            this.checkBoxRC2 = new System.Windows.Forms.CheckBox();
            this.checkBoxRC3 = new System.Windows.Forms.CheckBox();
            this.checkBoxRC4 = new System.Windows.Forms.CheckBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(121, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Project to Run...";
            this.label1.TextChanged += new System.EventHandler(this.label1_TextChanged);
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(124, 34);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(332, 21);
            this.comboBox1.Sorted = true;
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Pick RC you want to simulate if needed";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // checkBoxRC1
            // 
            this.checkBoxRC1.AutoSize = true;
            this.checkBoxRC1.Location = new System.Drawing.Point(219, 64);
            this.checkBoxRC1.Name = "checkBoxRC1";
            this.checkBoxRC1.Size = new System.Drawing.Size(47, 17);
            this.checkBoxRC1.TabIndex = 4;
            this.checkBoxRC1.Text = "RC1";
            this.checkBoxRC1.UseVisualStyleBackColor = true;
            this.checkBoxRC1.CheckedChanged += new System.EventHandler(this.checkBoxRC1_CheckedChanged);
            // 
            // checkBoxRC2
            // 
            this.checkBoxRC2.AutoSize = true;
            this.checkBoxRC2.Location = new System.Drawing.Point(281, 64);
            this.checkBoxRC2.Name = "checkBoxRC2";
            this.checkBoxRC2.Size = new System.Drawing.Size(47, 17);
            this.checkBoxRC2.TabIndex = 5;
            this.checkBoxRC2.Text = "RC2";
            this.checkBoxRC2.UseVisualStyleBackColor = true;
            this.checkBoxRC2.CheckedChanged += new System.EventHandler(this.checkBoxRC2_CheckedChanged);
            // 
            // checkBoxRC3
            // 
            this.checkBoxRC3.AutoSize = true;
            this.checkBoxRC3.Location = new System.Drawing.Point(346, 65);
            this.checkBoxRC3.Name = "checkBoxRC3";
            this.checkBoxRC3.Size = new System.Drawing.Size(47, 17);
            this.checkBoxRC3.TabIndex = 6;
            this.checkBoxRC3.Text = "RC3";
            this.checkBoxRC3.UseVisualStyleBackColor = true;
            this.checkBoxRC3.CheckedChanged += new System.EventHandler(this.checkBoxRC3_CheckedChanged);
            // 
            // checkBoxRC4
            // 
            this.checkBoxRC4.AutoSize = true;
            this.checkBoxRC4.Location = new System.Drawing.Point(409, 64);
            this.checkBoxRC4.Name = "checkBoxRC4";
            this.checkBoxRC4.Size = new System.Drawing.Size(47, 17);
            this.checkBoxRC4.TabIndex = 7;
            this.checkBoxRC4.Text = "RC4";
            this.checkBoxRC4.UseVisualStyleBackColor = true;
            this.checkBoxRC4.CheckedChanged += new System.EventHandler(this.checkBoxRC4_CheckedChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(212, 93);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(244, 21);
            this.comboBox2.Sorted = true;
            this.comboBox2.TabIndex = 9;
            this.comboBox2.Text = "Select a Tool to Map to";
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(15, 91);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "Map to a Tool";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(15, 134);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(92, 23);
            this.button3.TabIndex = 11;
            this.button3.Text = "VNC to a Tool";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(124, 136);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(332, 21);
            this.comboBox3.Sorted = true;
            this.comboBox3.TabIndex = 12;
            this.comboBox3.Text = "Select a Tool to VNC to";
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(124, 91);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 13;
            this.button4.Text = "UnMap";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(209, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 14;
            this.label3.Click += new System.EventHandler(this.label3_Click_1);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(209, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 15;
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 178);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.checkBoxRC4);
            this.Controls.Add(this.checkBoxRC3);
            this.Controls.Add(this.checkBoxRC2);
            this.Controls.Add(this.checkBoxRC1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxRC1;
        private System.Windows.Forms.CheckBox checkBoxRC2;
        private System.Windows.Forms.CheckBox checkBoxRC3;
        private System.Windows.Forms.CheckBox checkBoxRC4;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

