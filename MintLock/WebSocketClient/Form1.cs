using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocket4Net;

namespace WebSocketClient
{
    public partial class Form1 : Form
    {
        int socketCount = 0;
        int deviceCount = 1;
        string device = "abcdefgh";
        string securityKey = "7FF4AF03BF95D182B2546BA9FC85EA93";
        WebSocket socket = new WebSocket("ws://114.55.100.103:80");

        string userID = string.Empty;
        string tradeNo = string.Empty;

        SynchronizationContext context = new WindowsFormsSynchronizationContext();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            socket.MessageReceived += Socket_MessageReceived;
            socket.Opened += Socket_Opened;
            socket.Closed += Socket_Closed;
            socket.Open();

            this.tbDevice2.Text = device + deviceCount.ToString().PadLeft(4, '0');
            deviceCount++;

           
        }

        private void Socket_Closed(object sender, EventArgs e)
        {
            context.Post(OnClosed, null);
        }

        private void Socket_Opened(object sender, EventArgs e)
        {
            context.Post(OnOpened, null);
        }

        private void OnOpened(object o)
        {
            this.button1.Enabled = true;
            this.lbStatus.Text = "Connected";
        }
        private void OnClosed(object o)
        {
            this.button1.Enabled = false;
            this.lbStatus.Text = "Disconnected";
        }

        private void Socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            context.Post(HandleMessage, e.Message);
        }

        private void HandleMessage(object o)
        {
            string msg = o.ToString();
            this.tbMessage.Text = tbMessage.Text + Environment.NewLine + "Received:" + msg;
            string seperator = "\r\n";
            string[] s = new string[] { seperator };
            string[] info = msg.Split(s, StringSplitOptions.None);
            if (info[0].Contains("LOGIN")  && info[0].Contains("OK"))
            {
                this.button2.Enabled = true;
                this.lbStatus.Text = "Logined";
            }
            else if (info[0].Contains("RECORD") && info[0].Contains("OK"))
            {

            }
            else if (info[0].Contains("OPEN"))
            {
                string openResponse = "OPEN 1\r\n" + Environment.NewLine + 0;

                tbMessage.Text = tbMessage.Text  + Environment.NewLine + "Send:" + openResponse;
                socket.Send(openResponse);

                this.lbStatus.Text = "OPEN";
                this.button2.Enabled = true;

                string[] strs = info[1].Split(',');
                this.userID = strs[1];
                tradeNo = strs[2];

            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.tbDevice2.Text.Length != 12)
            {
                MessageBox.Show("DeviceID should have 12 characters.");
                return;
            }

            if (socket.State != WebSocketState.Open)
            {
                socket.Open();
                MessageBox.Show("Reconnecting");
                return;

            }
            string loginTest = string.Format("LOGIN {0}\r\n", socketCount) + this.tbDevice2.Text + securityKey + "test";
            socket.Send(loginTest);

            this.tbMessage.Text = this.tbMessage.Text + Environment.NewLine + "Send:" + loginTest;
            socketCount++;

            button1.Enabled = false;
            tbDevice2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string request = "RECORD 123\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + ",1," + userID + "," + tradeNo + "," + tbLongi.Text + "," + tbLati.Text + "," + tbVoltage.Text;

            socket.Send(request);
            this.tbMessage.Text = this.tbMessage.Text + Environment.NewLine + "Send:" + request;
            this.button2.Enabled = false;
            this.lbStatus.Text = "Closed";
        }
    }
}
