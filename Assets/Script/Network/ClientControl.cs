using UnityEngine;

public class ClientControl : MonoBehaviour
{
    public static Client client;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        client = FindObjectOfType<Client>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
