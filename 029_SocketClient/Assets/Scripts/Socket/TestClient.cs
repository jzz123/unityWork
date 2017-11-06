using UnityEngine;
using System.Collections;
using Net;  

public class TestClient : MonoBehaviour {
    // Use this for initialization  
    ClientSocket mSocket;
    void Start()
    {
        mSocket = new ClientSocket();
        mSocket.ConnectServer("127.0.0.1", 1035);
        mSocket.SendMessage("服务器傻逼！");
    }

    // Update is called once per frame  
    void Update()
    {

    }

    void OnDisable()
    {
        mSocket.Close();
    }
}
