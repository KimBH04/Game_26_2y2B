using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DungeonGameClient : MonoBehaviour
{
    [SerializeField] private string serverURL = "http://localhost:3000";
    [SerializeField] private Text playerText;
    [SerializeField] private Text roomText;
    [SerializeField] private Text messageText;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button chestButton;
    [SerializeField] private Button restButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button returnButton;

    private void Start()
    {
        enterButton.onClick.AddListener(() => StartCoroutine(Post("/api/dungeon/enter", "{}")));
        attackButton.onClick.AddListener(() => SendAction("ATTACK"));
        chestButton.onClick.AddListener(() => SendAction("OPEN_CHEST"));
        restButton.onClick.AddListener(() => SendAction("REST"));
        nextButton.onClick.AddListener(() => SendAction("NEXT_ROOM"));
        returnButton.onClick.AddListener(() => SendAction("RETURN"));
    }

    private void SendAction(string action)
    {
        string json = JsonUtility.ToJson(new DungeonActionRequest { action = action });
        StartCoroutine(Post("/api/dungeon/action", json));
    }

    private IEnumerator GetState()
    {
        using UnityWebRequest request = UnityWebRequest.Get(serverURL + "/api/game/state");

        yield return request.SendWebRequest();
        ReadResponse(request);
    }

    private IEnumerator Post(string path, string json)
    {
        using UnityWebRequest request = new(serverURL + path, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        ReadResponse(request);
    }

    private void SetActionButton(bool attack, bool chest, bool rest, bool next, bool returnToTown)
    {
        attackButton.interactable = attack;
        chestButton.interactable = chest;
        restButton.interactable = rest;
        nextButton.interactable = next;
        returnButton.interactable = returnToTown;
    }

    private string GetRoomDescription(DungeonRoomData room)
    {
        return room.type switch
        {
            "MONSTER" =>
                $"{room.monsterName} 출현!\n" +
                $"몬스터 HP : {room.monsterHP}\n" +
                $"몬스터 공격력 : {room.monsterAttack}\n" +
                $"처치 보상 : {room.rewardGold} Gold",

            "TREASURE" =>
                $"보물 상자를 발견했습니다.\n" +
                $"보상 : {room.rewardGold} Gold",
            
            "HEAL" =>
                $"회복의 샘을 발견했습니다.\n" +
                $"휴식하면 HP를 30 회복합니다.",
            
            "EXIT" =>
                $"던전 출구를 발견했습니다.\n" +
                $"귀환하면 탐험 골드를 획득합니다.",

            _ => "알 수 없는 방입니다.",
        };
    }

    private void UpdateButtons(DungeonRoomData room, string runState)
    {
        bool playing = runState == "IN_PROGRESS";
        bool cleared = room.state == "CLEARED";

        enterButton.interactable = !playing;
        SetActionButton(
            playing && room.type == "MONSTER" && !cleared,
            playing && room.type == "TREASURE" && !cleared,
            playing && room.type == "HEAL" && !cleared,
            playing && cleared,
            playing && room.type == "EXIT"
        );
    }

    private void Draw(DungeonStateData state)
    {
        DungeonPlayerData player = state.player;
        playerText.text =
            $"{player.name} Lv.{player.level}\n" +
            $"HP {player.hp} / {player.maxHP}\n" +
            $"Gold {player.gold}";

        if (state.run == null)
        {
            roomText.text = "진행 중인 던전 없음";
            enterButton.interactable = true;
            SetActionButton(false, false, false, false, false);
            return;
        }

        DungeonRoomData room = state.run.currentRoom;
        roomText.text =
            $"방 {room.index + 1}\n\n" +
            $"{GetRoomDescription(room)}\n\n" +
            $"방 상태 : {room.state}\n" +
            $"탐험 골드 : {state.run.runGold}";

        UpdateButtons(room, state.run.state);
    }

    private void ReadResponse(UnityWebRequest request)
    {
        string json = request.downloadHandler.text;
        DungeonResponse response = JsonUtility.FromJson<DungeonResponse>(json);

        if (response == null)
        {
            messageText.text = "서버 응답을 읽을 수 없습니다.";
            Debug.LogError(json);
            return;
        }

        messageText.text = response.message;
        if (!response.success || response.data == null)
        {
            Debug.LogError($"{response.code} : {response.message}");
            return;
        }

        Draw(response.data);
    }
}
