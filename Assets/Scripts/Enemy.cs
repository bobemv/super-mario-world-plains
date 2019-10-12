using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public bool isOnAir = false;
    public float currentSpeedY = 0f;
    const float PRECISION_COLLISION_DETECTION = 0.001f;
    public BoxCollider2D boxColliderPlayer;
    public Vector2 bottomLeftBoxPlayer;
    public Vector2 bottomRightBoxPlayer;

    public float _gravity = -35f;
    public void CheckIsOnAir()
    {
        RaycastHit2D hitLeft, hitRight, hit;
        //Lay mask = LayerMask.GetMask("Ground");

        hitLeft = Physics2D.Raycast(bottomLeftBoxPlayer, Vector2.down, PRECISION_COLLISION_DETECTION);
        hitRight = Physics2D.Raycast(bottomRightBoxPlayer, Vector2.down, PRECISION_COLLISION_DETECTION);
        
        if (hitLeft.collider == null && hitRight.collider == null || currentSpeedY > 0) {
            isOnAir = true;
            return;
        }

        hit = hitRight;

        if (hitRight.collider == null)
        {
            hit = hitLeft;
        }

        if (hit.collider.tag != "Ground") {
            return;
        }

        BoxCollider2D box = hit.collider as BoxCollider2D;
        transform.position = new Vector3(transform.position.x, hit.collider.transform.position.y + box.size.y / 2 + box.offset.y + boxColliderPlayer.size.y / 2);
             
        //Debug.DrawRay(new Vector3(bottomLeftBoxPlayer.x, bottomLeftBoxPlayer.y), Vector3.down);
        currentSpeedY = 0f;
        isOnAir = false;
    }
}
