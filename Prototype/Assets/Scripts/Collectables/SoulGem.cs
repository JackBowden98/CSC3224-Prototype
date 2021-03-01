using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulGem : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currentHealth;

    bool destroyable;
    int soulCount;
    public GameObject souls;

    HitPause hitPause;

    void Start()
    {
        currentHealth = maxHealth;
        hitPause = GetComponent<HitPause>();
        destroyable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Damage(float[] attackDetails)
    {
        if (destroyable)
        {

            // 0 in the array is the amount of damage being recieved
            currentHealth -= attackDetails[0];


            if (currentHealth <= 0.0f)
            {
                string level = "Complete";
                // go to complete screen
                LevelComplete(level);
            }
        }

    }

    public void LevelComplete(string level)
    {
        Application.LoadLevel(level);
    }

    public void setDestroyable()
    {
        destroyable = true;
    }
}
