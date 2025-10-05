using System;
using System.Collections;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Gameplay.Characters;
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
    public class Base : Character
    {
        public override FactionType faction { get; set; } = FactionType.Player;
        
        [Inject]
        private Camera _mainCamera;

        [Inject]
        private IBaseService _bases;
        
        [Inject]
        private TransientLight.Pool _explosionVFXPool;

        private BlackboardReference _blackboard;

        protected override void Awake()
        {
            base.Awake();
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

        //TODO:Peut-être modifier pour attaquer les ennemis dans le range plutot que d'attendre d'être attaqué
        public override void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            base.OnHit(ctx, callback);
            
            if (ctx.Instigator.Health.IsDead) return;
            _blackboard.SetVariableValue("Target", ctx.Instigator.GameObject);
            if (_deaggroCoroutine != null)
            {
                StopCoroutine(_deaggroCoroutine);
            }
            _deaggroCoroutine = StartCoroutine(DeaggroAfterDelay());
        }
        
        private Coroutine _deaggroCoroutine;
        
        private IEnumerator DeaggroAfterDelay()
        {
            yield return new WaitForSeconds(1.0f);
            _blackboard.GetVariableValue<GameObject>("Target", out var target);
            
            if (target && target.TryGetComponent(out Character character))
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