using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{	
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Laser Sight")]
	/// <summary>
	/// Add this class to a weapon and it'll project a laser ray towards the direction the weapon is facing
	/// </summary>
	public class WeaponLaserSight : MonoBehaviour 
	{
        [Header("General Settings")]
        /// if this is false, raycasts won't be computed for this laser sight
        public bool PerformRaycast = true;
        [Condition("PerformRaycast")]
        /// if this is false, the laser won't be drawn
        public bool DrawLaser = true;

        [Header("Raycast Settings")]
		/// the origin of the raycast used to detect obstacles
		public Vector3 RaycastOriginOffset;
		/// the origin of the visible laser
		public Vector3 LaserOriginOffset;
		/// the maximum distance to which we should draw the laser
		public float LaserMaxDistance = 50;
		/// the collision mask containing all layers that should stop the laser
		public LayerMask LaserCollisionMask;

        [Header("Laser")]
		/// the width of the laser
		public Vector2 LaserWidth = new Vector2(0.05f, 0.05f);
		/// the material used to render the laser
		public Material LaserMaterial;

		public LineRenderer _line { get; protected set; }
        public RaycastHit _hit { get; protected set; }
        public Vector3 _origin { get; protected set; }
        public Vector3 _destination { get; protected set; }
        public Vector3 _laserOffset { get; protected set; }

        /// <summary>
        /// Initialization
        /// </summary>
        protected virtual void Start()
		{
			Initialization();
		}

		/// <summary>
		/// On init we create our line if needed
		/// </summary>
		protected virtual void Initialization()
		{
            if (DrawLaser)
            {
                _line = gameObject.AddComponent<LineRenderer>();
                _line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                _line.receiveShadows = true;
                _line.startWidth = LaserWidth.x;
                _line.endWidth = LaserWidth.y;
                _line.material = LaserMaterial;
            }			
		}

		/// <summary>
		/// Every frame we draw our laser
		/// </summary>
		protected virtual void Update()
		{
			ShootLaser();
		}

		/// <summary>
		/// Draws the actual laser
		/// </summary>
		protected virtual void ShootLaser()
		{
			if (!PerformRaycast)
            {
                return;
            }

			_laserOffset = LaserOriginOffset;
		
			// our laser will be shot from the weapon's laser origin
			_origin = MMMaths.RotatePointAroundPivot (this.transform.position + _laserOffset, this.transform.position, this.transform.rotation);

			// we cast a ray in front of the weapon to detect an obstacle
			_hit = MMDebug.Raycast3D(_origin, this.transform.forward, LaserMaxDistance, LaserCollisionMask, Color.red, true);

			// if we've hit something, our destination is the raycast hit
			if (_hit.transform != null)
			{
				_destination = _hit.point;
			}
			// otherwise we just draw our laser in front of our weapon 
			else
			{
				_destination = _origin + this.transform.forward * LaserMaxDistance;
			}

            // we set our laser's line's start and end coordinates
            if (DrawLaser)
            {
                _line.SetPosition(0, _origin);
                _line.SetPosition(1, _destination);
            }					
		}

		/// <summary>
		/// Turns the laser on or off depending on the status passed in parameters
		/// </summary>
		/// <param name="status">If set to <c>true</c> status.</param>
		public virtual void LaserActive(bool status)
		{
			_line.enabled = status;
		}

	}
}