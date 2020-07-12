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

    void LateUpdate() 
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
        int childsNeededX = (int)Mathf.Ceil(ScreenBounds.x * 3 / objectWidth);
        int childsNeededY = (int)Mathf.Ceil(ScreenBounds.y * 3 / objectHeight);

        GameObject emptyClone = Instantiate(obj);

        for (int i = -1; i <= childsNeededY; i++)
        {
            GameObject clone = Instantiate(emptyClone);
            clone.transform.SetParent(obj.transform);
            for (int j = -1; j <= childsNeededX; j++)
            {
                GameObject c = Instantiate(emptyClone);
                c.transform.SetParent(clone.transform);
                c.transform.position = new Vector3(objectWidth * j, objectHeight * i, obj.transform.position.z);
                c.name = obj.name + i + j;
            }

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
        float comparationHalfObjectWidth = halfObjectWidth * 0.5f;
        float halfObjectHeight = lastTestChild.GetComponent<SpriteRenderer>().bounds.extents.y;
        float comparationHalfObjectHeight = halfObjectHeight * 0.9f;

        if(transform.position.x + ScreenBounds.x > lastTestChild.transform.position.x + comparationHalfObjectWidth) 
        {
            needsAdjustX = 1;
        } 
        else if(transform.position.x - ScreenBounds.x < firstTestChild.transform.position.x - comparationHalfObjectWidth) 
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
        Transform topTestChild = topRoot.GetChild(0);
        Transform bottomRoot = obj.transform.GetChild(1);
        Transform bottomTestChild = bottomRoot.GetChild(0);
        
        if(transform.position.y + ScreenBounds.y > topTestChild.position.y + comparationHalfObjectHeight) 
        {
            needsAdjustY = 1;
        } 
        else if(transform.position.y - ScreenBounds.y < bottomTestChild.position.y - comparationHalfObjectHeight) 
        {
            needsAdjustY = -1;
        }

        if(needsAdjustY != 0) {
            Transform[] topChildren = topRoot.GetComponentsInChildren<Transform>();
            Transform[] bottomChildren = bottomRoot.GetComponentsInChildren<Transform>();

            if(needsAdjustY > 0)
            {
                float topY = topChildren[0].transform.position.y;
                for (int i = 0; i < bottomChildren.Length; i++)
                {
                    Transform child = bottomChildren[i];
                    child.transform.position = new Vector3(child.transform.position.x, topY + halfObjectHeight * 2, child.transform.position.z);
                }
                bottomRoot.SetAsLastSibling();
            }
            else
            {
                float bottomY = bottomChildren[0].transform.position.y;
                for (int i = 0; i < topChildren.Length; i++)
                {
                    Transform child = topChildren[i];
                    child.transform.position = new Vector3(child.transform.position.x, bottomY - halfObjectHeight * 2, child.transform.position.z);
                }
                topRoot.SetAsFirstSibling();
            }
        }
    }
}