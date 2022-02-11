using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] GameObject keyBgPrefab;
    [SerializeField] List<Sprite> keySprites;
    GameObject background;

    private void Start()
    {
        background = Instantiate(keyBgPrefab, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            transform.parent.GetComponent<Tile>().CountKeysToOpen();
            Destroy(background);
            Destroy(gameObject);
        }        
    }

    public void SetKeySprite(int spriteIndex)
    {
        GetComponent<SpriteRenderer>().sprite = keySprites[spriteIndex];
    }
}
