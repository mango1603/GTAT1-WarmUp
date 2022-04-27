using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Container component to hold power up references
    /// </summary>
    public class PowerUp : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public PowerUpType powerUpType;
    }

    public enum PowerUpType
    {
        //Spawn extra weapon
        ExtraWeapon,
        //Increase the current health point
        IncreaseHealth,
        //Update the weapon level
        UpdateWeapon
    }
}