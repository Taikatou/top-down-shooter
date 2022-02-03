using MoreMountains.Tools;
using Research.CharacterDesign.Scripts;
using UnityEngine;

namespace Research.LevelDesign.Scripts.AI
{
    public class SearchAiAction : AIAction
    {
        public GameObject otherPlayer;
        public MapAccessor mapAccessor;
        public override void PerformAction()
        {
            var startPos = mapAccessor.GetPosition(transform.position);
            var endPos = mapAccessor.GetPosition(otherPlayer.transform.position);
            var newPositions = AStar.FindPath(mapAccessor.GetMap(),
                new Vector2Int(startPos.x, startPos.y),
                new Vector2Int(endPos.x, endPos.y));
        }
    }
}
