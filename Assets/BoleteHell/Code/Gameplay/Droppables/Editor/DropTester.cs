using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Droppables.Editor
{
    //Prefab
    // Pour tester des valeurs et trouver la curve qui donne une dispersion aproprié
    public class DropTester : MonoBehaviour
    {
        [SerializeReference]
        public DropRangeContext rangeCtx;

        [SerializeField]
        public int numberOfLoops;
    }

    [CustomEditor(typeof(DropTester))]
    public class DropTesterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DropTester tester = (DropTester)target;

            if (GUILayout.Button("Run Drop Test"))
            {
                ClearConsole();
                RunTest(tester);
            }
        }

        private void RunTest(DropTester tester)
        {
            int[] buckets = new int[5];
            for (int i = 0; i < tester.numberOfLoops; i++)
            {
                float value = tester.rangeCtx.GetValueInRange();
                int bucket = Mathf.Clamp(Mathf.FloorToInt((value - tester.rangeCtx.min) / (tester.rangeCtx.max - tester.rangeCtx.min) * buckets.Length), 0, buckets.Length - 1);
                buckets[bucket]++;
            }

            Debug.Log("Value Drop Distribution:");
            for (int i = 0; i < buckets.Length; i++)
            {
                float rangeStart = tester.rangeCtx.min + i * (tester.rangeCtx.max -tester.rangeCtx.min) / buckets.Length;
                float rangeEnd = tester.rangeCtx.min + (i + 1) * (tester.rangeCtx.max - tester.rangeCtx.min) / buckets.Length;
                Debug.Log($"Value {rangeStart:F1}–{rangeEnd:F1}: {buckets[i]} ({(buckets[i] / (float)tester.numberOfLoops) * 100} %) drops");
            }
        }
    
        private static void ClearConsole()
        {
            var logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var clearMethod = logEntries?.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearMethod?.Invoke(null, null);
        }

    }
}