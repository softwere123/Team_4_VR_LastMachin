using UnityEngine;
using UnityEngine.UI;

public class EnemyHeartSystem : MonoBehaviour
{
    public Image targetImage;
    public EnemyHealth_HG enemyHealth;

    private void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (targetImage == null || enemyHealth == null)
            return;

        targetImage.fillAmount = (float)enemyHealth.GetCurrentHealth() / enemyHealth.maxHealth;
    }
}
