using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float jumpForce = 5f;
    public float forwardJumpForce = 10f;
    public float speedUpMultiplier = 1.5f;
    public float slowDownMultiplier = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    private bool isGrounded = true;
    private bool isGravChanged = false;
    private bool isReversed = false;
    private bool isReversing = false;
    private bool hasJumped = false; // Флаг для отслеживания прыжка
    private float currentSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentSpeed = walkSpeed;
    }

    private void FixedUpdate()
    {
        Debug.Log(rb.velocity);
        if (isGrounded && !isReversing)
        {
            if (hasJumped && Mathf.Abs(rb.velocity.y) < 0.1f) // Проверяем, действительно ли робот приземлился
            {
                currentSpeed = walkSpeed; // Возвращаем скорость на исходную
                hasJumped = false; // Сбрасываем флаг прыжка
            }

            MoveForward();
            PlayAnimation("Walk");
        }
        else if (!isGrounded && Mathf.Abs(rb.velocity.y) > 0.3f)
        {
            PlayAnimation("TakeOff");
        }
    }

    private void MoveForward()
    {
        rb.velocity = new Vector2((isReversed ? -1 : 1) * currentSpeed, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Проверяем, в каком направлении происходит столкновение
                Vector2 gravityDirection = Physics2D.gravity.normalized; // Нормализуем вектор гравитации
                float angle = Vector2.Angle(contact.normal, -gravityDirection); // Проверяем угол между нормалью и направлением гравитации

                if (angle < 45f) // Если угол меньше 45 градусов, считаем это приземлением
                {
                    isGrounded = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Jump":
                PerformJump(jumpForce, forwardJumpForce / 12f);
                break;
            case "Springboard":
                PerformJump(jumpForce / 1.64f, forwardJumpForce / 3.5f);
                break;
            case "ReverseZone":
                StartCoroutine(ReverseDirection());
                break;
            case "GravChange":
                ChangeGravity();
                break;
            case "SpeedUp":
                AdjustSpeed(speedUpMultiplier, speedUpMultiplier / 2f);
                break;
            case "SlowDown":
                AdjustSpeed(slowDownMultiplier, slowDownMultiplier);
                break;
            case "Damage":
                Damage();
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SpeedUp") || other.CompareTag("SlowDown"))
        {
            ResetSpeed();
        }
    }

    private IEnumerator ReverseDirection()
    {
        isReversing = true;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        PlayAnimation("Flip");
        yield return new WaitForSeconds(0.66f);

        isReversed = !isReversed;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = new Vector2((isReversed ? -1 : 1) * currentSpeed, rb.velocity.y);
        isReversing = false;
    }

    public void PerformJump(float jumpPower, float forwardPower)
    {
        Vector2 jumpDirection = new Vector2(0f, isGravChanged ? -jumpPower : jumpPower);
        rb.AddForce(jumpDirection, ForceMode2D.Impulse);

        currentSpeed = walkSpeed * forwardPower; // Увеличиваем скорость во время прыжка
        hasJumped = true; // Устанавливаем флаг прыжка
    }

    public void ChangeGravity()
    {
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -Physics2D.gravity.y);
        Vector3 scale = transform.localScale;
        scale.y *= -1;
        transform.localScale = scale;
        isGravChanged = !isGravChanged;
    }

    private void AdjustSpeed(float horizontalMultiplier, float verticalMultiplier)
    {
        if (isGrounded)
        {
            currentSpeed = walkSpeed * horizontalMultiplier;
        }
        else if (!isGrounded)
        {
            Vector2 speedDirection = new Vector2(isReversed ? -horizontalMultiplier : horizontalMultiplier, (isGravChanged ? -verticalMultiplier : verticalMultiplier));
            rb.AddForce(speedDirection, ForceMode2D.Impulse);
        }
    }

    private void ResetSpeed()
    {
        currentSpeed = walkSpeed;
    }

    public void Damage()
    {

    }


    public void PlayAnimation(string animationName)
    {
        animator.Play(isReversed ? $"Robot_Reversed_{animationName}" : $"Robot_{animationName}");
    }
}

