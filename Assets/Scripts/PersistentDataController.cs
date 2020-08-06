using UnityEngine;

public class PersistentDataController : MonoBehaviour
{
    public string UserName = null;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}