using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Utils;

namespace BoleteHell.Utils.Editor
{
    /// <summary>
    /// Generates Signed Distance Field (SDF) textures for meshes in the editor.
    /// These textures are used in shaders to create effects like outline rendering.
    ///
    /// Q: Do I need to invoke this generator manually?
    /// A: No. It will automatically run whenever you edit a spline in the scene.
    /// 
    /// Q: Should I commit the SDFTextureCache folder?
    /// A: Yes. It contains generated textures that are used by the shaders at runtime. The folder should auto-cleanup
    ///    unused textures over time - if it doesn't, it's a bug lol
    /// </summary>
    [CustomEditor(typeof(SDFGenerator))]
    public class SDFGeneratorEditor : UnityEditor.Editor
    {
        private const string BasePath = "Assets/SDFTextureCache";

        private static readonly int SDFPropertyId = Shader.PropertyToID("_SDF");

        public override void OnInspectorGUI()
        {
            var generator = (SDFGenerator)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Generate SDF"))
            {
                GenerateSDF(generator);
            }
            
            if (GUILayout.Button("Cleanup unused SDF"))
            {
                CleanupUnusedTextures();
            }
        }

        private void GenerateSDF(SDFGenerator generator)
        {
            Mesh mesh = generator.gameObject.GetComponent<MeshFilter>()?.sharedMesh;
            if (mesh == null)
            {
                Debug.LogError("No MeshFilter found or no mesh assigned.");
                return;
            }
                
            if (generator.baseResolution <= 0)
            {
                Debug.LogError("Base resolution must be greater than zero.");
                return;
            }

            Texture2D sdfTexture = SDFGeneratorImpl.GenerateSDF(mesh, generator.baseResolution, generator.padding, generator.blurRadius, generator.useSubsampling);
            if (!sdfTexture)
            {
                Debug.LogError("Failed to generate SDF texture.");
                return;
            }
                
            SaveAsset(sdfTexture);
            AssignToMaterial(generator, sdfTexture);
        }

        private static void AssignToMaterial(SDFGenerator generator, Texture2D sdfTexture)
        {
            MeshRenderer renderer = generator.gameObject.GetComponent<MeshRenderer>();
            if (!renderer)
            {
                Debug.LogWarning("No MeshRenderer found.");
                return;
            }
                    
            // get existing material property block
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetTexture(SDFPropertyId, sdfTexture);
            renderer.SetPropertyBlock(propertyBlock);
        }

        private void SaveAsset(Texture2D texture)
        {
            var generator = (SDFGenerator)target;
            GlobalObjectId id = GlobalObjectId.GetGlobalObjectIdSlow(generator);
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
        
        /// <summary>
        /// Fixme: This is not super robust lol
        /// </summary>
        private void CleanupUnusedTextures()
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
                if (obj == null)
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