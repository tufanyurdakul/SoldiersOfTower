using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using UnityEngine.SceneManagement;
using TMPro;

public class UnitClicks : MonoBehaviour
{
    public Button close;
    public List<Button> unitButtons;
    public List<Sprite> unitSprite;
    private NetworkClient nc;
    public GameObject inf;
    private UnitData ud;
    public GameObject unitData;
    public GameObject[] barracks;
    public GameObject[] chats;
    private List<GameObject> openedSprites;
    private List<float> timer;
    private string[] choosen;
    public int where { get; set; }
    void Start()
    {
        choosen = PlayerPrefs.GetString("choosen").Split(',');
        openedSprites = new List<GameObject>();
        timer = new List<float>();
        ud = unitData.GetComponent<UnitData>();
        nc = inf.GetComponent<NetworkClient>();
        foreach (var item in unitButtons)
        {
            item.onClick.AddListener(() => create(item));
            Image[] buttonImage = item.GetComponentsInChildren<Image>();
            TextMeshProUGUI tmpPrice = item.GetComponentInChildren<TextMeshProUGUI>();
            int which = int.Parse(item.name.Replace("Unit", "")) - 1;
            int character = int.Parse(choosen[which]);
            buttonImage[1].sprite = unitSprite[which];
            Statics s = new Statics();
            s = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit" + character));
            tmpPrice.SetText($"{s.Price}");
        }
        close.onClick.AddListener(closePanel);
    }
    private void create(Button ClickedButton)
    {
        int whichButton = int.Parse(ClickedButton.name.Replace("Unit", "")) - 1;
        string character = choosen[whichButton];
        Statics s = new Statics();
        s = JsonUtility.FromJson<Statics>(PlayerPrefs.GetString("Unit" + character));
        if (ud.money >= s.Price && Time.timeScale != 0)
        {
            Position p = new Position();
            p.unitId = s.UnitId;
            p.id = nc.myId;
            p.placeId = where;
            p.health = s.Health;
            p.attackDamage = s.AttackDamage;
            p.abilityPower = s.AbilityPower;
            p.armour = s.Armour;
            p.resistance = s.Resistance;
            p.attackSpeed = (int)(s.AttackSpeed * 10);
            p.roomId = nc.roomId;
            nc.Emit("spawn", new JSONObject(JsonUtility.ToJson(p)));
            write(s.Price);
        }
        else
        {
            show();
        }
    }

    private void show()
    {
        chats[where].SetActive(true);
        openedSprites.Add(chats[where]);
        timer.Add(0.5f);
    }
    private void write(int price)
    {
        ud.money -= price;
        closePanel();
    }
    private void closePanel()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (openedSprites.Count > 0)
        {
            for (int i = 0; i < openedSprites.Count; i++)
            {
                timer[i] -= Time.deltaTime;
                if (timer[i] <= 0)
                {
                    openedSprites[i].SetActive(false);
                    timer.RemoveAt(i);
                    openedSprites.RemoveAt(i);
                }
            }
        }
    }
    [SerializeField]
    public class Position
    {
        public string id;
        public int unitId;
        public int placeId;
        public int health;
        public int attackDamage;
        public int abilityPower;
        public int armour;
        public int resistance;
        public int attackSpeed;
        public string roomId;
    }
}
