using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserHomeManager : MonoBehaviour
{
    [SerializeField] private Transform roomListParent;   // GridLayoutGroupやVerticalLayoutGroupをアタッチした親
    [SerializeField] private Transform searchListParent;
    [SerializeField] private GameObject goToRoomButtonPrefab;
    [SerializeField] private GameObject goToAndJoinRoomButtonPrefab;
    [SerializeField] TextMeshProUGUI userNameGUI;
    [SerializeField] TextMeshProUGUI searchFiled;
    [SerializeField] GameObject searchResultViewCanvas;
    public Dictionary<string, string> usersRooms ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //searchResultViewCanvas.SetActive(false);
        //userNameGUI.text = AuthManagerBase.Instance.CurrentUserName + AuthManagerBase.Instance.CrrentUserUintId.ToString();
        usersRooms = new Dictionary<string, string>();
        ChatRoomsManagerBase.Instance.OnChatRoomsReceived += OnRoomLoad;
        ChatRoomsManagerBase.Instance.OnFoundRoomsReceived += OnSeachRooms;
        ChatRoomsManagerBase.Instance.LoadRoomsAsync();
    }

    // Update is called once per frame
    void OnDestroy()
    {
        ChatRoomsManagerBase.Instance.OnChatRoomsReceived -= OnRoomLoad;
        ChatRoomsManagerBase.Instance.OnFoundRoomsReceived -= OnSeachRooms;
    }
    private void OnSeachRooms(string roomId, string roomName)
    {
        if (!usersRooms.ContainsKey(roomId))
        {
            GameObject buttonobj = Instantiate(goToAndJoinRoomButtonPrefab, searchListParent);
            RoomJoinButton button = buttonobj.GetComponent<RoomJoinButton>();
            button.Setup(roomId, roomName);
        }
            
        
    }
    public void OnRoomLoad(string roomName, string roomId)
    {
        usersRooms.Add(roomId, roomName);
        GameObject buttonObj = Instantiate(goToRoomButtonPrefab, roomListParent);
        RoomButton button = buttonObj.GetComponent<RoomButton>();
        button.Setup(roomId, roomName);
    }
    public void OnSignOutButtonClick()
    {
        AuthManagerBase.Instance.SignOut();
        Debug.Log("SiginOut Success!");
    }
    public void OnCreateRoomButtonClick()
    {
        string userid = AuthManagerBase.Instance.CurrentUserId;
        Debug.Log($"CreateButton OnClick User:{userid}");
        ChatRoomsManagerBase.Instance.MakeRoomAsync(userid);
        ChatRoomsManagerBase.Instance.LoadRoomsAsync();
    }
    public void OnSearchButtonClick()
    {
        searchResultViewCanvas.SetActive(true);
        ChatRoomsManagerBase.Instance.SearchGroupsByNamePrefix(searchFiled.text);
    }
}
