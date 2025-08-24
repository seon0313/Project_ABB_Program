using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    private static Client instance;
    private static GameObject connectionPanel;
    private static GameObject killClickPanel;
    private static GameObject loginPanel;
    public WebSocketSharp.WebSocket ws;
    public static bool hasLogin = false;
    public broadcastItem robot;
    private static readonly ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

    void Start()
    {
        connectionPanel = GameObject.FindGameObjectWithTag("connectionPanel");
        killClickPanel = GameObject.FindGameObjectWithTag("killClickPanel");
        loginPanel = GameObject.FindGameObjectWithTag("loginPanel");
    }

    public bool getHasLogin() { return hasLogin; }
    public void setHasLogin(bool val) { hasLogin = val; }

    public void HelloWorld(string val)
    {
        Debug.Log("Hello World from " + val);
        try
        {
            connectionPanel.GetComponentsInChildren<TMP_Text>()[0].SetText("Connecting to " + val);
        }
        catch { }
    }

    public bool Connection(broadcastItem data)
    {
        hasLogin = false;

        if (killClickPanel != null)
        {
            killClickPanel.SetActive(false);
        }
        Debug.Log("Connecting to " + data.ip + ":" + data.port);
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
        ws = new WebSocketSharp.WebSocket("ws://" + data.ip + ":" + data.port);
        ws.OnMessage += (sender, e) => OnMessage(sender, e);
        ws.OnOpen += (sender, e) =>
        {
            mainThreadActions.Enqueue(() =>
            {
                killClickPanel = GameObject.FindGameObjectWithTag("killClickPanel");
                loginPanel = GameObject.FindGameObjectWithTag("loginPanel");
                Debug.Log("Connection opened");
                Debug.Log("" + (killClickPanel == null) + " - " + (loginPanel == null));
                robot = data;
                if (killClickPanel != null)
                {
                    killClickPanel.transform.GetChild(0).gameObject.SetActive(false);
                }
                Debug.Log("hasLogin: " + hasLogin + ", loginPanel == null: " + (loginPanel == null));
                if (!hasLogin && loginPanel != null)
                {
                     loginPanel.transform.GetChild(0).gameObject.SetActive(true);
                }
            });
        };
        ws.OnClose += (sender, e) =>
        {
            mainThreadActions.Enqueue(() =>
            {
                Debug.Log("Connection closed: " + e.Reason);
                robot = null;
                hasLogin = false;
                ConnectionBarSet();
                if (killClickPanel != null)
                {
                    killClickPanel.transform.GetChild(0).gameObject.SetActive(true);
                }
            });
        };
        ws.Connect();

        return true;
    }

    void OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
    {
        Debug.Log("Message received: " + e.Data);
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.Data);
        
        string command = data.ContainsKey("command") && data["command"] != null ? data["command"].ToString() : "";
        string msgData = data.ContainsKey("data") && data["data"] != null ? data["data"].ToString() : "";

        mainThreadActions.Enqueue(() =>
        {
            if (command == "login" && msgData == "pass")
            {
                hasLogin = true;
                if(loginPanel != null) loginPanel.SetActive(false);
            }
            else
            {
                GameObject consol = GameObject.FindGameObjectWithTag("ConsolContent");
                Debug.Log("Consol: " + (consol == null));
                if (consol != null)
                {
                    var logText = new GameObject("LogText");
                    logText.transform.SetParent(consol.transform, false);
                    logText.AddComponent<TextMeshProUGUI>();

                    var tmp = logText.GetComponent<TextMeshProUGUI>();
                    if (command == "log")
                    {
                        tmp.text = msgData;
                    }
                    else
                    {
                        tmp.text = "전달받음:    " + e.Data;
                    }
                    
                    tmp.fontSize = 18;

                    var filter = logText.AddComponent<ContentSizeFitter>();
                    filter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    filter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                    consol.GetComponentInParent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
                }
            }
        });
    }

    public void ConnectionBarSet()
    {
        string text = "연결 끊김";
        if (connectionPanel != null && robot != null)
        {
            if (hasLogin)
            {
                text = robot.name + "에 연결되었습니다.";
            }
            connectionPanel.GetComponentsInChildren<TMP_Text>()[0].SetText(text);
        }
        else if (connectionPanel != null)
        {
             connectionPanel.GetComponentsInChildren<TMP_Text>()[0].SetText(text);
        }
    }

    void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChange;
        }
    }

    private void OnSceneChange(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.Scene mode)
    {
        connectionPanel = GameObject.FindGameObjectWithTag("connectionPanel");
        ConnectionBarSet();
    }
    void Update()
    {
        while (mainThreadActions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }
}