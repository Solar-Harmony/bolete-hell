#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dreamteck.Splines;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.SDF
{
    // Editor-only utilities
    public partial class SDFGenerator
    {
        private const string BasePath = "Assets/SDFTextureCache";
        
        // TODO: onRebuild is called too often. Should add divergence in Dreamteck to expose a better event for
        //       spline edits that actually change the mesh.
        private void AutoRefreshOnSplineEdit()
        {
            SplineComputer splineComputer = GetComponent<SplineComputer>();
            if (!splineComputer)
                return;
            
            splineComputer.onRebuild -= GenerateSDF;
            splineComputer.onRebuild += GenerateSDF;
        }

        private void Update()
        {
            if (Application.isPlaying)
                return;
            
            ApplyToMaterial();
        }

        public async void GenerateSDF()
        {
            try
            {
                Mesh mesh = GetComponent<MeshFilter>()?.sharedMesh;
                if (!mesh)
                {
                    Debug.LogError("No MeshFilter found or no mesh assigned.");
                    return;
                }
                
                if (baseResolution <= 0)
                {
                    Debug.LogError("Base resolution must be greater than zero.");
                    return;
                }

                Texture2D sdf = await SDFGeneratorImpl.GenerateSDF(mesh, baseResolution, padding, blurRadius, useSubsampling);
                if (!sdf)
                {
                    Debug.LogError("Failed to generate SDF texture.");
                    return;
                }
            
                StartCoroutine(DeferSaveAsset(sdf));
                sdfTexture = sdf; 
                ApplyToMaterial();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error generating SDF texture: {ex.Message}");
            }
        }
        
        private IEnumerator DeferSaveAsset(Texture2D texture)
        {
            yield return null; // Wait for the next frame
            SaveAsset(texture);
        }         

        private void SaveAsset(Texture2D texture)
        {
            GlobalObjectId id = GlobalObjectId.GetGlobalObjectIdSlow(this);
            texture.name = $"{id.ToString()}";

            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
            
            string path = Path.Combine(BasePath, $"{texture.name}.asset");
                
            AssetDatabase.CreateAsset(texture, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CleanupUnusedTextures()
        {
            string[] assetPaths = AssetDatabase.FindAssets("t:Texture2D", new[] { BasePath });
            foreach (string assetPath in assetPaths)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetPath);
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (!GlobalObjectId.TryParse(fileName, out var objectId))
                {
                    AssetDatabase.DeleteAsset(path);
                    continue;
                }
                
                Object obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(objectId);
                if (!obj)
                {
                    AssetDatabase.DeleteAsset(path);
                    continue;
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif