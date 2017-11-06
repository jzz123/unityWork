using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Net
{
    public class ClientSocket
    {
        private static byte[] result = new byte[1024];
        private static Socket clientSocket;
        //是否已连接的标识  
        public bool IsConnected = false;

        public ClientSocket()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>  
        /// 连接指定IP和端口的服务器  
        /// </summary>  
        /// <param name="ip"></param>  
        /// <param name="port"></param>  
        public void ConnectServer(string ip, int port)
        {
            IPAddress mIp = IPAddress.Parse(ip);
            IPEndPoint ip_end_point = new IPEndPoint(mIp, port);

            try
            {
                clientSocket.Connect(ip_end_point);
                IsConnected = true;
                Debug.Log("连接服务器成功");
            }
            catch
            {
                IsConnected = false;
                Debug.Log("连接服务器失败");
                return;
            }
            Thread thread = new Thread(RecieveMessage);
            thread.Start(clientSocket);
            //服务器下发数据长度  
            //int receiveLength = clientSocket.Receive(result);
            //ByteBuffer buffer = new ByteBuffer(result);
            //int len = buffer.ReadShort();
            //string data = buffer.ReadString();
            //Debug.Log("服务器返回数据：" + data);
        }
        
        /// <summary>
        /// 接收服务器的消息  
        /// </summary>
        void RecieveMessage(object clientSocket)
        {
            Socket mSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    int receiveNumber = mSocket.Receive(result, 1024, 0);
                    recieve_Mes = System.Text.Encoding.Unicode.GetString(result, 0, receiveNumber);
                    Debug.Log("收到数据内容：" + recieve_Mes);
                    DisposeMes(recieve_Mes);
                    
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.Message);
                    mSocket.Shutdown(SocketShutdown.Both);
                    mSocket.Close();
                    break;
                }
            }
        }
        /// <summary>
        /// 收到的字符串
        /// </summary>
        public string recieve_Mes, recieve_Pos, recieve_Ror;
        string[] strArr;
        /// <summary>
        /// 处理收到的消息
        /// </summary>
        /// <param name="str"></param>
        void DisposeMes(string str)
        {
            strArr = str.Split('|');
            if (strArr.Length != 2) return;
            switch (strArr[0])
            {
                case "P":
                    recieve_Pos = strArr[1];break;
                case "R": 
                    recieve_Ror = strArr[1];break;
            }
        }
        /// <summary>  
        /// 发送数据给服务器  
        /// </summary>  
        public void SendMessage(string data)
        {
            if (IsConnected == false)
                return;
            try
            {
                clientSocket.Send(System.Text.Encoding.Unicode.GetBytes(data));
            }
            catch
            {
                Debug.Log("发送数据异常");
                IsConnected = false;
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }

        /// <summary>  
        /// 数据转换，网络发送需要两部分数据，一是数据长度，二是主体数据  
        /// </summary>  
        /// <param name="message"></param>  
        /// <returns></returns>  
        private static byte[] WriteMessage(byte[] message)
        {
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                ushort msglen = (ushort)message.Length;
                writer.Write(msglen);
                writer.Write(message);
                writer.Flush();
                return ms.ToArray();
            }
        }
        public void Close()
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}