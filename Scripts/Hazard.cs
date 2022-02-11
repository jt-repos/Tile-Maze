using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] GameObject solidPrefab;
    [SerializeField] AudioClip waterEnterSFX;
    [SerializeField] float flashTime = 0.05f;
    [SerializeField] float changeColorTime = 1f;
    [SerializeField] float changeColorTimeFast = 0.5f;
    [SerializeField] Color blue;
    bool hasFinishedFirstLoop = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var gameSession = FindObjectOfType<GameSession>();
        gameSession.PlayDeathWaterVFX(transform.position);
        gameSession.SwitchDeathEffects(waterEnterSFX, false);
        StartCoroutine(FlashBlue());
        StartCoroutine(gameSession.LevelLost());
    }

    public void SwapToSolid()
    {
        var solid = Instantiate(solidPrefab, transform.position, Quaternion.identity);
        solid.GetComponent<Solid>().SwitchToSolid(false);
        Destroy(gameObject);
    }

    public IEnumerator FlashBlue() //flash (go bright and dark), then loop very bright and normal red
    {
        float repeats;
        while(this != null)
        {
            repeats = hasFinishedFirstLoop ? 60 * changeColorTimeFast : 60 * flashTime;
            yield return StartCoroutine(ChangeColor(Color.white, blue, repeats));
            repeats = 60 * changeColorTime;
            yield return StartCoroutine(ChangeColor(blue, Color.white, repeats));
            hasFinishedFirstLoop = true;
        }
    }

    private IEnumerator ChangeColor(Color startColor, Color endColor, float repeats)
    {
        GetComponent<SpriteRenderer>().color = startColor;
        for (int i = 0; i < repeats; i++)
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(startColor, endColor, i / repeats);
            yield return new WaitForFixedUpdate();
        }
        GetComponent<SpriteRenderer>().color = endColor;
        yield return new WaitForFixedUpdate();
    }
}
