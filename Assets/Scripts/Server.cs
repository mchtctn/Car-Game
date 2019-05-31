using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
    #region private members 	
    [SerializeField]
    private int playerCount = 5;
    [SerializeField]
    private GameObject spawnPlayerObj;
    [SerializeField]
   // private GameObject planeCollider;
    public GameObject[] playerObject;
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    private float carElevation = 2f;
    public string clientMessage;
    private bool needResponse = false;
    #endregion

    // Use this for initialization
    void Start()
    {
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
        for(int i = 0; i < playerCount; i++)
        {
            //Instantiate(planeCollider, new Vector3(0, i * 2, 0), Quaternion.identity);
            //Instantiate(spawnPlayerObj, new Vector3(0.48f, i*2, -0.75f), Quaternion.identity);
            
        }
        playerObject = GameObject.FindGameObjectsWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        if (needResponse)
        {
            SendMessage();
        }
    }

    private void ListenForIncommingRequests()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), 8080);
            tcpListener.Start();
            Debug.Log("Server is listening");
            Byte[] bytes = new Byte[2048];
            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    using (NetworkStream stream = connectedTcpClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            clientMessage = Encoding.ASCII.GetString(incommingData);
                            Debug.Log("client message received as: " + clientMessage); //0:NN 2
                            needResponse = true;
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }
    private void SendMessage()
    {
        if (connectedTcpClient == null)
        {
            return;
        }

        try
        {
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                playerObject = GameObject.FindGameObjectsWithTag("Player");
                //string serverMessage = "";
                string tempString = "";
                List<string> serverMessage = new List<string>();
                for (int i = 0; i < playerCount; i++)
                {
                    //id:1 L:10 R:10 F:10 isHit:false speed:100 , id:2

                    tempString = "{'id':" + i
                        + " ,'L':'" + playerObject[i].GetComponent<Raycast>().leftSensorHitDistance + "'"
                        + " ,'R':'" + playerObject[i].GetComponent<Raycast>().rightSensorHitDistance + "'"
                        + " ,'F':'" + playerObject[i].GetComponent<Raycast>().frontSensorHitDistance + "'" 
                        + " ,'isHit': '" + playerObject[i].GetComponent<CarControl>().isCrashed + "'" 
                        + " ,'speed': '" + String.Format("{0:0.##}", playerObject[i].GetComponent<CarControl>().currentSpeed)+"'"
                        + " ,'point': '" + playerObject[i].GetComponent<CarControl>().point+"'}";

                    serverMessage.Add(tempString);
                }
                tempString = "[" + string.Join(",", serverMessage.ToArray()) + "]";
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes( tempString );
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                Debug.Log("Server sent his message - should be received by client");
                needResponse = false;
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}