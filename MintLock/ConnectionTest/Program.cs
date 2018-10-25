using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;

namespace ConnectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int socketCount = 0;
            int deviceCount = 0;
            string device = "abcdefgh";
            string securityKey = "7FF4AF03BF95D182B2546BA9FC85EA93";

            while (socketCount < 300)
            {
                WebSocket socket = new WebSocket("ws://114.55.100.103:80");
                socket.DataReceived += Socket_DataReceived;
                socket.MessageReceived += Socket_MessageReceived;
                socket.Open();
                Thread.Sleep(500);
                deviceCount++;

                string loginTest = string.Format("LOGIN {0}\r\n", socketCount) + device + deviceCount.ToString().PadLeft(4, '0') + securityKey + "test";
                socket.Send(loginTest);
                socketCount++;
            }

            Console.Read();
            

        }

        private static void Socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void Socket_DataReceived(object sender, DataReceivedEventArgs e)
        {
            string text = Encoding.Default.GetString(e.Data);
            Console.WriteLine(text);
        }
    }
}
