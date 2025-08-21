using UnityEngine;

public class Movement : MonoBehaviour
{
    public static Client client;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        client = FindObjectOfType<Client>();
    }

    public void Move(string direction)
    {
        if (client != null && client.ws != null && client.ws.IsAlive)
        {
            string message = "{\"command\":\"move\",\"direction\":\"" + direction + "\", \"value\": 1}";
            client.ws.Send(message);
        }
    }
}