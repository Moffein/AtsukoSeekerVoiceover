using BaseVoiceoverLib;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AtsukoSeekerVoiceover.Components
{
    public class AtsukoSeekerVoiceoverComponent : BaseVoiceoverComponent
    {
        private bool acquiredScepter = false;

        private float levelCooldown = 0f;
        private float blockedCooldown = 0f;
        private float lowHealthCooldown = 0f;
        private float specialCooldown = 0f;
        private float shrineFailCooldown = 0f;

        public static NetworkSoundEventDef nseBlock, nseShout, nseShrineFail, nseMove, nseMoveLong, nseEx1, nseEx2, nseEx3, nseExL1, nseExL2, nseExL3;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (specialCooldown > 0f) specialCooldown -= Time.fixedDeltaTime;
            if (levelCooldown > 0f) levelCooldown -= Time.fixedDeltaTime;
            if (blockedCooldown > 0f) blockedCooldown -= Time.fixedDeltaTime;
            if (lowHealthCooldown > 0f) lowHealthCooldown -= Time.fixedDeltaTime;
            if (shrineFailCooldown > 0f) shrineFailCooldown -= Time.fixedDeltaTime;
        }

        protected override void Start()
        {
            base.Start();
            if (inventory && inventory.GetItemCount(scepterIndex) > 0) acquiredScepter = true;
        }
        public override void PlayLevelUp()
        {
            if (levelCooldown > 0f) return;
            bool played;
            if (Util.CheckRoll(50f))
            {
                if (Util.CheckRoll(50f))
                {
                    played = TryPlaySound("Play_AtsukoSeeker_LevelUp_1", 7f, false);
                }
                else
                {
                    played = TryPlaySound("Play_AtsukoSeeker_LevelUp_2", 6f, false);
                }
            }
            else
            {
                if (Util.CheckRoll(50f))
                {
                    played = TryPlaySound("Play_AtsukoSeeker_LevelUp_3", 4.1f, false);
                }
                else
                {
                    played = TryPlaySound("Play_AtsukoSeeker_LevelUp_4", 2.7f, false);
                }
            }
            if (played) levelCooldown = 60f;
        }

        public override void PlayDeath()
        {
            TryPlaySound("Play_AtsukoSeeker_Defeat", 1.6f, true);
        }

        public override void PlayTeleporterFinish()
        {
            TryPlaySound("Play_AtsukoSeeker_Victory", 1.6f, false);
        }

        public override void PlayTeleporterStart()
        {
            TryPlaySound("Play_AtsukoSeeker_TeleStart",3f, false);
        }

        public override void PlayDamageBlockedServer()
        {
            if (!NetworkServer.active || blockedCooldown > 0f) return;
            bool played = TryPlayNetworkSound(nseBlock, 0.55f, false);
            if (played) blockedCooldown = 30f;
        }

        public void PlayAcquireScepter()
        {
            if (acquiredScepter) return;
            TryPlaySound("Play_AtsukoSeeker_AcquireScepter", 21f, true);
            acquiredScepter = true;
        }

        public void PlayAcquireLegendary()
        {
            if (Util.CheckRoll(50f))
            {
                if (Util.CheckRoll(50f))
                {
                    TryPlaySound("Play_AtsukoSeeker_Relationship_1", 3.5f, false);
                }
                else
                {
                    TryPlaySound("Play_AtsukoSeeker_Relationship_2", 5.1f, false);
                }
            }
            else
            {
                if (Util.CheckRoll(50f))
                {
                    TryPlaySound("Play_AtsukoSeeker_Relationship_3", 7.6f, false);
                }
                else
                {
                    TryPlaySound("Play_AtsukoSeeker_Relationship_4", 7.2f, false);
                }
            }
        }

        public void PlayFlowerItem()
        {
            TryPlaySound("Play_AtsukoSeeker_Flower", 3.6f, false);
        }

        public void PlayBadItem()
        {
            TryPlaySound("Play_AtsukoSeeker_BadItem", 1f, false);
        }

        protected override void Inventory_onItemAddedClient(ItemIndex itemIndex)
        {
            base.Inventory_onItemAddedClient(itemIndex);
            if (scepterIndex != ItemIndex.None && itemIndex == scepterIndex)
            {
                PlayAcquireScepter();
            }
            else
            {
                ItemDef id = ItemCatalog.GetItemDef(itemIndex);
                if (id == RoR2Content.Items.TPHealingNova || id == RoR2Content.Items.RepeatHeal)
                {
                    PlayFlowerItem();
                }
                else
                if (id == RoR2Content.Items.Squid || id == RoR2Content.Items.Plant || id == RoR2Content.Items.SlowOnHit)
                {
                    PlayBadItem();
                }
                else if (id && id.deprecatedTier == ItemTier.Tier3)
                {
                    PlayAcquireLegendary();
                }
            }
        }

        public override void PlaySecondaryAuthority(GenericSkill skill)
        {
            TryPlayNetworkSound(nseShout, 0f, false);
        }

        public override void PlayUtilityAuthority(GenericSkill skill)
        {
            if (Util.CheckRoll(75f))
            {
                TryPlayNetworkSound(nseMove, 1f, false);
            }
            else
            {
                TryPlayNetworkSound(nseMoveLong, 1.4f, false);
            }
        }

        public override void PlaySpecialAuthority(GenericSkill skill)
        {
            if (specialCooldown > 0f) return;

            if (Util.CheckRoll(0.5f))
            {
                if (Util.CheckRoll(1f / 3f))
                {
                    TryPlayNetworkSound(nseEx1, 3.3f, false);
                }
                else
                {
                    if (Util.CheckRoll(0.5f))
                    {
                        TryPlayNetworkSound(nseEx2, 2.8f, false);
                    }
                    else
                    {
                        TryPlayNetworkSound(nseEx3, 1.7f, false);
                    }
                }
            }
            else
            {
                if (Util.CheckRoll(1f / 3f))
                {
                    TryPlayNetworkSound(nseExL1, 2.2f, false);
                }
                else
                {
                    if (Util.CheckRoll(0.5f))
                    {
                        TryPlayNetworkSound(nseExL2, 1.9f, false);
                    }
                    else
                    {
                        TryPlayNetworkSound(nseExL3, 3f, false);
                    }
                }
            }
                
            specialCooldown = 10f;
        }

        public override void PlaySpawn()
        {
            TryPlaySound("Play_AtsukoSeeker_Spawn", 1.4f, true);
        }

        public override void PlayHurt(float percentHPLost)
        {
            if (percentHPLost >= 0.1f)
            {
                TryPlaySound("Play_AtsukoSeeker_Hurt", 0f, false);
            }
        }

        public override void PlayLowHealth()
        {
            if (lowHealthCooldown > 0f) return;
            bool playedSound;

            if (Util.CheckRoll(50f))
            {
                playedSound = TryPlaySound("Play_AtsukoSeeker_LowHealth_1", 2.4f, false);
            }
            else
            {
                playedSound = TryPlaySound("Play_AtsukoSeeker_LowHealth_2", 4f, false);
            }

            if (playedSound) lowHealthCooldown = 60f;
        }

        public override void PlayVictory()
        {
            TryPlaySound("Play_AtsukoSeeker_Memorial", 24f, true);
        }

        public override void PlayShrineOfChanceFailServer()
        {
            if (shrineFailCooldown > 0f) return;
            if (Util.CheckRoll(15f))
            {
                bool played;
                played = TryPlayNetworkSound(nseShrineFail, 1.2f, false);
                if (played) shrineFailCooldown = 60f;
            }
        }

        public override bool ComponentEnableVoicelines()
        {
            return AtsukoSeekerVoiceover.enableVoicelines.Value;
        }
    }
}
