using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuVFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        SetUpSingleton();
    }

    private void Update()
    {
        if(FindObjectsOfType<GameSession>().Length > 0)
        {
            Destroy(gameObject);
        }
    }

    private void SetUpSingleton()
    {
        var vfx = FindObjectsOfType<MenuVFX>();
        if(vfx.Length > 2)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
