using System.Collections;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    [SerializeField] float cameraPosY;
    [SerializeField] float cameraShakeTime = 0.1f;
    [SerializeField] float cameraShakeMagnitude = 0.1f;
    [SerializeField] float flashTime = 0.05f;
    [SerializeField] float changeColorTime = 1f;
    [SerializeField] float changeColorTimeFast = 0.5f;
    [SerializeField] Color white;
    [SerializeField] Color red;
    [SerializeField] Color brightRed;
    [SerializeField] Color veryBrightRed;
    [SerializeField] Color blue;
    [SerializeField] Color darkBlue;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, cameraPosY, transform.position.z);
    }

    public IEnumerator Flash()
    {
        var repeats = 60 * flashTime;
        yield return StartCoroutine(ChangeColor(Color.black, white, repeats));
        yield return StartCoroutine(ChangeColor(white, Color.black, repeats));
    }

    public IEnumerator ChangeColorBlue()
    {
        var repeats = 60 * changeColorTime;
        yield return StartCoroutine(ChangeColor(Color.black, blue, repeats));
    }

    public IEnumerator FlashBrightRed() //flash (go bright and dark), then loop very bright and normal red
    {
        StartCoroutine(ShakeCamera());
        var repeats = 60 * flashTime;
        yield return StartCoroutine(ChangeColor(Color.black, brightRed, repeats));
        repeats = 60 * changeColorTime;
        yield return StartCoroutine(ChangeColor(brightRed, red, repeats));
        StartCoroutine(ChangeColorVeryBrightRed());
    }

    public IEnumerator ChangeColorVeryBrightRed()
    {
        var repeats = 60 * changeColorTimeFast;
        yield return StartCoroutine(ChangeColor(red, veryBrightRed, repeats));
        StartCoroutine(ChangeColorRed());
    }

    public IEnumerator ChangeColorRed()
    {
        var repeats = 60 * changeColorTime;
        yield return StartCoroutine(ChangeColor(veryBrightRed, red, repeats));
        StartCoroutine(ChangeColorVeryBrightRed());
    }

    public IEnumerator ChangeColorBlack()
    {
        var repeats = 60 * changeColorTimeFast;
        yield return StartCoroutine(ChangeColor(blue, Color.black, repeats));
    }

    private IEnumerator ChangeColor(Color startColor, Color endColor, float repeats)
    {
        GetComponent<Camera>().backgroundColor = startColor;
        for (int i = 0; i < repeats; i++)
        {
            GetComponent<Camera>().backgroundColor = Color.Lerp(startColor, endColor, i / repeats);
            yield return new WaitForFixedUpdate();
        }
        GetComponent<Camera>().backgroundColor = endColor;
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator ShakeCamera()
    {
        var startPos = transform.position;
        var repeats = 60 * cameraShakeTime;
        for (int i = 0; i < repeats; i++)
        {
            transform.position = startPos + new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)) * cameraShakeMagnitude;
            yield return new WaitForEndOfFrame();
        }
        transform.position = startPos;
    }
}
