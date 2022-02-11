using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Hint Tile")]
    [SerializeField] bool isHintTile;
    [SerializeField] [Range(-2, 2)] List<int> hintList; //1 up, 2 right, -1 down, -2 left

    [Header("Standard")]
    [SerializeField] List<Sprite> tileLevels;
    [SerializeField] List<Sprite> hintTileLevels;
    [SerializeField] int tileIndex;
    [SerializeField] GameObject solidPrefab;

    [Header("Gate")]
    [SerializeField] GameObject lockBgPrefab;
    [SerializeField] GameObject lockPrefab;
    [SerializeField] List<Sprite> lockSprites;
    [SerializeField] float timeOfAnimation;
    [SerializeField] float changeInSize;
    GameObject lockObject;
    GameObject lockBackground;
    int numberOfKeys;
    bool isGate = false;
    bool isProcessOnCooldown = false;

    [Header("SFX")]
    [SerializeField] List<AudioClip> tileSounds;
    [SerializeField] AudioClip openLockSound;
    [SerializeField] [Range(0f, 1f)] List<float> tileVolumes; //0 is default, 2 and 3 were too loud

    //cached
    Collider2D myCollider;
    SpriteRenderer mySpriteRenderer;
    Sprite currentSprite;
    bool isPlayerInsideHint;

    // Start is called before the first frame update
    void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();
        UpdateSprite();
        if (GetComponentsInChildren<Key>().Length > 0) //tile is a gate
        {
            SetUpGateTile();
        }
    }

    private void SetUpGateTile()
    {
        isGate = true;
        gameObject.layer = 12;
        myCollider.isTrigger = false;
        var gateIndex = FindObjectOfType<GameSession>().GetNewGateIndex();
        lockBackground = Instantiate(lockBgPrefab, transform.position, Quaternion.identity);
        lockObject = Instantiate(lockPrefab, transform.position, Quaternion.identity);
        lockObject.GetComponent<SpriteRenderer>().sprite = lockSprites[gateIndex];
        foreach (Key key in GetComponentsInChildren<Key>())
        {
            numberOfKeys++;
            key.SetKeySprite(gateIndex);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !isGate && !isProcessOnCooldown)
        {
            StartCoroutine(SetProcessCooldown());
            ProcessIfHint();
            ProcessPlayerTrigger();
        }
    }

    private IEnumerator SetProcessCooldown()
    {
        isProcessOnCooldown = true;
        yield return new WaitForFixedUpdate();
        isProcessOnCooldown = false;
    }

    private void OnTriggerExit2D(Collider2D collision) //out of index because it calls these below once deleted since only solid left when you step on tile with index 1
    {
        if (collision.tag == "Player" && !isGate && isPlayerInsideHint) //only if hint and tile greater than 1
        {
            isHintTile = false;
            UpdateSprite();
        }
    }


    private void ProcessIfHint()
    {
        if (isHintTile && !isPlayerInsideHint)
        {
            isPlayerInsideHint = true;
            FindObjectOfType<HintButton>().SetActive(true);
            foreach (Player player in FindObjectsOfType<Player>())
            {
                player.SetCurrentHintTile(this);
            }
        }
    }

    private void ProcessPlayerTrigger()
    {
        tileIndex--;
        if (tileIndex >= 0)
        {
            UpdateSprite();
        }
        else
        {
            mySpriteRenderer.color = Color.clear;
            FindObjectOfType<GameSession>().DecreaseTileCount();
            var solid = Instantiate(solidPrefab, transform.position, Quaternion.identity);
            if(isHintTile)
            {
                solid.GetComponent<Solid>().SetIsHint(isHintTile);
            }
        }
        AudioSource.PlayClipAtPoint(tileSounds[tileIndex + 1], Camera.main.transform.position, tileVolumes[tileIndex + 1]);
    }

    public void CountKeysToOpen() //for gates only
    {
        numberOfKeys--;
        if (numberOfKeys <= 0)
        {
            isGate = false;
            gameObject.layer = 0;
            myCollider.isTrigger = true;
            StartCoroutine(DestroyLockAnimation());
        }
    }

    private IEnumerator DestroyLockAnimation()
    {
        AudioSource.PlayClipAtPoint(openLockSound, Camera.main.transform.position, tileVolumes[0]);
        var repeats = 60 * timeOfAnimation; //60 frames
        Color color = GetComponent<SpriteRenderer>().color;
        for (int i = 0; i < repeats; i++)
        {
            lockObject.transform.localScale += new Vector3(changeInSize / repeats, changeInSize / repeats);
            color.a -= 1f / repeats;
            lockObject.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForFixedUpdate();
        }
        Destroy(lockObject);
        Destroy(lockBackground);
    }

    public bool GetIsGate()
    {
        return isGate;
    }

    public int GetTileIndex()
    {
        return tileIndex;
    }

    public List<int> GetHints()
    {
        return hintList;
    }

    public void SetIsHintTile(bool boolVar)
    {
        isHintTile = boolVar;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if(tileIndex >= 0)
        {
            currentSprite = isHintTile ? hintTileLevels[tileIndex] : tileLevels[tileIndex];
            mySpriteRenderer.sprite = currentSprite;
        }
    }
}
