namespace MediaLib.UI
{
    partial class AddRootForm
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
            this.ChoseDirBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.typeCombox = new System.Windows.Forms.ComboBox();
            this.dirPathTextBox = new System.Windows.Forms.TextBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.depthTextBox = new System.Windows.Forms.TextBox();
            this.Finished = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ChoseDirBtn
            // 
            this.ChoseDirBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ChoseDirBtn.Location = new System.Drawing.Point(404, 72);
            this.ChoseDirBtn.Name = "ChoseDirBtn";
            this.ChoseDirBtn.Size = new System.Drawing.Size(44, 28);
            this.ChoseDirBtn.TabIndex = 0;
            this.ChoseDirBtn.Text = "...";
            this.ChoseDirBtn.UseVisualStyleBackColor = true;
            this.ChoseDirBtn.Click += new System.EventHandler(this.ChoseDirBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 217);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "Search Depth";
            // 
            // typeCombox
            // 
            this.typeCombox.FormattingEnabled = true;
            this.typeCombox.Location = new System.Drawing.Point(144, 173);
            this.typeCombox.Name = "typeCombox";
            this.typeCombox.Size = new System.Drawing.Size(121, 26);
            this.typeCombox.TabIndex = 4;
            // 
            // dirPathTextBox
            // 
            this.dirPathTextBox.Location = new System.Drawing.Point(25, 72);
            this.dirPathTextBox.Name = "dirPathTextBox";
            this.dirPathTextBox.Size = new System.Drawing.Size(356, 28);
            this.dirPathTextBox.TabIndex = 5;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(144, 132);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(121, 28);
            this.nameTextBox.TabIndex = 6;
            // 
            // depthTextBox
            // 
            this.depthTextBox.Location = new System.Drawing.Point(144, 214);
            this.depthTextBox.Name = "depthTextBox";
            this.depthTextBox.Size = new System.Drawing.Size(121, 28);
            this.depthTextBox.TabIndex = 7;
            this.depthTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.depthTextBox_KeyPress);
            // 
            // Finished
            // 
            this.Finished.Location = new System.Drawing.Point(345, 130);
            this.Finished.Name = "Finished";
            this.Finished.Size = new System.Drawing.Size(103, 110);
            this.Finished.TabIndex = 8;
            this.Finished.Text = "Finished";
            this.Finished.UseVisualStyleBackColor = true;
            this.Finished.Click += new System.EventHandler(this.Finished_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(143, 18);
            this.label4.TabIndex = 9;
            this.label4.Text = "Media Directory";
            // 
            // AddRootForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 264);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Finished);
            this.Controls.Add(this.depthTextBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.dirPathTextBox);
            this.Controls.Add(this.typeCombox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ChoseDirBtn);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(491, 320);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(491, 320);
            this.Name = "AddRootForm";
            this.Text = "Media Directory";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ChoseDirBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox typeCombox;
        private System.Windows.Forms.TextBox dirPathTextBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox depthTextBox;
        private System.Windows.Forms.Button Finished;
        private System.Windows.Forms.Label label4;
    }
}