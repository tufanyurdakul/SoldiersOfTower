using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClick : MonoBehaviour
{
    public GameObject Bg;
    private int nameIs;
    private UnitClicks unitClick;
    public List<Button> barracks;
    private void Start()
    {
        unitClick = Bg.GetComponent<UnitClicks>();
        foreach (var item in barracks)
        {
            item.onClick.AddListener(() => BarracksClick(item));
        }
    }
    private void BarracksClick(Button clickButton)
    {
        int which = int.Parse(clickButton.name.Replace("Button",""));
        Bg.SetActive(true);
        unitClick.where = which - 1;
    }
    //private void OnMouseDown()
    //{
    //    nameIs = int.Parse(gameObject.name.Replace("Castle", "")) - 1;
    //    if (nameIs < 3)
    //    {
    //        Bg.SetActive(true);
    //        unitClick.where = nameIs;
    //    }
    //}

}
