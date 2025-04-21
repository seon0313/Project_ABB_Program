using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

[Serializable]
public class Message
{
    public string type;
    public string data;
    public string time;
}

public class client : MonoBehaviour
{
    WebSocket websocket;
    private bool is_open = false;
    private Camera Camera
    {
        get
        {
            if (!_camera)
            {
                _camera = Camera.main;
            }
            return _camera;
        }
    }
    private Camera _camera;


    byte[] capture()
    {
        // ���� RenderTexture ����
        RenderTexture activeRenderTexture = RenderTexture.active;

        // RenderTexture ũ�⸦ ���� �н��� �°� ����
        RenderTexture tempRT = new RenderTexture(649, 365, 16); // ���� �޽����� ��� �ػ�
        Camera.targetTexture = tempRT;
        RenderTexture.active = tempRT;

        Camera.Render();

        Texture2D image = new Texture2D(tempRT.width, tempRT.height);
        image.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        image.Apply();

        // ����
        RenderTexture.active = activeRenderTexture;
        Camera.targetTexture = null; // ���� ���·� ����
        byte[] bytes = image.EncodeToPNG();
        Destroy(image);
        Destroy(tempRT); // �ӽ� RenderTexture ����

        return bytes;
    }

    // Start is called before the first frame update
    async void Start()
    {
        // websocket = new WebSocket("ws://echo.websocket.org");
        websocket = new WebSocket("ws://localhost:8765");

        websocket.OnOpen += async () =>
        {
            Debug.Log("Connection open!");
            Message msg = new Message();
            msg.type = "first";
            msg.data = "robot";
            is_open = true;
            await websocket.SendText(JsonUtility.ToJson(msg));
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            is_open = false;
        };


        websocket.OnMessage += async (bytes) =>
        {
            // Reading a plain text message
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
            Message data = JsonUtility.FromJson<Message>(message);

            string type = data.type;
            if (type == "get_image")
            {
                string image = Convert.ToBase64String(capture());
                Message msg = new Message();
                msg.type = "image";
                msg.data = image;
                string i = JsonUtility.ToJson(msg);
                Debug.Log(">>>>>>>>>>>>>"+i);
                await websocket.SendText(i);
            }
        };

        // Keep sending messages at every 0.3s
        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        await websocket.Connect();
    }

    async void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
                websocket.DispatchMessageQueue();
        #endif
        if (is_open){
            string image = Convert.ToBase64String(capture());
            Message msg = new Message();
            msg.type = "image";
            msg.data = image;
            string i = JsonUtility.ToJson(msg);
            Debug.Log(">>>>>>>>>>>>>"+i);
            await websocket.SendText(i);
        }
    }

    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await websocket.SendText("plain text message");
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
