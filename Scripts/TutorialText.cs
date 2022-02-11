using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    [SerializeField] string text1;
    [SerializeField] string text2;
    [SerializeField] string text3;
    [SerializeField] float intervalBetweenFade;
    TextMeshProUGUI tutorialText;
    GameSession gameSession;
    GameObject canvas;
    float timeToFadeCanvas;


    // Start is called before the first frame update
    void Start()
    {
        canvas = transform.parent.gameObject;
        gameSession = FindObjectOfType<GameSession>();
        timeToFadeCanvas = gameSession.GetTimeToFadeCanvas();
        tutorialText = GetComponent<TextMeshProUGUI>();
        tutorialText.text = text1;
    }

    public IEnumerator FinishTutorial()
    {
        StartCoroutine(ChangeText(text2));
        yield return new WaitForSeconds(intervalBetweenFade + timeToFadeCanvas);
        StartCoroutine(ChangeText(text3));
        yield return new WaitForSeconds(intervalBetweenFade + timeToFadeCanvas);
        StartCoroutine(FindObjectOfType<GameSession>().FadeCanvasAlpha(0f, canvas));
        yield return new WaitForSeconds(timeToFadeCanvas);
        FindObjectOfType<Level>().LoadMainMenu();
    }

    public IEnumerator ChangeText(string text)
    {
        StartCoroutine(gameSession.FadeCanvasAlpha(0f, canvas));
        yield return new WaitForSeconds(timeToFadeCanvas);
        tutorialText.text = text;
        StartCoroutine(gameSession.FadeCanvasAlpha(1f, canvas));
        yield return new WaitForSeconds(timeToFadeCanvas);
    }
}
