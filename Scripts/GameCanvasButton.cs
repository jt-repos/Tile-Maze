using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCanvasButton : MonoBehaviour
{
    [SerializeField] float timeToAddCoins = 0.3f;
    [SerializeField] AudioClip addCoinsSFX;
    Level level;
    TextMeshProUGUI textComponent;
    bool isNextLevelUnlocked;
    int coins;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        GetComponent<Button>().enabled = true;
        level = FindObjectOfType<Level>();
        SetIsNextLevelUnlocked();
        SetButtonName();
    }

    private void SetIsNextLevelUnlocked()
    {
        var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < level.GetNumberOfLevels()) //if last level, assume next is locked (i.e. doesnt exist, return to menu)
        {
            isNextLevelUnlocked = level.IsLevelUnlocked(nextSceneIndex);
        }
        else
        {
            isNextLevelUnlocked = false;
        }
    }

    private void SetButtonName()
    {
        if (tag == "Win Button")
        {
            if (level.GetIsDisplayCoinsAnimation())
            {
                coins = level.GetCoinsPerLevel();
                textComponent.text = coins.ToString() + " Points";
            }
            else
            {
                if (isNextLevelUnlocked)
                {
                    textComponent.text = "Continue";
                }
                else
                {
                    textComponent.text = "Return";
                }
            }

        }
        else if (tag == "Lose Button")
        {
            textComponent.text = "Retry";
        }
        else if (tag == "Dismiss Button")
        {
            textComponent.text = "Continue";
        }
    }

    public void LoadNextLevelOnPress()
    {
        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        GetComponent<Button>().enabled = false;
        if (tag == "Win Button" && level.GetIsDisplayCoinsAnimation())
        {
            AudioSource.PlayClipAtPoint(addCoinsSFX, Camera.main.transform.position);
            var addCoinInterval = timeToAddCoins / coins;
            for (; coins >= 0; coins--)
            {
                textComponent.text = coins + " Coins";
                yield return new WaitForSeconds(addCoinInterval);
            }
        }
        FindObjectOfType<GameSession>().FadeOutEndCanvas();
        StartCoroutine(level.LoadNextLevel());
    }

    public void RestartLevel()
    {
        GetComponent<Button>().enabled = false;
        level.RestartLevel();
    }

    public void HideWarningCanvas()
    {
        FindObjectOfType<GameSession>().HideWarningCanvas();
    }
}
