using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Editor
{
    public class StatusEffectDebugger : EditorWindow, IRequestManualInject
    {
        [Inject]
        private IStatusEffectService _statusEffectService;
        
        bool IRequestManualInject.IsInjected { get; set; }
        
        // UI State
        private Vector2 _scrollPosition;
        private string _targetFilter = "";
        private string _effectFilter = "";
        private SortColumn _sortColumn = SortColumn.EffectName;
        private bool _sortDescending = false;
        
        // Column widths
        private readonly float _effectNameWidth = 150f;
        private readonly float _targetWidth = 120f;
        private readonly float _durationWidth = 80f;
        private readonly float _ticksWidth = 60f;
        private readonly float _timeRemainingWidth = 100f;
        private readonly float _transientWidth = 70f;
        private readonly float _statusWidth = 80f;
        
        private enum SortColumn
        {
            EffectName,
            Target,
            Duration,
            TicksLeft,
            TimeRemaining,
            IsTransient
        }
        
        [MenuItem("Window/Bolete Hell/Status Effects")]
        public static void ShowWindow()
        {
            var window = GetWindow<StatusEffectDebugger>("Bolete Hell Status Effects");
            ((IRequestManualInject)window).InjectDependencies();
            
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                {
                    ((IRequestManualInject)window).InjectDependencies();
                }
            };
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Bolete Hell Status Effect Debugger is only available during play mode.", MessageType.Info);
                return;
            }

            if (_statusEffectService == null)
            {
                ((IRequestManualInject)this).InjectDependencies();
            }
            
            DrawFilters();
            EditorGUILayout.Space();
            DrawHeader();
            DrawStatusEffectsList();

            Repaint();
        }
        
        private void DrawFilters()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Filters:", GUILayout.Width(50f));

            EditorGUI.BeginChangeCheck();
            _effectFilter = EditorGUILayout.TextField("Effect:", _effectFilter, GUILayout.Width(200f));
            _targetFilter = EditorGUILayout.TextField("Target:", _targetFilter, GUILayout.Width(200f));

            if (GUILayout.Button("Clear", GUILayout.Width(60f)))
            {
                _effectFilter = "";
                _targetFilter = "";
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            DrawSortableHeader("Effect", SortColumn.EffectName, _effectNameWidth);
            DrawSortableHeader("Target", SortColumn.Target, _targetWidth);
            DrawSortableHeader("Duration", SortColumn.Duration, _durationWidth);
            DrawSortableHeader("Ticks", SortColumn.TicksLeft, _ticksWidth);
            DrawSortableHeader("Time Left", SortColumn.TimeRemaining, _timeRemainingWidth);
            DrawSortableHeader("Transient", SortColumn.IsTransient, _transientWidth);
            DrawSortableHeader("Status", SortColumn.EffectName, _statusWidth);
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawSortableHeader(string label, SortColumn column, float width)
        {
            var content = new GUIContent(label);
            if (_sortColumn == column)
            {
                content.text += _sortDescending ? " ▼" : " ▲";
            }
            
            if (GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Width(width)))
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
        
        private void DrawStatusEffectsList()
        {
            var activeEffects = _statusEffectService.GetActiveStatusEffects();
            var filteredEffects = FilterEffects(activeEffects);
            var sortedEffects = SortEffects(filteredEffects).ToList();
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            EditorGUILayout.LabelField($"{sortedEffects.Count} effects", EditorStyles.boldLabel);
            
            if (!sortedEffects.Any())
            {
                EditorGUILayout.HelpBox("No active status effects.", MessageType.Info);
            }
            else
            {
                foreach (var effect in sortedEffects)
                {
                    DrawStatusEffectRow(effect);
                }
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawStatusEffectRow(StatusEffectInstance effect)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Effect Name
            var effectNameStyle = effect.IsExpired ? EditorStyles.label : EditorStyles.boldLabel;
            EditorGUILayout.LabelField(effect.EffectName, effectNameStyle, GUILayout.Width(_effectNameWidth));
            
            // Target
            EditorGUILayout.LabelField(effect.TargetName, GUILayout.Width(_targetWidth));
            
            // Duration
            EditorGUILayout.LabelField($"{effect.config.duration:F1}s", GUILayout.Width(_durationWidth));
            
            // Ticks Left
            if (effect.TicksLeft <= 1)
            {
                var originalColor = GUI.color;
                GUI.color = Color.red;
                EditorGUILayout.LabelField(effect.TicksLeft.ToString(), GUILayout.Width(_ticksWidth));
                GUI.color = originalColor;
            }
            else
            {
                EditorGUILayout.LabelField(effect.TicksLeft.ToString(), GUILayout.Width(_ticksWidth));
            }
            
            // Time Remaining
            var timeRemaining = effect.TimeRemaining;
            var timeColor = timeRemaining < 1f ? Color.red : (timeRemaining < 5f ? Color.yellow : Color.white);
            var originalTextColor = GUI.color;
            GUI.color = timeColor;
            EditorGUILayout.LabelField($"{timeRemaining:F1}s", GUILayout.Width(_timeRemainingWidth));
            GUI.color = originalTextColor;
            
            // Transient
            EditorGUILayout.LabelField(effect.config.isTransient ? "Yes" : "No", GUILayout.Width(_transientWidth));
            
            // Status
            var status = effect.IsExpired ? "Expired" : "Active";
            var statusColor = effect.IsExpired ? Color.gray : Color.green;
            GUI.color = statusColor;
            EditorGUILayout.LabelField(status, GUILayout.Width(_statusWidth));
            GUI.color = originalTextColor;
            
            EditorGUILayout.EndHorizontal();
        }
        
        private IEnumerable<StatusEffectInstance> FilterEffects(IEnumerable<StatusEffectInstance> effects)
        {
            return effects.Where(effect =>
            {
                var effectNameMatch = string.IsNullOrEmpty(_effectFilter) || 
                                    effect.EffectName.IndexOf(_effectFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                
                var targetNameMatch = string.IsNullOrEmpty(_targetFilter) || 
                                    effect.TargetName.IndexOf(_targetFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                
                return effectNameMatch && targetNameMatch;
            });
        }
        
        private IEnumerable<StatusEffectInstance> SortEffects(IEnumerable<StatusEffectInstance> effects)
        {
            var query = _sortColumn switch
            {
                SortColumn.EffectName => _sortDescending 
                    ? effects.OrderByDescending(e => e.EffectName)
                    : effects.OrderBy(e => e.EffectName),
                SortColumn.Target => _sortDescending 
                    ? effects.OrderByDescending(e => e.TargetName)
                    : effects.OrderBy(e => e.TargetName),
                SortColumn.Duration => _sortDescending 
                    ? effects.OrderByDescending(e => e.config.duration)
                    : effects.OrderBy(e => e.config.duration),
                SortColumn.TicksLeft => _sortDescending 
                    ? effects.OrderByDescending(e => e.TicksLeft)
                    : effects.OrderBy(e => e.TicksLeft),
                SortColumn.TimeRemaining => _sortDescending 
                    ? effects.OrderByDescending(e => e.TimeRemaining)
                    : effects.OrderBy(e => e.TimeRemaining),
                SortColumn.IsTransient => _sortDescending 
                    ? effects.OrderByDescending(e => e.config.isTransient)
                    : effects.OrderBy(e => e.config.isTransient),
                _ => effects.OrderBy(e => e.EffectName)
            };
            
            return query;
        }

    }
}