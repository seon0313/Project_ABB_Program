using UnityEngine;

public class itemScript : MonoBehaviour
{
    public string key;
    public string value;

    public string getkeyAndValueString(bool isFirst)
    {

        if (!string.IsNullOrEmpty(key))
        {
            string first = "";
            if (!isFirst) first = ", ";
            return first + "\""+ key + "\": \"" + value + "\"";
        }
        return "";
    }
}
