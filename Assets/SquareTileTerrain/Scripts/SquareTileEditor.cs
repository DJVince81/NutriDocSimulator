#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;

namespace SquareTileTerrainEditor
{
    public class SquareTileEditor : EditorWindow
    {
        enum normalDir { None, Up, Right, Forward };

        TileRuleList obj;
        ReorderableList ruleReorderableList;
        SerializedObject serializedObject;
        SerializedProperty serializedProperty;

        string generateError = "";
        string terrainConfigError = "";
        string terrainConfigWarning = "";
        string terrainConfigInfo = "";

        Vector2 tileListScrollPos;
        Vector2 wholeEditorScrollPos;

        string autofillPrefabPath = "Assets/SquareTileTerrain/Example/TilePrefabs";
        string autofillRulePath = "Assets/SquareTileTerrain/Example/RulesSprites";
        string tileNotFoundPrefabPath = "Assets/SquareTileTerrain/Example/Other/Tile_TNF.prefab";

        string treePath = "Assets/SquareTileTerrain/Example/Tree/TreePrefab.prefab";
        Texture2D treemap;
        int treeMaxDensity = 1;
        GameObject treePrefab;


        float tileSizeX = 10;
        float tileSizeZ = 10;

        int tileQtyX = 32;
        int tileQtyZ = 32;

        Texture2D heightmap;
        Texture2D tilemap;
        float heightDelta = 8;

        normalDir normalDirection = normalDir.None;

        bool keepPos = true;
        int terrainIdx = 0;

        bool[] sections = { true, true, true, true };

        private void OnEnable()
        {
            /* Some Unity stuff */
            /* Create Tile reorderable list */
            obj = ScriptableObject.CreateInstance<TileRuleList>();
            serializedObject = new UnityEditor.SerializedObject(obj);
            serializedProperty = serializedObject.FindProperty("tileList");

            ruleReorderableList = new ReorderableList(serializedObject,
                                                    serializedProperty,
                                                    true, true, true, true);

            ruleReorderableList.drawElementCallback = DrawListItems;

        }



        [MenuItem("Tools/Square Tile Editor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SquareTileEditor));
        }

        /* Draw editor on GUI */
        private void OnGUI()
        {
            wholeEditorScrollPos = EditorGUILayout.BeginScrollView(wholeEditorScrollPos, true, false);

            sections[0] = EditorGUILayout.Foldout(sections[0], ("Tile management (" + obj.tileList.Count + ")"));

            if (sections[0])
            {
                tileListScrollPos = EditorGUILayout.BeginScrollView(tileListScrollPos, true, true, GUILayout.Height(400));

                serializedObject.Update();
                ruleReorderableList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();

                EditorGUILayout.EndScrollView();

                autofillPrefabPath = EditorGUILayout.TextField("Prefab path", autofillPrefabPath);
                autofillRulePath   = EditorGUILayout.TextField("Rule path", autofillRulePath);
                tileNotFoundPrefabPath   = EditorGUILayout.TextField("Default Tile Prefab", tileNotFoundPrefabPath);

                if(GUILayout.Button("Autofill"))
                {
                    obj.FillTileRuleList(autofillRulePath, autofillPrefabPath, out generateError);
                }

                EditorGUILayout.Separator();
            }
            
            sections[1] = EditorGUILayout.Foldout(sections[1], "Tile Properties");

            if(sections[1])
            {
                tileSizeX = EditorGUILayout.FloatField("X Size", tileSizeX);
                tileSizeZ = EditorGUILayout.FloatField("Z Size", tileSizeZ);
                heightmap = (Texture2D) EditorGUILayout.ObjectField("Heightmap", heightmap, typeof(Texture2D), false);
                if(heightmap)
                {
                    heightDelta = EditorGUILayout.FloatField("Height Multiplier", heightDelta);
                }
                tilemap = (Texture2D) EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Texture2D), false);
                terrainIdx = EditorGUILayout.IntField("Terrain Index", terrainIdx);
                
                if(GUILayout.Button("Load terrain configuration")) {
                    LoadTerrainConfig(terrainIdx, out terrainConfigError, out terrainConfigInfo);
                    obj.FillTileRuleList(autofillRulePath, autofillPrefabPath, out generateError);
                }
                if(GUILayout.Button("Save terrain configuration"))
                {
                    SaveTerrainConfig(terrainIdx, out terrainConfigError, out terrainConfigWarning, out terrainConfigInfo);
                }
                
                if     (terrainConfigError != "")   EditorGUILayout.HelpBox(terrainConfigError,   MessageType.Error);
                else if(terrainConfigWarning != "") EditorGUILayout.HelpBox(terrainConfigWarning, MessageType.Warning);
                else if(terrainConfigInfo != "")    EditorGUILayout.HelpBox(terrainConfigInfo,    MessageType.Info);

                EditorGUILayout.Separator();
            }

            sections[2] = EditorGUILayout.Foldout(sections[2], "Tree Generation");

            if(sections[2])
            {
                treePath = EditorGUILayout.TextField("Tree Prefab", treePath);

                treemap = (Texture2D) EditorGUILayout.ObjectField("Treemap", treemap, typeof(Texture2D), false);

                treeMaxDensity = EditorGUILayout.IntField("Tree max density", treeMaxDensity);

                if (treemap == null)
                {
                    EditorGUILayout.HelpBox("If no tree map is defined, trees will not be added.", MessageType.Info);
                }

                EditorGUILayout.Separator();
            }
            
            sections[3] = EditorGUILayout.Foldout(sections[3], "Terrain Generation");

            if(sections[3])
            {
                normalDirection = (normalDir) EditorGUILayout.EnumPopup("(Light) Rearrange normals", normalDirection);

                keepPos = EditorGUILayout.Toggle("Keep terrain position", keepPos);

                if(generateError != "") EditorGUILayout.HelpBox(generateError, MessageType.Warning);

                if(GUILayout.Button("Generate Terrain !"))
                {

                    if(CheckSettings(out generateError))
                    {
                        Vector3    terrainPos = Vector3.zero;
                        Quaternion terrainRot = Quaternion.identity;
                        Vector3    terrainScl = Vector3.one;

                        if(GameObject.Find("Terrain_" + terrainIdx))
                        {
                            if(keepPos)
                            {
                                terrainPos = GameObject.Find("Terrain_" + terrainIdx).transform.position;
                                terrainRot = GameObject.Find("Terrain_" + terrainIdx).transform.rotation;
                                terrainScl = GameObject.Find("Terrain_" + terrainIdx).transform.localScale;
                            }
                            EraseTerrain(terrainIdx);
                        }
                        if(GenerateTerrain(terrainIdx, out generateError));
                        {
                            GameObject.Find("Terrain_" + terrainIdx).transform.position = terrainPos;
                            GameObject.Find("Terrain_" + terrainIdx).transform.rotation = terrainRot;
                            GameObject.Find("Terrain_" + terrainIdx).transform.localScale = terrainScl;
                        }
                    }
                } 

                if(GUILayout.Button("Erase Terrain"))
                {
                    EraseTerrain(terrainIdx);
                }
                EditorGUILayout.Separator();
            }

            EditorGUILayout.EndScrollView();
        }

        /* Content of rule list */
        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = ruleReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            rect.height = 2 * EditorGUIUtility.singleLineHeight;
            ruleReorderableList.elementHeight = rect.height;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, rect.height), "Rule n°"+ (index+1));

            EditorGUI.PropertyField(
            new Rect(rect.x+70, rect.y, 80, rect.height),
            element.FindPropertyRelative("tilePrefab"),
            GUIContent.none
            );

            EditorGUI.PropertyField(
            new Rect(rect.x+160, rect.y, 100, rect.height),
            element.FindPropertyRelative("tileRuleSprite"),
            GUIContent.none
            );

            /* Draw rule previews */
            Texture2D texture = AssetPreview.GetAssetPreview(element.FindPropertyRelative("tileRuleSprite").objectReferenceValue);
            float coordBegin = rect.x + 270;
            for(int i=0; i < (texture.width/3); i++)
            {
                if(texture.GetPixel(i*3+0,0) != Color.black) EditorGUI.DrawRect(new Rect(coordBegin + i*40+00, rect.y+22, 10, 10), texture.GetPixel(i*3+0,0));
                if(texture.GetPixel(i*3+1,0) != Color.black) EditorGUI.DrawRect(new Rect(coordBegin + i*40+10, rect.y+22, 10, 10), texture.GetPixel(i*3+1,0));
                if(texture.GetPixel(i*3+2,0) != Color.black) EditorGUI.DrawRect(new Rect(coordBegin + i*40+20, rect.y+22, 10, 10), texture.GetPixel(i*3+2,0));
                if(texture.GetPixel(i*3+0,1) != Color.black) EditorGUI.DrawRect(new Rect(coordBegin + i*40+00, rect.y+12, 10, 10), texture.GetPixel(i*3+0,1));
                if(texture.GetPixel(i*3+2,1) != Color.black) EditorGUI.DrawRect(new Rect(coordBegin + i*40+20, rect.y+12, 10, 10), texture.GetPixel(i*3+2,1));
                if(texture.GetPixel(i*3+0,2) != Color.black) EditorGUI.DrawRect(new Rect(coordBegin + i*40+00, rect.y+02, 10, 10), texture.GetPixel(i*3+0,2));
                if(texture.GetPixel(i*3+1,2) != Color.black) EditorGUI.DrawRect(new Rect(coordBegin + i*40+10, rect.y+02, 10, 10), texture.GetPixel(i*3+1,2));
                if(texture.GetPixel(i*3+2,2) != Color.black) EditorGUI.DrawRect(new Rect(coordBegin + i*40+20, rect.y+02, 10, 10), texture.GetPixel(i*3+2,2));

                EditorGUI.DrawRect(new Rect(coordBegin + i*40+09, rect.y+11, 12, 12), Color.white);
                EditorGUI.DrawRect(new Rect(coordBegin + i*40+10, rect.y+12, 10, 10), texture.GetPixel(i*3+1,1));
            }
        }

        /* Check that all conditions are reunited to process terrain generation.
        returns : true if terrain can be generated, false otherwise.
        out :     potential error message as string
        */
        bool CheckSettings(out string errorMessage)
        {
            errorMessage = "";

            FileInfo treeFile = new FileInfo(treePath);
            FileInfo tileNotFoundFile = new FileInfo(tileNotFoundPrefabPath);

            /* Terrain can be generated if :
            - Tilemap is given
            - Tile rules array is not empty
            - Tree map is either not given, or given and tree prefab path valid
            - Default tile "Tile not found" field is either empty, or given and valid
            */
            if(tilemap && 
            obj.tileList.Count > 0 &&
            (treemap == null || (treemap && treeFile.Exists)) &&
            (tileNotFoundPrefabPath == "" || (tileNotFoundPrefabPath != "" && tileNotFoundFile.Exists)))
            {
                return true;
            }

            else
            {
                if(tilemap == null) { errorMessage = "Tilemap is not defined."; }
                else if(obj.tileList.Count < 1) { errorMessage = "No tiles defined (Array is empty).";}
                else if(treemap && !treeFile.Exists) { errorMessage = "Tree prefab path is invalid : " + treeFile.FullName; }
                else if((tileNotFoundPrefabPath != "" && !tileNotFoundFile.Exists)) { errorMessage = "TileNotFound prefab path is invalid : " + tileNotFoundFile.FullName; }
                else { errorMessage = "Unknown error while checking settings."; }
                return false;
            }
        }

        /* Generation terrain tiles as child of empty with name from given index
        returns : terrain generation is successful
        out : potential error message as string
        */
        bool GenerateTerrain(int tIdx, out string errorMessage)
        {
            GameObject terrainEmpty = new GameObject("Terrain_" + tIdx);
            treePrefab = AssetDatabase.LoadAssetAtPath(treePath, typeof(GameObject)) as GameObject;

            tileQtyX = tilemap.width;
            tileQtyZ = tilemap.height;

            /* Generate each tile */
            for (int xIdx = 0; xIdx < tileQtyX; xIdx++)
            {
                for (int zIdx = 0; zIdx < tileQtyZ; zIdx++)
                {
                    int rotation = 0;
                    GameObject prefab = FindCorrespondingTile(xIdx, zIdx, out rotation, out errorMessage);

                    if(prefab != null)
                    {
                        GameObject tile = Instantiate(prefab, new Vector3(xIdx * tileSizeX, 0, zIdx * tileSizeZ), Quaternion.identity * Quaternion.Euler(0, -90 * rotation, 0), terrainEmpty.transform);
                        tile.name = "Tile_T" + tIdx + "_X" + xIdx + "_Z" + zIdx;

                        if (heightmap)
                        {
                            EditorUtility.DisplayProgressBar("Work in progress", "Setting heightmap (" + (((xIdx * tileQtyX) + zIdx)) +"/" + ((tileQtyX * tileQtyZ)) +")...", (float)((xIdx*tileQtyX)+ zIdx) / (tileQtyX*tileQtyZ));
                            SetHeightOnTile(tile, xIdx, zIdx, rotation);
                        }

                        if(treemap)
                        {
                            AddTrees(tIdx, xIdx, zIdx);
                        }
                    }
                    else
                    {
                        /* errorMessage = error in FindCorrespondingTile */
                        return false;
                    }

                }
            }
            EditorUtility.ClearProgressBar();

            errorMessage = "";
            return true;
        }

        void EraseTerrain(int tIdx)
        {
            DestroyImmediate(GameObject.Find("Terrain_" + tIdx));
        }

        /* Add trees prefab to each tile according to Treemap image
        returns : true
        */
        bool AddTrees(int tIdx, int xIdx, int zIdx)
        {
            /* How many trees on this tile */
            int treeNb = (int)(treemap.GetPixel(xIdx, zIdx).r * treeMaxDensity); // Number of trees from 0 to treeMaxDensity

            /* Tile on which trees are */
            GameObject correspondingTile = GameObject.Find("Tile_T" + tIdx + "_X" + xIdx + "_Z" + zIdx);

            /* Height at each corner of tile defined by corresponding 4-pixels on heightmap */
            float[,] heightCorner = new float[2, 2];
            if(heightmap)
            {
                heightCorner[0, 0] = heightmap.GetPixel(xIdx, zIdx).r;          // -X / -Z corner
                heightCorner[1, 0] = heightmap.GetPixel(xIdx + 1, zIdx).r;      //  X / -Z corner
                heightCorner[0, 1] = heightmap.GetPixel(xIdx, zIdx + 1).r;      // -X /  Z corner
                heightCorner[1, 1] = heightmap.GetPixel(xIdx + 1, zIdx + 1).r;  //  X /  Z corner
            }
            else
            {
                heightCorner[0, 0] = 0;
                heightCorner[1, 0] = 0;
                heightCorner[0, 1] = 0;
                heightCorner[1, 1] = 0;
            }

            float meanColor = (heightCorner[0,0] + heightCorner[1,0] + heightCorner[0,1] + heightCorner[1,1]) / 4.0f;

            /* Set trees at different spots for each tiles but make sure seed will be identical for each generation */
            UnityEngine.Random.seed = xIdx + tileQtyZ * zIdx;

            for(int i=0; i< treeNb; i++)
            {
                GameObject tree = Instantiate(treePrefab, Vector3.zero, Quaternion.identity);
                tree.transform.parent = correspondingTile.transform;
                tree.transform.localPosition = Vector3.zero;
                tree.transform.rotation = Quaternion.identity;

                tree.transform.localPosition = new Vector3(Mathf.Lerp((-tileSizeX/2), (tileSizeX/2), UnityEngine.Random.value),
                                                        0,
                                                        Mathf.Lerp((-tileSizeZ/2), (tileSizeZ/2), UnityEngine.Random.value));

                /* XZ position factor from 0 to 1 (to interpolate height between corners) */
                float xFactor = ((tree.transform.localPosition.x) + (tileSizeX / 2.0f)) / tileSizeX;
                float zFactor = ((tree.transform.localPosition.z) + (tileSizeZ / 2.0f)) / tileSizeZ;

                Vector3 verticalPos = Vector3.zero;

                verticalPos.y += (Mathf.Lerp(heightCorner[0, 0], heightCorner[1, 0], xFactor)-meanColor) * (1 - zFactor) * heightDelta/2;
                verticalPos.y += (Mathf.Lerp(heightCorner[0, 1], heightCorner[1, 1], xFactor)-meanColor) * (zFactor) * heightDelta/2;
                verticalPos.y += (Mathf.Lerp(heightCorner[0, 0], heightCorner[0, 1], zFactor)-meanColor) * (1 - xFactor) * heightDelta/2;
                verticalPos.y += (Mathf.Lerp(heightCorner[1, 0], heightCorner[1, 1], zFactor)-meanColor) * (xFactor) * heightDelta/2;

                tree.transform.localPosition += verticalPos;
            }
            return true;
        }

        /* Set each vertex specific height according to their XZ position in prefab */
        void SetHeightOnTile(GameObject tile, int xIdx, int zIdx, int rotation)
        {
            /* Height at each corner of tile defined by corresponding 4-pixels on heightmap */
            float[,] heightCorner = new float[2, 2];
            heightCorner[0, 0] = heightmap.GetPixel(xIdx, zIdx).r;          // -X / -Z corner
            heightCorner[1, 0] = heightmap.GetPixel(xIdx + 1, zIdx).r;      //  X / -Z corner
            heightCorner[0, 1] = heightmap.GetPixel(xIdx, zIdx + 1).r;      // -X /  Z corner
            heightCorner[1, 1] = heightmap.GetPixel(xIdx + 1, zIdx + 1).r;  //  X /  Z corner

            /* Rotate heightmap to match rotated mask */
            for(int i = 0; i < rotation; i++) heightCorner = RotateHeightMap(heightCorner);

            /* Get every children and grand children in prefab */
            List<GameObject> children = new List<GameObject>();
            FindChildrenAndGrandChildrenAndSoOn(tile.transform, children);

            /* Raise tile to mean height */
            float meanColor = (heightCorner[0,0] + heightCorner[1,0] + heightCorner[0,1] + heightCorner[1,1]) / 4.0f;
            tile.transform.position = new Vector3(tile.transform.position.x,
                                                tile.transform.position.y + meanColor * heightDelta,
                                                tile.transform.position.z);

            /* Deform the mesh filters and colliders */
            foreach (GameObject objet in children)
            {
                MeshFilter meshFilter = objet.GetComponent<MeshFilter>();

                if(meshFilter != null)
                {
                    Mesh mesh = meshFilter.mesh;
                    Vector3[] verts = mesh.vertices;
                    MeshCollider meshCol = objet.GetComponent<MeshCollider>();

                    /* Recalculate normals */
                    Vector3[] normals = mesh.normals;
                    if(normalDirection != normalDir.None)
                    for (int normIdx = 0; normIdx < normals.Length; normIdx++)
                    {
                        Vector3 v = Vector3.zero;
                        switch(normalDirection)
                        {
                            case normalDir.Up : v = Vector3.up; break;
                            case normalDir.Right : v = Vector3.right; break;
                            case normalDir.Forward : v = Vector3.forward;break;
                        }
                        normals[normIdx] = v;
                    }
                    mesh.normals = normals;

                    for (int vertIdx = 0; vertIdx < verts.Length; vertIdx++)
                    {
                        /* XZ position factor from 0 to 1 (to interpolate height between corners) */
                        float xFactor = ((tile.transform.InverseTransformPoint(meshFilter.gameObject.transform.TransformPoint(verts[vertIdx])).x) + (tileSizeX / 2.0f)) / tileSizeX;
                        float zFactor = ((tile.transform.InverseTransformPoint(meshFilter.gameObject.transform.TransformPoint(verts[vertIdx])).z) + (tileSizeZ / 2.0f)) / tileSizeZ;

                        Vector3 globalVert = meshFilter.gameObject.transform.TransformPoint(verts[vertIdx]);
                        globalVert.y += ((Mathf.Lerp(heightCorner[0, 0], heightCorner[1, 0], xFactor)-meanColor) * (1 - zFactor) * heightDelta/2);
                        globalVert.y += ((Mathf.Lerp(heightCorner[0, 1], heightCorner[1, 1], xFactor)-meanColor) * (zFactor)     * heightDelta/2);
                        globalVert.y += ((Mathf.Lerp(heightCorner[0, 0], heightCorner[0, 1], zFactor)-meanColor) * (1 - xFactor) * heightDelta/2);
                        globalVert.y += ((Mathf.Lerp(heightCorner[1, 0], heightCorner[1, 1], zFactor)-meanColor) * (xFactor)     * heightDelta/2);
                        verts[vertIdx] = meshFilter.gameObject.transform.InverseTransformPoint(globalVert);
                    }
                    meshFilter.mesh.vertices = verts;
                    mesh.RecalculateBounds();
                    

                    /* Mesh collider */
                    if (meshCol)
                    {
                        meshCol.sharedMesh = mesh;
                    }
                }
            }
        }

        /* Find right tile by looping through every rule
        returns : corresponding tile prefab
        out : rotation from 0 to 3 (0° to 270°)
        out : potential error message string */
        private GameObject FindCorrespondingTile(int xIdx, int zIdx, out int rot, out string errorMessage)
        {
            FileInfo tileNotFoundFile = new FileInfo(tileNotFoundPrefabPath);

            /* Set default tile */
            var adequatePrefab                          = AssetDatabase.LoadAssetAtPath(tileNotFoundPrefabPath, typeof(GameObject)) as GameObject;
            if(!tileNotFoundFile.Exists) adequatePrefab = obj.tileList[0].tilePrefab;
            
            int rotation = 0;
            foreach(TileListElement info in obj.tileList)
            {
                /* Color of tile + its 8 neighbors */
                Color[,] tilePxel = new Color[3, 3];
                tilePxel[0, 0] = tilemap.GetPixel(xIdx-1, zIdx-1);
                tilePxel[1, 0] = tilemap.GetPixel(xIdx, zIdx-1);
                tilePxel[2, 0] = tilemap.GetPixel(xIdx+1, zIdx-1);

                tilePxel[0, 1] = tilemap.GetPixel(xIdx-1, zIdx);
                tilePxel[1, 1] = tilemap.GetPixel(xIdx, zIdx);  // Pixel at the center
                tilePxel[2, 1] = tilemap.GetPixel(xIdx+1, zIdx);

                tilePxel[0, 2] = tilemap.GetPixel(xIdx-1, zIdx+1);
                tilePxel[1, 2] = tilemap.GetPixel(xIdx, zIdx+1);
                tilePxel[2, 2] = tilemap.GetPixel(xIdx+1, zIdx+1);
                
                if (DoesMaskFitsTileMap(info.tileRuleSprite, tilePxel, out rotation, out errorMessage))
                {
                    adequatePrefab = info.tilePrefab;
                    break;
                }
                else
                {
                    if(errorMessage != "")
                    {
                        rot = 0;
                        return null;
                    }
                }
            }

            rot = rotation;
            errorMessage = "";
            return adequatePrefab;
        }

        /* Check that one rule of a tile could match the tested tile
        returns : true if a rule matches the tested tile
        out : rotation applied to rule from 0 to 3 (0° to 270°) in order to match */
        private bool DoesMaskFitsTileMap(Texture2D mask, Color[,] tilemapMorceau, out int rot, out string errorMessage)
        {
            Color notImportantColor = Color.black;

            if(mask.height != 3) { errorMessage = "Mask " + mask.name + " is not 3 pixels high."; rot  = 0; return false; }
            if(mask.width % 3 != 0) { errorMessage = "Mask " + mask.name + " width is not multiple of 3."; rot = 0; return false; }

            int MasksNb = mask.width / 3;

            for (int maskIndex = 0; maskIndex < MasksNb; maskIndex++)
            {
                /* Get the maskIndexth rule and check every rotation of it */
                Color[,] tempMask = Texture2DToColorArray(mask, maskIndex);

                for (int rotation = 0; rotation <= 3; rotation++)
                {
                    if ((tilemapMorceau[0, 0] == tempMask[0, 0] || tempMask[0, 0] == notImportantColor) &&
                        (tilemapMorceau[1, 0] == tempMask[1, 0] || tempMask[1, 0] == notImportantColor) &&
                        (tilemapMorceau[2, 0] == tempMask[2, 0] || tempMask[2, 0] == notImportantColor) &&
                        (tilemapMorceau[0, 1] == tempMask[0, 1] || tempMask[0, 1] == notImportantColor) &&
                        (tilemapMorceau[1, 1] == tempMask[1, 1]) &&
                        (tilemapMorceau[2, 1] == tempMask[2, 1] || tempMask[2, 1] == notImportantColor) &&
                        (tilemapMorceau[0, 2] == tempMask[0, 2] || tempMask[0, 2] == notImportantColor) &&
                        (tilemapMorceau[1, 2] == tempMask[1, 2] || tempMask[1, 2] == notImportantColor) &&
                        (tilemapMorceau[2, 2] == tempMask[2, 2] || tempMask[2, 2] == notImportantColor))
                    {
                        rot = rotation;
                        errorMessage = "";
                        return true;
                    }

                    tempMask = RotateRule(tempMask);
                }
            }

            errorMessage = "";
            rot = 0;
            return false;
        }

        /* Read 3x3 pixels portion of a rule and store it a color array */
        public Color[,] Texture2DToColorArray(Texture2D input, int index)
        {
            Color[,] temp = new Color[input.width, input.height];

            for(int i = 0; i < 3; i++)
            {
                for(int j=0; j< 3; j++)
                {
                    temp[i, j] = input.GetPixel((3*index) + i, j);
                }
            }
            return temp;
        }

        /* Rotate 3x3 rule by 90 degrees clockwise */
        public Color[,] RotateRule(Color[,] tex)
        {
            Color[,] ret = new Color[3,3];

            ret[0,0] = tex[0,2];
            ret[1,0] = tex[0,1];
            ret[2,0] = tex[0,0];
            ret[0,1] = tex[1,2];
            ret[1,1] = tex[1,1];   //Center pixel remains the same
            ret[2,1] = tex[1,0];
            ret[0,2] = tex[2,2];
            ret[1,2] = tex[2,1];
            ret[2,2] = tex[2,0];

            return ret;
        }

        /* Rotate 2x2 rule by 90 degrees counterclockwise */
        public float[,] RotateHeightMap(float[,] tex)
        {
            float[,] ret = new float[2,2];

            ret[0,0] = tex[1,0];
            ret[1,0] = tex[1,1];
            ret[0,1] = tex[0,0];
            ret[1,1] = tex[0,1];

            return ret;
        }

        private void FindChildrenAndGrandChildrenAndSoOn(Transform parent, List<GameObject> list)
        {
            foreach (Transform child in parent)
            {
                list.Add(child.gameObject);
                FindChildrenAndGrandChildrenAndSoOn(child, list);
            }
        }

        bool LoadTerrainConfig(int terrainIdx, out string errorMessage, out string infoMessage)
        {
            infoMessage = "";
            errorMessage = "";
            string path = "Assets/SquareTileTerrain/Configs/config" + terrainIdx + ".txt";

            try {
                string[] lines = System.IO.File.ReadAllLines(path);
                autofillPrefabPath = lines[0];
                autofillRulePath = lines[1];
                tileNotFoundPrefabPath = lines[2];
                tileSizeX = int.Parse(lines[3]);
                tileSizeZ = int.Parse(lines[4]);
                heightmap = AssetDatabase.LoadAssetAtPath(lines[5], typeof(Texture2D)) as Texture2D;
                heightDelta = int.Parse(lines[6]);
                tilemap = AssetDatabase.LoadAssetAtPath(lines[7], typeof(Texture2D)) as Texture2D;
                treePath = lines[8];
                treemap = AssetDatabase.LoadAssetAtPath(lines[9], typeof(Texture2D)) as Texture2D;
                treeMaxDensity = int.Parse(lines[10]);
                normalDirection = (normalDir) (int.Parse(lines[11]));
                keepPos = (lines[12] == "True") ? true : false;
                infoMessage = "Configuration loaded.";
                return true;
            }
            catch
            {
                errorMessage = "Error trying to load terrain configuration.";
                return false;
            }
        }

        bool SaveTerrainConfig(int terrainIdx, out string errorMessage, out string warningMessage, out string infoMessage)
        {
            errorMessage = "";
            warningMessage = "";
            infoMessage = "";
            string savePath = "Assets/SquareTileTerrain/Configs/config" + terrainIdx + ".txt";

            try
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                var sr = File.CreateText(savePath);

                sr.WriteLine(autofillPrefabPath);
                sr.WriteLine(autofillRulePath);
                sr.WriteLine(tileNotFoundPrefabPath);
                sr.WriteLine(tileSizeX);
                sr.WriteLine(tileSizeZ);
                sr.WriteLine(AssetDatabase.GetAssetPath(heightmap));
                sr.WriteLine(heightDelta);
                sr.WriteLine(AssetDatabase.GetAssetPath(tilemap));
                sr.WriteLine(treePath);
                sr.WriteLine(AssetDatabase.GetAssetPath(treemap));
                sr.WriteLine(treeMaxDensity.ToString());
                sr.WriteLine(((int)normalDirection).ToString());
                sr.WriteLine(keepPos);
                sr.Close();

                if(AssetDatabase.GetAssetPath(heightmap) == "" || AssetDatabase.GetAssetPath(tilemap) == "" || AssetDatabase.GetAssetPath(treemap) == "")
                    warningMessage = "Getting Tilemap/Heightmap/Treemap path returned an empty string. Check AssetDatabase.GetAssetPath() method behaviour. Configuration has been saved anyway.";

                infoMessage = "Configuration saved to " + savePath + ".";

                return true;
            }
            catch
            {
                errorMessage = "Error trying to save terrain configuration.";
                return false;
            }
        }
    }
}
#endif
