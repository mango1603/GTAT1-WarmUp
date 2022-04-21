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
            var gunLevel = _runGameController.gunLevel;
            var gunNum = _runGameController.gunNum;
            if (gunLevel >= laserPrefabs.Length)
            {
                gunLevel = laserPrefabs.Length - 1;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire(frontRotate, gunLevel);
                if (gunNum > 0)
                {
                    Fire(backRotate, gunLevel);
                }

                if (gunNum > 1)
                {
                    Fire(side1Rotate, gunLevel);
                    Fire(side2Rotate, gunLevel);
                }
            }
        }

        private void UpdateRotation()
        {
            frontRotate = transform.rotation;
            side1Rotate = transform.rotation * Quaternion.Euler(0, 0, 90);
            side2Rotate = transform.rotation * Quaternion.Euler(0, 0, -90);
            backRotate = transform.rotation * Quaternion.Euler(0, 0, 180);
        }

        private void Fire(Quaternion rotation, int level)
        {
            laserPrefabs[level].initialVelocity = ship.movementObject.CurrentVelocity;
            Instantiate(laserPrefabs[level], transform.position, rotation);
        }
    }
}