using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Editor
{
    // TODO: Marchera plus pour l'instant mais honnetement on s'en fout un peu lol
    public class StatusEffectDebugger : EditorWindow
    {
        private IStatusEffectService _statusEffectService;
        
        private Vector2 _scrollPosition;
        private string _targetFilter = "";
        private string _effectFilter = "";
        private bool _sortDescending = false;
        
        private readonly List<Column> _columns = new();
        private Column _sortColumn;
        
        private class Column
        {
            public Column(string name, float width, Func<StatusEffectInstance, string> valueSelector, [CanBeNull] Func<StatusEffectInstance, object> sortKeySelector = null)
            {
                Name = name;
                Width = width;
                ValueSelector = valueSelector;
                SortKeySelector = sortKeySelector ?? valueSelector;
            }

            public string Name { get; }
            public float Width { get; }
            public Func<StatusEffectInstance, string> ValueSelector { get; }
            public Func<StatusEffectInstance, object> SortKeySelector { get; }
        }
        
        private int _filteredEffectsCount = 0;
        
        [MenuItem("Window/Bolete Hell/Status Effects")]
        public static void ShowWindow()
        {
            var window = GetWindow<StatusEffectDebugger>("Bolete Hell Status Effects");
        }
        
        private void RegisterColumns(out Column sortColumn)
        {
            const float effectNameWidth = 150f;
            const float targetWidth = 120f;
            const float durationWidth = 80f;
            const float ticksWidth = 60f;
            const float timeRemainingWidth = 100f;
            const float transientWidth = 70f;
            
            _columns.Add(new Column("Effect",    effectNameWidth,    e => e.EffectName));
            _columns.Add(new Column("Target",    targetWidth,        e => e.TargetName));
            _columns.Add(new Column("Duration",  durationWidth,      e => $"{e.config.duration:F1}s", e => e.config.duration));
            _columns.Add(new Column("Ticks",     ticksWidth,         e => e.TicksLeft.ToString(),     e => e.TicksLeft));
            _columns.Add(new Column("Time Left", timeRemainingWidth, e => $"{e.TimeRemaining:F1}s",   e => e.TimeRemaining));
            _columns.Add(new Column("Transient", transientWidth,     e => e.config.isTransient ? "Yes" : "No"));
            
            sortColumn = _columns[0];
        }

        private void CreateGUI()
        {
            ServiceLocator.Get(ref _statusEffectService);
            RegisterColumns(out _sortColumn);
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Bolete Hell Status Effect Debugger is only available during play mode.", MessageType.Info);
                return;
            }

            // If still null after injection attempt, show error
            if (_statusEffectService == null)
            {
                EditorGUILayout.HelpBox("Status Effect Service is not available. Make sure the game is running and dependencies are properly configured.", MessageType.Warning);
                return;
            }
            
            DrawFilters();
            EditorGUILayout.Space();
            DrawHeader();
            DrawListView();

            Repaint();
        }
        
        private void DrawFilters()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Filter by", EditorStyles.boldLabel, GUILayout.Width(50f));

            EditorGUI.BeginChangeCheck();
            _effectFilter = EditorGUILayout.TextField("name", _effectFilter);
            _targetFilter = EditorGUILayout.TextField("target", _targetFilter, GUILayout.MaxWidth(180f));
            Label($"-> {_filteredEffectsCount} effects");
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear", GUILayout.Width(60f)))
            {
                _effectFilter = "";
                _targetFilter = "";
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            foreach (var column in _columns)
            {
                var content = new GUIContent(column.Name);
                if (_sortColumn == column)
                {
                    content.text += _sortDescending ? " ▼" : " ▲";
                }
            
                if (GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Width(column.Width)))
                {
                    if (_sortColumn == column)
                    {
                        _sortDescending = !_sortDescending;
                    }
                    else
                    {
                        _sortColumn = column;
                        _sortDescending = false;
                    }
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawListView()
        {
            IReadOnlyCollection<StatusEffectInstance> activeEffects = _statusEffectService.GetActiveStatusEffects();
            IEnumerable<StatusEffectInstance> filteredEffects       = FilterEffects(activeEffects);
            List<StatusEffectInstance> sortedEffects                = SortEffects(filteredEffects).ToList();
            
            _filteredEffectsCount = sortedEffects.Count;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            if (sortedEffects.Any())
            {
                foreach (var effect in sortedEffects)
                {
                    DrawListViewRow(effect);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No active status effects.", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
        }
        
        private void DrawListViewRow(StatusEffectInstance effect)
        {
            EditorGUILayout.BeginHorizontal();

            foreach (var column in _columns)
            {
                EditorGUILayout.LabelField(column.ValueSelector(effect), GUILayout.Width(column.Width));
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private IEnumerable<StatusEffectInstance> FilterEffects(IEnumerable<StatusEffectInstance> effects)
        {
            return effects.Where(effect =>
            {
                bool effectNameMatch = string.IsNullOrEmpty(_effectFilter) || 
                                       effect.EffectName.IndexOf(_effectFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                
                bool targetNameMatch = string.IsNullOrEmpty(_targetFilter) || 
                                       effect.TargetName.IndexOf(_targetFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                
                return effectNameMatch && targetNameMatch;
            });
        }
        private static readonly GUIContent GC = new GUIContent();

        private static void Label(string text, float? fixedWidth = null, GUIStyle style = null)
        {
            style ??= EditorStyles.label;
            GC.text = text;
            Vector2 size = style.CalcSize(GC);
            float w = fixedWidth ?? size.x;
            Rect r = GUILayoutUtility.GetRect(w, size.y, style, GUILayout.Width(w));
            GUI.Label(r, GC, style);
        }
        
        private IEnumerable<StatusEffectInstance> SortEffects(IEnumerable<StatusEffectInstance> effects)
        {
           return _sortDescending
                ? effects.OrderByDescending(_sortColumn.SortKeySelector)
                : effects.OrderBy(_sortColumn.SortKeySelector);
        }
    }
}