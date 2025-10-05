using System;
using System.Collections;
using BoleteHell.Code.Arsenal.HitHandler;
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
    //Changé pour herité de character car c'est pratiquement un character juste avec un movement speed de 0
    //Et ca permet d'utiliser Character comme instigateur dans les tir ce qui facilite grandement l'accès aux informations nécéssaire
    //Fait que les bases  sont affecter par les éffets de tir ce qu'on ne veut peut-être pas (Sortir IStatusEffectTarget de character et le mettre dans Enemy+Player)
    public class Base : Character.Character
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
            _blackboard.SetVariableValue("Target", ctx.Instigator.gameObject);
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