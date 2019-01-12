namespace ScanAPIDemo
{
    partial class MessageDlg
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
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_lblMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Location = new System.Drawing.Point(286, 24);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 0;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.OnCancelClicked);
            // 
            // m_lblMessage
            // 
            this.m_lblMessage.Location = new System.Drawing.Point(12, 9);
            this.m_lblMessage.Name = "m_lblMessage";
            this.m_lblMessage.Size = new System.Drawing.Size(268, 55);
            this.m_lblMessage.TabIndex = 1;
            this.m_lblMessage.Text = "label1";
            this.m_lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MessageDlg
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 73);
            this.Controls.Add(this.m_lblMessage);
            this.Controls.Add(this.m_btnCancel);
            this.Name = "MessageDlg";
           // this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
            //this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label m_lblMessage;
    }
}