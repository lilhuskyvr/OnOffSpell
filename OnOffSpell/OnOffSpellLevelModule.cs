﻿using System.Collections;
using System.Collections.Generic;
using ThunderRoad;

// ReSharper disable UnusedMember.Local

namespace OnOffSpell
{
    public class OnOffSpellLevelModule : LevelModule
    {
        public List<ChanceOfStrip> chanceOfStrips = new List<ChanceOfStrip>();
        private OnOffSpellController _onOffSpellController;

        public override IEnumerator OnLoadCoroutine()
        {
            EventManager.onLevelLoad += EventManagerOnonLevelLoad;
            EventManager.onCreatureSpawn += EventManagerOnonCreatureSpawn;

            return base.OnLoadCoroutine();
        }

        private void EventManagerOnonCreatureSpawn(Creature creature)
        {
            if (GameManager.local.gameObject.GetComponent<OnOffSpellController>() != null)
            {
                GameManager.local.gameObject.GetComponent<OnOffSpellController>().strippedCreatures.Remove(creature);
            }
        }

        private void EventManagerOnonLevelLoad(LevelData leveldata, EventTime eventtime)
        {
            if (GameManager.local.gameObject.GetComponent<OnOffSpellController>() == null)
            {
                _onOffSpellController = GameManager.local.gameObject.AddComponent<OnOffSpellController>();
                _onOffSpellController.chanceOfStrips = chanceOfStrips;
            }
            else
            {
                GameManager.local.gameObject.GetComponent<OnOffSpellController>().strippedCreatures =
                    new List<Creature>();
            }
        }
    }
}