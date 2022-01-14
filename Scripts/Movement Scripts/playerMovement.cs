using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour

{
    public Rigidbody2D rb;
    private float movementX;
    private bool isGrounded;
    private float jumpPressedRemember = 0f;
    private float jumpPressedRememberTime = 0.2f;

    // movement variables
    public float moveForce = 100f;
    [Range(100.0f,1000.0f)] public float upwardForce;   //650.0f
    [Range(0.00f, 1.00f)] public float horizontalDampingWhenStopping;   //0.50f
    [Range(0.00f, 1.00f)] public float horizontalDampingWhenTurning;    //0.60f
    [Range(0.00f, 1.00f)] public float horizontalDampingBasic;  //0.20f
    [Range(0.00f, 1.00f)] public float cutJumpheight;   //0.50f

    //gravity manipulation variables
    public float gravityScale = 1.3f;
    public float fallGravityMultiplier = 200f;

    //use when calculating physics 
    void FixedUpdate() {
        PlayerMovement();
        PlayerJump();
    }

    void PlayerMovement()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        float horizontalVelocity = rb.velocity.x;
        horizontalVelocity += movementX ;

        //character acceleration and deceleration 
        if (Mathf.Abs(movementX) < 0.01f) 
        {
            horizontalVelocity *= Mathf.Pow(1f - horizontalDampingWhenStopping, Time.deltaTime * 10f);
        }
        else if(Mathf.Sign(movementX)!= Mathf.Sign(horizontalVelocity))
        {
            horizontalVelocity *= Mathf.Pow(1f - horizontalDampingWhenTurning, Time.deltaTime * 10f);
        }
        else
        {
            horizontalVelocity *= Mathf.Pow(1f - horizontalDampingBasic, Time.deltaTime * 10f);
        }
        rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);
    }

    //TODO: - Implement wall Jump mechanics

    void PlayerJump() {
        jumpPressedRemember -= Time.deltaTime;
        if (Input.GetButtonDown("Jump")) jumpPressedRemember = jumpPressedRememberTime;

        if (Input.GetButtonUp("Jump")) 
        {
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector2.down * rb.velocity.y * (1- cutJumpheight * Time.deltaTime), ForceMode2D.Impulse);
            }
        }

        //implementing Coyote time for more responsive jump
        if (jumpPressedRemember > 0 && isGrounded) {
            isGrounded = false;
            jumpPressedRemember = 0;
            rb.AddForce(new Vector2(0f, upwardForce) * Time.deltaTime, ForceMode2D.Impulse);
        }

        //gravity manipulation when character reaches maximum height
        if (rb.velocity.y < 0 )
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier * Time.deltaTime;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    //check if plauer is grounded
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
        }
    }
}


