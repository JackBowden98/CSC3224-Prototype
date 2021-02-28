using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableManager : MonoBehaviour
{
    public static CollectableManager instance;
    [SerializeField] private int souls = 0;
    [SerializeField] private Text soulText;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void IncrementScore(int soulValue)
    {
        souls += soulValue;
        soulText.text = "Souls: " + souls.ToString();
    }
}
