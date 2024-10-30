using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private CharacterDictionary characterDictionary = null;
    public CharacterDictionary CharacterDictionary { get { return characterDictionary; } }

    public void Init()
    {
        characterDictionary = Manager.Resource.Load<CharacterDictionary>("ScriptableObject/CharacterDictionary");
    }
}
