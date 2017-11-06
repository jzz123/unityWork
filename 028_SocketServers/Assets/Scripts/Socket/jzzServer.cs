using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Net;
using System.IO;
using System;

public class jzzServer : MonoBehaviour {
    public static jzzServer Instance;
    private const int port = 8080;
    private static string IpStr = "127.0.0.1";
    //private const int port = 1035;
    //private static string IpStr = "127.0.0.1";

    private static Socket serverSocket;
    private static byte[] result = new byte[1024];

    //private struct SocketMessage
    //{
    //    public Socket socket;
    //    public byte[] result = new byte[1024];
    //}
    //private static SocketMessage[] messageList = new SocketMessage[10];
    // 用于初始化 变量
    void Start()
    {
        Instance = this;
        IPAddress ip = IPAddress.Parse(IpStr);
        IPEndPoint ip_end_point = new IPEndPoint(ip, port);

        //创建服务器Socket对象，并设置相关属性  
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //绑定ip和端口  
        serverSocket.Bind(ip_end_point);
        //设置最长的连接请求队列长度  
        serverSocket.Listen(10);
        Debug.Log("启动监听 " + serverSocket.LocalEndPoint.ToString() + " 成功");
        Thread thread = new Thread(ClientConnectListen);
        thread.Start();  
    }

    public Transform vr_camera_Me;
    public Transform vr_camera_Client;
    /// <summary>
    /// 脚本启用时调用 用于初始化特殊设置
    /// </summary>
    void OnEnable()
    {

    }

    // Update is called once per frame  
    void Update()
    {
        if (clientSocket != null && clientSocket.Connected)
        {
            SendPosAndRor();
            RecievePosAndRor();
        }
        
    }
    /// <summary>
    /// 发送眼镜位置和方向
    /// </summary>
    void SendPosAndRor()
    {
        try {
            clientSocket.Send(System.Text.Encoding.Unicode.GetBytes("P|" + vr_camera_Me.localPosition.ToString("f3")));
            clientSocket.Send(System.Text.Encoding.Unicode.GetBytes("R|" + vr_camera_Me.localRotation.eulerAngles.ToString("f3")));

        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    /// <summary>
    /// 接收眼镜位置和方向
    /// </summary>
    void RecievePosAndRor()
    {
        vr_camera_Client.localPosition = StrToVec3(recieve_Pos);
        vr_camera_Client.localRotation = Quaternion.Euler(StrToVec3(recieve_Ror));

    }
    Socket clientSocket;
    /// <summary>  
    /// 客户端连接请求监听  
    /// </summary>  
    private void ClientConnectListen()
    {
        while (true)
        {
            //为新的客户端连接创建一个Socket对象  
            clientSocket = serverSocket.Accept();
            Debug.Log("客户端成功连接" + clientSocket.RemoteEndPoint.ToString());
            //向连接的客户端发送连接成功的数据  
            clientSocket.Send(System.Text.Encoding.Unicode.GetBytes("Connected Server"));
            //每个客户端连接创建一个线程来接受该客户端发送的消息  
            Thread thread = new Thread(RecieveMessage);
            thread.Start(clientSocket);
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
    /// <summary>  
    /// 接收指定客户端Socket的消息  
    /// </summary>  
    /// <param name="clientSocket"></param>  
    private void RecieveMessage(object clientSocket)
    {
        Socket mClientSocket = (Socket)clientSocket;
        while (true)
        {
            //if (mClientSocket.Poll(1000000, SelectMode.SelectRead))
            //{
            //    Debug.Log(mClientSocket.RemoteEndPoint.ToString() + "断开连接");
            //    mClientSocket.Shutdown(SocketShutdown.Both);
            //    mClientSocket.Close();
            //    break;
            //}
            try
            {
                int receiveNumber = mClientSocket.Receive(result,1024,0);
                recieve_Mes = System.Text.Encoding.Unicode.GetString(result, 0, receiveNumber);
                Debug.Log("收到数据内容：" + recieve_Mes);
                DisposeMes(recieve_Mes);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                mClientSocket.Shutdown(SocketShutdown.Both);
                mClientSocket.Close();
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
                recieve_Pos = strArr[1]; break;
            case "R":
                recieve_Ror = strArr[1]; break;
        }
    }
    /// <summary>
    /// 字符串转Vector3
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    Vector3 StrToVec3(string str)
    {
        if (string.IsNullOrEmpty(str)) return Vector3.zero;
        str = str.Replace("(", "").Replace(")", "");
        string[] s = str.Split(',');
        if (s.Length != 3) return Vector3.zero;
        return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
    }
    /// <summary>
    /// 检视面板中变量改变时调用
    /// </summary>
    void OnValidate()
    {
    }
    /// <summary>
    /// 脚本隐藏或者物体隐藏时调用 用于消除内存占用 
    /// 一般用于Destroy stop StopCoroutine
    /// </summary>
    void OnDisable()
    {
        serverSocket.Close();
    }
}
