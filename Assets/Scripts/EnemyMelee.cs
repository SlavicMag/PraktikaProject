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

    [Tooltip("Точка под ногами врага.")]
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float groundCheckRadius = 0.15f;

    [Tooltip("Точка на уровне тела для проверки стены.")]
    [SerializeField] private Transform obstacleCheck;

    [SerializeField] private float obstacleCheckDistance = 0.5f;

    [Tooltip("Точка в нижней передней части врага.")]
    [SerializeField] private Transform platformCheck;

    [Tooltip("С какого расстояния впереди начинаем искать платформу.")]
    [SerializeField] private float platformCheckDistance = 0.2f;

    [Tooltip("Максимальная ширина области поиска платформы.")]
    [SerializeField] private float platformSearchWidth = 3f;

    [Tooltip("Максимальная высота платформы, на которую можно запрыгнуть.")]
    [SerializeField] private float maxPlatformHeight = 2.5f;

    [Tooltip("Минимальная высота платформы над врагом.")]
    [SerializeField] private float minPlatformHeight = 0.15f;

    [Tooltip("Допуск по краям платформы при расчёте места приземления.")]
    [SerializeField] private float landingMargin = 0.15f;

    [Tooltip("Время между попытками прыжка.")]
    [SerializeField] private float jumpCooldown = 0.3f;

    [Tooltip("Если прыжок не удался, столько секунд не пробуем снова.")]
    [SerializeField] private float failedJumpCooldown = 1.0f;

    [SerializeField] private LayerMask groundLayer;

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
    private float failedJumpTimer = 0f;

    private bool isJumping = false;
    private bool hasLeftGround = false;
    private bool isKnockedBack = false;

    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        FindPlayer();
    }

    private void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        if (player == null)
        {
            Debug.LogError(
                "EnemyMelee: Игрок не найден. Проверь Tag = Player."
            );
        }

        if (playerHealth == null && player != null)
        {
            Debug.LogError(
                "EnemyMelee: PlayerHealth не найден на игроке."
            );
        }

        if (rb == null)
        {
            Debug.LogError(
                "EnemyMelee: Rigidbody2D не найден на враге."
            );
        }
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
        }

        if (player == null || playerHealth == null || rb == null)
        {
            return;
        }

        if (playerHealth.IsDead)
        {
            StopHorizontalMovement();
            return;
        }

        // =========================
        // ТАЙМЕРЫ
        // =========================

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
        }

        if (failedJumpTimer > 0f)
        {
            failedJumpTimer -= Time.deltaTime;
        }

        // =========================
        // ОТБРАСЫВАНИЕ
        // =========================

        if (isKnockedBack)
        {
            return;
        }

        // =========================
        // ПРОВЕРКА ЗЕМЛИ
        // =========================

        bool grounded = IsGrounded();

        if (!grounded)
        {
            hasLeftGround = true;
        }

        if (grounded && hasLeftGround)
        {
            isJumping = false;
            hasLeftGround = false;
        }

        FacePlayer();

        // =========================
        // РАССТОЯНИЕ ДО ИГРОКА
        // =========================

        float distanceX = Mathf.Abs(
            player.position.x - transform.position.x
        );

        float distanceY = Mathf.Abs(
            player.position.y - transform.position.y
        );

        // =========================
        // АТАКА
        // =========================

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

        // =========================
        // ПРЫЖОК
        // =========================

        TryJump(grounded);

        bool running = grounded && Mathf.Abs(rb.velocity.x) > 0.1f && !isJumping && !isKnockedBack;
        animator.SetBool("isRunning", running);
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
        if (!grounded)
        {
            return;
        }

        if (isJumping)
        {
            return;
        }

        if (jumpTimer > 0f)
        {
            return;
        }

        if (failedJumpTimer > 0f)
        {
            return;
        }

        float direction = player.position.x > transform.position.x
            ? 1f
            : -1f;

        // =========================
        // 1. СТЕНА
        // =========================

        if (HasObstacle(direction))
        {
            Jump();
            return;
        }

        // =========================
        // 2. УМНЫЙ ПОИСК ПЛАТФОРМЫ
        // =========================

        Collider2D platform = FindReachablePlatform(
            direction,
            out float platformY
        );

        if (platform != null)
        {
            if (CanReachPlatform(
                platform,
                platformY,
                direction
            ))
            {
                Jump();
            }
        }
    }

    private bool HasObstacle(float direction)
    {
        if (obstacleCheck == null)
        {
            return false;
        }

        RaycastHit2D obstacle = Physics2D.Raycast(
            obstacleCheck.position,
            Vector2.right * direction,
            obstacleCheckDistance,
            groundLayer
        );

        return obstacle.collider != null;
    }

    private Collider2D FindReachablePlatform(
        float direction,
        out float platformY
    )
    {
        platformY = 0f;

        if (platformCheck == null)
        {
            return null;
        }

        float enemyFeetY = GetFeetY();

        /*
         * Проверяем несколько точек впереди.
         */
        int rayCount = 9;

        Collider2D bestPlatform = null;
        float bestHeight = float.MaxValue;

        for (int i = 0; i < rayCount; i++)
        {
            float t = rayCount == 1
                ? 0.5f
                : (float)i / (rayCount - 1);

            float offset = Mathf.Lerp(
                0f,
                platformSearchWidth,
                t
            );

            float rayX =
                platformCheck.position.x +
                direction *
                (platformCheckDistance + offset);

            /*
             * Начинаем высоко над врагом.
             */
            Vector2 rayStart = new Vector2(
                rayX,
                enemyFeetY + maxPlatformHeight
            );

            RaycastHit2D[] hits = Physics2D.RaycastAll(
                rayStart,
                Vector2.down,
                maxPlatformHeight + 0.3f,
                groundLayer
            );

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null)
                {
                    continue;
                }

                float height =
                    hit.point.y - enemyFeetY;

                /*
                 * Игнорируем:
                 * - землю на том же уровне;
                 * - слишком высокие платформы.
                 */
                if (height < minPlatformHeight)
                {
                    continue;
                }

                if (height > maxPlatformHeight)
                {
                    continue;
                }

                /*
                 * Берём ближайшую по высоте
                 * подходящую поверхность.
                 */
                if (height < bestHeight)
                {
                    bestHeight = height;
                    bestPlatform = hit.collider;
                    platformY = hit.point.y;
                }
            }
        }

        return bestPlatform;
    }

    private bool CanReachPlatform(
        Collider2D platform,
        float platformY,
        float direction
    )
    {
        if (platform == null)
        {
            return false;
        }

        float enemyFeetY = GetFeetY();

        float heightDifference =
            platformY - enemyFeetY;

        if (heightDifference < minPlatformHeight)
        {
            return false;
        }

        if (heightDifference > maxPlatformHeight)
        {
            return false;
        }

        /*
         * Получаем гравитацию Rigidbody2D.
         */
        float gravity =
            Mathf.Abs(
                Physics2D.gravity.y *
                rb.gravityScale
            );

        if (gravity <= 0.01f)
        {
            return false;
        }

        /*
         * Проверяем, способен ли прыжок вообще
         * достичь этой высоты.
         *
         * Формула:
         *
         * v² = v0² - 2gh
         */
        float velocitySquared =
            jumpForce * jumpForce -
            2f * gravity * heightDifference;

        if (velocitySquared < 0f)
        {
            /*
             * Платформа слишком высоко.
             */
            return false;
        }

        /*
         * Время, за которое враг достигает
         * платформы на ВОСХОДЯЩЕЙ траектории.
         */
        float sqrt =
            Mathf.Sqrt(velocitySquared);

        float timeToPlatform =
            (jumpForce - sqrt) / gravity;

        /*
         * Также получаем время на нисходящей
         * части траектории.
         */
        float landingTime =
            (jumpForce + sqrt) / gravity;

        /*
         * Нас интересует нормальное приземление
         * на платформу сверху, поэтому используем
         * нисходящую часть траектории.
         */
        float time =
            landingTime;

        /*
         * Сколько враг пролетит по X.
         */
        float horizontalDistance =
            moveSpeed * time;

        /*
         * Допустимый диапазон приземления платформы.
         */
        Bounds bounds =
            platform.bounds;

        float left = bounds.min.x + landingMargin;
        float right = bounds.max.x - landingMargin;

        /*
         * Необходимая точка по X.
         */
        float predictedX =
            transform.position.x +
            direction *
            horizontalDistance;

        /*
         * Если предсказанная точка попадает
         * внутрь платформы — прыжок возможен.
         */
        bool insidePlatform =
            predictedX >= left &&
            predictedX <= right;

        /*
         * Иногда predictedX может немного не дойти
         * до платформы, но сам Collider врага всё равно
         * попадёт на неё.
         *
         * Поэтому используем небольшой запас.
         */
        if (!insidePlatform)
        {
            float enemyWidth =
                GetComponent<Collider2D>() != null
                    ? GetComponent<Collider2D>().bounds.extents.x
                    : 0.25f;

            float expandedLeft =
                left - enemyWidth;

            float expandedRight =
                right + enemyWidth;

            insidePlatform =
                predictedX >= expandedLeft &&
                predictedX <= expandedRight;
        }

        return insidePlatform;
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

        animator.SetTrigger("Jump");
        Debug.Log("Ближний враг прыгнул");
    }

    private float GetFeetY()
    {
        if (groundCheck != null)
        {
            return groundCheck.position.y;
        }

        Collider2D collider =
            GetComponent<Collider2D>();

        if (collider != null)
        {
            return collider.bounds.min.y;
        }

        return transform.position.y;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        Collider2D ground =
            Physics2D.OverlapCircle(
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

        animator.SetTrigger("Attack"); 
        attackTimer = attackCooldown;
    }

    public void DealAttackDamage()
    {
        if (playerHealth == null || playerHealth.IsDead)
        {
            return;
        }

        playerHealth.TakeDamage(attackDamage);

        Debug.Log("Ближний враг атаковал игрока");
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
            transform.localScale =
                new Vector3(1f, 1f, 1f);
        }
        else
        {
            transform.localScale =
                new Vector3(-1f, 1f, 1f);
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

        // Platform search area
        if (platformCheck != null)
        {
            float feetY = GetFeetY();

            Vector3 start = new Vector3(
                platformCheck.position.x +
                direction *
                platformCheckDistance,

                feetY +
                maxPlatformHeight,

                0f
            );

            Vector3 end = new Vector3(
                platformCheck.position.x +
                direction *
                (platformCheckDistance +
                 platformSearchWidth),

                feetY,

                0f
            );

            Gizmos.DrawLine(
                start,
                end
            );
        }

        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);

        Vector3 attackCenter = new Vector3(
            transform.position.x,
            transform.position.y,
            0f
        );

        Vector3 attackSize = new Vector3(
            attackDistance * 2f,
            attackHeightDifference * 2f,
            0f
        );

        Gizmos.DrawCube(attackCenter, attackSize);
    }
}