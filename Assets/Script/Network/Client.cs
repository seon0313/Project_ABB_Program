using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    private static Client instance;
    private static GameObject connectionPanel;
    private static GameObject killClickPanel;
    private static GameObject loginPanel;
    public WebSocketSharp.WebSocket ws;
    public static bool hasLogin = false;
    public broadcastItem robot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        connectionPanel = GameObject.FindGameObjectWithTag("connectionPanel");
        killClickPanel = GameObject.FindGameObjectWithTag("killClickPanel");
        loginPanel = GameObject.FindGameObjectWithTag("loginPanel");
    }
    
    public bool getHasLogin() { return hasLogin; }
    public void setHasLogin(bool val) { hasLogin = val;}

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
            killClickPanel = GameObject.FindGameObjectWithTag("killClickPanel");
            loginPanel = GameObject.FindGameObjectWithTag("loginPanel");
            Debug.Log("Connection opened");
            Debug.Log("" + (killClickPanel == null) + " - " + (loginPanel == null));
            robot = data;
            if (killClickPanel != null)
            {
                GameObject.FindGameObjectsWithTag("killClickPanel")[0].GetComponentInChildren<GameObject>().SetActive(false);
            }
            Debug.Log("hasLogin: " + hasLogin + ", loginPanel == null: " + (loginPanel == null));
            if (!hasLogin)
            {
                if (loginPanel != null)
                {
                    GameObject.FindGameObjectsWithTag("loginPanel")[0].GetComponentInChildren<GameObject>().SetActive(true);
                }
            }
        };
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("Connection closed: " + e.Reason);
            robot = null;
            hasLogin = false;
            ConnectionBarSet();
            if (killClickPanel != null)
            {
                killClickPanel.GetComponentInChildren<GameObject>().SetActive(true);
            }
        };
        ws.Connect();
        
        return true;
    }

    void OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
    {
        Debug.Log("Message received: " + e.Data);
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.Data);
        Debug.Log("Command: " + (data.ContainsKey("command") ? data["command"] : "null") + " Data: " + (data.ContainsKey("data") ? data["data"] : "null"));
        string command = data.ContainsKey("command") && data["command"] != null ? data["command"].ToString() : "";
        string msgData = data.ContainsKey("data") && data["data"] != null ? data["data"].ToString() : "";

        if (command == "login" && msgData == "pass")
        {
            hasLogin = true;
            loginPanel.SetActive(false);
        }

        if (command == "log")
        {
            Debug.Log("Log command received");
            GameObject consol = GameObject.FindGameObjectWithTag("ConsolContent");
            Debug.Log("Consol: " + (consol == null));
            if (consol != null)
            {
                var logText = new GameObject("LogText");
                var tmp = logText.AddComponent<TMP_Text>();
                tmp.text = msgData;
                tmp.fontSize = 18;
                logText.transform.SetParent(consol.transform, false);
            }
        }
    }

    public void ConnectionBarSet()
    {
        string text = "No device";
        if (connectionPanel != null && robot != null)
        {
            if (hasLogin)
            {
                text = "Connected to " + robot.name;
            }
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
