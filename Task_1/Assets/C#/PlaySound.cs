using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    private void OnEnable() => Coin.OnCollected += Play;

    private void OnDisable() => Coin.OnCollected -= Play;
    
    private void Play() => GetComponent<AudioSource>().Play();
}
