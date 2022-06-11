using System;
using ThunderRoad;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable ParameterHidesMember

namespace OnOffSpell
{
    public class SpellOnOff : SpellCastProjectile
    {
        protected override void OnProjectileCollision(ItemMagicProjectile projectile,
            CollisionInstance collisionInstance)
        {
            try
            {
                var targetCreature =
                    collisionInstance.targetCollider.attachedRigidbody.GetComponentInParent<Creature>();

                if (targetCreature != null)
                {
                    if (!targetCreature.isPlayer
                        && targetCreature.factionId != 2
                    )
                        GameManager.local.GetComponent<OnOffSpellController>().OnOffCreature(targetCreature);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        public override bool OnImbueCollisionStart(CollisionInstance collisionInstance)
        {
            try
            {
                var targetCreature =
                    collisionInstance.targetCollider.attachedRigidbody.GetComponentInParent<Creature>();

                if (targetCreature != null)
                {
                    if (!targetCreature.isPlayer
                        && targetCreature.factionId != 2
                    )
                        GameManager.local.GetComponent<OnOffSpellController>().OnOffCreature(targetCreature);
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return base.OnImbueCollisionStart(collisionInstance);
        }
    }
}