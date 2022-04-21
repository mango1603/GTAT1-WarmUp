using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Scripts
{
    /// <summary>
    /// Game controller handling asteroids and intersection of components.
    /// </summary>
    public class AsteroidGameController : MonoBehaviour
    {
        public Asteroid[] bigAsteroids;
        public Asteroid[] mediumAsteroids;
        public Asteroid[] smallAsteroids;

        public PowerUp[] powerUps;

        [SerializeField] private Vector3 maximumSpeed, maximumSpin;
        [SerializeField] private PlayerShip playerShip;
        [SerializeField] private Transform spawnAnchor;
        [SerializeField] private int maxShipHealth = 100;
        [SerializeField] private int currentShipHealth;

        private List<Asteroid> activeAsteroids;
        private List<PowerUp> activePowerUps;
        private Random random;

        private const int PowerUpThreshold = 17;
        private const int LargeAsteroidDamage = 20;
        private const int MediumAsteroidDamage = 15;
        private const int SmallAsteroidDamage = 10;

        public int gunLevel = 1;
        private void Start()
        {
            activeAsteroids = new List<Asteroid>();
            activePowerUps = new List<PowerUp>();
            currentShipHealth = maxShipHealth;
            random = new Random();
            // spawn some initial asteroids
            for (var i = 0; i < 5; i++)
            {
                SpawnAsteroid(bigAsteroids, Camera.main.OrthographicBounds());
            }
        }

        /// <summary>
        /// Behaviour to spawn an asteroid within the screen
        /// If there is a parent given, the velocity of that parent is put into consideration
        /// </summary>
        private void SpawnAsteroid(Asteroid[] prefabs, Bounds inLocation, Asteroid parent = null)
        {
            // get a random prefab from the list
            var prefab = prefabs[random.Next(prefabs.Length)];
            // create an instance of the prefab
            var newObject = Instantiate(prefab, spawnAnchor);
            // position it randomly within the box given (either the parent asteroid or the camera)
            newObject.transform.position = RandomPointInBounds(inLocation);
            // we can randomly invert the x/y scale to mirror the sprite. This creates overall more variety
            newObject.transform.localScale = new Vector3(UnityEngine.Random.value > 0.5f ? -1 : 1,
                UnityEngine.Random.value > 0.5f ? -1 : 1, 1);
            // renaming, I'm also sometimes lazy typing
            var asteroidSprite = newObject.spriteRenderer;

            // try to position the asteroid somewhere where it doesn't hit the player or another active asteroid
            for (var i = 0;
                 playerShip.shipSprite.bounds.Intersects(asteroidSprite.bounds) ||
                 activeAsteroids.Any(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(asteroidSprite.bounds));
                 i++)
            {
                // give up after 15 tries.
                if (i > 15)
                {
                    DestroyImmediate(newObject.gameObject);
                    return;
                }

                newObject.transform.position = RandomPointInBounds(inLocation);
            }

            // take parent velocity into consideration
            if (parent != null)
            {
                var offset = parent.transform.position - newObject.transform.position;
                var parentVelocity = parent.movementObject.CurrentVelocity.magnitude *
                                     (UnityEngine.Random.value * 0.4f + 0.8f);
                newObject.movementObject.Impulse(offset.normalized * parentVelocity, RandomizeVector(maximumSpeed));
            }
            // otherwise randomize just some velocity
            else
            {
                newObject.movementObject.Impulse(RandomizeVector(maximumSpeed), RandomizeVector(maximumSpin));
            }

            activeAsteroids.Add(newObject);
        }

        /// <summary>
        /// Spawn a random power up in the given power up prefabs in the given location
        /// </summary>
        private void SpawnPowerUp(PowerUp[] prefabs, Bounds inLocation)
        {
            // get a random prefab from the list
            var prefab = prefabs[random.Next(prefabs.Length)];

            // create an instance of the prefab
            var newObject = Instantiate(prefab, spawnAnchor);
            var transform1 = newObject.transform;
            transform1.position = inLocation.center;
            transform1.localScale = new Vector2(3 , 3);

            activePowerUps.Add(newObject);
        }

        /// <summary>
        /// Checks if a laser is intersecting with an asteroid and executes gameplay behaviour on that
        /// </summary>
        public void LaserIntersection(SpriteRenderer laser)
        {
            // go through all asteroids, check if they intersect with a laser and stop after the first
            var asteroid = activeAsteroids
                .FirstOrDefault(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(laser.bounds));

            // premature exit: this laser hasn't hit anything
            if (asteroid == null)
            {
                return;
            }

            // otherwise remove the asteroid from the tracked asteroid
            activeAsteroids.Remove(asteroid);
            var bounds = asteroid.spriteRenderer.bounds;

            // create a random power up
            if (RandomSpawn(PowerUpThreshold))
            {
                SpawnPowerUp(powerUps, bounds);
            }

            // get the correct set of prefabs to spawn asteroids in place of the asteroid that now explodes
            var prefabs = asteroid.asteroidSize switch
            {
                AsteroidSize.Large => mediumAsteroids,
                AsteroidSize.Medium => smallAsteroids,
                _ => null
            };
            // remote the asteroid gameobject with all its components
            Destroy(asteroid.gameObject);
            // premature exit: we have no prefabs (ie: small asteroids exploding)
            if (prefabs == null)
            {
                return;
            }

            // randomize two to six random asteroids
            var objectCountToSpawn = (int) (UnityEngine.Random.value * 4 + 2);
            for (var i = 0; i < objectCountToSpawn; i++)
            {
                SpawnAsteroid(prefabs, bounds);
            }

            // oh, also get rid of the laser now
            Destroy(laser.gameObject);
        }

        private bool RandomSpawn(int threshold)
        {
            if (threshold > 100) return false;

            var num = random.Next(100);
            return num <= threshold;
        }

        public void ShipIntersection(SpriteRenderer ship)
        {
            ShipAsteroidsInteraction(ship);
            ShipPowerUpsInteraction(ship);
        }

        private void ShipAsteroidsInteraction(SpriteRenderer ship)
        {
            // go through all asteroids, check if they intersect with the ship and stop after the first
            var asteroid = activeAsteroids
                .FirstOrDefault(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(ship.bounds));

            // premature exit: this ship hasn't hit anything
            if (asteroid == null)
            {
                return;
            }

            // otherwise remove the asteroid from the tracked asteroid
            activeAsteroids.Remove(asteroid);

            // remove the asteroid gameobject with all its components
            Destroy(asteroid.gameObject);

            //Decrease ship health depends on the hit asteroid
            DamageShip(asteroid);

            // get rid of the ship when the health goes below 0
            if (currentShipHealth <= 0)
            {
                Destroy(ship.gameObject);
            }
        }
        
        private void ShipPowerUpsInteraction(SpriteRenderer ship)
        {
            // go through all powers, check if they intersect with the ship and stop after the first
            var powerUp = activePowerUps
                .FirstOrDefault(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(ship.bounds));

            // premature exit: this power up hasn't hit anything
            if (powerUp == null)
            {
                return;
            }

            switch (powerUp.powerUpType)
            {
                //otherwise power up the ship based on the acquired power
                //increase gun power
                case PowerUpType.Gun:
                    gunLevel++;
                    break;
                //increase ship health
                case PowerUpType.Shield:
                    currentShipHealth += 25;
                    break;
                //increase ship speed or decrease asteroid speed
                case PowerUpType.Speed:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //remove the powerUp from the tracked powerUps
            activePowerUps.Remove(powerUp);

            // remove the powerUp gameobject with all its components
            Destroy(powerUp.gameObject);
        }

        private void DamageShip(Asteroid asteroid)
        {
            //Health lost depends on the size of the hit asteroid
            var healthLoss = asteroid.asteroidSize switch
            {
                AsteroidSize.Large => LargeAsteroidDamage,
                AsteroidSize.Medium => MediumAsteroidDamage,
                _ => SmallAsteroidDamage
            };

            currentShipHealth -= healthLoss;
        }

        private static float RandomPointOnLine(float min, float max)
        {
            return UnityEngine.Random.value * (max - min) + min;
        }

        private static Vector2 RandomPointInBox(Vector2 min, Vector2 max)
        {
            return new Vector2(RandomPointOnLine(min.x, max.x), RandomPointOnLine(min.y, max.y));
        }

        private static Vector2 RandomPointInBounds(Bounds bounds)
        {
            return RandomPointInBox(bounds.min, bounds.max);
        }

        private static Vector3 RandomizeVector(Vector3 maximum)
        {
            // that is an inline method - it's good enough to just get a float [-1...+1]
            float RandomValue()
            {
                return UnityEngine.Random.value - 0.5f * 2;
            }

            maximum.Scale(new Vector3(RandomValue(), RandomValue(), RandomValue()));
            return maximum;
        }
    }
}