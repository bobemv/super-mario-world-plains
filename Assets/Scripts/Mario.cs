using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mario : MonoBehaviour
{

    //TODO: jump depends on how much time you press the button

    const float PRECISION_COLLISION_DETECTION = 0.001f;
    private bool isOnAir = false, isFacingRight = true, isPreviousFacingDifferent = false, isMoving = false, isWalking = false, isJumping = false, isFalling = false, isRunning = false, isSprinting = false, isDrifting = false, isJumpingDisabled = false, isDead = false, hasChompedEnemy = false, hasCape = false;

    public bool isFlying = false;
    private GameObject enemyUnderPlayer;

    [SerializeField]
    private float _jumpSpeed = 3f, _gravity = -35f, _speedWalking = 6.0f, _speedRunning = 9.0f, _speedSprinting = 12.0f, _timeUntilSprint = 1.0f, _acceleration = 18.0f, speedModifierOnAir = 0.4f, _accelerationModifierDrifting = 1.5f, _maxVerticalSpeed = 13f, _accelerationChompingEnemy, _accelerationChompingEnemyHigher;

    [SerializeField]
    private Animator animatorPosition;

    public float currentSpeedY = 0f;
    public float currentSpeedX = 0f;

    public float desiredSpeedX = 0f;
    public float timeRunning = 0f;
    private float accelerationSign = 0;

    private Animator animatorPlayer;
    private SpriteRenderer spriteRendererPlayer;
    private BoxCollider2D boxColliderPlayer;
    private Vector2 bottomLeftBoxPlayer;
    private Vector2 bottomRightBoxPlayer;

    [SerializeField]
    private AudioClip _soundJump;

    [SerializeField]
    private AudioClip _soundPowerup;
    private void Start()
    {
        animatorPlayer = GetComponent<Animator>();
        if (animatorPlayer == null)
        {
            Debug.LogError("Animator not found for player!");
        }

        spriteRendererPlayer = GetComponent<SpriteRenderer>();
        if (spriteRendererPlayer == null)
        {
            Debug.LogError("Sprite renderer not found for player!");
        }


        boxColliderPlayer = transform.GetComponent<BoxCollider2D>();
        if (boxColliderPlayer == null)
        {
            Debug.LogError("Box collider not found for player!");
        }

        bottomLeftBoxPlayer = new Vector2(transform.position.x - boxColliderPlayer.size.x / 2, transform.position.y - boxColliderPlayer.size.y / 2);
        bottomRightBoxPlayer = new Vector2(transform.position.x + boxColliderPlayer.size.x / 2, transform.position.y - boxColliderPlayer.size.y / 2);

    }
    private void FixedUpdate()
    {
        if (isDead) {
            return;
        }
        //Debug.Log("[FixedUpdate]");
        bottomLeftBoxPlayer = new Vector2(transform.position.x - boxColliderPlayer.size.x / 2, transform.position.y - boxColliderPlayer.size.y / 2);
        bottomRightBoxPlayer = new Vector2(transform.position.x + boxColliderPlayer.size.x / 2, transform.position.y - boxColliderPlayer.size.y / 2);

        MoveVerticalPlayer();

        RaycastHit2D hit = GetHitUnderPlayer();

        MakePlayerAndEnemyStateChangesByHit(hit);

        UpdateVerticalSpeed();


    }
    private void MoveVerticalPlayer()
    {
        transform.Translate(Vector3.up * currentSpeedY * Time.deltaTime);
    }
    private void UpdatePlayerState() {
        isFlying = hasCape && isSprinting && isJumping;
        if (currentSpeedY < 0)
        {
            isOnAir = true;
            isFalling = true;
            isJumping = false;
        }
        else if (currentSpeedY > 0)
        {
            isOnAir = true;
            isFalling = false;
            isJumping = true;
        }
        else if (currentSpeedY == 0 && !isOnAir)
        {
            isFalling = false;
            isJumping = false;
            isJumpingDisabled = false;
        }
        if (currentSpeedY >= _maxVerticalSpeed) {
            isJumpingDisabled = true;
        }
        
        float currentSpeedXAbsolute = Mathf.Abs(currentSpeedX);
        if (currentSpeedXAbsolute > 0)
        {
            isWalking = true;
            if (currentSpeedXAbsolute == _speedSprinting || _speedRunning < currentSpeedXAbsolute && currentSpeedXAbsolute < _speedSprinting && Mathf.Abs(desiredSpeedX) - Mathf.Abs(currentSpeedX) > 0) {
                isSprinting = true;
            }
            if (currentSpeedXAbsolute <= _speedRunning || currentSpeedXAbsolute < _speedSprinting && Mathf.Abs(desiredSpeedX) - Mathf.Abs(currentSpeedX) < 0) {
                isSprinting = false;
            }
            if (currentSpeedXAbsolute < _speedRunning) {
                timeRunning = 0;
            }
            if (_speedRunning <= currentSpeedXAbsolute) {
                timeRunning += Time.deltaTime;
            }
        }
        else {
            isWalking = false;
            isRunning = false;
            isSprinting = false;
            timeRunning = 0;
        }
        isDrifting = Mathf.Sign(currentSpeedX) != Mathf.Sign(desiredSpeedX) && !isOnAir && desiredSpeedX != 0;

    }
    void Update()
    {
        if (isDead) {
            return;
        }
        CheckInput();
        
        UpdateHorizontalSpeed();
        accelerationSign = Mathf.Sign(desiredSpeedX - currentSpeedX);

        MoveHorizontalPlayer();
        UpdatePlayerState();
        SetAnimation();
    }

    private void CheckInput()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isFacingRight = true;
            desiredSpeedX = _speedWalking;

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isFacingRight = false;
            desiredSpeedX = _speedWalking;

        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.Z))
            {
                desiredSpeedX = _speedRunning;
                if (timeRunning > _timeUntilSprint) {
                    desiredSpeedX = _speedSprinting;
                }
            }
        }
        else {
            desiredSpeedX = 0f;
        }
        if (Input.GetKey(KeyCode.X) && !isJumpingDisabled)
        {
            //Debug.Log("JUMP");
            currentSpeedY += _jumpSpeed;
        }
        if (Input.GetKeyDown(KeyCode.X) && !isJumpingDisabled) {
            AudioSource.PlayClipAtPoint(_soundJump, transform.position);
        }

    }

    private void SetAnimation()
    {
        spriteRendererPlayer.flipX = !isFacingRight;
        if (hasCape && isDrifting) {
            spriteRendererPlayer.flipX = !spriteRendererPlayer.flipX;          
        }

        animatorPlayer.SetBool("isWalking", isWalking);
        animatorPlayer.SetBool("isJumping", isJumping);
        animatorPlayer.SetBool("isFalling", isFalling);
        animatorPlayer.SetBool("isSprinting", isSprinting);
        animatorPlayer.SetBool("isDrifting", isDrifting);
        animatorPlayer.SetBool("isDead", isDead);
        animatorPlayer.SetBool("hasCape", hasCape);
        animatorPosition.SetBool("isDead", isDead);

    }
    private void MoveHorizontalPlayer()
    {
        transform.Translate(Vector3.right * currentSpeedX * Time.deltaTime);
        //Debug.Log("transform: " + transform.localPosition);
        if (transform.localPosition.x < -0.5f) {
            transform.localPosition = new Vector3(-0.5f, transform.localPosition.y, transform.localPosition.z);
        }
    }


    //Returns if there is no colliders under Mario and, if hit, it returns it as parameter.
    private RaycastHit2D GetHitUnderPlayer()
    {
        RaycastHit2D hitLeft, hitRight, hit;
        //Lay mask = LayerMask.GetMask("Ground");

        hitLeft = Physics2D.Raycast(bottomLeftBoxPlayer, Vector2.down, PRECISION_COLLISION_DETECTION);
        hitRight = Physics2D.Raycast(bottomRightBoxPlayer, Vector2.down, PRECISION_COLLISION_DETECTION);
        
        hit = hitRight;

        if (hitRight.collider == null)
        {
            hit = hitLeft;
        }

        return hit;
        /* if (hitLeft.collider == null && hitRight.collider == null || currentSpeedY > 0) {
            isOnAir = true;
            return;
        }

        

        BoxCollider2D box = hit.collider as BoxCollider2D;
        transform.position = new Vector3(transform.position.x, hit.collider.transform.position.y + box.size.y / 2 + box.offset.y + boxColliderPlayer.size.y / 2);
             
        //Debug.DrawRay(new Vector3(bottomLeftBoxPlayer.x, bottomLeftBoxPlayer.y), Vector3.down);
        currentSpeedY = 0f;
        isOnAir = false;
        */
    }

    private void MakePlayerAndEnemyStateChangesByHit(RaycastHit2D hit) {
        enemyUnderPlayer = null;
        if (hit.collider == null) {
            isOnAir = true;
            return;
        }

        if (hit.collider.tag == "Enemy" && currentSpeedY <= 0) {
            //Destroy(hit.collider.gameObject);
            enemyUnderPlayer = hit.collider.gameObject;
            Debug.Log("enemyUnderPlayer:" + enemyUnderPlayer);
            hit.collider.gameObject.GetComponent<SuperKoopa>().Damaged();
            currentSpeedY = _jumpSpeed;
            hasChompedEnemy = true;

            return;
        }
        if (hit.collider.tag == "Ground" && currentSpeedY <= 0) {
            BoxCollider2D box = hit.collider as BoxCollider2D;
            transform.position = new Vector3(transform.position.x, hit.collider.transform.position.y + box.size.y / 2 + box.offset.y + boxColliderPlayer.size.y / 2);
            currentSpeedY = 0f;
            isOnAir = false;
        }
    }

    private void UpdateVerticalSpeed() {
        if (!isOnAir) {
            return;
        }
        currentSpeedY += GetVerticalAcceleration() * Time.deltaTime;
        currentSpeedY = Mathf.Min(currentSpeedY, _maxVerticalSpeed);
        currentSpeedY = Mathf.Max(currentSpeedY, -_maxVerticalSpeed);
    }

    private float GetVerticalAcceleration() {
        float verticalAcceleration = _gravity;

        if (hasChompedEnemy) {
            hasChompedEnemy = false;
            verticalAcceleration += Input.GetKey(KeyCode.X) ? _accelerationChompingEnemyHigher : _accelerationChompingEnemy;
        }

        if (hasCape && isSprinting && isJumping) {
            verticalAcceleration = 1.0f;
        }
        return verticalAcceleration;
    }
    private void UpdateHorizontalSpeed() {
        desiredSpeedX = isFacingRight ? desiredSpeedX : -desiredSpeedX;
        float direction = Mathf.Sign(desiredSpeedX - currentSpeedX);
        if (currentSpeedX == desiredSpeedX) {
            return;            
        }
      
        currentSpeedX += GetHorizontalAcceleration() * Time.deltaTime;;
                                    
        if (direction > 0 && currentSpeedX > desiredSpeedX) {
            //Debug.Log("ACHIEVED TOP SPEED");
            currentSpeedX = desiredSpeedX;
            return;
        }
        if (direction < 0 && currentSpeedX < desiredSpeedX) {
            //Debug.Log("ACHIEVED DOWN SPEED");

            currentSpeedX = desiredSpeedX;
            return;
        }  
    }

    private float GetHorizontalAcceleration() {
        float horizontalAcceleration = _acceleration;
        
        if (isOnAir) {
            horizontalAcceleration *= speedModifierOnAir;
        }

        if (isDrifting) {
            horizontalAcceleration *= _accelerationModifierDrifting;
        }

        if (Mathf.Sign(desiredSpeedX - currentSpeedX) < 0) {
            horizontalAcceleration = -horizontalAcceleration;
        }

        return horizontalAcceleration;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("collision: " + other.gameObject.name);
        if (other.gameObject == null || other.gameObject.tag == "Ground" || enemyUnderPlayer == other.gameObject) {
            return;
        }

        Debug.Log("gameobject: " + other.gameObject.name);
        if (other.gameObject.GetComponent<Feather>() != null && other.gameObject.GetComponent<Feather>().isPickable) {
            hasCape = true;
            AudioSource.PlayClipAtPoint(_soundPowerup, transform.position);
            GetComponent<BoxCollider2D>().size = new Vector2(0.98f, 1.65f);
            GetComponent<BoxCollider2D>().offset = new Vector2(0.09f, 0.02f);
            Destroy(other.gameObject.transform.parent.gameObject);
            return;
        }
        
        if (other.gameObject.tag == "Enemy") {
            //PlayerDamaged();
        }
    }

    private void PlayerDamaged() {
        isJumping = false;
        isWalking = false;
        isFalling = false;
        isSprinting = false;
        isDrifting = false;
        isDead = true;
        SetAnimation();
        //pause scene
        //change animation to "Mario_Death"
        //pause 0.5 second
        //move mario a little bit up, then down offscreen while animating
        //cut to black
    }
}
