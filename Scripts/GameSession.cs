using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] bool isTutorial;
    [SerializeField] GameObject backgroundVFX;

    [Header("Sprite")]
    [SerializeField] List<Color> exitSpriteColors;
    int currentColorIndex;

    [Header("Dialog Canvas")]
    [SerializeField] GameObject inGameCanvasPrefab;
    [SerializeField] GameObject winCanvasPrefab;
    [SerializeField] GameObject newLevelsCanvasPrefab;
    [SerializeField] GameObject loseCanvasPrefab;
    [SerializeField] GameObject loseStreakCanvasPrefab;
    [SerializeField] GameObject hintCanvasPrefab;
    [SerializeField] GameObject warningCanvasPrefab;
    [SerializeField] int loseStreakRequired;
    [SerializeField] float timeToFadeCanvas;
    [SerializeField] float endCanvasAlpha;

    [Header("VFX")]
    [SerializeField] GameObject winVFX;
    [SerializeField] GameObject playHintVFX;
    [SerializeField] GameObject deathNormalVFX;
    [SerializeField] GameObject deathWaterVFX;

    [Header("SFX")]
    [SerializeField] AudioClip loseSFX;
    [SerializeField] AudioClip winSFX;
    [SerializeField] AudioClip winNewLevelsSFX;
    [SerializeField] AudioClip discardSFX;
    [SerializeField] AudioClip loadHintCanvasSFX;
    [SerializeField] AudioClip playHintSFX;
    [SerializeField] AudioClip warningSFX;
    GameObject winCanvas;
    GameObject loseCanvas;
    GameObject hintCanvas;
    GameObject warningCanvas;
    Level level;
    bool levelHasFinish;
    bool isPlayerOnFinish;
    int tileCount;
    int gateIndex;
    bool levelCompleted = false;
    float currentTime;
    bool isTimeOn = false;
    int playerHintFails; //to test how many players crashed into a wall during hint
    bool playNormalDeathVFX = true;
    bool isHintPlaying;

    // Start is called before the first frame update
    void Start()
    {
        level = FindObjectOfType<Level>();
        SetUpLevelFeatures();
        SetUpExitGate();
        CountTiles();
        FreezePlayerIfTooltip();
    }

    private void SetUpLevelFeatures()
    {
        if (!isTutorial)
        {
            Instantiate(inGameCanvasPrefab, transform.position, Quaternion.identity);
            Instantiate(backgroundVFX, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            Debug.Log(222);
        }
    }

    private void CountTiles()
    {
        var tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles)
        {
            tileCount++;
        }
    }

    private void SetUpExitGate()
    {
        if (transform.childCount > 0)
        {
            levelHasFinish = true;
        }
        else
        {
            AlphaOut();
        }
    }

    private static void FreezePlayerIfTooltip()
    {
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.tag == "Tooltip Canvas")
            {
                FindObjectOfType<Player>().FreezeMovementAll(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimeOn)
        {
            currentTime += Time.deltaTime;
        }
        if (levelHasFinish && !levelCompleted)
        {
            if (transform.childCount > 0)
            {
                var newSpriteIndex = transform.GetComponentInChildren<Tile>().GetTileIndex();
                GetComponent<SpriteRenderer>().color = exitSpriteColors[newSpriteIndex];
            }
            else
            {
                AlphaOut();
            }
        }
    }

    private void AlphaOut()
    {
        var color = GetComponent<SpriteRenderer>().color;
        color.a = 0;
        GetComponent<SpriteRenderer>().color = color;
    }

    public void DecreaseTileCount()
    {
        tileCount--;
        if (tileCount <= 0)
        {
            if (!levelHasFinish || isPlayerOnFinish)
            {
                if (!isTutorial)
                {
                    LevelWon();
                }
            }
        }
    }

    public void LevelWon()
    {
        SetIsTimeOn(false);
        levelCompleted = true;
        var level = FindObjectOfType<Level>();
        level.OverridePlayerPrefs();
        SpawnWinCanvas(level);
        Instantiate(winVFX);
        StartCoroutine(FindObjectOfType<CameraController>().ChangeColorBlue());
        FindObjectOfType<Player>().FreezeMovementAll(true);
        winCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        StartCoroutine(FadeCanvasAlpha(endCanvasAlpha, winCanvas));
        foreach (Hazard hazardTile in FindObjectsOfType<Hazard>())
        {
            hazardTile.SwapToSolid();
        }
    }

    public IEnumerator LevelLost()
    {
        if(playNormalDeathVFX)
        {
            PlayDeathNormalVFX();
        }
        AudioSource.PlayClipAtPoint(loseSFX, Camera.main.transform.position, 0.8f);
        StartCoroutine(FindObjectOfType<CameraController>().FlashBrightRed());
        SetIsTimeOn(false);
        level.IncrementLoseStreak();
        foreach (Player player in FindObjectsOfType<Player>())
        {
            player.FreezeMovementAll(true);
        }
        yield return new WaitForSeconds(timeToFadeCanvas); //tego nie musi byc ale robi dramatyczną pauze wiec zostaje
        SpawnLoseCanvas();
        StartCoroutine(FadeCanvasAlpha(endCanvasAlpha, loseCanvas));
    }

    private void SpawnWinCanvas(Level level)
    {
        if (level.isUnlockingNewLevels() && level.GetIsDisplayCoinsAnimation())
        {
            winCanvas = Instantiate(newLevelsCanvasPrefab, new Vector2(0, 0), Quaternion.identity);
            AudioSource.PlayClipAtPoint(winNewLevelsSFX, Camera.main.transform.position, 0.7f);
        }
        else
        {
            winCanvas = Instantiate(winCanvasPrefab, new Vector2(0, 0), Quaternion.identity);
            AudioSource.PlayClipAtPoint(winSFX, Camera.main.transform.position, 0.3f);
        }
    }

    private void SpawnLoseCanvas()
    {
        var loseStreak = PlayerPrefs.GetInt("loseStreak");
        if (loseStreak != 0 && loseStreak % loseStreakRequired == 0)
        {
            loseCanvas = Instantiate(loseStreakCanvasPrefab, new Vector2(0, 0), Quaternion.identity);
        }
        else
        {
            loseCanvas = Instantiate(loseCanvasPrefab, new Vector2(0, 0), Quaternion.identity);
        }
    }

    public void FadeOutTooltipCanvas()
    {
        StartCoroutine(StopPlayerFreezeAfterInterval());
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.tag == "Tooltip Canvas")
            {
                StartCoroutine(FadeCanvasAlpha(0f, canvas.gameObject));
                AudioSource.PlayClipAtPoint(discardSFX, Camera.main.transform.position); 
            }
        }
    }

    private IEnumerator StopPlayerFreezeAfterInterval()
    {
        yield return new WaitForSeconds(0.1f);
        FindObjectOfType<Player>().FreezeMovementAll(false);
    }

    public void FadeInEndCanvas()
    {
        var canvas = FindObjectOfType<Canvas>().gameObject;
        StartCoroutine(FadeCanvasAlpha(1f, canvas));
    }

    public void FadeOutEndCanvas()
    {
        var canvas = FindObjectOfType<Canvas>().gameObject;
        StartCoroutine(FadeCanvasAlpha(0f, canvas));
    }

    public IEnumerator FadeCanvasAlpha(float newAlpha, GameObject canvas)
    {
        var currentAlpha = canvas.GetComponent<CanvasGroup>().alpha;
        var alphaChangePerTick = (newAlpha - currentAlpha) / (timeToFadeCanvas * 60);
        for (int i = 0; i < timeToFadeCanvas * 60; i++)
        {
            canvas.GetComponent<CanvasGroup>().alpha += alphaChangePerTick;
            yield return new WaitForSeconds(timeToFadeCanvas / 60);
        }
        if (newAlpha <= 0 && !isTutorial)
        {
            Destroy(canvas);
        }
    }

    public void SpawnBuyHintCanvas()
    {
        SetIsTimeOn(false);
        FindObjectOfType<Player>().FreezeMovementAll(true);
        hintCanvas = Instantiate(hintCanvasPrefab, new Vector2(0, 0), Quaternion.identity);
        hintCanvas.GetComponent<CanvasGroup>().alpha = 0;
        StartCoroutine(FadeCanvasAlpha(1f, hintCanvas));
        AudioSource.PlayClipAtPoint(loadHintCanvasSFX, Camera.main.transform.position);
    }

    public IEnumerator SpawnWarningCanvas()
    {
        yield return new WaitForSeconds(timeToFadeCanvas);
        SetIsTimeOn(false);
        FindObjectOfType<Player>().FreezeMovementAll(true);
        warningCanvas = Instantiate(warningCanvasPrefab, new Vector2(0, 0), Quaternion.identity);
        warningCanvas.GetComponent<CanvasGroup>().alpha = 0;
        StartCoroutine(FadeCanvasAlpha(1f, warningCanvas));
        AudioSource.PlayClipAtPoint(warningSFX, Camera.main.transform.position, 0.5f);
    }

    public void HideWarningCanvas()
    {
        SetIsTimeOn(true);
        FindObjectOfType<Player>().FreezeMovementAll(false);
        AudioSource.PlayClipAtPoint(discardSFX, Camera.main.transform.position);
        StartCoroutine(FadeCanvasAlpha(0f, warningCanvas));
        Destroy(warningCanvas.gameObject, timeToFadeCanvas + 0.5f);
    }

    public void HideCanvasPlayHint()
    {
        HideBuyHintCanvas(false);
        FindObjectOfType<Player>().FreezeMovementAll(false);
        if(!isHintPlaying)
        {
            PlayHint();
        }
    }

    public void PlayHint()
    {
        isHintPlaying = true;
        FindObjectOfType<HintButton>().SetActive(false);
        foreach(Player player in FindObjectsOfType<Player>())
        {
            StartCoroutine(player.PlayHints());
        }
        StartCoroutine(FindObjectOfType<CameraController>().ChangeColorBlue());
        Instantiate(playHintVFX);
        AudioSource.PlayClipAtPoint(playHintSFX, Camera.main.transform.position, 0.5f);
    }

    public void HideBuyHintCanvas(bool playSound)
    {
        SetIsTimeOn(true);
        FindObjectOfType<Player>().FreezeMovementAll(false);
        StartCoroutine(FadeCanvasAlpha(0f, hintCanvas));
        Destroy(hintCanvas.gameObject, timeToFadeCanvas + 0.5f);
        if(playSound)
        {
            AudioSource.PlayClipAtPoint(discardSFX, Camera.main.transform.position);
        }
    }

    public void PlayDeathWaterVFX(Vector3 position)
    {
        Instantiate(deathWaterVFX, position, Quaternion.identity);
    }

    public void PlayDeathNormalVFX()
    {
        foreach(Player player in FindObjectsOfType<Player>())
        {
            var pos = player.transform.position;
            Instantiate(deathNormalVFX, pos, Quaternion.identity);
        }
    }

    public void SwitchDeathEffects(AudioClip clip, bool playDeathEffects)
    {
        loseSFX = clip;
        playNormalDeathVFX = playDeathEffects;
    }

    public int GetNewGateIndex()
    {
        gateIndex++;
        return gateIndex - 1;
    }

    public bool GetIsLevelCompleted()
    {
        return levelCompleted;
    }

    public float GetTimeToFadeCanvas()
    {
        return timeToFadeCanvas;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isPlayerOnFinish = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerOnFinish = false;
    } 

    public void SetIsTimeOn(bool variable)
    {
        isTimeOn = variable;
        FindObjectOfType<AdController>().SetAdTimer(variable);
    }

    public bool IsTimeOn()
    {
        return isTimeOn;
    }


    public void IncreasePlayerHintFails()
    {
        playerHintFails++;
        StartCoroutine(CheckIfAllPlayerHintsFailed());
    }

    public IEnumerator CheckIfAllPlayerHintsFailed()
    {
        yield return new WaitForFixedUpdate(); //waits for all players to send their hint status, i.e. failed or not
        var playerCount = FindObjectsOfType<Player>().Length;
        if (playerHintFails == playerCount)
        {
            isHintPlaying = false; //if all stuck, hint is not playing
            StartCoroutine(SpawnWarningCanvas());
            foreach (Player player in FindObjectsOfType<Player>())
            {
                player.SetInterruptHints(true);
            }
        }
    }

    public void ResetPlayerHintFails()
    {
        playerHintFails = 0;
    }

    public void SetIsHintPlaying(bool boolVar)
    {
        isHintPlaying = boolVar;
    }
}
