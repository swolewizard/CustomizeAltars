﻿using BepInEx;
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
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Loading config for {config.AltarPrefabName + "(Clone)"} failed. {e.Message}");
                    }
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
            altarConfigs.Add(MountainsAltarConfig);

            var PlainsAltarConfig = new AltarConfig();
            PlainsAltarConfig.AltarPrefabName = "GoblinKing";
            PlainsAltarConfig.Name = "Mystical altar";
            PlainsAltarConfig.BossPrefab = "GoblinKing";
            PlainsAltarConfig.SacrificeItem = "GoblinTotem";
            PlainsAltarConfig.SacrificeAmount = 1;
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
    }

}
