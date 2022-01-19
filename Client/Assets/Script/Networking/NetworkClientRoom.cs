using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using System.Globalization;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkClientRoom : SocketIOComponent
{
    
    // Start is called before the first frame update

    private string myId;
    public override void Start()
    {
        Debug.Log("a");
        base.Start();
        Debug.Log("b");

        setUpEvents();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    private void setUpEvents()
    {
        On("open", (E) =>
        {
            Debug.Log("connection maded");

        });

        On("register", (E) =>
        {
            myId = E.data["id"].ToString().Replace("'", "").Replace("\"", "");
            Debug.Log(myId);
        });

        On("findMatch", (E) =>
        {
            string id1 = E.data["yourSocketId"].ToString().Replace("'", "").Replace("\"", "");
            string id2 = E.data["enemySocketId"].ToString().Replace("'", "").Replace("\"", "");
            string roomId = E.data["roomId"].ToString().Replace("'", "").Replace("\"", "");
            if (id1 == myId || id2 == myId)
            {
                Room.roomId = roomId;
                SceneManager.LoadScene("Game");
            }

        });
    }

}
public class SendRoom
{
    public string roomId;
    public string myId;
}
public static class Room
{
    public static string roomId { get; set; }
}


