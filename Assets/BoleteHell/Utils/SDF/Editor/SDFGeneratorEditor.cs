using System;
using UnityEditor;
using UnityEngine;
using Utils.SDF;

namespace BoleteHell.Utils.SDF.Editor
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
        public override void OnInspectorGUI()
        {
            var generator = (SDFGenerator)target;
            
            DrawDefaultInspector();

            if (GUILayout.Button("Generate SDF"))
            {
                generator.GenerateSDF();
            }
            
            if (GUILayout.Button("Cleanup unused SDF"))
            {
                SDFGenerator.CleanupUnusedTextures();
            }
        }
    }
}