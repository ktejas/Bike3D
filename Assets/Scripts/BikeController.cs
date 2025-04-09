using UnityEngine;
//using UnityEngine.InputSystem;

public class BikeController : MonoBehaviour
{

    float moveInput, steerInput, currentVelocityOffset;

    [HideInInspector]
    public Vector3 velocity;
    public float maxSpeed, acceleration, steerStrength, gravity, bikeXTiltInrement = 0.9f, zTiltAngle = 45f, handleRotation = 30f, handleRotationSpeed = 0.15f, skidmarkWidth = 0.062f, minSkidmarkVelocity = 10f, tyreRotSpeed = 10000f, normalDrag = 2f, driftDrag = 0.5f;

    [Range(1,10)]
    public float brakingFactor;

    float rayLength;
    RaycastHit hit;
    public LayerMask drivableSurface;
    public TrailRenderer skidmarksTrailRenderer;
    public AudioSource engineSound;
    public AudioSource skidSound;
    [Range(0,1)]
    public float minPitch;
    [Range(1,5)]
    public float maxPitch;

    public Rigidbody sphereRB, bikeBodyRB;
    public GameObject handle;

    public GameObject frontTyre, backTyre;

    public ParticleSystem smokePartSystem;
    public AnimationCurve turningCurve;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sphereRB.transform.parent = null;
        bikeBodyRB.transform.parent = null;

        rayLength = sphereRB.GetComponent<SphereCollider>().radius + 0.2f;

        skidmarksTrailRenderer.startWidth = skidmarkWidth;
        skidmarksTrailRenderer.emitting = false;
        skidSound.mute = true;

        Smoke();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        //moveInput = InputSystem.actions.FindAction("Move").ReadValue<UnityEngine.Vector2>().x;
        //steerInput = InputSystem.actions.FindAction("Move").ReadValue<UnityEngine.Vector2>().y;

        transform.position = sphereRB.transform.position;

        velocity = bikeBodyRB.transform.InverseTransformDirection(bikeBodyRB.linearVelocity);
        currentVelocityOffset = velocity.z / maxSpeed;
    }

    private void FixedUpdate()
    {
        Movement();

        //Visuals
        Skidmarks();

        //Sounds
        EngineSound();

        //Type Rotation
        frontTyre.transform.Rotate(Vector3.right, tyreRotSpeed * Time.deltaTime * moveInput, Space.Self);
        backTyre.transform.Rotate(Vector3.right, tyreRotSpeed * Time.deltaTime * moveInput, Space.Self);

        Smoke();
    }

    private void Movement()
    {
        if(IsGrounded())
        {
            //if(Keyboard.current[Key.Space].wasPressedThisFrame)
            if(!Input.GetKey(KeyCode.Space))
            {
                Acceleration();
            }
            Rotation();
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
        transform.Rotate(0, steerInput * moveInput * turningCurve.Evaluate(Mathf.Abs(currentVelocityOffset)) * steerStrength * Time.deltaTime, 0f,  Space.World);

        handle.transform.localRotation = Quaternion.Slerp(handle.transform.localRotation, Quaternion.Euler(handle.transform.localRotation.eulerAngles.x, handleRotation * steerInput, handle.transform.localRotation.eulerAngles.z), handleRotationSpeed);
    }

    private void Brake()
    {
        //if(Keyboard.current[Key.Space].wasPressedThisFrame)
        if(Input.GetKey(KeyCode.Space))
        {
            sphereRB.linearVelocity *= brakingFactor / 10f;
            sphereRB.linearDamping = driftDrag;
        }
        else
        {
            sphereRB.linearDamping = normalDrag;
        }
    }

    bool IsGrounded()
    {
        float radius = rayLength - 0.02f;
        Vector3 origin = sphereRB.transform.position + Vector3.up * radius;

        //if(Physics.Raycast(sphereRB.position, Vector3.down, out hit, rayLength, drivableSurface))
        if(Physics.SphereCast(origin, radius + 0.2f, -transform.up, out hit, rayLength, drivableSurface))
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

    void Skidmarks()
    {
        if(IsGrounded() && Mathf.Abs(velocity.x) > minSkidmarkVelocity || Input.GetKey(KeyCode.Space))
        //if(IsGrounded() && Mathf.Abs(velocity.x) > minSkidmarkVelocity || Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            skidmarksTrailRenderer.emitting = true;
            skidSound.mute = false;
        }
        else
        {
            skidmarksTrailRenderer.emitting = false;
            skidSound.mute = true;
        }
    }

    void EngineSound()
    {
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(currentVelocityOffset));
    }

    void Smoke()
    {
        if(skidmarksTrailRenderer.emitting)
        {
            smokePartSystem.Play();
        }
        else if(!skidmarksTrailRenderer.emitting)
        {
            smokePartSystem.Stop();
        }
    }

    public void ExternalAccPress()
    {
        moveInput = 1;
    }

    public void ExternalAccRelease()
    {
        moveInput = 0;
    }

    public void ExternalBrakePress()
    {
        sphereRB.linearVelocity *= brakingFactor / 10f;
        sphereRB.linearDamping = driftDrag;
    }

    public void ExternalBrakeRelease()
    {
        sphereRB.linearDamping = normalDrag;
    }

    public void ExternalLeftPress()
    {
        steerInput = -1;
    }

    public void ExternalLeftRelease()
    {
        steerInput = 0;
    }

    public void ExternalRightPress()
    {
        steerInput = 1;
    }

    public void ExternalRightRelease()
    {
        steerInput = 0;
    }
}