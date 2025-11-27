using System;
using BoleteHell.Code.Arsenal.Rays;
using BoleteHell.Code.Gameplay.Characters;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.HitHandler
{
    public interface ITargetable
    {
        public record Context
        (
            GameObject HitObject,
            IInstigator Instigator,
            LaserInstance Projectile,
            RaycastHit2D RayHit,
            Vector2 Direction,
            IProjectileData Data
        );

        public record Response(
            RaycastHit2D Position, 
            Vector2 Direction,
            //Pour les laserbeams, permet d'arreter le check des autres choses dans le raycast,
            //pour les projectiles, rien pour l'instant, pourrais arreter le mouvement du projectile, donc un mur pourrait récolter tout les projectile en les
            //pourrait peut-être avoir une matrice qui détermine comment le ITargetable interagi avec les types de lasers
            bool BlockProjectile,
            bool RequestDestroyProjectile
        )
        {
            public Response(Context ctx) : this(ctx.RayHit, ctx.Direction, false, false) {}
        }

        public void OnHit(Context ctx, Action<Response> callback = null);
    }
}