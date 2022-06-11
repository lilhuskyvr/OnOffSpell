using System.Collections;
using IngameDebugConsole;
using ThunderRoad;

// ReSharper disable UnusedMember.Global

namespace OnOffSpell
{
    public class OnOffSpellTestingLevelModule : LevelModule
    {
        public override IEnumerator OnLoadCoroutine()
        {
            DebugLogConsole.AddCommandInstance("onoff_creatures",
                "On Off Creatures", "OnOffCreatures",
                this);


            return base.OnLoadCoroutine();
        }

        private void OnOffCreatures()
        {
            foreach (var creature in Creature.allActive)
            {
                if (!creature.isPlayer)
                {
                    GameManager.local.gameObject.GetComponent<OnOffSpellController>().OnOffCreature(creature);
                }
            }
        }
    }
}