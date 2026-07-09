using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance { get; private set; }

    public int coinCount = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCoin(int amount = 1)
    {
        coinCount += amount;
    }

    public bool TrySpendCoin(int amount = 1)
    {
        if (coinCount < amount)
            return false;

        coinCount -= amount;
        return true;
    }
}
