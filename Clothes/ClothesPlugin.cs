using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Clothes
{
    //credit for custom palette code goes to only_going_up_fr0m_here with tournamentedition

    public class Configger
    {
        public static bool anal;
        public static bool funny;

        public static int pandemoniumColors;

        public static void DoConfig(BepInEx.Configuration.ConfigFile config)
        {
            string section = "you are cool";
            anal =
                config.Bind(section,
                            "Analysis",
                            false,
                            "Enable debug robes.").Value;

            funny =
                config.Bind(section,
                            "Funny names",
                            false,
                            "Just for me.").Value;
            string pandemoniumSection = "Pandemonium Robe";

            pandemoniumColors =
                config.Bind(pandemoniumSection,
                            "Random Robe Colors",
                            37,
                            "Amount of random robe colors generated. recommend prime numbers. (doesn't work with TournamentEdition installed (atm)").Value;
        }
    }

    [BepInDependency("Amber.TournamentEdition", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("TheTimeSweeper.CustomPalettes", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("TheTimeSweeper.Clothes", "Clothes", "0.6.0")]
    public class ClothesPlugin : BaseUnityPlugin {

        public static PluginInfo PluginInfo;

        public static bool empowered;
        public static bool tournamentEditionInstalled;
        public static bool palettesPluginInstalled;

        void Awake() {
            PluginInfo = Info;
            Log.Init(Logger);

            Assets.Init();

            Configger.DoConfig(this.Config);

            tournamentEditionInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Amber.TournamentEdition");
            palettesPluginInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("TheTimeSweeper.CustomPalettes");

            Clothes.Init();
            
            if (!tournamentEditionInstalled && !palettesPluginInstalled) { 
			    Palettes.Init();
            }

            if (TestValueManager.testingEnabled)
            {
                gameObject.AddComponent<TestValueManager>();
            }

            On.GameController.Start += GameController_Start_LateInit;

            IL.TailorNpc.UpgradePlayerOutfit += TailorNpc_UpgradePlayerOutfit;
        }

        void Start()
        {
            if (tournamentEditionInstalled)
            {
                SetFunnyMax();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetFunnyMax()
        {
            try
            {
                PandemoniumCloak.RandomEndIndex = Mythical.ContentLoader.palettes.Count + 32;
            } catch { }
        }

        private static void TailorNpc_UpgradePlayerOutfit(MonoMod.Cil.ILContext il)
        {
            //changing this.currentMod.modType == OutfitModStat.OutfitModType.Health 
            //to this.currentMod.modType != OutfitModStat.OutfitModType.Cooldown
            //for the upgrade outfit code to exclude negative modifiers
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<TailorNpc>("currentMod"),
                x => x.MatchLdfld<OutfitModStat>("modType"),
                x => x.MatchLdcI4(1)
                );
            c.Index--;
            c.Remove();
            c.Emit(OpCodes.Ldc_I4_S, (sbyte)9);
            c.Next.OpCode = Mono.Cecil.Cil.OpCodes.Beq;
        }

        private void GameController_Start_LateInit(On.GameController.orig_Start orig, GameController self)
        {
            orig(self);
            //StolenFunnyCurse.Init();
            //broken when not wearing cloak
                //beat sura with infinite halo lol
            //fixed but just keep it off just to be safe
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                for (int i = 0; i < GameController.activePlayers.Length; i++)
                {
                    Player player = GameController.activePlayers[i];
                    if (CustomOutfitModManager.PlayerHasMod(player, "Sweep_Anal"))
                    {
                        bool? empowered = null;
                        foreach (Player.SkillState skillState in player.skillsDict.Values)
                        {
                            if(empowered == null)
                            {
                                empowered = skillState.IsEmpowered;
                            }
                            skillState.SetEmpowered(!empowered.Value, EmpowerStatMods.DefaultEmpowerMod);
                        }
                    }
                }
            }

            CustomOutfitModManager.UpdateMods();
        }
    }
}

/*
 * 
				Outfit.baseHope,
				Outfit.basePatience,
				new Outfit("Vigor", 20, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, 0.1f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false)
				}, false, false),
				new Outfit("Grit", 17, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Armor, 0.1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.HealAmount, 0f, 0.1f, 0f, false)
				}, false, false),
				new Outfit("Greed", 18, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Platinum, 1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Gold, 0f, 0.1f, 0f, false)
				}, false, false),
				new Outfit("Pink", 14, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.HealAmount, 0f, 0.2f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.HealCrit, 0.1f, 0f, 0f, false)
				}, false, false),
				new Outfit("Pace", 3, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)2, 0f, 0.15f, 0f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)3, 0f, -0.3f, 0f, false)
				}, false, false),
				new Outfit("Tempo", 5, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.15f, 0f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)2, 0f, 0.1f, 0f, false)
				}, false, false),
				new Outfit("Switch", 4, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)4, 0.1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0.05f, 0f, 0f, false)
				}, false, false),
				new Outfit("Awe", 6, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0.1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.CritDamage, 0.25f, 0f, 0f, false)
				}, false, false),
				new Outfit("Fury", 8, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.ODRate, 0f, 0.2f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.ODDamage, 0f, 0.25f, 0f, false)
				}, false, false),
				new Outfit("Rule", 19, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, 0.1f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Armor, 0.1f, 0f, 0f, false)
				}, false, false),
				new Outfit(Outfit.lvlStr, 7, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, -0.5f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, -0.5f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.ODRate, 0f, -0.5f, 0f, false)
				}, false, true),
				new Outfit("Venture", 2, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, -0.4f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, 0.1f, 0f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)2, 0f, 0.1f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false)
				}, false, false),
				new Outfit("Fall", 9, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, 0f, 100f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)11, 0f, 0f, 0f, true),
					new OutfitModStat(OutfitModStat.OutfitModType.AllowUpgrade, 0f, 0f, 0f, false)
				}, false, false),
				new Outfit("Pride", 15, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, 0f, 1f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)10, 0f, 0f, -1f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.AllowUpgrade, 0f, 0f, 0f, false)
				}, false, false)
 */