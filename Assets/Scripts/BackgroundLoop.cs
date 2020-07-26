using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    public GameObject[] Objects;
    private Camera MainCamera;
    private Vector2 ScreenBounds;

    void Awake()
    {
        MainCamera = UnityEngine.Object.FindObjectOfType<Camera>();
        ScreenBounds = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z));
        foreach (var obj in Objects)
        {
            LoadChildObjects(obj);
        }
    }

    void Update() 
    {
        foreach (var obj in Objects)
        {
            RepositionChildObjects(obj);
        }
    }

    void LoadChildObjects(GameObject obj)
    {
        float objectWidth = obj.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        float objectHeight = obj.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        int childsNeededX = (int)Mathf.Ceil(ScreenBounds.x * 4 / objectWidth);
        int childsNeededY = (int)Mathf.Ceil(ScreenBounds.y * 4 / objectHeight);

        GameObject emptyClone = Instantiate(obj);

        for (int i = -childsNeededY; i <= childsNeededY; i++)
        {
            GameObject clone = Instantiate(emptyClone);
            clone.transform.SetParent(obj.transform);
            for (int j = -childsNeededX; j <= childsNeededX; j++)
            {
                GameObject c = Instantiate(emptyClone);
                c.transform.SetParent(clone.transform);
                c.transform.position = new Vector3(objectWidth * j, 0, obj.transform.position.z);
                c.name = obj.name + i + j;
            }
            clone.transform.position = new Vector3(0, objectHeight * i, obj.transform.position.z);

            Destroy(clone.GetComponent<SpriteRenderer>());
        }

        Destroy(obj.GetComponent<SpriteRenderer>());
        Destroy(emptyClone);
    }

    void RepositionChildObjects(GameObject obj) 
    {
        int needsAdjustX = 0;

        Transform[] testChildren = obj.transform.GetChild(0).GetComponentsInChildren<Transform>();
        GameObject firstTestChild = testChildren[1].gameObject;
        GameObject lastTestChild = testChildren[testChildren.Length - 1].gameObject;

        float halfObjectWidth = lastTestChild.GetComponent<SpriteRenderer>().bounds.extents.x;
        float halfObjectHeight = lastTestChild.GetComponent<SpriteRenderer>().bounds.extents.y;

        if(transform.position.x + ScreenBounds.x > lastTestChild.transform.position.x) 
        {
            needsAdjustX = 1;
        } 
        else if(transform.position.x - ScreenBounds.x < firstTestChild.transform.position.x) 
        {
            needsAdjustX = -1;
        }

        if(needsAdjustX != 0) 
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);

                Transform[] children = child.GetComponentsInChildren<Transform>();
                GameObject firstChild = children[1].gameObject;
                GameObject lastChild = children[children.Length - 1].gameObject;
                if(needsAdjustX > 0)
                {
                    firstChild.transform.SetAsLastSibling();
                    firstChild.transform.position = new Vector3(lastChild.transform.position.x + halfObjectWidth * 2, firstChild.transform.position.y, firstChild.transform.position.z);
                }
                else
                {
                    lastChild.transform.SetAsFirstSibling();
                    lastChild.transform.position = new Vector3(firstChild.transform.position.x - halfObjectWidth * 2, lastChild.transform.position.y, lastChild.transform.position.z);
                }
            }
        }

        int needsAdjustY = 0;

        Transform topRoot = obj.transform.GetChild(obj.transform.childCount - 1);
        Transform bottomRoot = obj.transform.GetChild(0);
        
        if(transform.position.y + ScreenBounds.y > topRoot.position.y) 
        {
            needsAdjustY = 1;
        } 
        else if(transform.position.y - ScreenBounds.y < bottomRoot.position.y) 
        {
            needsAdjustY = -1;
        }

        if(needsAdjustY != 0) 
        {
            if(needsAdjustY > 0)
            {
                bottomRoot.transform.position = new Vector3(bottomRoot.transform.position.x, topRoot.transform.position.y + halfObjectHeight * 2, bottomRoot.transform.position.z);
                bottomRoot.SetAsLastSibling();
            }
            else
            {
                topRoot.transform.position = new Vector3(topRoot.transform.position.x, bottomRoot.transform.position.y - halfObjectHeight * 2, topRoot.transform.position.z);
                topRoot.SetAsFirstSibling();
            }
        }
    }
}