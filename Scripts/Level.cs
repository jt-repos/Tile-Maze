using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField] List<int> chapterIntervals;
    [SerializeField] float delayBeforeLoad;
    [SerializeField] int coinsPerLevel;
    [SerializeField] int levelsPerUnlock = 10;
    [SerializeField] int levelsRequiredToUnlock = 8;
    [SerializeField] int levelsRequiredToUnlockFullGame = 12;
    [SerializeField] float minimumLoadingTime = 0.3f;
    [SerializeField] GameObject loadingCanvasPrefab;
    [SerializeField] List<int> levelsCompletedInEachChapter;
    string coinsKeyName = "coins";
    string recentLevelIndexKeyName = "recentLevelIndex";
    string loseStreakKeyName = "loseStreak";
    string tutorialPlayedKeyName = "walkthroughCompleted";
    int recentLevelIndex;
    bool willDisplayCoinsAnimation;
    bool isNewHighScore;
    int levelsInChapter;
    int numberOfLevels;
    bool loadAdPostWin;
    public static bool isInMainMenu = false;

    [Header("SFX")]
    [SerializeField] AudioClip loadingSound;

    // Start is called before the first frame update
    private void Awake()
    {
        SetUpSingleton();
        CheckIfLoadTutorial();
        Screen.orientation = ScreenOrientation.Portrait;
        numberOfLevels = chapterIntervals[chapterIntervals.Count - 1]; //last item from index interval list indicates how many levels are there in 
        for (int i = 0; i < chapterIntervals.Count - 1; i++)
        {
            levelsCompletedInEachChapter.Add(0);
        }
        SetLevelsCompletedEachChapter();
    }

    private void SetUpSingleton()
    {
        var levels = FindObjectsOfType<Level>();
        int numberLevels = levels.Length;
        if (numberLevels > 1)
        {
            foreach (Level level in levels)
            {
                if (level != this)
                {
                    Destroy(level.gameObject);
                }
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    private void CheckIfLoadTutorial()
    {

        if (!PlayerPrefs.HasKey(tutorialPlayedKeyName))
        {
            PlayerPrefs.SetInt(tutorialPlayedKeyName, 1);
            isInMainMenu = true;
            LoadTutorial();
        }
        else if (!isInMainMenu)
        {
            LoadMainMenu();
        }
    }

    private void LoadT()
    {
        SceneManager.LoadScene("Walkthrough");
    }

    //////////////////////////////////////////////////////////////////Save Related//////////////////////////////////////////////////////////////////////////////
    public void OverridePlayerPrefs()
    {
        var time = FindObjectOfType<GameSession>().GetCurrentTime();
        var currentSceneIndex = GetCurrentSceneIndex();
        loadAdPostWin = true;
        if (!PlayerPrefs.HasKey(currentSceneIndex.ToString()))
        {
            isNewHighScore = true;
            willDisplayCoinsAnimation = true;
            var newLoseStreak = 0;
            var newCoins = PlayerPrefs.GetInt(coinsKeyName) + coinsPerLevel;
            PlayerPrefs.SetInt(coinsKeyName, newCoins);
            PlayerPrefs.SetFloat(currentSceneIndex.ToString(), time); //save that this index completed with given time
            PlayerPrefs.SetInt(loseStreakKeyName, newLoseStreak);
            var chapterIndex = GetChapterGivenLevel(currentSceneIndex);
            UpdateLevelsCompleted(chapterIndex);
        }
        else
        {
            willDisplayCoinsAnimation = false;
            isNewHighScore = false;
            if (PlayerPrefs.GetFloat(currentSceneIndex.ToString()) > time)
            {
                isNewHighScore = true;
                PlayerPrefs.SetFloat(currentSceneIndex.ToString(), time);
            }
        }
    }

    private void SetLevelsCompletedEachChapter()
    {
        int currentChapterIndex = 0; //keeps track of current chapter and whether it changes
        for (int levelIndex = 0; levelIndex < numberOfLevels; levelIndex++) //goes through every level index
        {
            if (PlayerPrefs.HasKey(levelIndex.ToString())) //if level index is completed
            {
                var chapterIndex = GetChapterGivenLevel(levelIndex); //get corresponding chapter for this level index
                if(currentChapterIndex != chapterIndex) //this means that the chapter has changed
                {
                    currentChapterIndex = chapterIndex; //so update current chapter index
                    levelsCompletedInEachChapter[chapterIndex] = 0; //and reset levels completed in the new chapter to 0
                }
                levelsCompletedInEachChapter[chapterIndex]++; //increment the levels completed in the new chapter
            }
        }
    }

    private void UpdateLevelsCompleted(int chapterIndex)
    {
        levelsCompletedInEachChapter[chapterIndex]++;
    }

    public void SaveHintPurchased(string tileName)
    {
        string hintSaveName = tileName + GetCurrentSceneIndex().ToString(); //save that hint is bought as tile name + level index
        PlayerPrefs.SetInt(hintSaveName, 1); //set as purchased
    }

    public void ResetSaveFile()
    {
        PlayerPrefs.DeleteAll();
    }

    public void IncrementLoseStreak()
    {
        var currentSceneIndexKey = GetCurrentSceneIndex().ToString();
        if(!PlayerPrefs.HasKey(currentSceneIndexKey))
        {
            var newLoseStreak = PlayerPrefs.GetInt(loseStreakKeyName) + 1;
            PlayerPrefs.SetInt(loseStreakKeyName, newLoseStreak);
        }
    }

    public void ChangeCoinCount(int coinValue)
    {
        var currentCoins = PlayerPrefs.GetInt(coinsKeyName);
        PlayerPrefs.SetInt(coinsKeyName, currentCoins + coinValue);
    }

    /////////////////////////////////////////////////////////////////////Loaders////////////////////////////////////////////////////////////////////////////////
    public IEnumerator LoadNextLevel()
    {
        var nextSceneIndex = GetCurrentSceneIndex() + 1;
        if (IsLevelUnlocked(nextSceneIndex))
        {
            StartCoroutine(WaitAndLoad(nextSceneIndex));
        }
        else
        {
            var timeToFadeCanvas = FindObjectOfType<GameSession>().GetTimeToFadeCanvas();
            yield return new WaitForSeconds(delayBeforeLoad + timeToFadeCanvas);
            LoadLevelSelect();
        }
    }

    IEnumerator WaitAndLoad(int sceneIndex)
    {
        if(loadAdPostWin)
        {
            loadAdPostWin = false;
            GetComponent<AdController>().PlayVideoAd();
        }
        var timeToFadeCanvas = FindObjectOfType<GameSession>().GetTimeToFadeCanvas();
        yield return new WaitForSeconds(delayBeforeLoad + timeToFadeCanvas);
        SceneManager.LoadScene(sceneIndex);
        recentLevelIndex = sceneIndex;
        PlayerPrefs.SetInt(recentLevelIndexKeyName, recentLevelIndex);
    }

    public void LoadLevelByIndex(int index)
    {
        if (index < numberOfLevels) //since indexes start from 0 and not from 1
        {
            AsyncLoading(index);
            recentLevelIndex = index;
            PlayerPrefs.SetInt(recentLevelIndexKeyName, recentLevelIndex);
        }
        else
        {
            LoadMainMenu();
        }
    }

    public void LoadRecentLevel()
    {
        if (PlayerPrefs.HasKey(recentLevelIndexKeyName))
        {
            AsyncLoading(PlayerPrefs.GetInt(recentLevelIndexKeyName));
        }
        else
        {
            AsyncLoading(0);
        }
    }

    public void LoadMainMenu()
    {
        isInMainMenu = true;
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadLevelSelect()
    {
        AsyncLoading("Select");
    }

    public void AsyncLoading(string sceneName)
    {
        var loading = SceneManager.LoadSceneAsync(sceneName);
        StartCoroutine(FinishLoading(loading));
    }

    public void AsyncLoading(int sceneIndex)
    {
        var loading = SceneManager.LoadSceneAsync(sceneIndex);
        StartCoroutine(FinishLoading(loading));
    }

    public IEnumerator FinishLoading(AsyncOperation loading)
    {
        Instantiate(loadingCanvasPrefab);
        AudioSource.PlayClipAtPoint(loadingSound, Camera.main.transform.position);
        loading.allowSceneActivation = false;
        yield return new WaitForSeconds(minimumLoadingTime);
        loading.allowSceneActivation = true;
    }

    public void LoadCategorySelect()
    {
        SceneManager.LoadScene("Select Category");
    }

    public void LoadTutorial()
    {
        AsyncLoading("Walkthrough");
    }

    public void QuickRestartLevel() 
    {
        var currentSceneIndex = GetCurrentSceneIndex();
        FindObjectOfType<Level>().AsyncLoading(currentSceneIndex);
    }

    public void RestartLevel()
    {
        FindObjectOfType<GameSession>().FadeOutEndCanvas();
        var currentSceneIndex = GetCurrentSceneIndex();
        StartCoroutine(WaitAndLoad(currentSceneIndex));
    }

    public void RestartLevelAfterFinish()
    {
        FindObjectOfType<Level>().RestartLevel();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    /////////////////////////////////////////////////////////////////////Getters////////////////////////////////////////////////////////////////////////////////
    public int GetStartLevelIndex(int index)
    {
        return chapterIntervals[index];
    }

    public int GetNumberOfLevels()
    {
        return numberOfLevels;
    }

    public bool IsFullGameUnlocked()
    {
        return levelsCompletedInEachChapter[0] >= levelsRequiredToUnlockFullGame; //returns true if completed given number of levels in first chapter
    }

    public int GetLevelsCompletedInChapter(int index)
    {
        return levelsCompletedInEachChapter[index]; 
    }

    public int GetCoinsPerLevel()
    {
        return coinsPerLevel;
    }

    public int GetLevelsPerUnlock()
    {
        return levelsPerUnlock;
    }

    public int GetLevelsRequiredToUnlock()
    {
        return levelsRequiredToUnlock;
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        if(levelIndex < numberOfLevels) //would throw an error if index is greater than in game levels
        {
            Debug.Log(11);
            int chapterIndex = GetChapterGivenLevel(levelIndex);
            int startLevelIndex = chapterIntervals[chapterIndex]; //get the first level in the chapter this level is in
            int levelsCompletedInChapter = levelsCompletedInEachChapter[chapterIndex]; //get levels completed in the chapter this level is in
            return levelsCompletedInChapter / levelsRequiredToUnlock >= (levelIndex - startLevelIndex) / levelsPerUnlock;
        }
        else
        {
            return false;
        }
    }

    public bool isUnlockingNewLevels() 
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex; //get current level index
        int chapterIndex = GetChapterGivenLevel(levelIndex);
        int levelsCompletedInChapter = levelsCompletedInEachChapter[chapterIndex]; //get levels completed in the chapter this level is in
        bool isAnyLevelCompleted = levelsCompletedInChapter != 0; //ensures that the new levels message is not displayed at the very start
        bool isUnlockingNewLevels = levelsCompletedInChapter % levelsRequiredToUnlock == 0; //checks if criteria is met to unlock new levels
        bool hasLockedLevels = levelsCompletedInChapter < numberOfLevels - levelsPerUnlock; //checks if there were any locked levels remaining
        return isAnyLevelCompleted && isUnlockingNewLevels && hasLockedLevels; //return true if all 3 conditions above are met
    }

    public int GetChapterGivenLevel(int levelIndex) //returns the chapter that the passed level index is in
    {
        for (int i = 0; i < chapterIntervals.Count; i++) //goes through every chapter interval from lowest to highest
        {
            if (levelIndex < chapterIntervals[i + 1]) //if a level index is lower than the upper bound of this interval (i.e. fits in it)
            {
                return i; //return the index of the lower bound of this interval
            }
        }
        return 0; //else return 0, as a negative number must have been inputted (invalid parameter)
    }

    public bool GetIsDisplayCoinsAnimation()
    {
        return willDisplayCoinsAnimation;
    }

    public bool GetIsNewHighScore()
    {
        return isNewHighScore;
    }

    public int GetLevelsInChapter()
    {
        return levelsInChapter;
    }

    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public string GetHintSaveName(string tileName)
    {
        var hintSaveName = tileName + GetCurrentSceneIndex().ToString();
        return hintSaveName;
    }

    public bool isHintPurchased(string tileName)
    {
        var isHintPurchased = PlayerPrefs.HasKey(GetHintSaveName(tileName));
        return isHintPurchased;
    }    
}
