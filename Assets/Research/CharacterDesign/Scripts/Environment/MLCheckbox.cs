using MoreMountains.TopDownEngine;

namespace Research.Scripts.Environment
{
    public class MLCheckbox : CheckPoint
    {
        public int playerId;
        public override void SpawnPlayer(Character player)
        {
            base.SpawnPlayer(player);
            player.PlayerID = "Player" + playerId;
        }
    }
}
