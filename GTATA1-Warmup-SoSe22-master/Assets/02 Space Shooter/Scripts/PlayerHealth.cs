using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    /// <summary>
    ///Handling the health system of the ship
    ///Credit to this tutorial
    ///https://www.youtube.com/watch?v=ZzkIn41DFFo&t=195s 
    /// </summary>
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
            //Update health text
            healthText.text = "" + currentHealth;
            
            //Limiting the current health
            if (currentHealth > MaxHealth) currentHealth = MaxHealth;

            //Smoothing the update process of the health bar 
            lerpSpeed = 3f * Time.deltaTime;

            HealthBarFilter();
            ColorChanger();
        }

        /// <summary>
        /// Adjusting the health bar
        /// </summary>
        private void HealthBarFilter()
        {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / MaxHealth, lerpSpeed);
        }

        /// <summary>
        /// Adjusting the color of the health bar
        /// Changing from green (maxHealth) to red (minHealth)
        /// </summary>
        private void ColorChanger()
        {
            var healthColor = Color.Lerp(Color.red, Color.green, (currentHealth / MaxHealth));
            healthBar.color = healthColor;
        }

        /// <summary>
        /// Decrease the current health based on the given point
        /// </summary>
        public void Damage(float damagePoints)
        {
            if (currentHealth > 0)
                currentHealth -= damagePoints;
        }

        /// <summary>
        /// Increase the current health based on the given point
        /// </summary>
        public void Heal(float healingPoints)
        {
            if (currentHealth < MaxHealth)
                currentHealth += healingPoints;
        }
    }
}