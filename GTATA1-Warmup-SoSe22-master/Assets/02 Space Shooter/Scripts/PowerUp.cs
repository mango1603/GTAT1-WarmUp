using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Container component to hold important references
    /// </summary>
    public class PowerUp : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public PowerUpType powerUpType;
    }

    public enum PowerUpType
    {
        ExtraWeapon,
        IncreaseHealth,
        UpdateWeapon
    }
}