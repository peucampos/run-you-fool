using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
    private CharacterController myController;

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
        
    //Wall Jump
    private Quaternion myRotation; //rotation of the player
    public bool hasJump;
    
    //Coin Pickup
    public int coinScore;
    public TMP_Text coinText;

    //Animation
    public Animator myAnimator;
    private const string ANIM_XSPEED = "XSpeed";
    private const string ANIM_YSPEED = "YSpeed";
    private const string ANIM_ISGROUNDED= "IsGrounded";
    private const string ANIM_ISWALLED = "IsWalled";
    
    void Start()
    {
        myController = GetComponent<CharacterController>();
        myRotation = transform.rotation;   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MyGravity();
        Jump();
        ForwardMovement();
        SpeedApply();
        //GroundLanding();
    }

    void Update() 
    {
        if (Input.GetButtonUp("Fire1"))
            hasJump = false;

        myAnimator.SetBool(ANIM_ISGROUNDED, myController.isGrounded);
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
            if (forwardSpeed <= runSpeed - 0.1f || forwardSpeed >= runSpeed + 0.1f)
                forwardSpeed = Mathf.Lerp(forwardSpeed, runSpeed, lerpTime);
            else
                forwardSpeed = runSpeed;
        }
    }

    void SpeedApply()
    {
        myController.Move(transform.forward * forwardSpeed * Time.deltaTime);   
        myAnimator.SetFloat(ANIM_XSPEED, myController.velocity.x);
        myController.Move(new Vector3(0f, ySpeed, 0f) * Time.deltaTime);
        myAnimator.SetFloat(ANIM_YSPEED, myController.velocity.y);
    }
    void GroundLanding()
    {
        if (myRotation != transform.rotation && myController.isGrounded)
        {
            transform.rotation = myRotation;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        WallJump(hit);
    }

    void WallJump(ControllerColliderHit hitSent)    
    {
        if (!myController.isGrounded && hitSent.normal.y < 0.1f && hitSent.normal.y > -0.1f)
        {
            myAnimator.SetBool(ANIM_ISWALLED, true);
            if (Input.GetButton("Fire1") && !hasJump)
            {
                hasJump = true;
                transform.forward = hitSent.normal;
                transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                forwardSpeed = runSpeed;
                hangTimer = hangTime;
                ySpeed = jumpForce;
                myAnimator.SetBool(ANIM_ISWALLED, false);
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
