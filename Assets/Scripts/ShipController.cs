using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float ShotCooldownTime = 1f;
    public GameObject Shot;
    public GameObject LookAhead;

    private Rigidbody2D Body;
    private bool Shoot = false;
    private float DesiredRotaton = 0f;
    private float DesiredMovement = 0f;

    private float ShotCooldown = 0f;

    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        DesiredRotaton = Input.GetAxis("Horizontal");
        DesiredMovement = Mathf.Max(0f, Input.GetAxis("Vertical"));
        Shoot = Input.GetButtonUp("Shoot");
    }

    void FixedUpdate()
    {
        Body.AddForce(DesiredMovement * Time.deltaTime * 500.0f * Vector2FromAngle(Body.rotation));
        Body.AddTorque(-2 * DesiredRotaton);

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
