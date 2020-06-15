namespace Research.Common.MapSensor.GridSpaceEntity
{
    public class NoTeamGetTeamId : IGetTeamId
    {
        public int teamId;

        public override int GetTeamId => teamId;
    }
}
