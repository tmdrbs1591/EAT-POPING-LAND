using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("DisconnectPanel")]
    public TMP_InputField NickNameInput;
    public GameObject DisconnectPanel;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public TMP_InputField RoomInput;
    public TMP_Text WelcomeText;
    public TMP_Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public TMP_Text ListText;
    public TMP_Text RoomInfoText;
    public TMP_Text[] ChatText;
    public TMP_InputField ChatInput;

    [Header("ETC")]
    public TMP_Text StatusText;
    public PhotonView PV;

    [Header("Room Code")]
    public TMP_InputField RoomCodeInput; // 방 코드를 입력받을 필드 추가
    public TMP_Text RoomCodeText; // 생성된 방 코드를 표시할 텍스트

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    [Header("LobbyPanel")]
    public GameObject StartButton;

    [SerializeField] Transform playerLisContent;
    [SerializeField] GameObject playerListItemPrefab;
    private Dictionary<string, GameObject> playerObjects = new Dictionary<string, GameObject>(); // 플레이어 오브젝트 관리 딕셔너리

    string roomCode; // 방 코드를 저장할 변수

    void Awake()
    {
        Disconnect();
        Connect();
        //Screen.SetResolution(960, 540, false);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
        SongManager.instance.SongChange(1);
}

    public void Start()
    {
        
        if (!PhotonNetwork.IsMasterClient)
        {
            StartButton.SetActive(false);
        }
    }

    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Send();
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("01.Ingame");
        }
    }

    #region 방리스트 갱신
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(1).GetComponent<TMP_Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(2).GetComponent<TMP_Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion

    #region 서버연결

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = UserInfo.Data.nickname;
        Debug.Log("UserInfo 닉네임: " + UserInfo.Data.nickname);  // 확인용 로그
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        myList.Clear();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        DisconnectPanel.SetActive(true);
    }
    #endregion

    #region 방 생성 및 참가

    // 방 이름에 방 코드를 포함하는 방식으로 생성
    public void CreateRoom()
    {
        roomCode = Random.Range(1000, 9999).ToString(); // 4자리 방 코드 생성
        string roomName = (RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text) + "_" + roomCode; // 방 이름에 방 코드 추가
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        RoomCodeText.text = "Room Code: " + roomCode; // UI에 방 코드 표시
    }

    // 사용자가 방 코드 입력 후 참가
    public void JoinRoomWithCode()
    {
        string inputCode = RoomCodeInput.text;

        // 방 목록에서 방 코드로 필터링하여 입장
        foreach (RoomInfo roomInfo in myList)
        {
            if (roomInfo.Name.EndsWith("_" + inputCode))
            {
                PhotonNetwork.JoinRoom(roomInfo.Name);
                return;
            }
        }

        // 방을 찾지 못하면 에러 메시지를 띄울 수 있음
        Debug.LogError("Room with code " + inputCode + " not found.");
    }

    // 랜덤 방 참가 기능
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = UserInfo.Data.nickname;

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Count(); i++)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab, playerLisContent);
            playerItem.GetComponent<PlayerListItem>().Setup(players[i]);
            playerObjects[players[i].NickName] = playerItem;
        }

        RoomPanel.SetActive(true);
        RoomRenewal();
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";

        StartButton.SetActive(PhotonNetwork.IsMasterClient);

        // 방 코드 표시
        string roomName = PhotonNetwork.CurrentRoom.Name;
        string roomCode = roomName.Split('_')[^1];
        RoomCodeText.text = "Room Code: " + roomCode;

        LobbyPanel.SetActive(false);
        DisconnectPanel.SetActive(false);

        CheckAutoStart(); // 입장 직후 자동 시작 체크
    }



    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomInput.text = "";
        CreateRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomInput.text = "";
        CreateRoom();
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");

        GameObject playerItem = Instantiate(playerListItemPrefab, playerLisContent);
        playerItem.GetComponent<PlayerListItem>().Setup(newPlayer);
        playerObjects[newPlayer.NickName] = playerItem;

        CheckAutoStart(); // 여기서 자동 시작 체크
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");

        if (PhotonNetwork.IsMasterClient)
        {
            StartButton.SetActive(true);
        }
    }

    void RoomRenewal()
    {
        ListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "명";
    }
    #endregion

    #region 채팅
    public void Send()
    {
        // 채팅 입력 필드가 비어 있으면 메시지를 보내지 않고 입력 필드에 포커스를 유지
        if (string.IsNullOrWhiteSpace(ChatInput.text))
        {
            ChatInput.ActivateInputField(); // 입력 필드에 포커스 유지
            return;
        }

        // 입력 필드에 메시지가 있으면 메시지를 보내고 입력 필드를 다시 활성화
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = ""; // 입력 필드 초기화
        ChatInput.ActivateInputField(); // 입력 필드에 포커스 유지
    }

    [PunRPC]
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput)
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion
    public bool isQuickMatch;
    void CheckAutoStart()
    {
        if (PhotonNetwork.IsMasterClient &&
            PhotonNetwork.CurrentRoom.PlayerCount == 4 && isQuickMatch) // 4명일 때 시작
        {
            PhotonNetwork.LoadLevel("01.Ingame");
        }
    }

    public void QuickMatchChange(bool b)
    {
        isQuickMatch = b;
    }

}

