using MoreMountains.TopDownEngine;

namespace Research.CharacterDesign.Scripts.Environment
{
    public class MLCheckbox : CheckPoint
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
    }
}
