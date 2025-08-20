using UnityEngine;

public class ClientControl : MonoBehaviour
{
    public static Client client;
    public GameObject loginPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        client = FindObjectOfType<Client>();
    }

    // Update is called once per frame

    public void Login(string pw)
    {
        client.SendMessage("{\"command\":\"login\",\"password\":\"" + pw + "\"}");
    }

    void Update()
    {
        if (client != null && client.getHasLogin())
        {
            // Load the main menu scene when login is successful
            UnityEngine.SceneManagement.SceneManager.LoadScene("Mainmenu");
        }
        if (client != null && client.ws != null && client.ws.IsAlive && !client.getHasLogin())
        {
            loginPanel.SetActive(true);
        }
    }
}
