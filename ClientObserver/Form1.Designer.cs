namespace KeyloggerServer
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
            buttonStart = new Button();
            textBoxMessage = new TextBox();
            textBoxLog = new TextBox();
            buttonStop = new Button();
            labelMessage = new Label();
            labelLog = new Label();
            SuspendLayout();
            // 
            // buttonStart
            // 
            buttonStart.Location = new Point(539, 123);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(131, 50);
            buttonStart.TabIndex = 0;
            buttonStart.Text = "Start";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += buttonStart_Click;
            // 
            // textBoxMessage
            // 
            textBoxMessage.Location = new Point(56, 47);
            textBoxMessage.Multiline = true;
            textBoxMessage.Name = "textBoxMessage";
            textBoxMessage.ReadOnly = true;
            textBoxMessage.ScrollBars = ScrollBars.Vertical;
            textBoxMessage.Size = new Size(320, 195);
            textBoxMessage.TabIndex = 1;
            // 
            // textBoxLog
            // 
            textBoxLog.Location = new Point(56, 316);
            textBoxLog.Multiline = true;
            textBoxLog.Name = "textBoxLog";
            textBoxLog.ReadOnly = true;
            textBoxLog.ScrollBars = ScrollBars.Vertical;
            textBoxLog.Size = new Size(320, 71);
            textBoxLog.TabIndex = 2;
            // 
            // buttonStop
            // 
            buttonStop.Location = new Point(539, 241);
            buttonStop.Name = "buttonStop";
            buttonStop.Size = new Size(131, 50);
            buttonStop.TabIndex = 3;
            buttonStop.Text = "Stop";
            buttonStop.UseVisualStyleBackColor = true;
            buttonStop.Click += buttonStop_Click;
            // 
            // labelMessage
            // 
            labelMessage.AutoSize = true;
            labelMessage.Location = new Point(65, 19);
            labelMessage.Name = "labelMessage";
            labelMessage.Size = new Size(92, 15);
            labelMessage.TabIndex = 4;
            labelMessage.Text = "Client Messages";
            // 
            // labelLog
            // 
            labelLog.AutoSize = true;
            labelLog.Location = new Point(65, 288);
            labelLog.Name = "labelLog";
            labelLog.Size = new Size(32, 15);
            labelLog.TabIndex = 5;
            labelLog.Text = "Logs";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(labelLog);
            Controls.Add(labelMessage);
            Controls.Add(buttonStop);
            Controls.Add(textBoxLog);
            Controls.Add(textBoxMessage);
            Controls.Add(buttonStart);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonStart;
        private TextBox textBoxMessage;
        private TextBox textBoxLog;
        private Button buttonStop;
        private Label labelMessage;
        private Label labelLog;
    }
}
