using UnityEngine;

public class delete : MonoBehaviour
{
    public void deleteSelf()
    {
        Destroy(gameObject);
    }
    public void deleteParent()
    {
        Destroy(transform.parent.gameObject);
    }
}
