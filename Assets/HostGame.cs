using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour {

    [SerializeField]
    private uint roomSize = 16;

    [SerializeField]
    private Button createButton;

    private string roomName;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName (string _name)
    {
        roomName = _name;
    }

    public void CreateRoom()
    {
        if (roomName != "" && roomName != null)
        {
            Debug.Log("Creating Room: " + roomName + " with " + roomSize + "max players.");
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
            createButton.interactable = false;
        }
    }

}
