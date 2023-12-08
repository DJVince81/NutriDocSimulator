#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SquareTileTerrainEditor
{
    [Serializable]
    public class TileListElement
    {
        public Texture2D  tileRuleSprite;   // Sprite containing rules for selecting attached prefab
        public GameObject tilePrefab;       // Tile prefab

        public TileListElement(Texture2D trs, GameObject tp)
        {
            tileRuleSprite = trs;
            tilePrefab = tp;
        }
    }
}
#endif
