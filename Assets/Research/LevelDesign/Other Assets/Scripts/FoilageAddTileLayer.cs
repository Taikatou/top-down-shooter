using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
[RequireComponent(typeof(Tilemap))]
public class FoilageAddTileLayer : MonoBehaviour
{
    private List<Vector3Int> positionList;
    private Tilemap tilemap;

    void Start()
    {
        positionList = new List<Vector3Int>();
        tilemap = GetComponent<Tilemap>();
    }

    public void AddUpdate(Vector3Int position)
    {
        positionList.Add(position);
    }

    public void LateUpdate()
    {
        foreach (var position in positionList)
        {
            var tile = tilemap.GetTile<PaintOnLayerRuleTile>(position);
            if (tile != null)
            {
                foreach (var paintTile in tile.paintTileList)
                {
                    if (!tilemap.HasTile(position + paintTile.offset))
                        tilemap.SetTile(position + paintTile.offset, paintTile.paintTile);
                }
            }
        }
        positionList.Clear();
    }
}