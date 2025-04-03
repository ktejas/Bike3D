using UnityEngine;

public class BikeController : MonoBehaviour
{

    float moveInput, steerInput, currentVelocityOffset;

    [HideInInspector]
    public Vector3 velocity;
    public float maxSpeed, acceleration, steerStrength, gravity, bikeXTiltInrement = 0.9f, zTiltAngle = 45f, handleRotation = 30f, handleRotationSpeed = 0.15f;
    [Range(1,10)]
    public float brakingFactor;

    float rayLength;
    RaycastHit hit;
    public LayerMask drivableSurface;

    public Rigidbody sphereRB, bikeBodyRB;
    public GameObject handle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sphereRB.transform.parent = null;
        bikeBodyRB.transform.parent = null;

        rayLength = sphereRB.GetComponent<SphereCollider>().radius + 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");

        transform.position = sphereRB.transform.position;

        velocity = bikeBodyRB.transform.InverseTransformDirection(bikeBodyRB.linearVelocity);
        currentVelocityOffset = velocity.z / maxSpeed;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        if(IsGrounded())
        {
            if(!Input.GetKey(KeyCode.Space))
            {
                Acceleration();
                Rotation();
            }
            Brake();
        }
        else
        {
            Gravity();
        }  
        BikeTilt(); 
    }

    private void Acceleration()
    {
        sphereRB.linearVelocity = Vector3.Lerp(sphereRB.linearVelocity, maxSpeed * moveInput * transform.forward, acceleration * Time.fixedDeltaTime);
    }

    private void Rotation()
    {
        transform.Rotate(0, steerInput * moveInput * currentVelocityOffset * steerStrength * Time.deltaTime, 0f,  Space.World);

        handle.transform.localRotation = Quaternion.Slerp(handle.transform.localRotation, Quaternion.Euler(handle.transform.localRotation.eulerAngles.x, handleRotation * steerInput, handle.transform.localRotation.eulerAngles.z), handleRotationSpeed);
    }

    private void Brake()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            sphereRB.linearVelocity *= brakingFactor / 10f;
        }
    }

    bool IsGrounded()
    {
        if(Physics.Raycast(sphereRB.position, Vector3.down, out hit, rayLength, drivableSurface))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Gravity()
    {
        sphereRB.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    void BikeTilt()
    {
        float xRotation = (Quaternion.FromToRotation(bikeBodyRB.transform.up, hit.normal) * bikeBodyRB.transform.rotation).eulerAngles.x;
        float zRotation = 0;

        if(currentVelocityOffset > 0)
        {
            zRotation = -zTiltAngle * steerInput * currentVelocityOffset;
        }
        

        Quaternion targetRotation = Quaternion.Slerp(bikeBodyRB.transform.rotation, Quaternion.Euler(xRotation, transform.eulerAngles.y, zRotation), bikeXTiltInrement);

        Quaternion newRotation = Quaternion.Euler(targetRotation.eulerAngles.x, transform.rotation.eulerAngles.y, targetRotation.eulerAngles.z);
       
        bikeBodyRB.MoveRotation(newRotation);
    }
}