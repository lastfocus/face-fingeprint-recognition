namespace Fingeprint
{
    partial class recognitionfingerprint
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
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label7;
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_lblImageSize = new System.Windows.Forms.Label();
            this.m_lblCompatibility = new System.Windows.Forms.Label();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOpenDevice = new System.Windows.Forms.Button();
            this.m_cmbInterfaces = new System.Windows.Forms.ComboBox();
            this.m_lblCurrentImageSize = new System.Windows.Forms.Label();
            this.SaveFrame = new System.Windows.Forms.Button();
            this.m_btnGetFrame = new System.Windows.Forms.Button();
            this.m_chkReceiveLongImage = new System.Windows.Forms.CheckBox();
            this.m_grpTests = new System.Windows.Forms.GroupBox();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_grpParameters = new System.Windows.Forms.GroupBox();
            this.m_lblEEPROMSize = new System.Windows.Forms.Label();
            this.m_chkFFD = new System.Windows.Forms.CheckBox();
            this.m_chkDetectFakeFinger = new System.Windows.Forms.CheckBox();
            this.m_picture = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.m_grpTests.SuspendLayout();
            this.m_grpParameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_picture)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(6, 38);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(63, 13);
            label6.TabIndex = 2;
            label6.Text = "Image size: ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(6, 16);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(107, 13);
            label5.TabIndex = 0;
            label5.Text = "Device compatibility: ";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(3, 164);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(77, 13);
            label8.TabIndex = 7;
            label8.Text = "EEPROM size:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(3, 140);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(99, 13);
            label7.TabIndex = 5;
            label7.Text = "Current image size: ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_lblImageSize);
            this.groupBox1.Controls.Add(label6);
            this.groupBox1.Controls.Add(this.m_lblCompatibility);
            this.groupBox1.Controls.Add(label5);
            this.groupBox1.Location = new System.Drawing.Point(6, 74);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(361, 63);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Device information";
            // 
            // m_lblImageSize
            // 
            this.m_lblImageSize.Location = new System.Drawing.Point(119, 38);
            this.m_lblImageSize.Name = "m_lblImageSize";
            this.m_lblImageSize.Size = new System.Drawing.Size(233, 13);
            this.m_lblImageSize.TabIndex = 3;
            // 
            // m_lblCompatibility
            // 
            this.m_lblCompatibility.Location = new System.Drawing.Point(119, 16);
            this.m_lblCompatibility.Name = "m_lblCompatibility";
            this.m_lblCompatibility.Size = new System.Drawing.Size(233, 13);
            this.m_lblCompatibility.TabIndex = 1;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Enabled = false;
            this.m_btnCancel.Location = new System.Drawing.Point(167, 329);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 25;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // m_btnOpenDevice
            // 
            this.m_btnOpenDevice.Location = new System.Drawing.Point(239, 14);
            this.m_btnOpenDevice.Name = "m_btnOpenDevice";
            this.m_btnOpenDevice.Size = new System.Drawing.Size(75, 23);
            this.m_btnOpenDevice.TabIndex = 21;
            this.m_btnOpenDevice.Text = "Open";
            this.m_btnOpenDevice.UseVisualStyleBackColor = true;
            this.m_btnOpenDevice.Click += new System.EventHandler(this.m_btnOpenDevice_Click);
            // 
            // m_cmbInterfaces
            // 
            this.m_cmbInterfaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cmbInterfaces.FormattingEnabled = true;
            this.m_cmbInterfaces.Location = new System.Drawing.Point(109, 16);
            this.m_cmbInterfaces.Name = "m_cmbInterfaces";
            this.m_cmbInterfaces.Size = new System.Drawing.Size(91, 21);
            this.m_cmbInterfaces.TabIndex = 20;
            this.m_cmbInterfaces.SelectedIndexChanged += new System.EventHandler(this.m_cmbInterfaces_SelectedIndexChanged);
            // 
            // m_lblCurrentImageSize
            // 
            this.m_lblCurrentImageSize.Location = new System.Drawing.Point(108, 140);
            this.m_lblCurrentImageSize.Name = "m_lblCurrentImageSize";
            this.m_lblCurrentImageSize.Size = new System.Drawing.Size(314, 13);
            this.m_lblCurrentImageSize.TabIndex = 6;
            // 
            // SaveFrame
            // 
            this.SaveFrame.Enabled = false;
            this.SaveFrame.Location = new System.Drawing.Point(269, 19);
            this.SaveFrame.Name = "SaveFrame";
            this.SaveFrame.Size = new System.Drawing.Size(75, 23);
            this.SaveFrame.TabIndex = 5;
            this.SaveFrame.Text = "Identify";
            this.SaveFrame.UseVisualStyleBackColor = true;
            this.SaveFrame.Click += new System.EventHandler(this.SaveFrame_Click);
            // 
            // m_btnGetFrame
            // 
            this.m_btnGetFrame.Location = new System.Drawing.Point(6, 19);
            this.m_btnGetFrame.Name = "m_btnGetFrame";
            this.m_btnGetFrame.Size = new System.Drawing.Size(75, 23);
            this.m_btnGetFrame.TabIndex = 4;
            this.m_btnGetFrame.Text = "Get Frame";
            this.m_btnGetFrame.UseVisualStyleBackColor = true;
            this.m_btnGetFrame.Click += new System.EventHandler(this.m_btnGetFrame_Click);
            // 
            // m_chkReceiveLongImage
            // 
            this.m_chkReceiveLongImage.AutoSize = true;
            this.m_chkReceiveLongImage.Location = new System.Drawing.Point(197, 25);
            this.m_chkReceiveLongImage.Name = "m_chkReceiveLongImage";
            this.m_chkReceiveLongImage.Size = new System.Drawing.Size(120, 17);
            this.m_chkReceiveLongImage.TabIndex = 9;
            this.m_chkReceiveLongImage.Text = "Receive long image";
            this.m_chkReceiveLongImage.UseVisualStyleBackColor = true;
            // 
            // m_grpTests
            // 
            this.m_grpTests.Controls.Add(this.SaveFrame);
            this.m_grpTests.Controls.Add(this.m_btnGetFrame);
            this.m_grpTests.Location = new System.Drawing.Point(18, 246);
            this.m_grpTests.Name = "m_grpTests";
            this.m_grpTests.Size = new System.Drawing.Size(380, 57);
            this.m_grpTests.TabIndex = 24;
            this.m_grpTests.TabStop = false;
            this.m_grpTests.Text = "Functions";
            // 
            // m_btnClose
            // 
            this.m_btnClose.Location = new System.Drawing.Point(320, 14);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 22;
            this.m_btnClose.Text = "Close";
            this.m_btnClose.UseVisualStyleBackColor = true;
            this.m_btnClose.Click += new System.EventHandler(this.m_btnClose_Click_1);
            // 
            // m_grpParameters
            // 
            this.m_grpParameters.Controls.Add(this.m_chkReceiveLongImage);
            this.m_grpParameters.Controls.Add(this.m_lblEEPROMSize);
            this.m_grpParameters.Controls.Add(label8);
            this.m_grpParameters.Controls.Add(this.m_lblCurrentImageSize);
            this.m_grpParameters.Controls.Add(label7);
            this.m_grpParameters.Controls.Add(this.groupBox1);
            this.m_grpParameters.Controls.Add(this.m_chkFFD);
            this.m_grpParameters.Controls.Add(this.m_chkDetectFakeFinger);
            this.m_grpParameters.Location = new System.Drawing.Point(18, 49);
            this.m_grpParameters.Name = "m_grpParameters";
            this.m_grpParameters.Size = new System.Drawing.Size(380, 191);
            this.m_grpParameters.TabIndex = 18;
            this.m_grpParameters.TabStop = false;
            this.m_grpParameters.Text = " Parameters ";
            // 
            // m_lblEEPROMSize
            // 
            this.m_lblEEPROMSize.Location = new System.Drawing.Point(108, 164);
            this.m_lblEEPROMSize.Name = "m_lblEEPROMSize";
            this.m_lblEEPROMSize.Size = new System.Drawing.Size(250, 13);
            this.m_lblEEPROMSize.TabIndex = 8;
            // 
            // m_chkFFD
            // 
            this.m_chkFFD.AutoSize = true;
            this.m_chkFFD.Location = new System.Drawing.Point(6, 48);
            this.m_chkFFD.Name = "m_chkFFD";
            this.m_chkFFD.Size = new System.Drawing.Size(160, 17);
            this.m_chkFFD.TabIndex = 2;
            this.m_chkFFD.Text = "Fast finger detection method";
            this.m_chkFFD.UseVisualStyleBackColor = true;
            // 
            // m_chkDetectFakeFinger
            // 
            this.m_chkDetectFakeFinger.AutoSize = true;
            this.m_chkDetectFakeFinger.Location = new System.Drawing.Point(6, 25);
            this.m_chkDetectFakeFinger.Name = "m_chkDetectFakeFinger";
            this.m_chkDetectFakeFinger.Size = new System.Drawing.Size(127, 17);
            this.m_chkDetectFakeFinger.TabIndex = 1;
            this.m_chkDetectFakeFinger.Text = "Live Finger Detection";
            this.m_chkDetectFakeFinger.UseVisualStyleBackColor = true;
            // 
            // m_picture
            // 
            this.m_picture.Location = new System.Drawing.Point(470, 8);
            this.m_picture.Name = "m_picture";
            this.m_picture.Size = new System.Drawing.Size(320, 480);
            this.m_picture.TabIndex = 23;
            this.m_picture.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Default interface: ";
            // 
            // recognitionfingerprint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 537);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOpenDevice);
            this.Controls.Add(this.m_cmbInterfaces);
            this.Controls.Add(this.m_grpTests);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_grpParameters);
            this.Controls.Add(this.m_picture);
            this.Controls.Add(this.label1);
            this.Name = "recognitionfingerprint";
            this.Text = "recognitionfingerprint";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.m_grpTests.ResumeLayout(false);
            this.m_grpParameters.ResumeLayout(false);
            this.m_grpParameters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label m_lblImageSize;
        private System.Windows.Forms.Label m_lblCompatibility;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOpenDevice;
        private System.Windows.Forms.ComboBox m_cmbInterfaces;
        private System.Windows.Forms.Label m_lblCurrentImageSize;
        private System.Windows.Forms.Button SaveFrame;
        private System.Windows.Forms.Button m_btnGetFrame;
        private System.Windows.Forms.CheckBox m_chkReceiveLongImage;
        private System.Windows.Forms.GroupBox m_grpTests;
        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.GroupBox m_grpParameters;
        private System.Windows.Forms.Label m_lblEEPROMSize;
        private System.Windows.Forms.CheckBox m_chkFFD;
        private System.Windows.Forms.CheckBox m_chkDetectFakeFinger;
        private System.Windows.Forms.PictureBox m_picture;
        private System.Windows.Forms.Label label1;
    }
}