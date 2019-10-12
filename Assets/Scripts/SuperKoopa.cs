using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperKoopa : Enemy
{

    private bool isFacingRight = false, isJumping = false, isFlying = false, isNoCape = false;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _speedNoCape;
    private float currentSpeedX;

    [SerializeField]
    private Animator _movementAnimator;
    private Animator _spriteAnimator;

    [SerializeField]
    private float _timeUntilJumping;
    [SerializeField]
    private float _timeUntilFlying;

    [SerializeField]
    private GameObject _powerup;

    private float currentTime;

    [SerializeField]
    private AudioClip _soundDie;


    void Start()
    {
        boxColliderPlayer = transform.GetComponent<BoxCollider2D>();
        if (boxColliderPlayer == null)
        {
            Debug.LogError("Box collider not found for enemy: " + gameObject.name);
        }

        bottomLeftBoxPlayer = new Vector2(transform.position.x - boxColliderPlayer.size.x / 2, transform.position.y - boxColliderPlayer.size.y / 2);
        bottomRightBoxPlayer = new Vector2(transform.position.x + boxColliderPlayer.size.x / 2, transform.position.y - boxColliderPlayer.size.y / 2);

        _spriteAnimator = transform.GetComponent<Animator>();
        if (_spriteAnimator == null)
        {
            Debug.LogError("Animator not found for enemy: " + gameObject.name);
        }
        currentTime = 0f;
        currentSpeedY = 0f;
    }

    private void FixedUpdate()
    {
        //Debug.Log("[FixedUpdate]");
        bottomLeftBoxPlayer = new Vector2(transform.position.x - boxColliderPlayer.size.x / 2, transform.position.y - boxColliderPlayer.size.y / 2);
        bottomRightBoxPlayer = new Vector2(transform.position.x + boxColliderPlayer.size.x / 2, transform.position.y - boxColliderPlayer.size.y / 2);

        if (isNoCape) {
            MoveVertical();
        }
        
        CheckIsOnAir();

        if (isNoCape) {
            UpdateVerticalSpeed();
        }


    }

    private void MoveVertical()
    {
        transform.Translate(Vector3.up * currentSpeedY * Time.deltaTime);
    }
    private void UpdateVerticalSpeed() {
        if (!isOnAir) {
            return;
        }
        currentSpeedY += GetVerticalAcceleration() * Time.deltaTime;
    }
    private float GetVerticalAcceleration() {
        float verticalAcceleration = _gravity;

        return verticalAcceleration;
    }





    void Update()
    {
        //CheckInput();
        CheckTimings();
        
        UpdateHorizontal();

        MoveHorizontal();
        SetAnimation();
    }

    private void MoveHorizontal()
    {
        transform.Translate(Vector3.right * currentSpeedX * Time.deltaTime);
    }



    private void CheckTimings() {
        currentTime += Time.deltaTime;
        if (currentTime > _timeUntilJumping) {
            isJumping = true;
        }
        if (currentTime > _timeUntilFlying) {
            isFlying = true;
        }
    }

    private void UpdateHorizontal() {
        currentSpeedX = isFacingRight ? _speed : -_speed;
        if (isNoCape) {
            currentSpeedX = isFacingRight ? _speedNoCape : -_speedNoCape;
        }
        //float direction = Mathf.Sign(desiredSpeedX - currentSpeedX);
        /* if (currentSpeedX == desiredSpeedX) {
            return;            
        }*/
      
        //currentSpeedX += GetHorizontalAcceleration() * Time.deltaTime;;
                                    
        /* if (direction > 0 && currentSpeedX > desiredSpeedX) {
            //Debug.Log("ACHIEVED TOP SPEED");
            currentSpeedX = desiredSpeedX;
            return;
        }
        if (direction < 0 && currentSpeedX < desiredSpeedX) {
            //Debug.Log("ACHIEVED DOWN SPEED");

            currentSpeedX = desiredSpeedX;
            return;
        }  */
    }

    private float GetHorizontalAcceleration() {
        //float horizontalAcceleration = _acceleration;
        
        //return horizontalAcceleration;
        return 0;
    }

    private void SetAnimation() {
        

        _movementAnimator.SetBool("isJumping", isJumping);
        _spriteAnimator.SetBool("isFlying", isFlying);
        _spriteAnimator.SetBool("isNoCape", isNoCape);
    }

    public void Damaged() {
        if (!isNoCape) {
            isNoCape = true;
            
            GameObject feather = Instantiate(_powerup);
            feather.GetComponentInChildren<Feather>().StartPosition(transform.position);
            AudioSource.PlayClipAtPoint(_soundDie, transform.position);
            return;
        }
            AudioSource.PlayClipAtPoint(_soundDie, transform.position);
        Destroy(gameObject.transform.parent.gameObject);
        Destroy(gameObject);
    }
}
