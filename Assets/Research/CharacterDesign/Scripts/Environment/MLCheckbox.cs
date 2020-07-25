using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.SpawnPoints;
using Research.LevelDesign.NuclearThrone.Scripts;

namespace Research.CharacterDesign.Scripts.Environment
{
    public class MlCheckbox : CheckPoint, IEntityClass
    {
        public int playerId;
        public override void SpawnPlayer(Character player)
        {
            base.SpawnPlayer(player);
            player.PlayerID = "Player" + playerId;
        }

        protected override void OnDrawGizmos()
        {

        }

        public void SetId(int id)
        {
            playerId = id;
        }

        public GridSpace GetGridSpace()
        {
            return playerId == 0? GridSpace.Spawn1 : GridSpace.Spawn2;
        }
        
        public int GetId()
        {
            return playerId;
        }
    }
}
