using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class broadcastItem
{
    public string ip;
    public int port;
    public string name;
    public string version;
    public int cameraPort;
}

public class getDeviceList : MonoBehaviour
{
    private Socket socket;
    public int PORT = 5891;
    public broadcastItem packetData = null;
    private EndPoint endPoint;

    public GameObject context;
    public GameObject baseItem;

    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, PORT);
        socket.ExclusiveAddressUse = false;
        socket.Bind(ipEndPoint);
        endPoint = (EndPoint)ipEndPoint;
    }

    void Update()
    {
        Receive();
    }

    void UpdateItem()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("DeviceItem");
        Debug.Log("Updating item: " + items.Length);
        var item_go = Instantiate(baseItem, context.transform);
        // do something with the instantiated item -- for instance
        item_go.GetComponentAtIndex<TMP_Text>(0).text = "Item #" + i;
        //parent the item to the content container
        item_go.transform.SetParent(context.transform, false);
        item_go.transform.localScale = Vector2.one;
        
    }


    void Receive()
    {
        if (socket.Available != 0)
        {
            byte[] packet = new byte[1024];

            try
            {
                socket.ReceiveFrom(packet, 0, packet.Length, SocketFlags.None, ref endPoint);
            }

            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                return;
            }

            packetData = JsonUtility.FromJson<broadcastItem>(System.Text.Encoding.UTF8.GetString(packet).TrimEnd('\0'));
            Debug.Log("Received packet: " + packetData.name);
            UpdateItem();
        }
    }
}
