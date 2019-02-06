using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    /// <summary>
    /// A projectile which deals damage to opposing units.
    /// </summary>
    public class Projectile : LevelObject
    {
        [SerializeField, Header("Basic Values")]
        private int damage;

        [SerializeField]
        private float shootingForce;

        [SerializeField, Header("Explosion Values")]
        private float explosionForce;

        [SerializeField]
        private float explosionRadius;

        /// <summary>
        /// A mask of targets the projectile can hit
        /// </summary>
        [SerializeField, HideInInspector]
        private int hitMask;

        /// <summary>
        /// An action which is passed from weapon and
        /// fired when the projectile hits something.
        /// </summary>
        private Action<Projectile> PassCollisionInfoToWeapon;

        //[SerializeField]
        //private ProjectileType type;

        //[Flags]
        //public enum ProjectileType
        //{
        //    None = 0,
        //    Player = 1,
        //    Enemy = 1 << 1,
        //    Neutral = 1 << 2
        //}

        private Weapon weapon;
        private Rigidbody rBody;

        /// <summary>
        /// The hit mark the projectile creates on collision
        /// </summary>
        private HitMark hitMark;
        private bool hitMarkCreated;

        /// <summary>
        /// Self-initializing property. Gets the reference to
        /// the Rigidbody component when used the first time.
        /// </summary>
        public Rigidbody Rigidbody
        {
            get
            {
                if (rBody == null)
                {
                    rBody = gameObject.GetOrAddComponent<Rigidbody>();
                }

                return rBody;
            }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="collisionCallback">A method which is called when
        /// the projectile hit something</param>
        public void Init(Action<Projectile> collisionCallback)
        {
            PassCollisionInfoToWeapon = collisionCallback;
        }

        public void SetDamage(int damage)
        {
            this.damage = damage;
        }

        public void SetForce(float force)
        {
            shootingForce = force;
        }

        public int GetDamage()
        {
            return damage;
        }

        /// <summary>
        /// Launches the projectile at the given direction.
        /// </summary>
        /// <param name="direction">Firing direction</param>
        public void Launch(Vector3 direction)
        {
            Rigidbody.AddForce(direction.normalized * shootingForce, ForceMode.Impulse);
            //hitMarkCreated = false;
        }

        /// <summary>
        /// Called when the projectile collides with something.
        /// </summary>
        /// <param name="collision">Collision info</param>
        protected void OnCollisionEnter(Collision collision)
        {
            if (!Destroyed)
            {
                //ApplyDamage();
                IDamageReceiver dmgRec = collision.transform.GetComponent<IDamageReceiver>();
                if (dmgRec != null)
                {
                    dmgRec.TakeDamage(damage);
                }

                // Stops the projectile
                Rigidbody.velocity = Vector3.zero;

                // Passes collision information to the weapon
                PassCollisionInfoToWeapon(this);

                // Creates a hit mark at the point of impact
                //if (!hitMarkCreated)
                //{
                //    CreateHitMark(collision);
                //    hitMarkCreated = true;
                //}

                DestroyObject();
            }
        }

        /// <summary>
        /// Deals damage to the hit target.
        /// </summary>
        private void ApplyDamage()
        {
            List<IDamageReceiver> alreadyDamaged = new List<IDamageReceiver>();

            // Gets all damage receiving colliders in
            // an area within the explosion radius
            Collider[] damageReceivers = Physics.OverlapSphere(
                transform.position, explosionRadius, hitMask);

            // Deals damage to the damage receivers if they are valid
            for (int i = 0; i < damageReceivers.Length; i++)
            {
                IDamageReceiver damageReceiver =
                    damageReceivers[i].GetComponentInParent<IDamageReceiver>();
                if (damageReceiver != null &&
                    !alreadyDamaged.Contains(damageReceiver))
                {
                    damageReceiver.TakeDamage(damage);
                    alreadyDamaged.Add(damageReceiver);
                    // TODO: Apply explosion force
                }
            }
        }

        /// <summary>
        /// Sets the hit mark this projectile creates on collision.
        /// </summary>
        /// <param name="hitMark">A mark in the hit position</param>
        public void SetHitMark(HitMark hitMark)
        {
            this.hitMark = hitMark;
        }

        /// <summary>
        /// Creates a hit mark at the point of collision.
        /// </summary>
        /// <param name="collision">Collision info</param>
        private void CreateHitMark(Collision collision)
        {
            if (hitMark != null && !hitMark.gameObject.activeSelf)
            {
                hitMark.transform.position = transform.position;

                Vector3 point = collision.contacts[0].point;
                //foreach (ContactPoint contact in collision.contacts)
                //{
                //    if (Vector3.Distance(transform.position, contact.point) <
                //        Vector3.Distance(transform.position, point))
                //    {
                //        point = contact.point;
                //    }
                //}

                // TODO: Fix the hit mark's rotation

                hitMark.transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - point);
                //hitMark.transform.rotation = Quaternion.LookRotation(Vector3.up, transform.position - point);

                hitMark.gameObject.SetActive(true);
            }
        }

        public override void DestroyObject()
        {
            Destroyed = true;
            gameObject.SetActive(false);
            base.DestroyObject();
        }

        public override void ResetObject()
        {
            Destroyed = false;
            gameObject.SetActive(false);
            base.ResetObject();
        }
    }
}
