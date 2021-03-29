using System.Collections;
using IngameDebugConsole;
using ThunderRoad;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace OnOffSpell
{
    public class OnOffSpellTestingLevelModule : LevelModule
    {
        public override IEnumerator OnLoadCoroutine(Level level)
        {
            DebugLogConsole.AddCommandInstance("onoff_creatures",
                "On Off Creatures", "OnOffCreatures",
                this);


            return base.OnLoadCoroutine(level);
        }

        private void OnOffCreatures()
        {
            foreach (var creature in Creature.list)
            {
                if (!creature.isPlayer)
                {
                    GameManager.local.gameObject.GetComponent<OnOffSpellController>().OnOffCreature(creature);
                }
            }
        }
    }
}