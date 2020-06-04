using MoreMountains.TopDownEngine;

namespace Research.Common.Weapons
{
    public class MlCloseToWall : WeaponPreventShootingWhenCloseToWalls2D
    {
        public bool CanShoot => _shootStopped;
    }
}
