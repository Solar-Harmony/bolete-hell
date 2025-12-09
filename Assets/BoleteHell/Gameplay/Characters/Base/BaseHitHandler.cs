using System;
using System.Collections;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Utils.Advisor;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters.Base
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    [RequireComponent(typeof(FactionComponent))]
    [RequireComponent(typeof(HealthComponent))] 
    public class BaseHitHandler : HitHandlerComponent
    {
        public Sprite DeathSprite;

        [Inject]
        private IEntityRegistry _entities;

        [Inject]
        private TutorialPopup _tutorial;
        
        [Inject]
        private Tutorial.Speakers _speakers;
        
        private BlackboardReference _blackboard;
        private Coroutine _deaggroCoroutine;

        protected override void Awake()
        {
            base.Awake();
            _blackboard = GetComponent<BehaviorGraphAgent>().BlackboardReference;
        }

        public override void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            base.OnHit(ctx, callback);

            if (_health.Percent < 0.9f)
            {   
                _tutorial.Show(new(_speakers.BaseAdvisor, "Here they come! Defend the base！", PreventDuplicates: true));
            }
            
            if (_health.Percent < 0.25f)
            {
                _tutorial.Show(new(_speakers.BaseAdvisor, "We cannot hold for much longer, commander!", PreventDuplicates: true));
            }
            
            if (_health.IsDead)
            {
                ShowDeathVFX();
                return;
            }

            if (ctx.Instigator)
            {
                // player cannot aggro the base
                if (_entities.GetPlayer() == ctx.Instigator)
                    return;
                
                var thisFaction = GetComponent<FactionComponent>();
                
                var instigatorFaction = ctx.Instigator.GetComponent<FactionComponent>();
                if (thisFaction.IsAffected(ctx.Projectile.AffectedSide, instigatorFaction))
                    return;
                
                var instigatorHealth = ctx.Instigator.GetComponent<HealthComponent>();
                if (instigatorHealth.IsDead) 
                    return;
                
                _blackboard.SetVariableValue("Target", ctx.Instigator);
            }
            
            if (_deaggroCoroutine != null)
            {
                StopCoroutine(_deaggroCoroutine);
            }
            _deaggroCoroutine = StartCoroutine(DeaggroAfterDelay());
        }
        
        private IEnumerator DeaggroAfterDelay()
        {
            yield return new WaitForSeconds(1.0f);
            _blackboard.GetVariableValue<GameObject>("Target", out var target);
            
            if (target && target.TryGetComponent(out HealthComponent health))
            {
                if (health.IsDead)
                {
                    _blackboard.SetVariableValue<GameObject>("Target", null);
                }
            }
            else
            {
                yield return null;
            }
        }
        
        private void ShowDeathVFX()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                spriteRenderer.sprite = DeathSprite;
            }
            
            var showOnDeath = transform.Find("ShowOnDeath");
            if (showOnDeath)
            {
                showOnDeath.gameObject.SetActive(true);
            }
        }
    }
}