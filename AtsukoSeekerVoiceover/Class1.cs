using BepInEx;
using System;

namespace AtsukoSeekerVoiceover
{
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.BaseVoiceoverLib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.Schale.AtsukoSeekerVoiceover", "AtsukoSeekerVoiceover", "1.0.0")]
    public class AtsukoSeekerVoiceover : BaseUnityPlugin
    {

    }
}
