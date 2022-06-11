using ThunderRoad;

namespace OnOffSpell
{
    // ReSharper disable once UnusedType.Global
    public class SpellOnOffMerge : SpellMergeData
    {
        public override void Merge(bool active)
        {
            base.Merge(active);

            if (active || currentCharge < 1.0f)
                return;

            foreach (var creature in Creature.allActive)
            {
                if (
                    !creature.isPlayer
                    && creature.factionId != 2
                    && !creature.data.id.Contains("Angel")
                    && !creature.data.id.Contains("Shadow")
                )
                {
                    GameManager.local.GetComponent<OnOffSpellController>().OnOffCreature(creature);
                }
            }

            currentCharge = 0;
        }
    }
}