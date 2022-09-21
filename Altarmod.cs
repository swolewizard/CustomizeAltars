using BepInEx;
using HarmonyLib;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CustomizeAltar
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class Altarmod : BaseUnityPlugin
    {
        private const string ModName = "Customize Altars - by Huntard";
        private const string ModVersion = "1.0.0";
        private const string ModGUID = "Huntard.CustomizeAltars";

        public static string configPath = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModGUID}.json");
        private Harmony _harmony;
            
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

						prefab.GetComponentInChildren<OfferingBowl>().m_name = config.Name;
						prefab.GetComponentInChildren<OfferingBowl>().m_bossPrefab = ZNetScene.instance.GetPrefab(config.BossPrefab).gameObject;
						prefab.GetComponentInChildren<OfferingBowl>().m_bossItem = ZNetScene.instance.GetPrefab(config.SacrificeItem).GetComponent<ItemDrop>();
						prefab.GetComponentInChildren<OfferingBowl>().m_bossItems = config.SacrificeAmount;
						var ItemsEmpty = new List<ItemDrop>();
						if (prefab.GetComponentsInChildren<ItemStand>(true).Count() == null)  //never because this gets overwritten
						{
							Debug.Log("Itemstand exist");
							Debug.Log(config.SacrificeItem);
							Debug.Log(prefab.GetComponentsInChildren<ItemStand>(true).Count());


							Debug.Log(ZNetScene.instance.GetPrefab(config.SacrificeItem).GetComponent<ItemDrop.ItemData>());
							var Items = new List<ItemDrop>
							{
								new ItemDrop
									{
										m_itemData = ZNetScene.instance.GetPrefab(config.SacrificeItem).GetComponent<ItemDrop.ItemData>()
									 }
							};

							var ItemStands = prefab.GetComponentsInChildren<ItemStand>(true);
							foreach (var ItemStand in ItemStands)
							{
								Debug.Log($"Itemstand  set to {config.SacrificeItem}");
								ItemStand.m_supportedItems = Items;
							}

						}
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
				//Debug.Log($"Postfix CanAttach");
				if (__instance.GetAttachPrefab(item.m_dropPrefab) == null)
				{
					Debug.Log($"Overwritting Attach Prefab in CanAttach, hopefully it has an attach- otherwise...");
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

				if (__instance.m_supportedTypes.Count == 0)
				{
					Debug.Log($"CanAttach types = 0 ");
					//return true;
				}
				if (__instance.m_supportedTypes.Contains(item.m_shared.m_itemType))
                		{
					//Debug.Log($"This is not a support Type");
					//__result = false;
					//return;

				}

				Debug.Log($"CanAttach Approved");
				__result = true;//  items have itemTypes that it also checks for which is stupid
			}
		}

	[HarmonyPatch(typeof(ItemStand), "Awake")]
	public static class ItemStandAwake
	{
		public static void Postfix(ItemStand __instance)
		{
			var altarConfigs = GetJson();
			var Items = new List<ItemDrop>();
			//var ItemsType = new List<ItemDrop.ItemData.ItemType>();

			foreach (var config in altarConfigs)
			{
				try
				{
					var stand = config.ItemStandName + "(Clone)";
					//Debug.Log($"Itemstand looking {__instance.gameObject.name} with config {config.ItemStandName}");

					if (__instance.gameObject.name == stand && config.ItemStandName != null)
					{
						var scraficeitem = ObjectDB.instance.GetItemPrefab(config.SacrificeItem);
						var ItemDrop = scraficeitem.GetComponentInChildren<ItemDrop>(true);
						var ItemData = ItemDrop.m_itemData;
						//var ItemType = ItemData.m_shared.m_itemType;
						if (Items.Count() == 0)
							Items.Add(ItemDrop);
						//if (ItemsType.Count() == 0 )
						//	ItemsType.Add(ItemType);

						__instance.m_supportedItems = Items;
						Debug.Log($"Itemstand exist for {__instance.m_name} with object name {__instance.gameObject.name} setting to {config.SacrificeItem} with name {ItemData.m_shared.m_name}");
					}
				}
				catch (Exception e) { Debug.LogError($"Loading config for {config.ItemStandName + "(Clone)"} failed. {e.Message}"); }
			}
		}
	}

        private void Awake()
        {
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModGUID);

            LoadConfig();
        }
        private void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                GenerateConfigFile();
                Debug.Log("Generated new configs");
                return;
            }
        }
	private void GenerateConfigFile()
	{
		var altarConfigs = new List<AltarConfig>();

		// Vanilla
		var MeadowsAltarConfig = new AltarConfig();
		MeadowsAltarConfig.AltarPrefabName = "Eikthyrnir";
		MeadowsAltarConfig.Name = "Mystical altar";
		MeadowsAltarConfig.BossPrefab = "Eikthyr";
		MeadowsAltarConfig.SacrificeItem = "TrophyDeer";
		MeadowsAltarConfig.SacrificeAmount = 2;
		altarConfigs.Add(MeadowsAltarConfig);

		var BlackForestAltarConfig = new AltarConfig();
		BlackForestAltarConfig.AltarPrefabName = "GDKing";
		BlackForestAltarConfig.Name = "Ancient bowl";
		BlackForestAltarConfig.BossPrefab = "gd_king";
		BlackForestAltarConfig.SacrificeItem = "AncientSeed";
		BlackForestAltarConfig.SacrificeAmount = 3;
		altarConfigs.Add(BlackForestAltarConfig);

		var SwampAltarConfig = new AltarConfig();
		SwampAltarConfig.AltarPrefabName = "Bonemass";
		SwampAltarConfig.Name = "Boiling death";
		SwampAltarConfig.BossPrefab = "Bonemass";
		SwampAltarConfig.SacrificeItem = "WitheredBone";
		SwampAltarConfig.SacrificeAmount = 10;
		altarConfigs.Add(SwampAltarConfig);

		var MountainsAltarConfig = new AltarConfig();
		MountainsAltarConfig.AltarPrefabName = "Dragonqueen";
		MountainsAltarConfig.Name = "Sacrificial altar";
		MountainsAltarConfig.BossPrefab = "Dragon";
		MountainsAltarConfig.SacrificeItem = "DragonEgg";
		MountainsAltarConfig.SacrificeAmount = 1;
		MountainsAltarConfig.ItemStandName = "dragoneggcup";
		altarConfigs.Add(MountainsAltarConfig);

		var PlainsAltarConfig = new AltarConfig();
		PlainsAltarConfig.AltarPrefabName = "GoblinKing";
		PlainsAltarConfig.Name = "Mystical altar";
		PlainsAltarConfig.BossPrefab = "GoblinKing";
		PlainsAltarConfig.SacrificeItem = "GoblinTotem";
		PlainsAltarConfig.SacrificeAmount = 1;
		PlainsAltarConfig.ItemStandName = "goblinking_totemholder";
		altarConfigs.Add(PlainsAltarConfig);


		// EVA
		var MistlandsAltarConfig = new AltarConfig();
		MistlandsAltarConfig.AltarPrefabName = "SvartalfrQueenAltar_New";
		MistlandsAltarConfig.Name = "Cursed Altar";
		MistlandsAltarConfig.BossPrefab = "SvartalfarQueen";
		MistlandsAltarConfig.SacrificeItem = "CursedEffigy";
		MistlandsAltarConfig.SacrificeAmount = 5;
		altarConfigs.Add(MistlandsAltarConfig);

		var DeepNorthAltarConfig = new AltarConfig();
		DeepNorthAltarConfig.AltarPrefabName = "JotunnAltar";
		DeepNorthAltarConfig.Name = "Frozen Altar";
		DeepNorthAltarConfig.BossPrefab = "Jotunn";
		DeepNorthAltarConfig.SacrificeItem = "YmirsSoulEssence";
		DeepNorthAltarConfig.SacrificeAmount = 5;
		altarConfigs.Add(DeepNorthAltarConfig);

		var AshlandsAltarConfig = new AltarConfig();
		AshlandsAltarConfig.AltarPrefabName = "BlazingDamnedOneAltar";
		AshlandsAltarConfig.Name = "Blazing Altar";
		AshlandsAltarConfig.BossPrefab = "HelDemon";
		AshlandsAltarConfig.SacrificeItem = "FenrirsHeart";
		AshlandsAltarConfig.SacrificeAmount = 5;
		altarConfigs.Add(AshlandsAltarConfig);


		var jsonText = JsonMapper.ToJson(altarConfigs);
		File.WriteAllText(configPath, jsonText);
	}
        internal static List<AltarConfig> GetJson()
        {
            var jsonText = File.ReadAllText(configPath);
            var altarConfigs = JsonMapper.ToObject<List<AltarConfig>>(jsonText);
            return altarConfigs;
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
        public string ItemStandName { get; set; }
    }

}
