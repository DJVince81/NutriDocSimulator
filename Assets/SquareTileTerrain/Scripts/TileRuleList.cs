#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace SquareTileTerrainEditor
{
    public class TileRuleList : ScriptableObject
    {
        [SerializeField] public List<TileListElement> tileList = new List<TileListElement>();

        private FileInfo[] prefabFiles, ruleFiles;

        /* Automatic fill by pressing 'Autofill' button */
        public bool FillTileRuleList(string rulesPath, string prefabPath, out string errorMessage)
        {
            errorMessage = "";

            tileList.Clear();

            /* Get prefabs list */
            var prefabDirectory = new DirectoryInfo(prefabPath);
            try { prefabFiles = prefabDirectory.GetFiles("*.prefab"); }
            catch { errorMessage = prefabDirectory.FullName + " is an invalid directory."; return false;}

            /* Get rule list */
            var ruleDirectory = new DirectoryInfo(rulesPath);
            try { ruleFiles = ruleDirectory.GetFiles(); }
            catch { errorMessage = ruleDirectory.FullName + " is an invalid directory."; return false; }
            List<FileInfo> ruleFilesWithoutMeta = new List<FileInfo>(ruleFiles);
            foreach (FileInfo ruleFile in ruleFiles) if(ruleFile.Extension.EndsWith("meta")) ruleFilesWithoutMeta.Remove(ruleFile);

            /* Check Prefab/Rule count */
            int TileCount = 0;
            if(prefabFiles.Length == ruleFilesWithoutMeta.Count) TileCount = prefabFiles.Length;
            else { errorMessage = "Tiles/Rules mismatch : " +  prefabFiles.Length + " tiles and " + ruleFilesWithoutMeta.Count + " rules have been found."; return false; }

            /* Fill reorderable list */
            for(int i = 0; i < TileCount ; i++)
            {
                Texture2D  tileRule   = AssetDatabase.LoadAssetAtPath((rulesPath + "/" + ruleFilesWithoutMeta[i].Name), typeof(Texture2D)) as Texture2D;
                GameObject tilePrefab = AssetDatabase.LoadAssetAtPath((prefabPath + "/" + prefabFiles[i].Name), typeof(GameObject)) as GameObject;

                tileList.Add(new TileListElement(tileRule, tilePrefab));
            }

            return true;
        }
    }
}
#endif