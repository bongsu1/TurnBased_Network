using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDictionary", menuName = "ScriptableObject/CharacterDictionary")]
public class CharacterDictionary : ScriptableObject
{
    [SerializeField] List<CharacterData> characterDatas = new List<CharacterData>();

    private Dictionary<int, CharacterData> characterDic = null;

    private void Init()
    {
        if (characterDic != null && characterDatas.Count == characterDic.Count)
            return;

        characterDic = new Dictionary<int, CharacterData>(31);

        for (int i = 0; i < characterDatas.Count; i++)
        {
            CharacterData data = characterDatas[i];
            characterDic.Add(data.charID, data);
        }
    }

    public CharacterData this[int charID]
    {
        get
        {
            Init();
            if (characterDic.TryGetValue(charID, out CharacterData data))
                return data;

            return null;
        }
    }

    public int[] GetIDs()
    {
        Init();
        int size = characterDatas.Count;
        int[] keys = new int[size];
        for (int i = 0; i < size; i++)
        {
            keys[i] = characterDatas[i].charID;
        }
        return keys;
    }
}
