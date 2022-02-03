using Research.LevelDesign.Scripts.MLAgents;

namespace Research.Common.MapSensor.GridSpaceEntity
{
    public class ProjectileMapPosition : BaseMapPosition
    {
        public override EntityMapReturn[] GetGridSpaceType(int teamId)
        {
            var teamID = GetComponentInParent<GetTeamID>().TeamId;
            var objectPooler = GetComponentInChildren<MLObjectPooler>();
            var positions = objectPooler.GetListObjects();
            var value  = teamID == 0 ? GridSpace.Projectile1 : GridSpace.Projectile2;
            var returnValues = new EntityMapReturn [positions.Count];
            for (var i = 0; i < returnValues.Length; i++)
            {
                returnValues[i] = new EntityMapReturn{ GridSpace=value, Position = positions[i] };
            }
            return returnValues;
        }
    }
}
