using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class JzzUDP : MonoBehaviour {
    private const int port = 1035;
    private static string IpStr = "127.0.0.1";
    private static byte[] result = new byte[1024];
    private static byte[] sendMes = new byte[1024];

    private static Socket serverSocket;
    private EndPoint ep;
	// Use this for initialization
	void Start () {
        IPAddress ip = IPAddress.Parse(IpStr);
        IPEndPoint ip_end_point = new IPEndPoint(ip, port);

        //创建服务器Socket对象，并设置相关属性  
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        ep = (EndPoint)ip_end_point; 
	}
    int i = 0;
	// Update is called once per frame
	void Update () {
		sendMes = Encoding.Default.GetBytes("傻瓜" + i++);
        serverSocket.SendTo(sendMes, SocketFlags.None, ep);
	}


    void OnDisable()
    {
        serverSocket.Close();
    }
}
