﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    public bool facingRight = true;
    public bool jump = false;
    public bool throwKnife = false;
    private bool drawVector = false;

    public float maxSpeed = 5f;
    public float jumpForce = 100f;        
    public float moveForce = 3650f;          

    private Transform groundCheck;          
    private bool grounded = true; 
    private Animator anim;
    private Rigidbody2D body;
    private LineRenderer lineRenderer;

    private Vector2 mouseClickPosition;
    private Vector2 throwVector;

    private CircleCollider2D knifeTopCollider;

    void Awake()
    {
        this.groundCheck = transform.Find("groundCheck");
        this.anim = GetComponent<Animator>();
        this.body = GetComponent<Rigidbody2D>();
        this.lineRenderer = GetComponent<LineRenderer>();
        this.knifeTopCollider = transform.Find("knife_top").GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        // If the jump button is pressed and the player is grounded then the player should jump.
        if (Input.GetButtonDown("Jump") && grounded)
        {
            this.jump = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            this.mouseClickPosition = Input.mousePosition;
            drawVector = true;
        }

        if(Input.GetMouseButtonUp(0)){
            this.throwVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - mouseClickPosition;
            lineRenderer.SetPosition(0, new Vector2(0, 0));
            lineRenderer.SetPosition(1, new Vector2(0, 0));
            drawVector = false;
            throwKnife = true;
        }
   }

    void FixedUpdate()
    {
        // Cache the horizontal input.
        float h = Input.GetAxis("Horizontal");

        // The Speed animator parameter is set to the absolute value of the horizontal input.
        anim.SetFloat("Speed", Mathf.Abs(h));

        // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
        if (h * body.velocity.x < maxSpeed)
            // ... add a force to the player.
            body.AddForce(Vector2.right * h * moveForce);

        // If the player's horizontal velocity is greater than the maxSpeed...
        if (Mathf.Abs(body.velocity.x) > maxSpeed)
            // ... set the player's velocity to the maxSpeed in the x axis.
            body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * maxSpeed, body.velocity.y);

        // If the input is moving the player right and the player is facing left...
        if (h > 0 && !facingRight)
            Flip();
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (h < 0 && facingRight)
            Flip();

        if (jump)
            Jump();

        if(drawVector)
            DrawVector();

        if(throwKnife)
            ThrowKnife();
        
    }

    private void DrawVector(){
        Vector2 start = Camera.main.ScreenToWorldPoint(this.mouseClickPosition);
        Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void ThrowKnife(){
        Debug.Log(throwVector);
        throwVector.Scale(new Vector2(4f, 4f));
        body.freezeRotation = false;
        
        Vector3 knifeColliderPosition = knifeTopCollider.transform.position;
        Debug.Log("Knife: " + knifeColliderPosition + " | Throw: " + throwVector);
        body.bodyType = RigidbodyType2D.Dynamic;

        body.AddForceAtPosition(throwVector, knifeColliderPosition);
        throwKnife = false;
    }

    private void Jump(){
        anim.SetTrigger("Jump");
        body.AddForce(new Vector2(0f, jumpForce));
        this.jump = false;
    }

    void Flip(){
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    //make sure u replace "floor" with your gameobject name.on which player is standing
    void OnCollisionEnter2D(Collision2D theCollision)
    {
        body.rotation = 0;
        body.freezeRotation = true;
        string collisionObjectTag = theCollision.gameObject.tag;
        Debug.Log("Collision object name: " + collisionObjectTag);
        switch (collisionObjectTag)
        {
            case "Ground":
                grounded |= true;
                break;
            case "Plank":
                Debug.Log("Magnitude: " + body.velocity.magnitude);

                if (theCollision.otherCollider == knifeTopCollider && body.velocity.magnitude > 1)
                {
                    Debug.Log("Jebło ostrzem w ścianę.");
                    body.bodyType = RigidbodyType2D.Static;
                }
                break;
        }
    }

    //consider when character is jumping .. it will exit collision.
    void OnCollisionExit2D(Collision2D theCollision)
    {
        grounded &= theCollision.gameObject.name != "floor";
    }
}
