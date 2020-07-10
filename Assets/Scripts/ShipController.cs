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
        if (Input.GetKey("space"))
        {
            Body.AddForce(Time.deltaTime * 500.0f * Vector2FromAngle(Body.rotation));
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Body.AddTorque(2);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Body.AddTorque(-2);
        }
    }

    private Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }
}
