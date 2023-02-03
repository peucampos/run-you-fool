using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
    //Movement & Physics
    public float gravityForce;
    public float ySpeed; //vertical speed
    public float jumpForce;
    public float hangTime; //maximun height based on holding the jump
    public float hangTimer;
    public float gravityModifier; //to allow higher jumps
    public float forwardSpeed;
    public float runSpeed;
    public float lerpTime; //time from current to run
    public bool idle;

    private CharacterController myController;
    
    //Wall Jump
    private Quaternion myRotation; //rotation of the player
    public bool hasJump;
    
    //Coin Pickup
    public int coinScore;
    public TMP_Text coinText;

    //Animation
    public Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        myController = GetComponent<CharacterController>();
        myRotation = transform.rotation;   
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AnimationApply();
        MyGravity();
        Jump();
        ForwardMovement();
        SpeedApply();
    }

    private void AnimationApply()
    {
        myAnimator.SetBool("Walking", !idle);
    }

    void Update() 
    {
        if (Input.GetButtonUp("Jump"))
            hasJump = false;

        if (Input.GetButtonDown("Fire1"))
            idle = !idle;
    }

    void MyGravity()
    {
        ySpeed = myController.velocity.y;
        ySpeed -= gravityForce * Time.deltaTime;
    }

    void Jump()
    {
        if (Input.GetButton("Jump"))
        {
            if (myController.isGrounded && !hasJump)
            {
                hasJump = true;
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
            if (idle)
            {
                forwardSpeed = 0;
            }
            else
            {
                if (forwardSpeed <= runSpeed - 0.1f || forwardSpeed >= runSpeed + 0.1f)
                    forwardSpeed = Mathf.Lerp(forwardSpeed, runSpeed, lerpTime);
                else
                    forwardSpeed = runSpeed;
            }
        }
    }

    void SpeedApply()
    {
        myController.Move(transform.forward * forwardSpeed * Time.deltaTime);
        myController.Move(new Vector3(0f, ySpeed, 0f) * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        //had to change Character Controller Skin Width to 0.0001 to fix isGrounded
        GroundLanding();
        WallJump(hit);
    }

    void GroundLanding()
    {
        if (myRotation != transform.rotation && myController.isGrounded)
        {
            transform.rotation = myRotation;
        }
    }

    void WallJump(ControllerColliderHit hitSent)    
    {
        if (!idle)
        {
            if (!myController.isGrounded && hitSent.normal.y < 0.1f && hitSent.normal.y > -0.1f)
            {
                if (Input.GetButton("Jump") && !hasJump)
                {
                    hasJump = true;
                    transform.forward = hitSent.normal;
                    transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                    forwardSpeed = runSpeed;
                    hangTimer = hangTime;
                    ySpeed = jumpForce;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.transform.tag == "Coin")
        {
            coinScore++;
            coinText.text = coinScore.ToString();
            Destroy(other.gameObject);            
            
        }        
    }
}
