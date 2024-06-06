using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static Action OnCollected;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}
