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

    private Transform groundCheck;
    private bool grounded = true;
    private Animator anim;
    private Rigidbody2D body;
    private LineRenderer lineRenderer;

    private Vector2 mouseClickPosition;
    private Vector2 throwVector;
    private Vector2 targetPosition;


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
            this.mouseClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            drawVector = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.throwVector = new Vector2(mousePosition.x, mousePosition.y) - mouseClickPosition;

            this.targetPosition = new Vector2(knifeTopCollider.transform.position.x, knifeTopCollider.transform.position.y) + this.throwVector;
            lineRenderer.SetPosition(0, body.position);
            lineRenderer.SetPosition(1, targetPosition);
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

        if (drawVector)
            DrawVector();

        if (throwKnife)
            ThrowKnife();

    }

    private void DrawVector()
    {
        Vector2 start = this.mouseClickPosition;
        Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void ThrowKnife()
    {
        body.freezeRotation = false;
        body.bodyType = RigidbodyType2D.Dynamic;

        Vector3 knifeColliderPosition = knifeTopCollider.transform.position;
        Debug.Log("Knife: " + knifeColliderPosition + " | Throw: " + throwVector + " | Target: " + targetPosition);

        Debug.Log("Sqr magnitude of throw vector: " + throwVector.sqrMagnitude);
        body.AddForceAtPosition(throwVector * 15f, knifeColliderPosition, ForceMode2D.Impulse);

        float angle = Mathf.Atan2(throwVector.y, throwVector.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        Debug.Log("Angle: " + angle);
        body.MoveRotation(angle - 90);

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
                if (theCollision.otherCollider == knifeTopCollider && body.velocity.magnitude > 1)
                {
                    Debug.Log("Jebło ostrzem w podłogę.");
                    body.bodyType = RigidbodyType2D.Static;
                }
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
        grounded &= theCollision.gameObject.name != "Ground";
    }
}
