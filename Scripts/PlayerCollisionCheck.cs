using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionCheck : MonoBehaviour
{
    public bool GetIsColliding()
    {
        var isColliding = false;
        if (GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Solid", "Player")))
        {
            isColliding = true;
        }
        return isColliding;
    }
}
