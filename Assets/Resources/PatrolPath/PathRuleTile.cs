using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "PathRuleTile", menuName = "Tiles/Path Rule Tile")]
public class PathRuleTile : RuleTile
{
    public bool isActive = true;
    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        if (!isActive) return false;
        return base.RuleMatch(neighbor, tile);
    }
}