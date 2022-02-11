using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintController : MonoBehaviour
{
    [SerializeField] int costToPlayHint = 50;

    // Start is called before the first frame update
    void Start()
    {
        var coins = PlayerPrefs.GetInt("coins");
        var isAdReady = FindObjectOfType<AdController>().IsAdReady();
        if (costToPlayHint > coins)
        {
            GameObject.FindGameObjectWithTag("Pay Coins Button").GetComponent<Button>().interactable = false;
        }
        if(!isAdReady)
        {
            GameObject.FindGameObjectWithTag("Watch Video Button").GetComponent<Button>().interactable = false;
        }
    }

    public void HideHintCanvas()
    {
        FindObjectOfType<GameSession>().HideBuyHintCanvas(true);
    }

    public void PlayHintCoins()
    {
        FindObjectOfType<Level>().ChangeCoinCount(-costToPlayHint);
        FindObjectOfType<DisplayScore>().UpdateCoinDisplay();
        FindObjectOfType<GameSession>().HideCanvasPlayHint();
    }

    public void PlayHintVideo()
    {
        FindObjectOfType<AdController>().PlayRewardedVideoAd();
    }
}
