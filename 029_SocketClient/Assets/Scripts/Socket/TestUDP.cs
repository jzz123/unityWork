using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
public class TestUDP : MonoBehaviour {
    private const int port = 1035;
    private static string IpStr = "127.0.0.1";
    private static byte[] result = new byte[1024];
    private static byte[] sendMes = new byte[1024];

    private static Socket serverSocket;
    private EndPoint ep;
	// Use this for initialization
	void Start () {
        try
        {
            IPAddress ip = IPAddress.Parse(IpStr);
            EndPoint iep_Recieve = new IPEndPoint(ip, port);
            ep = (EndPoint)iep_Recieve; 
            //创建服务器Socket对象，并设置相关属性  
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //绑定ip和端口  
            serverSocket.Bind(iep_Recieve);

            //StartCoroutine(recevieMes());
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
        
	}
    IEnumerator recevieMes()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            try
            {
                ///这里需要保证服务端后台一直运行，否则会一直停在接收消息这行代码上
                ///改成协程的方式
                int intReceiveLength = serverSocket.ReceiveFrom(result, ref ep);
                //转换数据为字符串  
                string strReceiveStr = Encoding.Default.GetString(result, 0, intReceiveLength);
                Debug.Log(strReceiveStr + ":" + intReceiveLength);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
	// Update is called once per frame
	void Update () {
        try
        {
            ///这里需要保证服务端后台一直运行，否则会一直停在接收消息这行代码上
            ///改成协程的方式
            int intReceiveLength = serverSocket.ReceiveFrom(result, ref ep);
            //转换数据为字符串  
            string strReceiveStr = Encoding.Default.GetString(result, 0, intReceiveLength);
            Debug.Log(strReceiveStr + ":" + intReceiveLength);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
	}

    void OnDisable()
    {
        if (serverSocket!= null)
            serverSocket.Close();
    }
}
