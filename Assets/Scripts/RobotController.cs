using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    private bool hasJumped = false;
    private bool isDead = false;
    private float currentSpeed;

    public AudioSource deathAudioSource;
    public AudioManager audioManager;
    public LevelEditor levelEditor;

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
        if (isGrounded && !isReversing)
        {
            if (hasJumped && Mathf.Abs(rb.velocity.y) < 0.1f)
            {
                currentSpeed = walkSpeed;
                hasJumped = false;
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
                Vector2 gravityDirection = Physics2D.gravity.normalized;
                float angle = Vector2.Angle(contact.normal, -gravityDirection);

                if (angle < 45f)
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
            case "LiftPad":
                PerformJump(jumpForce, forwardJumpForce / 12f);
                break;
            case "DashPad":
                PerformJump(jumpForce / 1.64f, forwardJumpForce / 3.5f);
                break;
            case "Redirect":
                StartCoroutine(ReverseDirection());
                break;
            case "GravFlip":
                ChangeGravity();
                break;
            case "AccelPad":
                AdjustSpeed(speedUpMultiplier, speedUpMultiplier / 2f);
                break;
            case "SlowPad":
                AdjustSpeed(slowDownMultiplier, slowDownMultiplier);
                break;
            case "Damage":
                Damage();
                break;
            case "LevelEnd":
                HandleLevelEnd();
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("AccelPad") || other.CompareTag("SlowPad"))
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

        currentSpeed = walkSpeed * forwardPower;
        hasJumped = true;
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
            if (horizontalMultiplier > 1f)
            {
                Vector2 speedDirection = new Vector2(isReversed ? -horizontalMultiplier : horizontalMultiplier,
                                                     (isGravChanged ? -verticalMultiplier : verticalMultiplier));
                rb.AddForce(speedDirection, ForceMode2D.Impulse);
            }
            else
            {
                Vector2 newVelocity = rb.velocity;
                newVelocity.x *= horizontalMultiplier;
                rb.velocity = newVelocity;
            }
        }
    }

    private void ResetSpeed()
    {
        currentSpeed = walkSpeed;
    }

    public void Damage()
    {
        if (isDead) return;

        isDead = true;
        Time.timeScale = 0f;

        deathAudioSource.Play();
        StartCoroutine(HandleDeathTransition());
    }

    private IEnumerator HandleDeathTransition()
    {
        float duration = deathAudioSource.clip.length;
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;

        if (audioManager != null)
            audioManager.RequestEditMode(force: true);

        isDead = false;
    }


    public void ResetGravity()
    {
        if (isGravChanged)
        {
            ChangeGravity();
        }
    }

    public void ResetDirection()
    {
        if (isReversed)
        {
            StartCoroutine(ReverseDirection());
        }
    }

    public void ResetPhysicsState()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void PlayAnimation(string animationName)
    {
        animator.Play(isReversed ? $"Robot_Reversed_{animationName}" : $"Robot_{animationName}");
    }

    void HandleLevelEnd()
    {
        string levelName = SceneManager.GetActiveScene().name;
        int capsuleCount = EnergyCapsuleManager.Instance.GetCollectedCount();

        FindObjectOfType<LevelEndUI>().ShowEndScreen(capsuleCount);
        SaveProgress(levelName, capsuleCount);

        Time.timeScale = 0f;
    }

    private void SaveProgress(string levelName, int capsules)
    {
        string capsuleKey = $"{levelName}_capsules";
        string completedKey = $"{levelName}_completed";

        int prevBest = PlayerPrefs.GetInt(capsuleKey, 0);
        if (capsules > prevBest)
            PlayerPrefs.SetInt(capsuleKey, capsules);

        PlayerPrefs.SetInt(completedKey, 1);

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            string nextSceneName = GetSceneNameByBuildIndex(nextIndex);
            PlayerPrefs.SetInt($"{nextSceneName}_unlocked", 1);
        }

        PlayerPrefs.Save();
    }

    private string GetSceneNameByBuildIndex(int index)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(index);
        string name = System.IO.Path.GetFileNameWithoutExtension(path);
        return name;
    }
}

