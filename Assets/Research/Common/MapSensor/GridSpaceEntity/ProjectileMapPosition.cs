using System.Collections.Generic;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public class ProjectileMapPosition : BaseMapPosition
    {
        public int bulletTeamId;
        public override EntityMapReturn[] GetGridSpaceType(int teamId)
        {
            var objectPooler = GetComponentInChildren<MLObjectPooler>();
            var positions = objectPooler.GetListObjects();
            var value  = bulletTeamId == teamId ? GridSpace.Projectile1 : GridSpace.Projectile2;
            var returnValues = new EntityMapReturn [positions.Count];
            for (var i = 0; i < returnValues.Length; i++)
            {
                returnValues[i] = new EntityMapReturn{ GridSpace=value, Position = positions[i] };
            }
            return returnValues;
        }
    }
}
