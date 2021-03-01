using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableManager : MonoBehaviour
{
    public static CollectableManager instance;
    [SerializeField] private int souls = 0;
    [SerializeField] private Text soulText;
    public SoulGem soulGem;
    public bool cheatAllSouls;

    public int maxSouls = 9;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        cheatAllSouls = false;
    }

    void Update()
    {
        if (souls >= 9)
        {
            soulGem.setDestroyable();
        }
        if (cheatAllSouls)
        {
            souls = maxSouls;
            soulText.text = souls.ToString();
        }
    }

    public void IncrementScore(int soulValue)
    {
        souls += soulValue;
        soulText.text = souls.ToString();

    }

    public void CheatAllSouls()
    {
        cheatAllSouls = true;
    }
}
