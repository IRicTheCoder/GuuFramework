using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Guu.Game
{
    /// <summary>
    /// A modded version of the Market UI to allow for improvements and fixes
    /// </summary>
    public class ModdedMarketUI : MarketUI
    {
        //+ VARIABLES
        private EconomyDirector econDir;
        private ProgressDirector progressDir;
        private LookupDirector lookupDir;
        private readonly Dictionary<PlortEntry, ModdedPriceEntry> displayMap = new Dictionary<PlortEntry, ModdedPriceEntry>();

        //+ BEHAVIOUR
        /// <summary>Awakes the behaviour, ran before any interactions</summary>
        public new void Awake()
        {
            displayMap.Clear();
            econDir = SceneContext.Instance.EconomyDirector;
            progressDir = SceneContext.Instance.ProgressDirector;
            lookupDir = GameContext.Instance.LookupDirector;

            FixEntryPrefab();
        }

        /// <summary>Starts the behaviour, interactions can happen now</summary>
        public new void Start()
        {
            int index = 0;
            int num = 0;

            PlayerState.GameMode gm = SceneContext.Instance.GameModel.currGameMode;
            
            foreach (PlortEntry plort in plorts)
            {
                if (plort.id == Identifiable.Id.SABER_PLORT && gm == PlayerState.GameMode.TIME_LIMIT_V2) continue;
                if (!IsPlortShown(plort, PlortsCollected(plort.id))) continue;

                GameObject obj = Instantiate(priceEntryPrefab, pricesPanels[index].panel.transform, false);
                    
                ModdedPriceEntry entry = obj.GetComponent<ModdedPriceEntry>();
                entry.itemIcon.sprite = lookupDir.GetIcon(plort.id);
                
                displayMap.Add(plort, entry);

                if (num++ < pricesPanels[index].entryCount) continue;
                
                index++;
                num = 0;

                //? This prevents it from exceeding the capacity and makes the market always work even when there are
                //? more plorts than spaces on it.
                if (index == pricesPanels.Length) break;
            }

            while (index < pricesPanels.Length)
            {
                Instantiate(priceEntryEmptyPrefab, pricesPanels[index].panel.transform, false);
                
                if (num++ < pricesPanels[index].entryCount) continue;
                
                index++;
                num = 0;
            }
            
            EconUpdate();
            
            econDir.didUpdateDelegate += EconUpdate;
            econDir.onRegisterSold += PlortCountUpdate;
            progressDir.onProgressChanged += EconUpdate;
        }

        /// <summary>Triggers when the behaviour (or it's game object) is destroyed</summary>
        [SuppressMessage("ReSharper", "DelegateSubtraction")]
        public new void OnDestroy()
        {
            econDir.didUpdateDelegate -= EconUpdate;
            econDir.onRegisterSold -= PlortCountUpdate;
            progressDir.onProgressChanged -= EconUpdate;
        }

        /// <summary>Updates the behaviour, multiple times per second</summary>
        public new void Update()
        {
            bool shutdown = econDir.IsMarketShutdown();
            if (pricesPanelGroup.activeInHierarchy != shutdown) return;
            
            pricesPanelGroup.SetActive(!shutdown);
            shutdownPanel.SetActive(shutdown);
            
            foreach (GameObject obj in toShutdown)
                obj.SetActive(!shutdown);
        }
        
        //+ MARKET UPDATE
        private void EconUpdate()
        {
            foreach (KeyValuePair<PlortEntry, ModdedPriceEntry> display in displayMap)
            {
                ModdedPriceEntry entry = display.Value;
                entry.UpdateValue(display.Key);
                entry.UpdateEntry(display.Key);
            }
        }
        
        private void PlortCountUpdate(Identifiable.Id id)
        {
            foreach (KeyValuePair<PlortEntry, ModdedPriceEntry> display in displayMap)
            {
                if (display.Key.id != id) continue;
                
                display.Value.UpdateEntry(display.Key);
                break;
            }
        }

        internal Sprite GetChangeIcon(int change, int collected)
        {
            if (SceneContext.Instance.GameModel.currGameMode == PlayerState.GameMode.TIME_LIMIT_V2)
                return !GameModeSettings.PlortBonusReached(collected) ? null : bonusCompleteImg;
            
            if (!SceneContext.Instance.GameModeConfig.GetModeSettings().plortMarketDynamic)
                return null;
            
            if (change == 0)
                return unchImg;
            
            return change >= 0 ? upImg : downImg;
        }
        
        //+ PROGRESS CHECK
        internal bool IsPlortUnlocked(PlortEntry plort, int collected)
        {
            if (collected > 0 || SceneContext.Instance.GameModeConfig.GetModeSettings().assumeExperiencedUser || plort.toUnlock.Length == 0)
                return true;
            
            if (plort is ModdedPlortEntry mPlort && mPlort.unlockOnlyWhenCollected) return false;
            
            for (int i = 0; i < plort.toUnlock.Length; i++)
            {
                int count = plort is ModdedPlortEntry modPlort ? modPlort.unlockCount[i] : 1;
                ProgressDirector.ProgressType type = plort.toUnlock[i];
                
                if (progressDir.GetProgress(type) >= count)
                    return true;
            }
            
            return false;
        }
        
        internal bool IsPlortShown(PlortEntry plort, int collected)
        {
            if (!(plort is ModdedPlortEntry modPlort)) return true;

            if (collected > 0 || SceneContext.Instance.GameModeConfig.GetModeSettings().assumeExperiencedUser || modPlort.toShow.Length == 0)
                return true;

            if (modPlort.showOnlyWhenCollected) return false;

            for (int i = 0; i < modPlort.toShow.Length; i++)
            {
                int count = modPlort.showCount[i];
                ProgressDirector.ProgressType type = modPlort.toShow[i];
                
                if (progressDir.GetProgress(type) >= count)
                    return true;
            }
            
            return false;
        }

        internal static int PlortsCollected(Identifiable.Id id)
        {
            SceneContext.Instance.AchievementsDirector.GetGameIdDictStat(AchievementsDirector.GameIdDictStat.PLORT_TYPES_SOLD).TryGetValue(id, out int collected);
            return collected;
        }

        //+ FIXES
        private void FixEntryPrefab()
        {
            PriceEntry old = priceEntryPrefab.GetComponent<PriceEntry>();
            ModdedPriceEntry entry = priceEntryPrefab.AddComponent<ModdedPriceEntry>();

            entry.amountText = old.amountText;
            entry.changeAmountText = old.changeAmountText;
            entry.changeIcon = old.changeIcon;
            entry.coinIcon = old.coinIcon;
            entry.itemIcon = old.itemIcon;
            entry.bonusFill = old.bonusFill;

            entry.market = this;
            entry.dir = econDir;
            
            Destroy(old);
        }
    }
}