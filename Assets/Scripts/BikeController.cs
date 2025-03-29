using UnityEngine;

public class BikeController : MonoBehaviour
{

    float moveInput;
    public float maxSpeed, acceleration;

    public Rigidbody sphereRB, bikeBodyRB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sphereRB.transform.parent = null;
        bikeBodyRB.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        transform.position = sphereRB.transform.position;
        bikeBodyRB.MoveRotation(transform.rotation);
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        sphereRB.linearVelocity = Vector3.Lerp(sphereRB.linearVelocity, maxSpeed * moveInput * transform.forward, acceleration * Time.fixedDeltaTime);
    }
}
