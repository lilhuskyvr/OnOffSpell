using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable ParameterHidesMember

namespace OnOffSpell
{
    public class SpellOnOff : SpellCastProjectile
    {
        protected override void OnProjectileCollision(CollisionInstance collisionInstance)
        {
            try
            {
                var targetCreature =
                    collisionInstance.targetCollider.attachedRigidbody.GetComponentInParent<Creature>();

                if (targetCreature != null)
                {
                    if (!targetCreature.isPlayer
                        && targetCreature.factionId != 2
                        && !targetCreature.data.id.Contains("Angel")
                        && !targetCreature.data.id.Contains("Shadow")
                    )
                        GameManager.local.GetComponent<OnOffSpellController>().OnOffCreature(targetCreature);
                }
            }
            catch (Exception)
            {
                
            }
        }

        public override void OnImbueCollisionStart(CollisionInstance collisionInstance)
        {
            try
            {
                var targetCreature =
                    collisionInstance.targetCollider.attachedRigidbody.GetComponentInParent<Creature>();

                if (targetCreature != null)
                {
                    if (!targetCreature.isPlayer
                        && targetCreature.factionId != 2
                        && !targetCreature.data.id.Contains("Angel")
                        && !targetCreature.data.id.Contains("Shadow")
                    )
                        GameManager.local.GetComponent<OnOffSpellController>().OnOffCreature(targetCreature);
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}