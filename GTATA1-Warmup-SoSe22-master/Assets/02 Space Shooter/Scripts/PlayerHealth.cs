using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class PlayerHealth : MonoBehaviour
    {
        public Text healthText;
        public Image healthBar;

        private float health;
        private const float MaxHealth = 100;
        private float lerpSpeed;

        private void Start()
        {
            health = MaxHealth;
        }

        private void Update()
        {
            healthText.text = "Health: " + health + "%";
            if (health > MaxHealth) health = MaxHealth;

            lerpSpeed = 3f * Time.deltaTime;

            HealthBarFilter();
            ColorChanger();
        }

        private void HealthBarFilter()
        {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, health / MaxHealth, lerpSpeed);
        }

        private void ColorChanger()
        {
            var healthColor = Color.Lerp(Color.red, Color.green, (health / MaxHealth));

            healthBar.color = healthColor;
        }

        public void Damage(float damagePoints)
        {
            if (health > 0)
                health -= damagePoints;
        }

        public void Heal(float healingPoints)
        {
            if (health < MaxHealth)
                health += healingPoints;
        }
    }
}