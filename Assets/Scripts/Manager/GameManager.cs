using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private Room roomInfo = null;
    public Room RoomInfo
    {
        get
        {
            // 테스트용
            if (roomInfo == null)
            {
                string[] testID = { "1P", "2P" };
                roomInfo = new Room() { key = "Test Key", uids = testID };
            }
            return roomInfo;
        }
    }

    private CharacterDictionary characterDictionary = null;
    public CharacterDictionary CharacterDictionary { get { return characterDictionary; } }

    private List<int>[] inBattleList = new List<int>[2];

    public void Init()
    {
        characterDictionary = Manager.Resource.Load<CharacterDictionary>("ScriptableObject/CharacterDictionary");
    }

    public void SetRoomInfo(Room room)
    {
        roomInfo = room;
    }

    public void SetPickList(List<int> firstPickList, List<int> secondPickList)
    {
        inBattleList[0] = firstPickList;
        inBattleList[1] = secondPickList;
    }

    public List<int> GetPickIDs(bool first)
    {
        var pickIds = first ? inBattleList[0] : inBattleList[1];
        if (pickIds == null)
        {
            Debug.Log("테스트용리스트");
            return first ? new List<int> { 1, 4, 7, 25 } : new List<int> { 133, 252, 255, 258 }; // 테스트용
        }
        return pickIds;
    }
}
