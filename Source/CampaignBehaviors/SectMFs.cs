using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;

namespace ImprovedMinorFactionsBeta.Source.CampaignBehaviors
{
    internal class SectMFs : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(OnSettlementEntered));
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(OnGameLoadFinished));
            // Location events
            CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(OnMissionEnded));

            // Debug listeners
            CampaignEvents.OnTroopRecruitedEvent.AddNonSerializedListener(this, new Action<Hero, Settlement, Hero, CharacterObject, int>(OnTroopRecruited));
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(OnHeroKilled));
        }

        //Hello slelyukh
        public void OnDailySettlementTick(Settlement s)
        {
            var mfHideout = Helpers.GetMFHideout(s);
            if (mfHideout == null || !mfHideout.OwnerClan.IsSect || IMFManager.Current.IsFullHideoutOccupationMF(mfHideout.OwnerClan))
                return;
        }

        public override void SyncData(IDataStore dataStore)
        {
            throw new NotImplementedException();
        }
    }
}
