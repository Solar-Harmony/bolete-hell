using System;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Character;
using BoleteHell.Code.Gameplay.Damage;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Base
{
    [RequireComponent(typeof(Renderer))]
    public class Base : MonoBehaviour, ITargetable, ISceneObject
    {
        public Vector2 Position => transform.position;

        [SerializeField]
        public Health health;
        Health IDamageable.Health => health;
        
        [Inject]
        private Camera _mainCamera;

        [Inject]
        private IBaseService _bases;

        private void Awake()
        {
            health.OnDeath += () =>
            {
                ShowDeathVFX();
                _bases.NotifyBaseDied(this);
            };
        }
        
        public Sprite deathSprite;

        private void ShowDeathVFX()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                spriteRenderer.sprite = deathSprite;
            }
            
            var showOnDeath = transform.Find("ShowOnDeath");
            if (showOnDeath)
            {
                showOnDeath.gameObject.SetActive(true);
            }
        }

        public void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            if (ctx.Data is not LaserCombo laser)
                return;
        
            laser.CombinedEffect(ctx.Position, this);
            callback?.Invoke(new ITargetable.Response(ctx));
        }
        
        private void OnGUI()
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y + GetComponent<Renderer>().bounds.size.y * 0.5f);
            Vector2 ss = _mainCamera.WorldToScreenPoint(position);
            ss.y = Screen.height - ss.y;
            Rect rect = new(ss, new Vector2(100, 50));
            GUI.skin.label.fontSize = 24;
            GUI.Label(rect, health.CurrentHealth + "hp");
        }
    }
}