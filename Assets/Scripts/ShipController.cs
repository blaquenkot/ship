using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    private Vector2 speed;
    private float acceleration;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) {
            this.speed += new Vector2(1.0f, 0.0f);
        }

        this.transform.localPosition = (Vector2)this.transform.localPosition + this.speed * Time.deltaTime;
    }
}
