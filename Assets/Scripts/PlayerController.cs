using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool facingRight = true;
    public bool jump = false;
    public bool throwKnife = false;
    private bool drawVector = false;

    public float maxSpeed = 5f;
    public float jumpForce = 100f;
    public float moveForce = 3650f;

    private bool grounded = true;
    private Animator anim;
    private Rigidbody2D body;
    private LineRenderer lineRenderer;

    private Vector2 mouseClickPosition;
    private Vector2 throwVector;
    private Vector2 targetPosition;

    private Collider2D knifeTopCollider;

    void Awake()
    {
        this.anim = GetComponent<Animator>();
        this.body = GetComponent<Rigidbody2D>();
        this.lineRenderer = GetComponent<LineRenderer>();
        this.knifeTopCollider = transform.Find("knife_top").GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            this.jump = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            this.mouseClickPosition = Input.mousePosition;
            drawVector = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            this.targetPosition = Input.mousePosition;
            this.throwVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - mouseClickPosition;
            drawVector = false;
            throwKnife = true;
            DrawVector();
        }

        if (Input.GetButtonDown("Vertical"))
        {
            GroundPlayer();
        }
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        HandlePlayerAcceleration(horizontalInput);
        HandlePlayerMaxVelocity();
        HandleFaceRotation(horizontalInput);

        if (jump)
            Jump();
        
        if (drawVector)
            DrawVector(this.mouseClickPosition, Input.mousePosition);

        if (throwKnife)
            ThrowKnife();

        if (Input.GetMouseButtonDown(1))
        {
            body.bodyType = RigidbodyType2D.Dynamic;
            //body.AddTorque(Mathf.Sign(body.rotation) * 70f);

            Vector2 transformation = Quaternion.Euler(0, 0, 180 + body.rotation) * new Vector2(0, 0.1f);
            body.position += transformation;
            body.angularVelocity += - Mathf.Sign(body.rotation) * 50f;
            
        }
    }

    private void HandlePlayerAcceleration(float horizontalVelocity){
        // The Speed animator parameter is set to the absolute value of the horizontal input.
        anim.SetFloat("Speed", Mathf.Abs(horizontalVelocity));

        // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
        if (horizontalVelocity * body.velocity.x < maxSpeed)
            // ... add a force to the player.
            body.AddForce(Vector2.right * horizontalVelocity * moveForce);
    }

    private void HandlePlayerMaxVelocity(){
        // If the player's horizontal velocity is greater than the maxSpeed...
        if (Mathf.Abs(body.velocity.x) > maxSpeed)
            body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * maxSpeed, body.velocity.y);
    }

    private void HandleFaceRotation(float horizontalInput){
        // If the input is moving the player right and the player is facing left...
        if (horizontalInput > 0 && !facingRight)
            Flip();
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (horizontalInput < 0 && facingRight)
            Flip();
    }

    private void DrawVector(Vector2 rawStart = new Vector2(), Vector2 rawEnd = new Vector2())
    {
        Vector2 start = Camera.main.ScreenToWorldPoint(rawStart);
        Vector2 end = Camera.main.ScreenToWorldPoint(rawEnd);
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void ThrowKnife()
    {
        body.freezeRotation = false;
        Vector2 knifeColliderPosition = knifeTopCollider.transform.position;
        body.AddForce(throwVector * 7f);
        body.angularVelocity += - Mathf.Sign(throwVector.x) * 100f;

        throwKnife = false;
    }

    private void Jump()
    {
        anim.SetTrigger("Jump");
        body.AddForce(new Vector2(0f, jumpForce));
        this.jump = false;
    }

    void Flip()
    {
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void GroundPlayer(){
        body.bodyType = RigidbodyType2D.Dynamic;
        body.rotation = 0;
        body.freezeRotation = true;
        grounded |= true;
    }

    //make sure u replace "floor" with your gameobject name.on which player is standing
    void OnCollisionEnter2D(Collision2D theCollision)
    {
        switch (theCollision.gameObject.tag)
        {
            case "Stone":
            case "Ground":
                GroundPlayer();
                break;
        }
    }

    void OnCollisionExit2D(Collision2D theCollision)
    {
        grounded &= theCollision.gameObject.tag != "Ground";
    }
}
