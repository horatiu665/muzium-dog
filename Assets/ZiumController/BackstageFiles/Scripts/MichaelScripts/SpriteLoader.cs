using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour
{

    [SerializeField]
    private Sprite[] _spriteList = null;
    public Sprite[] SpriteList {
        get { return _spriteList; }
    }

    private void Awake()
    {
    
        if(_spriteList.Length == 0)
        {
            Debug.LogWarning("Hey bub, you forgot to assign sprites in the Inspector.");
        }

        // TODO: later, load these from resources or something

    }


}
