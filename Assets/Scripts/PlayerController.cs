using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController myController;
    public float gravityForce;
    public float ySpeed; //vertical speed
    public float jumpForce;
    public float hangTime; //maximun height based on holding the jump
    public float hangTimer;
    public float gravityModifier; //to allow higher jumps
    public float forwardSpeed;
    public float runSpeed;
    public float lerpTime; //time from current to run

    // Start is called before the first frame update
    void Start()
    {
        myController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MyGravity();
        Jump();
        ForwardMovement();
        SpeedApply();
    }

    void MyGravity()
    {
        ySpeed = myController.velocity.y;
        ySpeed -= gravityForce * Time.deltaTime;
    }

    void Jump()
    {
        if (Input.GetButton("Fire1"))
        {
            if (myController.isGrounded)
            {
                hangTimer = hangTime;
                ySpeed = jumpForce;            
            }
            else
            {
                if (hangTimer > 0)
                {
                    hangTimer -= Time.deltaTime;
                    ySpeed += gravityModifier * hangTimer * Time.deltaTime;
                }
            }
        }
    }

    void ForwardMovement()
    {
        if (myController.isGrounded)
        {
            if (forwardSpeed <= runSpeed - 0.1f || forwardSpeed >= runSpeed + 0.1f)
                forwardSpeed = Mathf.Lerp(forwardSpeed, runSpeed, lerpTime);
            else
                forwardSpeed = runSpeed;
        }
    }

    void SpeedApply()
    {
        myController.Move(transform.forward * forwardSpeed * Time.deltaTime);
        myController.Move(new Vector3(0f, ySpeed, 0f) * Time.deltaTime);
    }
}
