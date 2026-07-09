using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;
    public AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        BulletDestroyed player = other.GetComponentInParent<BulletDestroyed>();
        if (player == null)
            return;

        if (CoinManager.instance != null)
            CoinManager.instance.AddCoin(value);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }
}
