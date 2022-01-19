using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickEvent : MonoBehaviour
{
    public Button[] buttons;
    public NetworkClientRoom ncr;
    private int rank = 100;
    void Start()
    {
        foreach (var item in buttons)
        {
            item.onClick.AddListener(() => clicking(item));
        }
    }
    private void clicking(Button item)
    {
        if (item.name == "MATCH")
        {
            Rank ranked = new Rank();
            ranked.rank = this.rank;
            ncr.Emit("rank", new JSONObject(JsonUtility.ToJson(ranked)));
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
public class Rank
{
    public int rank;
}