using System;
using System.Collections;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Character;
using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Graphics;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Base
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(BehaviorGraphAgent))]
    [RequireComponent(typeof(Health))]
    public class Base : MonoBehaviour, ITargetable, ISceneObject
    {
        public Vector2 Position => transform.position;
        
        public Health Health { get; private set; }
        
        [Inject]
        private Camera _mainCamera;

        [Inject]
        private IBaseService _bases;
        
        [Inject]
        private TransientLight.Pool _explosionVFXPool;

        private BlackboardReference _blackboard;

        private void Awake()
        {
            Health = GetComponent<Health>();
            Health.OnDeath += () =>
            {
                ShowDeathVFX();
                _bases.NotifyBaseDied(this);
                GetComponent<BehaviorGraphAgent>().enabled = false;
            };
            _blackboard = GetComponent<BehaviorGraphAgent>().BlackboardReference;
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
        
            laser.CombinedEffect(ctx.Position, this, ctx.Projectile);
            callback?.Invoke(new ITargetable.Response(ctx) { RequestDestroyProjectile = true });

            if (ctx.Instigator)
            {
                _blackboard.SetVariableValue<GameObject>("Target", ctx.Instigator);
                if (_deaggroCoroutine != null)
                {
                    StopCoroutine(_deaggroCoroutine);
                }
                _deaggroCoroutine = StartCoroutine(DeaggroAfterDelay());
            }
            
            _explosionVFXPool.Spawn(ctx.Position, 0.5f, 0.1f);
        }
        
        private Coroutine _deaggroCoroutine;
        
        private IEnumerator DeaggroAfterDelay()
        {
            yield return new WaitForSeconds(1.0f);
            _blackboard.GetVariableValue<GameObject>("Target", out var target);
            
            if (target && target.TryGetComponent(out Character.Character character))
            {
                if (character.Health.IsDead)
                {
                    _blackboard.SetVariableValue<GameObject>("Target", null);
                }
            }
            else
            {
                yield return null;
            }
        }
        
        private void OnGUI()
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y + GetComponent<Renderer>().bounds.size.y * 0.5f);
            Vector2 ss = _mainCamera.WorldToScreenPoint(position);
            ss.y = Screen.height - ss.y;
            Rect rect = new(ss, new Vector2(100, 50));
            GUI.skin.label.fontSize = 24;
            GUI.Label(rect, Health.CurrentHealth + "hp");
        }
    }
}