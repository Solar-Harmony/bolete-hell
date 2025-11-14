using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace BoleteHell.Code.Utils
{
    public class HideComponents : MonoBehaviour
    {
        public enum VisibilityMode
        {
            Show,
            ShowInPrefabEditor,
            Hide
        }

        [Serializable]
        public class ComponentVisibility
        {
            [SerializeField]
            [ReadOnly, HideLabel, HorizontalGroup("Row", Width = 0.65f)]
            private Component component;

            [SerializeField]
            [HideLabel, HorizontalGroup("Row", Width = 0.35f)]
            [DrawWithUnity]
            public VisibilityMode Visibility;

            public Component Component => component;

            public ComponentVisibility(Component component)
            {
                this.component = component;
                Visibility = VisibilityMode.Show;
            }

            public bool IsValid() => component;
        }

        [SerializeField, ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true, ShowFoldout = false)]
        private List<ComponentVisibility> componentVisibilities = new();

        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.update += UpdateComponentVisibility;
            EditorApplication.delayCall += UpdateComponentVisibility;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= UpdateComponentVisibility;
            ResetAllComponentVisibility();
#endif
        }

        private void OnValidate()
        {
            RefreshComponentList();
#if UNITY_EDITOR
            EditorApplication.delayCall += UpdateComponentVisibility;
#endif
        }

        private void RefreshComponentList()
        {
            Component[] allComponents = GetComponents<Component>();

            List<ComponentVisibility> newList = new();

            foreach (Component comp in allComponents)
            {
                if (!comp || comp is HideComponents)
                    continue;

                ComponentVisibility existing = componentVisibilities.FirstOrDefault(cv => cv.IsValid() && cv.Component == comp);

                newList.Add(existing ?? new ComponentVisibility(comp));
            }

            componentVisibilities = newList;
        }

#if UNITY_EDITOR
        private void UpdateComponentVisibility()
        {
            if (!this || !gameObject)
                return;

            bool isInPrefabStage = false;

            try
            {
                var stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null && stage.IsPartOfPrefabContents(gameObject))
                {
                    isInPrefabStage = true;
                }
            }
            catch (InvalidOperationException)
            {
                return;
            }

            foreach (ComponentVisibility cv in componentVisibilities)
            {
                if (!cv.IsValid())
                    continue;

                Component component = cv.Component;
                if (!component)
                    continue;

                switch (cv.Visibility)
                {
                    case VisibilityMode.Show:
                        component.hideFlags &= ~HideFlags.HideInInspector;
                        break;

                    case VisibilityMode.ShowInPrefabEditor:
                        if (isInPrefabStage)
                            component.hideFlags &= ~HideFlags.HideInInspector;
                        else
                            component.hideFlags |= HideFlags.HideInInspector;
                        break;

                    case VisibilityMode.Hide:
                        component.hideFlags |= HideFlags.HideInInspector;
                        break;
                }
            }

            EditorUtility.SetDirty(gameObject);
        }

        private void ResetAllComponentVisibility()
        {
            foreach (ComponentVisibility cv in componentVisibilities)
            {
                if (!cv.IsValid())
                    continue;

                Component component = cv.Component;
                if (component)
                {
                    component.hideFlags &= ~HideFlags.HideInInspector;
                }
            }
        }
#endif
    }
}