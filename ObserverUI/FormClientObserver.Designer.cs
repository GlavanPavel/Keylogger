namespace KeyloggerServer
{
    partial class ClientObserver
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
            listViewClients = new ListView();
            buttonRefreshList = new Button();
            menuStrip1 = new MenuStrip();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            buttonExit = new Button();
            buttonDownload = new Button();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // buttonStart
            // 
            buttonStart.Location = new Point(681, 139);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(131, 50);
            buttonStart.TabIndex = 0;
            buttonStart.Text = "Start";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += buttonStart_Click;
            // 
            // textBoxMessage
            // 
            textBoxMessage.Location = new Point(329, 54);
            textBoxMessage.Multiline = true;
            textBoxMessage.Name = "textBoxMessage";
            textBoxMessage.ReadOnly = true;
            textBoxMessage.ScrollBars = ScrollBars.Vertical;
            textBoxMessage.Size = new Size(328, 263);
            textBoxMessage.TabIndex = 1;
            // 
            // textBoxLog
            // 
            textBoxLog.Location = new Point(30, 363);
            textBoxLog.Multiline = true;
            textBoxLog.Name = "textBoxLog";
            textBoxLog.ReadOnly = true;
            textBoxLog.ScrollBars = ScrollBars.Vertical;
            textBoxLog.Size = new Size(627, 97);
            textBoxLog.TabIndex = 2;
            // 
            // buttonStop
            // 
            buttonStop.Location = new Point(681, 212);
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
            labelMessage.Location = new Point(329, 33);
            labelMessage.Name = "labelMessage";
            labelMessage.Size = new Size(92, 15);
            labelMessage.TabIndex = 4;
            labelMessage.Text = "Client Messages";
            // 
            // labelLog
            // 
            labelLog.AutoSize = true;
            labelLog.Location = new Point(30, 336);
            labelLog.Name = "labelLog";
            labelLog.Size = new Size(32, 15);
            labelLog.TabIndex = 5;
            labelLog.Text = "Logs";
            // 
            // listViewClients
            // 
            listViewClients.Location = new Point(30, 54);
            listViewClients.Name = "listViewClients";
            listViewClients.Size = new Size(272, 263);
            listViewClients.TabIndex = 7;
            listViewClients.UseCompatibleStateImageBehavior = false;
            listViewClients.View = View.List;
            listViewClients.SelectedIndexChanged += listViewClients_SelectedIndexChanged;
            // 
            // buttonRefreshList
            // 
            buttonRefreshList.Location = new Point(227, 29);
            buttonRefreshList.Name = "buttonRefreshList";
            buttonRefreshList.Size = new Size(75, 23);
            buttonRefreshList.TabIndex = 8;
            buttonRefreshList.Text = "Refresh";
            buttonRefreshList.UseVisualStyleBackColor = true;
            buttonRefreshList.Click += buttonRefreshList_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { helpToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(824, 24);
            menuStrip1.TabIndex = 9;
            menuStrip1.Text = "menuStrip1";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            helpToolStripMenuItem.Click += helpToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            // 
            // buttonExit
            // 
            buttonExit.Location = new Point(737, 437);
            buttonExit.Name = "buttonExit";
            buttonExit.Size = new Size(75, 23);
            buttonExit.TabIndex = 10;
            buttonExit.Text = "Exit";
            buttonExit.UseVisualStyleBackColor = true;
            buttonExit.Click += buttonExit_Click;
            // 
            // buttonDownload
            // 
            buttonDownload.Enabled = false;
            buttonDownload.Location = new Point(582, 323);
            buttonDownload.Name = "buttonDownload";
            buttonDownload.Size = new Size(75, 23);
            buttonDownload.TabIndex = 11;
            buttonDownload.Text = "Download";
            buttonDownload.UseVisualStyleBackColor = true;
            buttonDownload.Click += buttonDownload_Click;
            // 
            // ClientObserver
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(824, 472);
            Controls.Add(buttonDownload);
            Controls.Add(buttonExit);
            Controls.Add(buttonRefreshList);
            Controls.Add(listViewClients);
            Controls.Add(labelLog);
            Controls.Add(labelMessage);
            Controls.Add(buttonStop);
            Controls.Add(textBoxLog);
            Controls.Add(textBoxMessage);
            Controls.Add(buttonStart);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "ClientObserver";
            Text = "Keylogger";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
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
        private ListView listViewClients;
        private Button buttonRefreshList;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Button buttonExit;
        private Button buttonDownload;
    }
}
