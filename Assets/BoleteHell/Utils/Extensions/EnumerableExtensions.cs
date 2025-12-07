using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace BoleteHell.Utils.Extensions
{
    public static class EnumerableDistanceExtensions
    {
        [CanBeNull]
        public static T TakeClosestTo<T>(this IEnumerable<T> list, Func<T, Vector2> positionGetter, Vector2 position, out float distance) where T : class
        {
            var closestBase = list
                .Select(e => new { Object = e, Distance = Vector2.Distance(positionGetter(e), position) })
                .OrderBy(b => b.Distance)
                .FirstOrDefault();
            
            if (closestBase == null)
            {
                distance = float.MaxValue;
                return null;
            }
            
            distance = closestBase.Distance;
            return closestBase.Object;
        }
        
        [CanBeNull]
        public static GameObject TakeClosestTo(this IEnumerable<GameObject> list, Vector2 position, out float distance)
        {
            return TakeClosestTo(list, go => go.transform.position, position, out distance);
        }
        
        [CanBeNull]
        public static GameObject TakeClosestTo(this IEnumerable<GameObject> list, GameObject obj, out float distance)
        {
            return TakeClosestTo(list, go => go.transform.position, obj.transform.position, out distance);
        }
    }
    
    /// <summary>
    /// Homemade MinBy / MaxBy extensions for IEnumerable (only available in newer c# rip)
    /// </summary>
    public static class EnumerableMinMaxExtensions
    {
        public static TSource WithHighest<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) 
        {
            return ByImpl(source, keySelector, isMax: false);
        }
        
        public static TSource WithLowest<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) 
        {
            return ByImpl(source, keySelector, isMax: true);
        }
        
        // these overloads accept a Component in the selector lambda so you get rid of GetComponent boilerplate
        // e.g. .TakeBest((HealthComponent h) => h.CurrentHealth)
        public static GameObject WithHighest<TKey, TComponent>(
            this IEnumerable<GameObject> source,
            Func<TComponent, TKey> keySelector
        ) where TComponent : Component
        {
            return ByImplGameObject(source, keySelector, isMax: false);
        }

        public static GameObject WithLowest<TKey, TComponent>(
            this IEnumerable<GameObject> source,
            Func<TComponent, TKey> keySelector
        ) where TComponent : Component
        {
            return ByImplGameObject(source, keySelector, isMax: true);
        }
        
        private static GameObject ByImplGameObject<TKey, TComponent>(
            this IEnumerable<GameObject> source,
            Func<TComponent, TKey> keySelector,
            bool isMax
        ) where TComponent : Component
        {
            return ByImpl(source, Selector, isMax: isMax);
            TKey Selector(GameObject obj) => keySelector(obj?.GetComponent<TComponent>() ?? throw new InvalidOperationException($"GameObject {obj?.name} does not have component of type {typeof(TComponent)}"));
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private static TSource ByImpl<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool isMax)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            
            var list = source.ToList();
            if (list.Count == 0)
                throw new InvalidOperationException("Sequence contains no elements");

            var comparer = Comparer<TKey>.Default;
            TSource bestItem = list[0];
            TKey bestKey = keySelector(bestItem);

            for (int i = 1; i < list.Count; i++)
            {
                TSource item = list[i];
                TKey key = keySelector(item);
                int comparison = comparer.Compare(key, bestKey);
                
                bool isBetter = isMax ? comparison > 0 : comparison < 0;
                if (!isBetter) 
                    continue;
                
                bestItem = item;
                bestKey = key;
            }

            return bestItem;
        }
    }
}