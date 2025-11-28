using System;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace BoleteHell.BoleteUtils
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AnglePickerAttribute : Attribute { }

    public class AnglePickerDrawer : OdinValueDrawer<Vector2>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            const float radius = 1f;

            Vector2 vec = ValueEntry.SmartValue;
            float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
            Rect rect = EditorGUILayout.GetControlRect(false, 80);
            Rect discRect = new Rect(rect.x, rect.y + 20, rect.width, 60);
            Vector2 center = new Vector2(discRect.x + discRect.width / 2f, discRect.y + discRect.height / 2f);
            float radiusPx = Mathf.Min(discRect.width, discRect.height) * 0.45f * radius;
            int id = GUIUtility.GetControlID("AnglePickerDrawner".GetHashCode(), FocusType.Passive);
            Event e = Event.current;
            Vector2 mouse = e.mousePosition;
            
            if (ShouldUseEvent(e, discRect, mouse, id))
            {
                Vector2 local = -(mouse - center);
                if (local.sqrMagnitude > 0.00001f)
                {
                    angle = Mathf.Atan2(local.y, local.x) * Mathf.Rad2Deg;
                    vec = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                }

                e.Use();
            }

            DrawDialControl(label, rect, discRect, center, radiusPx, angle);

            ValueEntry.SmartValue = vec;
        }
        
        private static bool ShouldUseEvent(Event e, Rect discRect, Vector2 mouse, int id)
        {
            switch (e.type)
            {
                case EventType.MouseDown when e.button == 0 && discRect.Contains(mouse):
                    GUIUtility.hotControl = id;
                    return true;
                case EventType.MouseDrag when GUIUtility.hotControl == id:
                    return true;
                case EventType.MouseUp when GUIUtility.hotControl == id:
                    GUIUtility.hotControl = 0;
                    return true;
                default:
                    return false;
            }
        }

        private static void DrawDialControl(GUIContent label, Rect rect, Rect discRect, Vector2 center, float radiusPx, float angle)
        {
            EditorGUI.LabelField(rect, label);
            EditorGUI.DrawRect(discRect, new Color(0f, 0f, 0f, 0.08f));
            Handles.BeginGUI();
            {
                // outline
                Handles.color = Color.gray;
                Handles.DrawWireDisc(center, Vector3.forward, radiusPx);

                // angle line
                Vector2 dir = -new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                Vector3 to = center + dir * radiusPx;
                Handles.color = Color.yellow;
                Handles.DrawLine(center, to);

                // knob
                float knobSize = 6f;
                Rect knobRect = new Rect(to.x - knobSize * 0.5f, to.y - knobSize * 0.5f, knobSize, knobSize);
                EditorGUI.DrawRect(knobRect, Color.white);
            }
            Handles.EndGUI();
        }
    }
}