using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    byte maxPlayerPerRoom = 4;
    bool isConnecting;

    public InputField playerName;
    public Text feedbackText;
    string gameVersion = "1";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName.text = PlayerPrefs.GetString("PlayerName");
        }
    }

    public void ConnectNetwork()
    {
        feedbackText.text = "";
        isConnecting = true;

        PhotonNetwork.NickName = playerName.text;
        if(PhotonNetwork.IsConnected)
        {
            feedbackText.text += "\nJoining Room...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            feedbackText.text += "\nConnecting...";
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void SetName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
    }

    public void ConnectSingle()
    {
        SceneManager.LoadScene("Track1");
    }

    //Network CallBacks
    public override void OnConnectedToMaster()
    {
        if(isConnecting)
        {
            feedbackText.text += "\nOn Connected To master...";
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        feedbackText.text += "\nFailed To join random room";
        PhotonNetwork.CreateRoom(null,new RoomOptions { MaxPlayers=maxPlayerPerRoom});
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        feedbackText.text += "\nDisconnected beacuse" + cause;
        isConnecting = false;
    }

    public override void OnJoinedRoom()
    {
        feedbackText.text += "\nJoined room with" + PhotonNetwork.CurrentRoom.PlayerCount +" Players";
        PhotonNetwork.LoadLevel("Track1");
    }
}
