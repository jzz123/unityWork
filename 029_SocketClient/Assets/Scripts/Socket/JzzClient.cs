using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Net;  

public class JzzClient : MonoBehaviour {
    // Use this for initialization  
    ClientSocket mSocket;

    public Transform vr_camera;
    public Transform vr_camera_Server;
    void Start()
    {
        mSocket = new ClientSocket();
        mSocket.ConnectServer("127.0.0.1", 8080);
        //mSocket.SendMessage("服务器傻逼！");
        //这里的数值是保留小数点后一位，四舍五入
        //string str = vr_camera.localPosition.ToString();
    }
    

    // Update is called once per frame  
    void Update()
    {
        if (mSocket.IsConnected)
        {
            SendPosAndRor();
            RecievePosAndRor();
        }
        
    }
    /// <summary>
    /// 坐标
    /// </summary>
    public string str_Pos;
    /// <summary>
    /// 方向
    /// </summary>
    public string str_Ror;
    /// <summary>
    /// 发送眼镜位置和方向
    /// </summary>
    void SendPosAndRor()
    {
        str_Pos = vr_camera.localPosition.ToString("f3");
        str_Ror = vr_camera.localRotation.eulerAngles.ToString("f3");
        mSocket.SendMessage("P|" + str_Pos);
        mSocket.SendMessage("R|" + str_Ror);
    }
    Vector3 rec_Pos,rec_Ror;
    /// <summary>
    /// 接收眼镜位置和方向
    /// </summary>
    void RecievePosAndRor()
    {
        vr_camera_Server.localPosition = StrToVec3(mSocket.recieve_Pos);
        vr_camera_Server.localRotation = Quaternion.Euler(StrToVec3(mSocket.recieve_Ror));
        
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
    void OnDisable()
    {
        mSocket.Close();
    }
}
