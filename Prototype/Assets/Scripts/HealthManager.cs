using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager instance;
    [SerializeField] private float health = 100;
    [SerializeField] private Text healthText;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
    }

    public void SetHealth(float health)
    {
        healthText.text = health.ToString();
    }
}
