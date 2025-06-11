using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Progress = Pathfinding.Progress;

namespace BoleteHell.Utils.Editor
{
    [InitializeOnLoad]
    public static class ScanNavmesh
    {
        static ScanNavmesh()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();

            Rect rect = new Rect(48, 10, 110, 30);
            if (GUI.Button(rect, "Rescan navmesh"))
            {
                AstarPath.FindAstarPath();
                AstarPath.active.Scan();
            }

            Handles.EndGUI();
        }
    }

}