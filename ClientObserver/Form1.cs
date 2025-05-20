using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyloggerServer
{
    public partial class Form1 : Form
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;
        public Form1()
        {
            InitializeComponent();
        }

        private async void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                _cts = new CancellationTokenSource();
                _client = new TcpClient("127.0.0.1", 5000);
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
                AppendLog($"Connection error: {ex.Message}");
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

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    AppendMessage(msg);
                }
            }
            catch (OperationCanceledException)
            {
                AppendLog("Listening cancelled.");
            }
            catch (Exception ex)
            {
                AppendLog($"Error while receiving: {ex.Message}");
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
                AppendLog($"Error on stop: {ex.Message}");
            }
            finally
            {
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
            }
        }
    }
}
