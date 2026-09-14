using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private float attackHeightDifference = 1.5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 8f;

    [Tooltip("Точка у ног врага.")]
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float groundCheckRadius = 0.15f;

    [Tooltip("Точка на уровне тела врага для проверки препятствия.")]
    [SerializeField] private Transform obstacleCheck;

    [SerializeField] private float obstacleCheckDistance = 0.7f;

    [Tooltip("Точка впереди врага для поиска платформы выше.")]
    [SerializeField] private Transform platformCheck;

    [SerializeField] private float platformCheckDistance = 1.2f;
    [SerializeField] private float platformCheckHeight = 1.5f;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float jumpCooldown = 0.25f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 1;

    [Header("Knockback")]
    [SerializeField] private bool canBeKnockedBack = true;

    private Rigidbody2D rb;
    private PlayerHealth playerHealth;

    private float attackTimer = 0f;
    private float jumpTimer = 0f;
    private float knockbackTimer = 0f;

    private bool isJumping = false;
    private bool hasLeftGround = false;
    private bool isKnockedBack = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (player == null || playerHealth == null || rb == null)
        {
            return;
        }

        // Если игрок умер — враг останавливается
        if (playerHealth.IsDead)
        {
            StopHorizontalMovement();
            return;
        }

        // Таймер атаки
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // Таймер прыжка
        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }

        // Таймер отбрасывания
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
        }

        // Пока враг отлетает — обычное управление не работает
        if (isKnockedBack)
        {
            return;
        }

        bool grounded = IsGrounded();

        // Враг действительно покинул землю
        if (!grounded)
        {
            hasLeftGround = true;
        }

        // Враг приземлился
        if (grounded && hasLeftGround)
        {
            isJumping = false;
            hasLeftGround = false;
        }

        FacePlayer();

        float distanceX = Mathf.Abs(
            player.position.x - transform.position.x
        );

        float distanceY = Mathf.Abs(
            player.position.y - transform.position.y
        );

        // ==========================================
        // АТАКА
        // ==========================================

        bool canAttack =
            grounded &&
            distanceX <= attackDistance &&
            distanceY <= attackHeightDifference;

        if (canAttack)
        {
            StopHorizontalMovement();
            Attack();
        }
        else
        {
            MoveToPlayer();
        }

        // ==========================================
        // ПРЫЖОК
        // ==========================================

        TryJump(grounded);
    }

    private void MoveToPlayer()
    {
        float direction = player.position.x > transform.position.x
            ? 1f
            : -1f;

        rb.velocity = new Vector2(
            direction * moveSpeed,
            rb.velocity.y
        );
    }

    private void StopHorizontalMovement()
    {
        rb.velocity = new Vector2(
            0f,
            rb.velocity.y
        );
    }

    private void TryJump(bool grounded)
    {
        // В воздухе прыгать нельзя
        if (!grounded)
        {
            return;
        }

        // Уже идёт прыжок
        if (isJumping)
        {
            return;
        }

        // Кулдаун прыжка
        if (jumpTimer > 0f)
        {
            return;
        }

        float direction = player.position.x > transform.position.x
            ? 1f
            : -1f;

        // ==========================================
        // ПРОВЕРКА ПРЕПЯТСТВИЯ
        // ==========================================

        if (obstacleCheck != null)
        {
            RaycastHit2D obstacle = Physics2D.Raycast(
                obstacleCheck.position,
                Vector2.right * direction,
                obstacleCheckDistance,
                groundLayer
            );

            if (obstacle.collider != null)
            {
                Jump();
                return;
            }
        }

        // ==========================================
        // ПРОВЕРКА ПЛАТФОРМЫ
        // ==========================================

        if (platformCheck != null)
        {
            Vector2 checkPosition =
                platformCheck.position +
                Vector3.right * direction * platformCheckDistance;

            RaycastHit2D platform = Physics2D.Raycast(
                checkPosition,
                Vector2.down,
                platformCheckHeight,
                groundLayer
            );

            if (platform.collider != null)
            {
                float platformHeight =
                    platform.point.y - transform.position.y;

                // Платформа должна быть выше врага,
                // но не слишком высоко
                if (platformHeight > 0.1f &&
                    platformHeight < 2.5f)
                {
                    Jump();
                }
            }
        }
    }

    private void Jump()
    {
        if (isJumping)
        {
            return;
        }

        isJumping = true;
        hasLeftGround = false;

        jumpTimer = jumpCooldown;

        rb.velocity = new Vector2(
            rb.velocity.x,
            jumpForce
        );

        Debug.Log("Ближний враг прыгнул");
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        Collider2D ground = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        return ground != null;
    }

    private void Attack()
    {
        if (attackTimer > 0f)
        {
            return;
        }

        if (playerHealth.IsDead)
        {
            return;
        }

        playerHealth.TakeDamage(attackDamage);

        Debug.Log("Ближний враг атаковал игрока");

        attackTimer = attackCooldown;
    }

    public void ApplyKnockback(
        float direction,
        float force,
        float duration
    )
    {
        if (!canBeKnockedBack)
        {
            return;
        }

        isKnockedBack = true;
        knockbackTimer = duration;

        rb.velocity = new Vector2(
            direction * force,
            rb.velocity.y
        );
    }

    private void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // GroundCheck
        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }

        float direction = 1f;

        if (player != null &&
            player.position.x < transform.position.x)
        {
            direction = -1f;
        }

        // ObstacleCheck
        if (obstacleCheck != null)
        {
            Gizmos.DrawLine(
                obstacleCheck.position,
                obstacleCheck.position +
                Vector3.right *
                direction *
                obstacleCheckDistance
            );
        }

        // PlatformCheck
        if (platformCheck != null)
        {
            Vector3 start =
                platformCheck.position +
                Vector3.right *
                direction *
                platformCheckDistance;

            Vector3 end =
                start +
                Vector3.down *
                platformCheckHeight;

            Gizmos.DrawLine(start, end);
        }
    }
}