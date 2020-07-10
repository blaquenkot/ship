using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float ShotCooldownTime = 1f;
    public GameObject Shot;
    public GameObject LookAhead;

    private Rigidbody2D Body;
    private bool Shoot = false;
    private float ShotCooldown = 0f;

    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Shoot")) 
        {
            Shoot = true;
        } 
        else if(Input.GetButtonUp("Shoot")) 
        {
            Shoot = false;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
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

        ShotCooldown -= Time.deltaTime;
        if(ShotCooldown <= 0) 
        {
            if (Shoot) 
            {
                Vector2 lookAheadPosition = LookAhead.transform.position;
                ShotController shot = Instantiate(Shot, LookAhead.transform.position, Quaternion.identity, transform.parent).GetComponent<ShotController>();
                shot.Fire(lookAheadPosition.normalized, 0f);

                ShotCooldown = ShotCooldownTime;
            }
        }
    }

    private Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }
}
