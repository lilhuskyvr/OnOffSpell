using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace OnOffSpell
{
    public class OnOffSpellData
    {
    }

    public class OnOffSpellController : MonoBehaviour
    {
        public OnOffSpellData data = new OnOffSpellData();
        public List<Creature> strippedCreatures;
        public List<ChanceOfStrip> chanceOfStrips;
        public float chanceOfDisarmLeftWeapon, chanceOfDisarmRightWeapon;
        public Dictionary<int, string> leftHandItemIds = new Dictionary<int, string>();
        public Dictionary<int, string> rightHandItemIds = new Dictionary<int, string>();

        private void Awake()
        {
            strippedCreatures = new List<Creature>();
            chanceOfStrips = new List<ChanceOfStrip>();
        }

        private IEnumerator StripCreature(Creature creature)
        {
            yield return new WaitForSeconds(1);

            string[] items = { "Bra", "Underwear", "Wrist_Left", "Wrist_Right" };

            foreach (var renderer in creature.renderers)
            {
                foreach (var item in items)
                {
                    if (renderer.renderer.name.Contains(item))
                    {
                        renderer.renderer.enabled = false;
                    }
                }
            }

            yield return null;
        }

        private void ReEquipCreatureWeapon(Creature creature, Side side)
        {
            var creatureInstanceId = creature.GetInstanceID();
            var itemIds = side == Side.Left ? leftHandItemIds : rightHandItemIds;

            if (!itemIds.ContainsKey(creatureInstanceId)) return;
            var creatureHand = creature.GetHand(side);
            Catalog.GetData<ItemData>(itemIds[creatureInstanceId]).SpawnAsync(
                newItem => { creatureHand.Grab(newItem.GetMainHandle(side)); },
                creatureHand.transform.position, creatureHand.transform.rotation);
        }

        public void OnOffCreature(Creature creature)
        {
            var random = new System.Random();
            if (strippedCreatures.Contains(creature))
            {
                creature.equipment.EquipAllWardrobes(false);

                ReEquipCreatureWeapon(creature, Side.Left);
                ReEquipCreatureWeapon(creature, Side.Right);


                strippedCreatures.Remove(creature);
            }
            else
            {
                foreach (var content in creature.container.contents)
                {
                    if (content.itemData.type == ItemData.Type.Wardrobe)
                    {
                        var chance = 0f;
                        foreach (var chanceOfStrip in chanceOfStrips)
                        {
                            if (content.itemData.id.Contains(chanceOfStrip.slot))
                            {
                                chance = chanceOfStrip.chance;
                                break;
                            }
                        }

                        var roll = random.Next(1, 101);

                        if (roll <= chance)
                        {
                            var wardrobe = content.itemData.GetModule<ItemModuleWardrobe>().GetWardrobe(creature);
                            if (creature.equipment.GetWornContent(wardrobe.manikinWardrobeData.channels[0],
                                wardrobe.manikinWardrobeData.layers[0]) != null)
                            {
                                creature.equipment.UnequipWardrobe(content);
                            }
                            else
                            {
                                creature.equipment.EquipWardrobe(content);
                                creature.equipment.UnequipWardrobe(content);
                            }
                        }
                    }
                }

                if (random.Next(1, 101) <= chanceOfDisarmLeftWeapon)
                {
                    if (creature.equipment.GetHeldItem(Side.Left) != null)
                    {
                        leftHandItemIds[creature.GetInstanceID()] = creature.equipment.GetHeldItem(Side.Left).data.id;
                    }
                    creature.handLeft.TryRelease();
                }

                if (random.Next(1, 101) <= chanceOfDisarmRightWeapon)
                {
                    if (creature.equipment.GetHeldItem(Side.Right) != null)
                    {
                        rightHandItemIds[creature.GetInstanceID()] = creature.equipment.GetHeldItem(Side.Right).data.id;
                    }
                    creature.handRight.TryRelease();
                }

                StartCoroutine(StripCreature(creature));
                strippedCreatures.Add(creature);
            }
        }
    }
}