using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class PaintOnLayerRuleTile : RuleTile<PaintOnLayerRuleTile.Neighbor>
{
    [Serializable]
    public struct PaintTile
    {
        public Vector3Int offset;
        public TileBase paintTile;
    }
    public List<PaintTile> paintTileList = new List<PaintTile>();

    public class Neighbor : RuleTile.TilingRule.Neighbor 
    {
    }

    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        base.RefreshTile(location, tilemap);
        var foilageLayer = tilemap.GetComponent<FoilageAddTileLayer>();
        if (foilageLayer != null)
            foilageLayer.AddUpdate(location);
    }
}
