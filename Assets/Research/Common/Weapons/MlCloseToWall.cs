using MoreMountains.TopDownEngine;
using Unity.MLAgents.Sensors.Reflection;

namespace Research.Common.Weapons
{
    public class MlCloseToWall : WeaponPreventShootingWhenCloseToWalls2D
    {
        [Observable]
        public bool CanShoot => _shootStopped;
    }
}
