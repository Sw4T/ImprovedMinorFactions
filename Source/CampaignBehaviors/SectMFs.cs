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
    internal class SectMFsCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(OnDailySettlementTick));
            //CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(OnSettlementEntered));
            //CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(OnGameLoadFinished));
            // Location events
            // CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(OnMissionEnded));

            // Debug
            InformationManager.DisplayMessage(new InformationMessage("SectMFsBehaviour initialized."));
        }

        // Checking for daily trigger 
        public void OnDailySettlementTick(Settlement s)
        {
            var mfHideout = Helpers.GetMFHideout(s);
            Clan mfClan;
            if (mfHideout == null || !mfHideout.OwnerClan.IsSect || IMFManager.Current.IsFullHideoutOccupationMF(mfHideout.OwnerClan))
                return;
            else if (mfHideout.OwnerClan != null)
                mfClan = mfHideout.OwnerClan;
            else
                throw new Exception("SectMFs Exception : no clan has been found for this MFH");

            // Checking the average delay and randomizing the spawn inquisitor party event
            InformationManager.DisplayMessage(new InformationMessage($"{mfClan.Name} has a strength of {mfClan.TotalStrength} and an influence of {mfClan.Influence}"));
            // Checking the MF Strength and influence to create the party

            // Spawning the party with a raid behaviour 

        }

        public override void SyncData(IDataStore dataStore)
        {
            throw new NotImplementedException();
        }

        public void SpawnInquisitorParty(Settlement s)
        {
            throw new NotImplementedException();
        }
    }
}
