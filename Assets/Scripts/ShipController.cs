using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    private Rigidbody2D Body;

    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown("space"))
        {
            Body.AddForce(new Vector2(10000.0f * Time.deltaTime, 0.0f));
        }
    }
}
