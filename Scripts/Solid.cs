using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solid : MonoBehaviour
{
    [SerializeField] Sprite solidSprite;
    [SerializeField] Sprite spriteHint;

    private void OnTriggerExit2D(Collider2D collision)
    {
        SwitchToSolid(false);
    }

    public void SwitchToSolid(bool isHint)
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
        GetComponent<SpriteRenderer>().sprite = solidSprite;
        gameObject.layer = 12; //12 is solid
    }

    public void SetIsHint(bool boolVar)
    {
        GetComponent<SpriteRenderer>().sprite = spriteHint;
        FindObjectOfType<HintButton>().SetActive(true);
    }
}
