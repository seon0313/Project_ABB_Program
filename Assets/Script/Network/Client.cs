using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    private static Client instance;
    private static GameObject connectionPanel;
    private static GameObject killClickPanel;
    private static GameObject loginPanel;
    public WebSocketSharp.WebSocket ws;
    private static bool hasLogin = false;
    public broadcastItem robot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        connectionPanel = GameObject.FindGameObjectWithTag("connectionPanel");
        killClickPanel = GameObject.FindGameObjectWithTag("killClickPanel");
        loginPanel = GameObject.FindGameObjectWithTag("loginPanel");
    }

    public void HelloWorld(string val)
    {
        Debug.Log("Hello World from " + val);
        try
        {
            connectionPanel.GetComponentsInChildren<TMP_Text>()[0].SetText("Connecting to " + val);
        } catch {}
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
            Debug.Log("Connection opened");
            robot = data;
            if (killClickPanel != null)
            {
                killClickPanel.SetActive(true);
            }
            if (!hasLogin)
            {
                if (loginPanel != null)
                {
                    loginPanel.SetActive(true);
                }
            }
        };
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("Connection closed: " + e.Reason);
            robot = null;
            if (killClickPanel != null)
            {
                killClickPanel.SetActive(true);
            }
        };
        ws.Connect();
        
        return true;
    }

    void OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
    {
        Debug.Log("Message received: " + e.Data);
    }

    void ConnectionBarSet()
    {
        if (connectionPanel != null && robot != null)
        {
            connectionPanel.GetComponentsInChildren<TMP_Text>()[0].SetText(robot.name);
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
        killClickPanel = GameObject.FindGameObjectWithTag("killClickPanel");
        loginPanel = GameObject.FindGameObjectWithTag("loginPanel");
        ConnectionBarSet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
