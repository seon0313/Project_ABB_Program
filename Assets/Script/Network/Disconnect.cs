using UnityEngine;

public class Disconnect : MonoBehaviour
{
    public static Client client;
    void Start()
    {
        client = FindObjectOfType<Client>();
    }

    public void DisconnectClient()
    {
        if (client != null && client.ws != null && client.ws.IsAlive)
        {
            Debug.Log("Disconnecting from server...");
            client.ws.Close();
            client.setHasLogin(false);
            client.ConnectionBarSet();
        }
    }
}
