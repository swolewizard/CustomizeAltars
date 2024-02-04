using BepInEx;
using HarmonyLib;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

namespace CustomizeAltar
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class Altarmod : BaseUnityPlugin
    {
        private const string ModName = "Customize Altars";
        private const string ModVersion = "2.0.0";
        private const string ModGUID = "Huntard.CustomizeAltars";
        private static readonly string configPath = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModGUID}.json");

        private Harmony _harmony;
        private static Altarmod _instance;
        public static Altarmod Instance => _instance;

        private void Awake()
        {
            _instance = this;
            _harmony = Harmony.CreateAndPatchAll(typeof(Altarmod).Assembly, ModGUID);
        }

        [HarmonyPatch(typeof(LocationList), "Awake")]
        public static class LocationListAwakePatch
        {
            public static void Postfix(LocationList __instance)
            {
                Altarmod.Instance.LoadConfig();
            }
        }

        private void LoadConfig()
        {
            if (!File.Exists(configPath))
                GenerateConfigFile();
        }

        private void GenerateConfigFile()
        {
            var altarConfigs = new List<AltarConfig>();
            var itemStandSupportedItems = new Dictionary<string, List<ItemDrop>>();

            foreach (var itemStand in Resources.FindObjectsOfTypeAll<ItemStand>())
            {
                var supportedItems = new List<ItemDrop>();
                foreach (var supportedItemPrefab in itemStand.m_supportedItems)
                {
                    var itemDrop = supportedItemPrefab.GetComponent<ItemDrop>();
                    if (itemDrop != null)
                    {
                        supportedItems.Add(itemDrop);
                    }
                }
                itemStandSupportedItems[itemStand.name] = supportedItems;
            }

            foreach (var prefab in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                var offeringBowl = prefab.GetComponent<OfferingBowl>();
                if (offeringBowl != null)
                {
                    GameObject rootPrefab = GetRootPrefab(prefab);

                    var bossPrefabName = offeringBowl.m_bossPrefab != null ? offeringBowl.m_bossPrefab.name : "";
                    var sacrificeItemName = offeringBowl.m_bossItem != null ? offeringBowl.m_bossItem.name : "";
                    var itemStandName = offeringBowl.m_itemStandPrefix;
                    var itemStandEnabled = offeringBowl.m_useItemStands;

                    if (string.IsNullOrEmpty(sacrificeItemName) && itemStandEnabled)
                    {
                        List<ItemDrop> supportedItems;
                        if (itemStandSupportedItems.TryGetValue(itemStandName, out supportedItems) && supportedItems.Count > 0)
                        {
                            sacrificeItemName = supportedItems[0].name;
                        }
                    }

                    var altarConfig = new AltarConfig(
                        rootPrefab.name,
                        offeringBowl.m_name,
                        bossPrefabName,
                        sacrificeItemName,
                        offeringBowl.m_bossItems,
                        itemStandEnabled,
                        itemStandName
                    );
                    altarConfigs.Add(altarConfig);
                }
            }

            File.WriteAllText(configPath, JsonMapper.ToJson(altarConfigs));
        }
        private GameObject GetRootPrefab(GameObject gameObject)
        {
            Transform currentTransform = gameObject.transform;
            while (currentTransform != null)
            {
                GameObject currentObject = currentTransform.gameObject;

                if (currentObject.GetComponent<Location>() != null)
                {
                    return currentObject;
                }
                currentTransform = currentTransform.parent;
            }
            return gameObject;
        }

        internal static List<AltarConfig> GetJson()
        {
            var jsonText = File.ReadAllText(configPath);
            return JsonMapper.ToObject<List<AltarConfig>>(jsonText);
        }

        [HarmonyPatch(typeof(OfferingBowl), "Awake")]
        public static class AlterOfferBowlAwake
        {
            public static void Postfix(OfferingBowl __instance)
            {
                if (__instance == null) return;
                var altarConfigs = GetJson();
                foreach (var config in altarConfigs)
                {
                    try
                    {
                        if (GameObject.Find(config.AltarPrefabName + "(Clone)") != null)
                        {
                            var prefab = GameObject.Find(config.AltarPrefabName + "(Clone)");
                            var offeringBowl = prefab.GetComponentInChildren<OfferingBowl>();
                            offeringBowl.m_name = config.Name;
                            offeringBowl.m_bossPrefab = ZNetScene.instance.GetPrefab(config.BossPrefab).gameObject;
                            offeringBowl.m_bossItem = ZNetScene.instance.GetPrefab(config.SacrificeItem).GetComponent<ItemDrop>();
                            offeringBowl.m_bossItems = config.SacrificeAmount;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Loading config for {config.AltarPrefabName + "(Clone)"} failed. {e.Message}");
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ItemStand), "CanAttach")]
        public static class ItemStandUseItemAttach
        {
            public static void Postfix(ItemStand __instance, ItemDrop.ItemData item, ref bool __result)
            {
                if (__instance.GetAttachPrefab(item.m_dropPrefab) == null)
                {
                    Debug.Log($"Overwriting Attach Prefab in CanAttach, hopefully it has an attach- otherwise...");
                    __result = true;
                }

                if (__instance.IsUnsupported(item))
                {
                    Debug.Log($"CanAttach item is unsupported");
                    __result = false;
                    return;
                }

                if (!__instance.IsSupported(item))
                {
                    Debug.Log($"CanAttach IsSupported is false");
                    __result = false;
                    return;
                }

                Debug.Log($"CanAttach Approved");
                __result = true;
            }
        }

        [HarmonyPatch(typeof(ItemStand), "Awake")]
        public static class ItemStandAwake
        {
            public static void Postfix(ItemStand __instance)
            {
                var altarConfigs = GetJson();
                var Items = new List<ItemDrop>();

                foreach (var config in altarConfigs)
                {
                    try
                    {
                        if (config.ItemStandName != "" )
                        {
                            var stand = config.ItemStandName + "(Clone)";

                            if (__instance.gameObject.name == stand && config.ItemStandName != null)
                            {
                                var sacrificeitem = ObjectDB.instance.GetItemPrefab(config.SacrificeItem);
                                var ItemDrop = sacrificeitem.GetComponentInChildren<ItemDrop>(true);
                                var ItemData = ItemDrop.m_itemData;

                                if (Items.Count == 0)
                                    Items.Add(ItemDrop);

                                __instance.m_supportedItems = Items;
                                Debug.Log($"Itemstand exist for {__instance.m_name} with object name {__instance.gameObject.name} setting to {config.SacrificeItem} with name {ItemData.m_shared.m_name}");
                            }
                        }
                    }
                    catch (Exception e) { Debug.LogError($"Loading config for {config.ItemStandName + "(Clone)"} failed. {e.Message}"); }
                }
            }
        }
}

    [Serializable]
    public class AltarConfig
    {
        public string AltarPrefabName { get; set; }
        public string Name { get; set; }
        public string BossPrefab { get; set; }
        public string SacrificeItem { get; set; }
        public int SacrificeAmount { get; set; }
        public bool ItemStandEnabled { get; set; }
        public string ItemStandName { get; set; }

        // Default constructor
        public AltarConfig()
        {
        }
        public AltarConfig(string altarPrefabName, string name, string bossPrefab, string sacrificeItem, int sacrificeAmount, bool itemStandEnabled, string itemStandName = null)
        {
            AltarPrefabName = altarPrefabName;
            Name = name;
            BossPrefab = bossPrefab;
            SacrificeItem = sacrificeItem;
            SacrificeAmount = sacrificeAmount;
            ItemStandEnabled = itemStandEnabled;
            ItemStandName = itemStandName;
        }
    }
}
