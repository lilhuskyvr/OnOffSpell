using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace OnOffSpell
{

    public class OnOffSpellController : MonoBehaviour
    {
        public List<Creature> strippedCreatures;
        public List<ChanceOfStrip> chanceOfStrips;
        public float chanceOfDisarmLeftWeapon, chanceOfDisarmRightWeapon;
        public Dictionary<int, string> leftHandItemIds = new Dictionary<int, string>();
        public Dictionary<int, string> rightHandItemIds = new Dictionary<int, string>();
        private List<string> textureNames = new List<string>()
        {
            "LOD0_HumanFemale_Body_c"
        };
        
        private Dictionary<string, Texture> overrideTextures = new Dictionary<string, Texture>();

        private void Awake()
        {
            strippedCreatures = new List<Creature>();
            chanceOfStrips = new List<ChanceOfStrip>();
            LoadTextures();
        }
        
        private void LoadTextures()
        {
            foreach (var textureName in textureNames)
            {
                Catalog.LoadAssetAsync<GameObject>(textureName,
                    prefab =>
                    {
                        var cloned = Object.Instantiate(prefab);
                        overrideTextures[textureName] = cloned.GetComponent<MeshRenderer>().materials[0].mainTexture;
                    },
                    textureName);
            }
        }

        private IEnumerator ReplaceTextures(Creature creature)
        {
            foreach (var part in creature.manikinLocations.PartList.GetAllParts())
            {
                foreach (var renderer in part.GetRenderers())
                {
                    foreach (var rendererSharedMaterial in renderer.sharedMaterials)
                    {
                        foreach (var texturePropertyNameID in rendererSharedMaterial.GetTexturePropertyNameIDs())
                        {
                            var texture = rendererSharedMaterial.GetTexture(texturePropertyNameID);

                            if (texture == null)
                                continue;

                            if (!overrideTextures.ContainsKey(texture.name))
                                continue;

                            rendererSharedMaterial.SetTexture(texturePropertyNameID, overrideTextures[texture.name]);
                        }
                    }
                }
            }

            yield return null;
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

            yield return ReplaceTextures(creature);
        }

        private void ReEquipCreatureWeapon(Creature creature, Side side)
        {
            if (creature.equipment.GetHeldItem(side) != null)
                return;
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