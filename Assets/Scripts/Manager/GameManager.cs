using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private CharacterDictionary characterDictionary = null;
    public CharacterDictionary CharacterDictionary { get { return characterDictionary; } }

    private List<int>[] inBattleList = new List<int>[2];

    public void Init()
    {
        characterDictionary = Manager.Resource.Load<CharacterDictionary>("ScriptableObject/CharacterDictionary");
    }

    public void SetPickList(List<int> PickIDs, bool myPick)
    {
        if (myPick)
            inBattleList[0] = PickIDs;
        else
            inBattleList[1] = PickIDs;
    }

    public List<int> GetPickIDs(bool myPick)
    {
        if (inBattleList[0] == null)
            return myPick ? new List<int> { 1, 4, 7, 25 } : new List<int> { 133, 252, 255, 258 }; // 테스트용
        return myPick ? inBattleList[0] : inBattleList[1];
    }
}
