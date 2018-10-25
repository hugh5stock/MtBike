using Fleck;
using MintLock.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using YouTingParking.Models;
using YouTingParking.Utility;

namespace MintLock.WSServer
{
    public class LockServer
    {
        private static LockServer instance = new LockServer();
        private static int TrandeNo = 0;
        private static WebSocketServer server;

        ConcurrentDictionary<string, IWebSocketConnection> connectionList = new ConcurrentDictionary<string, IWebSocketConnection>();
        private string securityKey = "97FF4AF03BF95D182B2546BA9FC85EA9";
        public static LockServer Instance
        {
            get { return instance; }
        }

        private LogHelper log = new LogHelper();

        public LockServer()
        {
            
            Start();
        }

        public void Start()
        {

            server = new WebSocketServer("ws://114.55.100.103:80");
            server.Start(socket =>
            {
                socket.OnOpen = () => { OnConnectionOpen(socket); };
                socket.OnClose = () => { RemoveClosedConnection(socket); };
                socket.OnError = OnError;
                socket.OnBinary = OnBytes;


                socket.OnMessage = message =>
                {
                    HandlerMessage(socket, message);
                };
            });
        }


        private void OnConnectionOpen(IWebSocketConnection connection)
        {
            log.WriteLog("Connection Open:"+ connection.ConnectionInfo.Id);
        }

        private void OnBytes(byte[] bytes)
        {
            string str = string.Empty;
            foreach (Byte item in bytes)
            {
                str += item.ToString();
            }

            log.WriteLog("OnBytes:" + str);
        }

        private void OnError(Exception ex)
        {
            log.WriteLog("Connection error:" + ex.Message + "; " + ex.StackTrace);
        }

        private void RemoveClosedConnection(IWebSocketConnection connection)
        {
            var connInfo = connectionList.FirstOrDefault(v => v.Value.ConnectionInfo.Id == connection.ConnectionInfo.Id);
            if (!string.IsNullOrEmpty(connInfo.Key))
            {
                IWebSocketConnection conn;
                connectionList.TryRemove(connInfo.Key, out conn);
                conn = null;

                log.WriteLog("Connection Closed:" + connInfo.Key);
            }
            else
            {
                log.WriteLog("Connection Closed without login");
            }
           
        }


        public OpenLockResponse OpenLock(string deviceId)
        {
            OpenLockResponse response = new OpenLockResponse();
            if (!this.connectionList.ContainsKey(deviceId))
            {
                response.SuccessSend = false;
                response.Message = "The device has not beenn connected.";
                return response;
            }

            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string userid = "jason";
                TrandeNo++;
                int tn = TrandeNo;
                string text = deviceId + "," + timestamp + "," + userid + "," + tn;

                string mac = AESEncrypt(text, securityKey);

                string command = timestamp + "," + userid + "," + tn + "," + mac;
                string returnValue = "OPEN 1" + "\r\n" + command;

                var connection = connectionList[deviceId];
                log.WriteLog(returnValue);
                connection.Send(returnValue);
                response.SuccessSend = true;
            }
            catch (Exception ex)
            {

                response.Message = ex.Message;
            }



            return response;
        }

        public void HandlerMessage(IWebSocketConnection conn, string msg)
        {

            
            log.WriteLog(msg);

            string seperator = "\r\n";
            string[] s = new string[] { seperator };
            string[] info = msg.Split(s, StringSplitOptions.None);
            if (info[0].Contains("LOGIN"))
            {
                string deviceId = info[1].Substring(0, 12);
                securityKey = info[1].Substring(12, 32);
                if (connectionList.ContainsKey(deviceId))
                {
                    IWebSocketConnection connection = connectionList[deviceId];
                    if (conn != connection)
                    {
                        connectionList.TryRemove(deviceId, out connection);
                        connectionList.TryAdd(deviceId, conn);
                    }
                }
                else
                {
                    connectionList.TryAdd(deviceId, conn);
                }


                string result = info[0] + "\r\n0 OK\r\n";
                log.WriteLog("Send:" + result);
                conn.Send(result);

            }
            else if (info[0].Contains("RECORD"))
            {
                string result = info[0] + "\r\n0 OK\r\n";
                conn.Send(result);
                HandleRecord(conn, info[1]);
                log.WriteLog("Send:" + result);
            }
            else if (info[0].Contains("OPEN"))
            {
                HandleOpen(conn, info[0]);
            }
        }

        private void HandleRecord(IWebSocketConnection conn, string message)
        {
            try
            {
                string[] info = message.Split(',');

                if (info.Length != 7)
                {
                    log.WriteLog("Record info should have 7 parts.");
                    return;
                }
                var connInfo = connectionList.FirstOrDefault(v => v.Value.ConnectionInfo.Id == conn.ConnectionInfo.Id);
                if (!string.IsNullOrEmpty(connInfo.Key))
                {
                    string param = string.Format("deviceId={0}&time={1}&longi={2}&lati={3}&voltage={4}", connInfo.Key, info[0],
                   info[4], info[5], info[6]);
                    var url = ConfigurationManager.AppSettings["OnCloseUrl"];
                    if (string.IsNullOrEmpty(url))
                    {
                        log.WriteLog("Please config OnClose url");
                        return;
                    }

                    CommonHelper.PostWebRequest(url, param);
                }
            }
            catch (Exception ex)
            {

                log.WriteLog(ex.Message);
            }
           


        }

        private void HandleOpen(IWebSocketConnection connection, string message)
        {
            try
            {
                var connInfo = connectionList.FirstOrDefault(v => v.Value.ConnectionInfo.Id == connection.ConnectionInfo.Id);
                if (!string.IsNullOrEmpty(connInfo.Key))
                {
                    var url = ConfigurationManager.AppSettings["OnOpenUrl"];
                    if (string.IsNullOrEmpty(url))
                    {
                        log.WriteLog("Please config OnOpen Url");
                        return;
                    }
                    CommonHelper.PostWebRequest(url, connInfo.Key);
                }
            }
            catch (Exception ex)
            {

                log.WriteLog(ex.Message);
            }
            
        }

        public string AESEncrypt(string encryptStr, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(encryptStr);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    }
}