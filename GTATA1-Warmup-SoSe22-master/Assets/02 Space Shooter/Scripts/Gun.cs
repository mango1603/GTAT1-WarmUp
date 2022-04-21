using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Simple component to create a laser and shoot it forward 
    /// </summary>
    public class Gun : MonoBehaviour
    {
        [SerializeField] private Laser laserPrefab;
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire(frontRotate);
                if (_runGameController.gunLevel > 1)
                {
                    Fire(backRotate);
                }

                if (_runGameController.gunLevel > 2)
                {
                    Fire(side1Rotate);
                    Fire(side2Rotate);
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

        private void Fire(Quaternion rotation)
        {
            laserPrefab.initialVelocity = ship.movementObject.CurrentVelocity;
            Instantiate(laserPrefab, transform.position, rotation);
        }
    }
}