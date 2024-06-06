using UnityEngine;
using UnityEngine.UI;

public class CoinController : MonoBehaviour
{
    public int score;
    public Text text;
    public int initialCoins;
    public GameObject coin;

    private void OnEnable()
    {
        ObjectController.OnSpawned += Spawn;
        Coin.OnCollected += AddScore;
    }

    private void OnDisable()
    {
        ObjectController.OnSpawned -= Spawn;
        Coin.OnCollected -= AddScore;
    }

    private void Spawn(Vector3 pos)
    {
        for (int i = 0; i < initialCoins; i++)
            Instantiate(coin, new(pos.x + Random.Range(-2f, 2f), pos.y, pos.z + Random.Range(-2f, 2f)), Quaternion.identity);
    }

    private void AddScore() => score++;

    private void Update() => text.text = $"{score}s";
}
