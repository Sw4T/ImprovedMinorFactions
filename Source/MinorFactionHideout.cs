﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using System.Xml;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using System.Linq;

namespace ImprovedMinorFactions
{

    public class MFHideoutTypeDefiner : SaveableTypeDefiner
    {
        public MFHideoutTypeDefiner() : base(1_346_751)
        {
        }

        protected override void DefineClassTypes()
        {
            base.AddClassDefinition(typeof(MinorFactionHideout), 1);
        }

        protected override void DefineContainerDefinitions()
        {
            base.ConstructContainerDefinition(typeof(List<MinorFactionHideout>));
            base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, MinorFactionHideout>));
            base.ConstructContainerDefinition(typeof(Dictionary<string, MinorFactionHideout>));

        }
    }
    public class MinorFactionHideout : SettlementComponent, ISpottable
    {
        

        public CampaignTime NextPossibleAttackTime
        {
            get => _nextPossibleAttackTime;
        }

        [LoadInitializationCallback]
        private void OnLoad()
        {
            MFHideoutManager.initManagerIfNone();
            MFHideoutManager.Current.addLoadedMFHideout(this);
        }

        public void ActivateHideoutFirstTime()
        {
            HeroCreator.CreateHeroAtOccupation(Occupation.GangLeader, this.Settlement);
            HeroCreator.CreateHeroAtOccupation(Occupation.GangLeader, this.Settlement);
            activateHideout();
        }

        private void activateHideout(List<Hero> newNotables)
        {
            foreach (Hero notable in newNotables)
            {
                moveNotableIn(notable);
            }

            activateHideout();
        }

        private void activateHideout()
        {
            if (!Helpers.IsMFClanInitialized(this.OwnerClan))
            {
                if (this.OwnerClan == null)
                { 
                    InformationManager.DisplayMessage(new InformationMessage("HIDEOUT HAS NO CLAN CANT ACTIVATE"));
                    return;
                }
                Helpers.DetermineBasicTroopsForMinorFactionsCopypasta();
            }
            
            this._isActive = true;
            this._isSpotted = false;
            base.Settlement.Militia = 15;
            this.Hearth = 300;
            Helpers.callPrivateMethod(this.OwnerClan, "set_HomeSettlement", new object[] { this.Settlement });
            foreach (Hero hero in this.OwnerClan.Heroes)
            {
                hero.UpdateHomeSettlement();
            }
        }

        public void moveHideouts(MinorFactionHideout newHideout)
        {
            // TODO: move hideout inventory
            List<Hero> notables = this.Settlement.Notables.ToList<Hero>();
            newHideout.activateHideout(notables);
            this.deactivateHideout();
        }

        // updates notable info and removes notable from old settlement and adds to new settlement
        private void moveNotableIn(Hero notable)
        {
            notable.VolunteerTypes = new CharacterObject[6];
            notable.StayingInSettlement = this.Settlement;
            notable.UpdateHomeSettlement();
        }

        private void deactivateHideout()
        {
            this._isActive = false;
            this._isSpotted = false;
            this.Settlement.IsVisible = false;
            base.Settlement.Militia = 0;
            this.Hearth = 0;
        }

        public void DailyTick()
        {
            
            if (Helpers.IsMFClanInitialized(_ownerclan))
                base.Settlement.Militia += this.MilitiaChange.ResultNumber;
            this.Hearth += this.HearthChange.ResultNumber;
        }

        //TODO: add AllMinorFactionHideouts if needed
        //public static MBReadOnlyList<Hideout> All
        //{
        //    get
        //    {
        //        return Campaign.Current.AllHideouts;
        //    }
        //}

        protected override void OnInventoryUpdated(ItemRosterElement item, int count)
        {
            // TODO: implement if/when hideouts get an inventory
        }

        // TODO: make hideouts only attackable at night
        public void UpdateNextPossibleAttackTime()
        {
            _nextPossibleAttackTime = CampaignTime.Now + CampaignTime.Hours(12f);
        }

        public IEnumerable<PartyBase> GetDefenderParties(MapEvent.BattleTypes battleType)
        {
            yield return base.Settlement.Party;
            foreach (MobileParty mobileParty in base.Settlement.Parties)
            {

                // only minor faction members in hideout will defend
                if (mobileParty.MapFaction.Equals(Settlement.MapFaction) || mobileParty.IsMilitia)
                {
                    yield return mobileParty.Party;
                }
            }
            // List<MobileParty>.Enumerator enumerator = default(List<MobileParty>.Enumerator);
            yield break;
        }

        // Hideout copypasta
        public PartyBase GetNextDefenderParty(ref int partyIndex, MapEvent.BattleTypes battleType)
        {
            partyIndex++;
            if (partyIndex == 0)
            {
                return base.Settlement.Party;
            }
            for (int i = partyIndex - 1; i < base.Settlement.Parties.Count; i++)
            {
                MobileParty mobileParty = base.Settlement.Parties[i];
                if (mobileParty.MapFaction.Equals(Settlement.MapFaction))
                {
                    partyIndex = i + 1;
                    return mobileParty.Party;
                }
            }
            return null;
        }

        public string SceneName { get; private set; }


        public IFaction MapFaction
        {
            get => _ownerclan.MapFaction;
        }

        public bool IsSpotted
        {
            get => _isSpotted;
            set => _isSpotted = value;
        }

        public void SetScene(string sceneName)
        {
            this.SceneName = sceneName;
        }

        public MinorFactionHideout()
        {
            this._isSpotted = false;
        }

        public override void OnPartyEntered(MobileParty mobileParty)
        {
            base.OnPartyEntered(mobileParty);
        }

        
        public override void OnPartyLeft(MobileParty mobileParty)
        {
            base.OnPartyLeft(mobileParty);
        }

        public override void OnRelatedPartyRemoved(MobileParty mobileParty)
        {
            base.OnRelatedPartyRemoved(mobileParty);
        }

        public override void OnInit()
        {
            base.Settlement.IsVisible = false;
        }

        public override void Deserialize(MBObjectManager objectManager, XmlNode node)
        {
            base.BackgroundCropPosition = float.Parse(node.Attributes.GetNamedItem("background_crop_position").Value);
            base.BackgroundMeshName = node.Attributes.GetNamedItem("background_mesh").Value;
            base.WaitMeshName = node.Attributes.GetNamedItem("wait_mesh").Value;
            base.Deserialize(objectManager, node);
            if (node.Attributes.GetNamedItem("scene_name") != null)
            {
                this.SceneName = node.Attributes.GetNamedItem("scene_name").InnerText;
            }
        }

        // TODO: make reasonable militia calcs
        public ExplainedNumber MilitiaChange
        { 
            get
            {
                ExplainedNumber eNum = new ExplainedNumber(MFHideoutModels.GetMilitiaChange(this.Settlement));
                return eNum;
            }
        }

        public Clan OwnerClan
        {
            get => this._ownerclan;
            set => this._ownerclan = value;
        }

        public bool IsActive
        {
            get => this._isActive;
        }

        // TODO: make reasonable hearth calcs
        public ExplainedNumber HearthChange
        {
            get
            {
                ExplainedNumber eNum = new ExplainedNumber(MFHideoutModels.GetHearthChange(this.Settlement));
                return eNum;
            }
        }


        [SaveableField(420)]
        private bool _isSpotted;


        [SaveableField(422)]
        private CampaignTime _nextPossibleAttackTime;

        [SaveableField(423)]
        private Clan _ownerclan;

        [SaveableField(424)]
        public float Hearth;

        [SaveableField(425)]
        private bool _isActive;
        
    }
}