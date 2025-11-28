using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace BoleteHell.Utils.Extensions
{
    public static class ListDistanceExtensions
    {
        [CanBeNull]
        public static T FindClosestTo<T>(this List<T> list, Func<T, Vector2> positionGetter, Vector2 position, out float distance) where T : class
        {
            if (list.Count == 0)
            {
                distance = float.MaxValue;
                return null;
            }

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
    }
}