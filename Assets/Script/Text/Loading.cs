using TMPro;
using UnityEngine;

public class Loading : MonoBehaviour
{
    public TMP_Text targettext;
    public string text;
    public string loadingText;
    public float speed = 1;
    public int length = 3;

    private float time = 0;
    private int count = 0;
    private string value = "";

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targettext.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - time > speed)
        {
            time = Time.time;
            count++;
            if (count < length)
            {
                value += loadingText;
            }
            if (count >= length)
            {
                value = "";
                count = 0;
            }
            targettext.text = text + value;
        }
    }
}
