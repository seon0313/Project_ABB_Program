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
        socket.EnableBroadcast = true; // Allow broadcast reception
        endPoint = (EndPoint)ipEndPoint;
    }

    void Update()
    {
        Receive();
    }

    void UpdateItem(broadcastItem data)
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("DeviceItem");
        Debug.Log("Updating item: " + items.Length);
        bool onlist = false;
        for (int i = 0; i < items.Length; i++)
        {
            onlist = items[i].name == data.name;
        }

        if (!onlist)
        {
            var item_go = Instantiate(baseItem, context.transform);
            item_go.tag = "DeviceItem"; // Ensure the new item has the correct tag
            item_go.name = data.name; // Set the name of the item for easier identification
            item_go.SetActive(true); // Activate the item if it was inactive
            // do something with the instantiated item -- for instance
            item_go.GetComponentsInChildren<TMP_Text>()[0].SetText(data.name);
            item_go.GetComponentsInChildren<TMP_Text>()[1].SetText(data.version);
            //parent the item to the content container
            item_go.transform.SetParent(context.transform, false);
            item_go.transform.localScale = Vector2.one;
            
            item_go.GetComponent<Button>().onClick.AddListener(() =>
            {
                FindAnyObjectByType<Client>().Connection(data);
            });
        }
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
            UpdateItem(packetData);
        }
    }
}
