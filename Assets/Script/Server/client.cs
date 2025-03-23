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
        // 기존 RenderTexture 저장
        RenderTexture activeRenderTexture = RenderTexture.active;

        // RenderTexture 크기를 렌더 패스에 맞게 설정
        RenderTexture tempRT = new RenderTexture(649, 365, 16); // 오류 메시지의 기대 해상도
        Camera.targetTexture = tempRT;
        RenderTexture.active = tempRT;

        Camera.Render();

        Texture2D image = new Texture2D(tempRT.width, tempRT.height);
        image.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        image.Apply();

        // 정리
        RenderTexture.active = activeRenderTexture;
        Camera.targetTexture = null; // 원래 상태로 복구
        byte[] bytes = image.EncodeToPNG();
        Destroy(image);
        Destroy(tempRT); // 임시 RenderTexture 정리

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
            await websocket.SendText(JsonUtility.ToJson(msg));
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
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

    void Update()
    {
    #if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
    #endif
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
