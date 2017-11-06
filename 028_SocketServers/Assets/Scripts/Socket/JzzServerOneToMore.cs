using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;

public class JzzServerOneToMore : MonoBehaviour {
    public static JzzServerOneToMore Instance;
    private const int port = 8080;
    private static string IpStr = "127.0.0.1";
    private static Socket serverSocket;
    private static byte[] result = new byte[1024];
    // 用于初始化 变量
    void Awake()
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
    /// <summary>
    /// 晚于Awake
    /// </summary>
    void Start()
    {

    }
    /// <summary>
    /// 脚本启用时调用 用于初始化特殊设置
    /// </summary>
    void OnEnable()
    {

    }
    private List<Socket> socketList = new List<Socket>();
    /// <summary>  
    /// 客户端连接请求监听  
    /// </summary>  
    private void ClientConnectListen()
    {
        while (true)
        {
            //为新的客户端连接创建一个Socket对象  
            Socket clientSocket = serverSocket.Accept();
            Debug.Log("客户端成功连接" + clientSocket.RemoteEndPoint.ToString());
            //向连接的客户端发送连接成功的数据  
            clientSocket.Send(System.Text.Encoding.Unicode.GetBytes("Connected Server"));
            //每个客户端连接创建一个线程来接受该客户端发送的消息  
            Thread thread = new Thread(RecieveMessage);
            thread.Start(clientSocket);

            socketList.Add(clientSocket);

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
            try
            {
                int receiveNumber = mClientSocket.Receive(result, 1024, 0);
                string recieve_Mes = System.Text.Encoding.Unicode.GetString(result, 0, receiveNumber);
                Debug.Log("收到数据内容：" + recieve_Mes);

            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                Debug.Log("客户端连接断开" + mClientSocket.RemoteEndPoint.ToString());
                socketList.Remove(mClientSocket);
                //mClientSocket.Shutdown(SocketShutdown.Both);
                mClientSocket.Close();
                break;
            }

        }
    }
    void Update()
    {
        if (socketList.Count > 0)
            SendMsgToAll();
    }
    int num = 0;
    /// <summary>
    /// 广播消息
    /// </summary>
    private void SendMsgToAll()
    {
        for (int i = 0; i < socketList.Count; i++)
        {
            try
            {
                //socketList[i].Send(System.Text.Encoding.Unicode.GetBytes("Mes:" + num));
                string abc = "你好" + socketList[i].RemoteEndPoint.ToString() + ",我来自" + serverSocket.LocalEndPoint.ToString() + ":" + num;
                socketList[i].Send(System.Text.Encoding.Unicode.GetBytes(abc));
            }
            catch (System.Exception e)
            {
                Socket item = socketList[i];
                Debug.Log(item.RemoteEndPoint + ":" + e.Message);
                socketList.Remove(item);
                item.Close();
            }
        }
        num++;
    }
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "当前连接数：" + socketList.Count);
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
        serverSocket = null;
        for (int i = 0; i < socketList.Count; i++)
        {
            socketList[i].Close();
        }
    }
}
