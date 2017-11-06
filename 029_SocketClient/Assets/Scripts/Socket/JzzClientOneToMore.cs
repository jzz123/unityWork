using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

public class JzzClientOneToMore : MonoBehaviour {
    private const int port = 8080;
    private static string IpStr = "127.0.0.1";
    Socket mSockets;
    private static byte[] result = new byte[1024];
    //是否已连接的标识  
    public bool IsConnected = false;
    // 用于初始化 变量
    void Awake()
    {
        mSockets = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ConnectServer(IpStr, port);
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
            mSockets.Connect(ip_end_point);
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
        thread.Start(mSockets);
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
                string recieve_Mes = System.Text.Encoding.Unicode.GetString(result, 0, receiveNumber);
                Debug.Log("收到数据内容：" + recieve_Mes);
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

        mSockets.Close();
        mSockets = null;
    }
}
