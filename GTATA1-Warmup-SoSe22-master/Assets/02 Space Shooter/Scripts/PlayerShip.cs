using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Container component to keep references to common components on a ship
    /// </summary>
    internal class PlayerShip : MonoBehaviour
    {
        public MovementObject movementObject;
        public SpriteRenderer shipSprite;
        private static AsteroidGameController _runGameController;

        private void Start()
        {
            shipSprite = GetComponent<SpriteRenderer>();
            if (_runGameController == null) _runGameController = FindObjectOfType<AsteroidGameController>();
        }
        
        /// <summary>
        /// Late Update is a Unity Runtime function that gets executed after Update (and Coroutines)
        /// see: https://docs.unity3d.com/Manual/ExecutionOrder.html
        /// </summary>
        private void Update()
        {
            _runGameController.ShipIntersection(shipSprite);
        }
    }
}