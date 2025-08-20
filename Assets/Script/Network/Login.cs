using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    private static Client client;
    public TMP_InputField passwordInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [System.Obsolete]
    void Start()
    {
        client = FindObjectOfType<Client>();
    }

    // Update is called once per frame
    void Update()
    {
        if (client != null && client.getHasLogin())
        {
            SceneManager.LoadScene("Mainmenu");
        }
    }

    public void loginButtonClick()
    {
        client.ws.Send("{\"command\":\"login\", \"password\": \"" + passwordInput.text + "\"}" );
    }
}
