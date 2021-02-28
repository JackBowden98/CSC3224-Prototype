using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallSoul : MonoBehaviour
{
    public int soulValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CollectableManager.instance.IncrementScore(soulValue);
        }
    }
}
