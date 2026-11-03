using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Player
{
    public int player_id;
    public string username;
    public int level;
}

[System.Serializable]
public class InventoryItem
{
    public int item_id;
    public string name;
    public string description;
    public int value;
    public int quantity;
}

[System.Serializable]
public class Quest
{
    public int quest_id;
    public string title;
    public string description;
    public int reward_exp;
    public int status;
}

public class GameDataManager : MonoBehaviour
{
    private string serverURL = "http://localhost:3000";
    private Player currentPlayer = new();

    public List<InventoryItem> inventoryItems = new();
    public List<Quest> playerQuests = new();

    public delegate void OnQuestUpdateHandler(List<Quest> quests);

    public event OnQuestUpdateHandler OnQuestUpdate;

    private IEnumerator GetQuest()
    {
        using UnityWebRequest www = UnityWebRequest.Get(serverURL + "/quests/" + currentPlayer.player_id);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            playerQuests = JsonConvert.DeserializeObject<List<Quest>>(www.downloadHandler.text);

            foreach (var quest in playerQuests)
            {
                Debug.Log($"- {quest.title} {quest.status}");
            }

            OnQuestUpdate?.Invoke(playerQuests);
        }
        else
        {
            Debug.LogError("퀘스트 조회 실패: " + www.error);
        }
    }

    public Quest GetQuest(int questID)
    {
        return playerQuests.Find(quest => quest.quest_id == questID);
    }

    private void Start()
    {
        currentPlayer.player_id = 4;
        StartCoroutine(GetQuest());
    }
}