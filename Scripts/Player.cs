using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float velocity;
    [SerializeField] float startMoveCd;
    [SerializeField] bool isDummy;
    [SerializeField] float hintPlayInterval = 0.5f;
    Vector3 destinationPos;
    float raycastStartFraction = 0.3f;
    float orientationAngle;
    bool isMoving = false;
    bool isStuck = false;
    int solidLayer;
    int halfSolidLayer;
    Tile currentHintTile;
    bool interruptHints;

    [Header("Touch Screen")]
    [SerializeField] float minDisForSwipe;
    Vector2 fingerDownPos;
    Vector2 fingerUpPos;

    [Header("VFX")]
    [SerializeField] GameObject wallCollisionVFX;

    [Header("SFX")]
    [SerializeField] AudioClip hitWallSound;

    // Start is called before the first frame update
    void Start()
    {
        solidLayer = LayerMask.GetMask("Solid");
        halfSolidLayer = LayerMask.GetMask("Half Solid");
        FindObjectOfType<CompositeCollider2D>().gameObject.layer = 12;
        destinationPos = transform.position;
    }

    public IEnumerator PlayHints()
    {
        FreezeMovementAll(true);
        FindObjectOfType<Level>().SaveHintPurchased(currentHintTile.name); //save 
        yield return new WaitForSeconds(hintPlayInterval); //set 2x since it sometimes took long to finish the ad and load back the game
        var currentHints = currentHintTile.GetHints();
        foreach(int directionValue in currentHints)
        {
            if (directionValue == -2)
            {
                destinationPos = transform.position + Vector3.left;
                orientationAngle = 90f;
            }
            else if(directionValue == -1)
            {
                destinationPos = transform.position + Vector3.down;
                orientationAngle = 180f;
            }
            else if(directionValue == 1)
            {
                destinationPos = transform.position + Vector3.up;
                orientationAngle = 0f;
            }
            else if(directionValue == 2)
            {
                destinationPos = transform.position + Vector3.right;
                orientationAngle = -90f;
            }
            var didCollide = CheckCollisionAndMove();
            if(didCollide)
            {
                FindObjectOfType<GameSession>().IncreasePlayerHintFails();
            }
            else
            {
                FindObjectOfType<GameSession>().ResetPlayerHintFails();
            }
            if (interruptHints)
            {
                yield break;
            }
            yield return new WaitForSeconds(hintPlayInterval);
        }
        FindObjectOfType<GameSession>().SetIsHintPlaying(false);
        StartCoroutine(FindObjectOfType<CameraController>().ChangeColorBlack());
        FreezeMovementAll(false);
    }

    // Update is called once per frame
    void Update()
    {
        startMoveCd -= Time.deltaTime;
        if (!isStuck)
        {
            CheckIfStuck();
            if(!isMoving && startMoveCd < 0)
            {
                Move();
            }
        }
        if (Input.GetButtonDown("Reset")) //for dev
        {
            FindObjectOfType<Level>().QuickRestartLevel();
        }
    }

    private void CheckIfStuck()
    {

        foreach(PlayerCollisionCheck collisionCheck in FindObjectsOfType<PlayerCollisionCheck>())
        {
            if(!collisionCheck.GetIsColliding())
            {
                return;
            }
        }
        var gameSession = FindObjectOfType<GameSession>();
        if (!gameSession.GetIsLevelCompleted())
        {
            isStuck = true;
            if(isDummy)
            {
                StartCoroutine(FindObjectOfType<TutorialText>().FinishTutorial());
            }
            else
            {
                StartCoroutine(gameSession.LevelLost());
            }
        }
    }

    private void CheckIfStuckk()
    {
        var startPos = transform.position + (destinationPos - transform.position) * raycastStartFraction;
        var solidCollisionCheckRay = Physics2D.Raycast(startPos, destinationPos - transform.position, 1f - raycastStartFraction, solidLayer);
        var playerCollisionCheckRay = Physics2D.Raycast(startPos, destinationPos - transform.position, 1f - raycastStartFraction, halfSolidLayer);

        var gameSession = FindObjectOfType<GameSession>();
        if (!gameSession.GetIsLevelCompleted())
        {
            isStuck = true;
            if (isDummy)
            {
                StartCoroutine(FindObjectOfType<TutorialText>().FinishTutorial());
            }
            else
            {
                StartCoroutine(gameSession.LevelLost());
            }
        }
    }

    private void Move()
    {
        if(Input.touches.Length > 0)
        {
            MoveOnSwipe();
        }
        if (Input.GetButtonDown("Horizontal"))
        {
            var direction = Mathf.Sign(Input.GetAxisRaw("Horizontal")); //left or right, up or down
            destinationPos = transform.position + Vector3.right * direction;
            orientationAngle = direction > 0 ? -90f : 90f;
            CheckCollisionAndMove();
        }
        else if (Input.GetButtonDown("Vertical"))
        {
            var direction = Mathf.Sign(Input.GetAxisRaw("Vertical")); //left or right, up or down
            destinationPos = transform.position + Vector3.up * direction;
            orientationAngle = direction > 0 ? 0f : 180f;
            CheckCollisionAndMove();
        }
    }

    private void MoveOnSwipe()
    {
        if (Input.touches[0].phase == TouchPhase.Began)
        {
            fingerUpPos = Input.touches[0].position;
            fingerDownPos = Input.touches[0].position;
        }
        if (Input.touches[0].phase == TouchPhase.Ended)
        {
            fingerDownPos = Input.touches[0].position;
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        var verticalSwipeDis = Mathf.Abs(fingerDownPos.y - fingerUpPos.y);
        var horizontalSwipeDis = Mathf.Abs(fingerDownPos.x - fingerUpPos.x);
        if (verticalSwipeDis > minDisForSwipe || horizontalSwipeDis > minDisForSwipe)
        {
            if (verticalSwipeDis > horizontalSwipeDis)
            {
                var isPositive = fingerDownPos.y - fingerUpPos.y > 0;
                destinationPos = isPositive ? transform.position + Vector3.up : transform.position + Vector3.down;
                orientationAngle = isPositive ? 0f : 180f;
            }
            else
            {
                var isPositive = fingerDownPos.x - fingerUpPos.x > 0;
                destinationPos = isPositive ? transform.position + Vector3.right : transform.position + Vector3.left;
                orientationAngle = isPositive ? -90f : 90f;
            }
            CheckCollisionAndMove();
        }
    }

    private bool CheckCollisionAndMove()
    {
        var startPos = transform.position + (destinationPos - transform.position) * raycastStartFraction;
        Debug.DrawRay(startPos, destinationPos - transform.position, Color.yellow, 1f); //visual
        var solidCollisionCheckRay = Physics2D.Raycast(startPos, destinationPos - transform.position, 1f - raycastStartFraction, solidLayer);
        var playerCollisionCheckRay = Physics2D.Raycast(startPos, destinationPos - transform.position, 1f - raycastStartFraction, halfSolidLayer);
        if (solidCollisionCheckRay.collider == null && playerCollisionCheckRay.collider == null) 
        {
            StartCoroutine(MovePlayer());
            return false;
        }
        else
        {
            WallHitEffects();
            return true;
        }
    }

    private void WallHitEffects()
    {
        if(FindObjectsOfType<Player>().Length == 1) //dont do for multi
        {
            StartCoroutine(FindObjectOfType<CameraController>().Flash());
        }
        AudioSource.PlayClipAtPoint(hitWallSound, Camera.main.transform.position, 0.5f);
        var wallEffect = Instantiate(wallCollisionVFX, transform.position, Quaternion.identity);
        wallEffect.transform.rotation = Quaternion.Euler(0, 0, orientationAngle);
        Destroy(wallEffect, 1f);
    }

    public IEnumerator MovePlayer()
    {
        if(FindObjectsOfType<HintButton>().Length > 0) //no button in tutorial
        {
            FindObjectOfType<HintButton>().SetActive(false);
        }
        FindObjectOfType<GameSession>().SetIsTimeOn(true);
        isMoving = true;
        LeanTween.moveLocal(gameObject, destinationPos, 1f / velocity).setEaseOutExpo();
        yield return new WaitForSeconds(1f / velocity);
        transform.position = destinationPos;
        yield return new WaitForFixedUpdate();
        isMoving = false;
    }

    public void SetCurrentHintTile(Tile hintTile)
    {
        currentHintTile = hintTile;
        var isHintPurchased = FindObjectOfType<Level>().isHintPurchased(hintTile.name);
        FindObjectOfType<HintButton>().SetIsCurrentHintPurchased(isHintPurchased);
    }

    public void SetInterruptHints(bool boolVar)
    {
        interruptHints = boolVar;
    }

    public void FreezeMovementAll(bool boolVar)
    {
        foreach (Player player in FindObjectsOfType<Player>())
        {
            player.SetIsStuck(boolVar);
        }
    }

    public void SetIsStuck(bool boolVar)
    {
        isStuck = boolVar;
    }    
}
