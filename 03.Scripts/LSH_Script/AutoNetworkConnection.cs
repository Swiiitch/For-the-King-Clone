using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AutoNetworkConnection : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        PhotonNetwork.NickName = "AUTO"; // �г��� ����
        PhotonNetwork.AutomaticallySyncScene = true; // �÷��̾� �� �� �����ϰ� �ϴ� ����

        PhotonNetwork.ConnectUsingSettings(); // Photon ���� 
    }

    public override void OnConnectedToMaster()
    {
        print("Join lobby automatically");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("Join room automatically");
        PhotonNetwork.JoinRandomOrCreateRoom();
        Dictionary<int, CharacterType> characterTypes = GameObject.FindObjectOfType<FirebaseLoadManager>().CharacterOp;
        GameObject.Find("Player").GetComponent<UserInfo>().CType = characterTypes[1];
    }
}
