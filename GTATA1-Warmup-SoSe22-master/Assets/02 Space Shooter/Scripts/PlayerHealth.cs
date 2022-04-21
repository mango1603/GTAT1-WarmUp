using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    //Credit to this tutorial
    //https://www.youtube.com/watch?v=ZzkIn41DFFo&t=195s
    public class PlayerHealth : MonoBehaviour
    {
        public Text healthText;
        public Image healthBar;

        public float currentHealth;
        private const float MaxHealth = 100;
        private float lerpSpeed;

        private void Start()
        {
            currentHealth = MaxHealth;
        }

        private void Update()
        {
            healthText.text = "" + currentHealth;
            if (currentHealth > MaxHealth) currentHealth = MaxHealth;

            lerpSpeed = 3f * Time.deltaTime;

            HealthBarFilter();
            ColorChanger();
        }

        private void HealthBarFilter()
        {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / MaxHealth, lerpSpeed);
        }

        private void ColorChanger()
        {
            var healthColor = Color.Lerp(Color.red, Color.green, (currentHealth / MaxHealth));
            healthBar.color = healthColor;
        }

        public void Damage(float damagePoints)
        {
            if (currentHealth > 0)
                currentHealth -= damagePoints;
        }

        public void Heal(float healingPoints)
        {
            if (currentHealth < MaxHealth)
                currentHealth += healingPoints;
        }
    }
}