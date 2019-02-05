using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    /// <summary>
    /// A unit's weapon for firing projectiles.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        /// <summary>
        /// The projectile
        /// </summary>
        [SerializeField]
        private Projectile projectilePrefab;

        [SerializeField,
            Tooltip("A gameObject that appears at" +
            "the collision point for a limited time")]
        private HitMark hitMarkPrefab;

        [SerializeField, Tooltip("Projectiles / second")]
        private float firingRate = 1 / 3f;

        [SerializeField]
        private Transform shootingPoint;

        private Pool<Projectile> projectiles;
        //public Pool<Hole> holes;

        private LevelObject owner;
        private ParticleSystem shootParticles;
        private bool canFire = true;
        private float firingTimer = 0;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="owner"></param>
        public void Init(LevelObject owner)
        {
            this.owner = owner;
            shootParticles = GetComponent<ParticleSystem>();

            projectiles = new Pool<Projectile>
                (4, false, projectilePrefab, InitProjectile);

            //projectiles = new Pool<Projectile>(
            //    projectilePrefab, 4, false, item => InitItem(item));

            // FIXME
            //holes = new Pool<Hole>(holePrefab, 8, true);
        }

        /// <summary>
        /// Initializes the projectile. Alternative lambda parameter method.
        /// Called by the pool when it creates copies of the projectile.
        /// </summary>
        /// <param name="projectile"></param>
        private void InitProjectile(Projectile projectile)
        {
            projectile.Init(ProjectileHit);
        }

        /// <summary>
        /// A method that is passed to projectiles to call when they hit something.
        /// </summary>
        /// <param name="projectile">A projectile which hit something</param>
        private void ProjectileHit(Projectile projectile)
        {
            // Returns the projectile to the pool
            if (projectiles.ReturnObject(projectile))
            {
                // Plays particle effects
                //GameManager.Instance.SpawnExplosion
                //    (projectile.transform.position);
            }
            else
            {
                Debug.LogError("Could not return " +
                    "the projectile back to the pool.");
            }
        }

        /// <summary>
        /// Updates the object each frame.
        /// </summary>
        protected virtual void Update()
        {
            UpdateFiringTimer();
        }

        /// <summary>
        /// Makes the weapon fire at set intervals.
        /// </summary>
        private void UpdateFiringTimer()
        {
            if (!canFire)
            {
                firingTimer += Time.deltaTime;
            }

            if (firingTimer >= firingRate)
            {
                canFire = true;
                firingTimer = 0;
            }
        }

        /// <summary>
        /// Fires a projectile.
        /// </summary>
        /// <returns>Was a projectile fired successfully</returns>
        public bool Fire()
        {
            if (!canFire)
            {
                return false;
            }

            // Takes a projectile from the pool and launches it
            Projectile projectile = projectiles.GetPooledObject();

            if (projectile != null)
            {
                canFire = false;

                // Calculates the firing direction:
                // from the barrel's base to its tip in world space
                Vector3 firingDirection = shootingPoint.position - transform.position;

                // Changes the firing direction vector's distance to 1
                firingDirection.Normalize();

                // FIXME
                //projectile.SetHole(holes.GetPooledObject(false));

                projectile.transform.position = shootingPoint.position;
                projectile.Launch(firingDirection);

                // Plays particle effects
                if (shootParticles != null)
                {
                    shootParticles.Play();
                }

                return true;
            }

            // Creates a new projectile
            //Projectile newProjectile = Instantiate(projectilePrefab);
            //newProjectile.Init(barrelTipPos, firingDirection, 20);

            return false;
        }
    }
}
