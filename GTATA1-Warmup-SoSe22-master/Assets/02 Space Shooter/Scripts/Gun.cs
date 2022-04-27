using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Simple component to create a laser and shoot it forward 
    /// </summary>
    public class Gun : MonoBehaviour
    {
        [SerializeField] private Laser[] laserPrefabs;
        private static AsteroidGameController _runGameController;
        private PlayerShip ship;
        
        //Possible rotations to spawn bullet
        private Quaternion frontRotate;
        private Quaternion side1Rotate;
        private Quaternion side2Rotate;
        private Quaternion backRotate;

        private void Start()
        {
            if (_runGameController == null) _runGameController = FindObjectOfType<AsteroidGameController>();
            ship = GetComponent<PlayerShip>();
            UpdateRotation();
        }

        private void Update()
        {
            UpdateRotation();
            //Current gun attributes is based on the power level on the controller
            var gunLevel = _runGameController.gunLevel;
            var gunNum = _runGameController.gunNum;
            
            //Limiting the level
            if (gunLevel >= laserPrefabs.Length)
            {
                gunLevel = laserPrefabs.Length - 1;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Fire only in front
                Fire(frontRotate, gunLevel);
                
                //Fire also in the back
                if (gunNum > 0)
                {
                    Fire(backRotate, gunLevel);
                }

                //Fire in the both side
                if (gunNum > 1)
                {
                    Fire(side1Rotate, gunLevel);
                    Fire(side2Rotate, gunLevel);
                }
            }
        }

        /// <summary>
        /// Update the rotation for the bullet spawn position
        /// </summary>
        private void UpdateRotation()
        {
            frontRotate = transform.rotation;
            side1Rotate = transform.rotation * Quaternion.Euler(0, 0, 90);
            side2Rotate = transform.rotation * Quaternion.Euler(0, 0, -90);
            backRotate = transform.rotation * Quaternion.Euler(0, 0, 180);
        }

        /// <summary>
        /// Fire/Spawn the bullet based on the given rotation and current gun level
        /// </summary>
        private void Fire(Quaternion rotation, int level)
        {
            laserPrefabs[level].initialVelocity = ship.movementObject.CurrentVelocity;
            Instantiate(laserPrefabs[level], transform.position, rotation);
        }
    }
}