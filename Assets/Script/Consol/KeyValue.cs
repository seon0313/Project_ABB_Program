using System;
using TMPro;
using UnityEngine;

public class KeyValue : MonoBehaviour
{
    public TMP_InputField keyInput;
    public TMP_InputField valueInput;
    private Client client;
    public GameObject context;
    public GameObject targetItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Obsolete]
    void Start()
    {
        client = FindObjectOfType<Client>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClickClear()
    {
        var consol = GameObject.FindGameObjectWithTag("ConsolContent");
        if (consol == null) return;
        foreach (var item in consol.GetComponentsInChildren<Transform>())
        {
            if (item != consol.transform)
            {
                Destroy(item.gameObject);
            }
        }
    }

    public void onClickAdd()
    {
        string key = keyInput.text;
        string value = valueInput.text;

        var item_go = Instantiate(targetItem, context.transform);
        item_go.tag = "DeviceItem"; // Ensure the new item has the correct tag
        item_go.name = key;
        item_go.SetActive(true); // Activate the item if it was inactive
        item_go.GetComponent<itemScript>().key = key;
        item_go.GetComponent<itemScript>().value = value;
        // do something with the instantiated item -- for instance
        item_go.GetComponentsInChildren<TMP_Text>()[0].SetText(key);
        item_go.GetComponentsInChildren<TMP_Text>()[1].SetText(value);
        //parent the item to the content container
        item_go.transform.SetParent(context.transform, false);
        item_go.transform.localScale = Vector2.one;
    }

    public void onClickSubmit()
    {
        if (client != null && context != null)
        {
            var items = context.GetComponentsInChildren<itemScript>();
            string result = "{";
            bool first = true;
            foreach (var item in items)
            {
                result += item.getkeyAndValueString(first);
                first = false;
            }
            result += "}";
            client.ws.Send(result);
        }
    }
}
