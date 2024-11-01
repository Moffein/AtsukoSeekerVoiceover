using BepInEx;
using BepInEx.Configuration;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;
using AtsukoSeekerVoiceover.Modules;
using BaseVoiceoverLib;
using System.Collections.Generic;
using AtsukoSeekerVoiceover.Components;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Security;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace AtsukoSeekerVoiceover
{
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.BaseVoiceoverLib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.Schale.AtsukoSeekerVoiceover", "AtsukoSeekerVoiceover", "1.0.0")]
    public class AtsukoSeekerVoiceover : BaseUnityPlugin
    {
        public static ConfigEntry<bool> enableVoicelines;
        public static bool playedSeasonalVoiceline = false;
        public static AssetBundle assetBundle;
        public static SurvivorDef survivorDef = Addressables.LoadAssetAsync<SurvivorDef>("RoR2/DLC2/Seeker/Seeker.asset").WaitForCompletion();

        private void Awake()
        {
            Files.PluginInfo = this.Info;
            RoR2.RoR2Application.onLoad += OnLoad;
            new Content().Initialize();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AtsukoSeekerVoiceover.atsukoseekerbundle"))
            {
                assetBundle = AssetBundle.LoadFromStream(stream);
            }

            SoundBanks.Init();

            InitNSE();

            enableVoicelines = base.Config.Bind<bool>(new ConfigDefinition("Settings", "Enable Voicelines"), true, new ConfigDescription("Enable voicelines when using the Atsuko Seeker Skin."));
            enableVoicelines.SettingChanged += EnableVoicelines_SettingChanged;

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                RiskOfOptionsCompat();
            }
        }

        private void EnableVoicelines_SettingChanged(object sender, EventArgs e)
        {
            RefreshNSE();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void RiskOfOptionsCompat()
        {
            RiskOfOptions.ModSettingsManager.SetModIcon(assetBundle.LoadAsset<Sprite>("atsuko_grin"));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(enableVoicelines));
        }

        private void OnLoad()
        {
            SkinDef atsukoSkin = null;
            SkinDef atsukoSkin2 = null;
            SkinDef[] skins = SkinCatalog.FindSkinsForBody(BodyCatalog.FindBodyIndex("SeekerBody"));
            foreach (SkinDef skinDef in skins)
            {
                if (skinDef.name == "AtsukoSkinDef")
                {
                    atsukoSkin = skinDef;
                }
                else if (skinDef.name == "AtsukoSkinDefMaskless")
                {
                    atsukoSkin2 = skinDef;
                }

                if (atsukoSkin && atsukoSkin2) break;
            }

            if (!atsukoSkin)
            {
                Debug.LogError("AtsukoSeekerVoiceover: Atsuko Seeker SkinDef not found. Voicelines will not work!");
            }
            else
            {
                VoiceoverInfo voiceoverInfo = new VoiceoverInfo(typeof(AtsukoSeekerVoiceoverComponent), atsukoSkin, "SeekerBody");
                voiceoverInfo.selectActions += AtsukoSelect;
            }

            if (!atsukoSkin2)
            {
                Debug.LogError("AtsukoSeekerVoiceover: Atsuko Seeker (Maskless) SkinDef not found. Voicelines will not work!");
            }
            else
            {
                VoiceoverInfo voiceoverInfo = new VoiceoverInfo(typeof(AtsukoSeekerVoiceoverComponent), atsukoSkin2, "SeekerBody");
                voiceoverInfo.selectActions += AtsukoSelect;
            }

            RefreshNSE();
        }

        private void AtsukoSelect(GameObject mannequinObject)
        {
            if (!enableVoicelines.Value) return;

            bool played = false;
            if (!playedSeasonalVoiceline)
            {
                if (System.DateTime.Today.Month == 1 && System.DateTime.Today.Day == 1)
                {
                    Util.PlaySound("Play_AtsukoSeeker_Lobby_Newyear", mannequinObject);
                    played = true;
                }
                else if (System.DateTime.Today.Month == 1 && System.DateTime.Today.Day == 20)
                {
                    Util.PlaySound("Play_AtsukoSeeker_Lobby_bday", mannequinObject);
                    played = true;
                }
                else if (System.DateTime.Today.Month == 10 && System.DateTime.Today.Day == 31)
                {
                    Util.PlaySound("Play_AtsukoSeeker_Lobby_Halloween", mannequinObject);
                    played = true;
                }
                else if (System.DateTime.Today.Month == 12 && System.DateTime.Today.Day == 25)
                {
                    Util.PlaySound("Play_AtsukoSeeker_Lobby_xmas", mannequinObject);
                    played = true;
                }

                if (played) playedSeasonalVoiceline = true;
            }
            if (!played)
            {
                if (Util.CheckRoll(5f))
                {
                    Util.PlaySound("Play_AtsukoSeeker_TitleDrop", mannequinObject);
                }
                else
                {
                    Util.PlaySound("Play_AtsukoSeeker_Lobby", mannequinObject);
                }
            }
        }

        private void InitNSE()
        {
            AtsukoSeekerVoiceoverComponent.nseBlock = RegisterNSE("Play_AtsukoSeeker_Muda");
            AtsukoSeekerVoiceoverComponent.nseShout = RegisterNSE("Play_AtsukoSeeker_Shout");
            AtsukoSeekerVoiceoverComponent.nseShrineFail = RegisterNSE("Play_AtsukoSeeker_ShrineFail");
            AtsukoSeekerVoiceoverComponent.nseMove = RegisterNSE("Play_AtsukoSeeker_Move");
            AtsukoSeekerVoiceoverComponent.nseMoveLong = RegisterNSE("Play_AtsukoSeeker_Move_Long");
            AtsukoSeekerVoiceoverComponent.nseEx1 = RegisterNSE("Play_AtsukoSeeker_ExSkill_1");
            AtsukoSeekerVoiceoverComponent.nseEx2 = RegisterNSE("Play_AtsukoSeeker_ExSkill_2");
            AtsukoSeekerVoiceoverComponent.nseEx3 = RegisterNSE("Play_AtsukoSeeker_ExSkill_3");

            AtsukoSeekerVoiceoverComponent.nseExL1 = RegisterNSE("Play_AtsukoSeeker_ExSkill_Level_1");
            AtsukoSeekerVoiceoverComponent.nseExL2 = RegisterNSE("Play_AtsukoSeeker_ExSkill_Level_2");
            AtsukoSeekerVoiceoverComponent.nseExL3 = RegisterNSE("Play_AtsukoSeeker_ExSkill_Level_3");
        }

        public void RefreshNSE()
        {
            foreach (NSEInfo nse in nseList)
            {
                nse.ValidateParams();
            }
        }

        private NetworkSoundEventDef RegisterNSE(string eventName)
        {
            NetworkSoundEventDef nse = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            nse.eventName = eventName;
            Content.networkSoundEventDefs.Add(nse);
            nseList.Add(new NSEInfo(nse));
            return nse;
        }

        public static List<NSEInfo> nseList = new List<NSEInfo>();
        public class NSEInfo
        {
            public NetworkSoundEventDef nse;
            public uint akId = 0u;
            public string eventName = string.Empty;

            public NSEInfo(NetworkSoundEventDef source)
            {
                this.nse = source;
                this.akId = source.akId;
                this.eventName = source.eventName;
            }

            private void DisableSound()
            {
                nse.akId = 0u;
                nse.eventName = string.Empty;
            }

            private void EnableSound()
            {
                nse.akId = this.akId;
                nse.eventName = this.eventName;
            }

            public void ValidateParams()
            {
                if (this.akId == 0u) this.akId = nse.akId;
                if (this.eventName == string.Empty) this.eventName = nse.eventName;

                if (!enableVoicelines.Value)
                {
                    DisableSound();
                }
                else
                {
                    EnableSound();
                }
            }
        }
    }
}
