using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    public PhotonView pv;

    public Text playerName;
    public GameObject leftArrowButton;
    public GameObject rightArrowButton;
    public GameObject leaveRoomButton;
    public Text characterName;
    public GameObject readyButton;
    public Text readyText;

    Hashtable playerProperties;
    public GameObject character;
    [Tooltip("0: Warrior, 1: Archer, 2: Magician")]
    public GameObject[] characters;

    Player player;
    Dictionary<int, CharacterType> characterTypes;

    private void Awake()
    {
        // ���� -> �ü� -> ������ ����
        characterTypes = GameObject.Find("DBManager").GetComponent<FirebaseLoadManager>().CharacterOp;
        // StartSelectionButton
        this.transform.GetChild(0).gameObject.SetActive(true);
        // Selection
        this.transform.GetChild(1).gameObject.SetActive(false);
    }

    public override void OnDisable()
    {
        // StartSelectionButton
        this.transform.GetChild(0).gameObject.SetActive(true);
        // Selection
        this.transform.GetChild(1).gameObject.SetActive(false);
        // �÷��̾ ���� ������ �� �� properties �ʱ�ȭ
        if (!(bool)PhotonNetwork.CurrentRoom.CustomProperties["start"])
            PhotonNetwork.SetPlayerCustomProperties(null);
    }

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        characterName.text = characterTypes[0].Name;
        playerProperties = new Hashtable();
        playerProperties["isReady"] = false;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);

        UpdatePlayerItem(player);
    }

    public void ApplyLocalChanges()
    {
        leftArrowButton.SetActive(true);
        rightArrowButton.SetActive(true);
        leaveRoomButton.SetActive(true);
        readyButton.SetActive(true);
    }

    public void OnClickLeftArrow()
    {
        if ((int)playerProperties["avatarIndex"] == 0)
        {
            playerProperties["avatarIndex"] = characters.Length - 1;
        }
        else
        {
            playerProperties["avatarIndex"] = (int)playerProperties["avatarIndex"] - 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void OnClickRightArrow()
    {
        if ((int)playerProperties["avatarIndex"] == characters.Length - 1)
        {
            playerProperties["avatarIndex"] = 0;
        }
        else
        {
            playerProperties["avatarIndex"] = (int)playerProperties["avatarIndex"] + 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    // remote player�� property�� ����� ������ ȣ���
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }


    void UpdatePlayerItem(Player player)
    {
        if (player.CustomProperties.ContainsKey("avatarIndex"))
        {
            Destroy(character.transform.GetChild(0).gameObject);

            GameObject newAvatar = characters[(int)player.CustomProperties["avatarIndex"]].gameObject;

            ChangeLayerRecursively(newAvatar.transform, "UI"); // �ڽ� ������Ʈ���� ���̾� ��� ����
            Instantiate(newAvatar, character.transform);

            characterName.text = characterTypes[(int)playerProperties["avatarIndex"]].Name;

            playerProperties["avatarIndex"] = (int)player.CustomProperties["avatarIndex"];
        }
        else
        {
            playerProperties["avatarIndex"] = 0;
        }
    }

    void ChangeLayerRecursively(Transform transform, string name)
    {
        transform.gameObject.layer = LayerMask.NameToLayer(name);
        foreach(Transform child in transform)
        {
            ChangeLayerRecursively(child, name);
        }
    }

    public void SetPlayerReady()
    {
        // ���� ����
        // SetPlayerReady -> NetworkManager.CheckPlayersReadyAndStartGame -> LoadScene -> OnDisable -> PunClasses.OnPlayerPropertiesUpdate
        bool isReady;
        if (!(bool)player.CustomProperties["isReady"])
        {
            playerProperties["isReady"] = true;
            isReady = true;
            player.SetCustomProperties(playerProperties);
        }
        else
        {
            playerProperties["isReady"] = false;
            isReady = false;
            player.SetCustomProperties(playerProperties);  
        }

        pv.RPC("RPCPlayerReady", RpcTarget.All, isReady);
    }

    [PunRPC]
    void RPCPlayerReady(bool isReady)
    {
        ColorBlock buttonColor = readyButton.GetComponent<Button>().colors;
        if (isReady)
        {
            buttonColor.selectedColor = new Color32(69, 199, 247, 255); // mint
            readyText.gameObject.SetActive(true);
        }
        else
        {
            buttonColor.selectedColor = new Color32(255, 255, 255, 255); // white
            readyText.gameObject.SetActive(false);
        }
        readyButton.GetComponent<Button>().colors = buttonColor;
    }
}
