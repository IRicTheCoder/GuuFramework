using System;

namespace Guu.Game
{
    /// <summary>
    /// A modded version of the Market's Price Entry
    /// </summary>
    public class ModdedPriceEntry : PriceEntry
    {
        public ModdedMarketUI market;
        public EconomyDirector dir;

        public void Awake()
        {
            bonusFill.minValue = 0;
            bonusFill.maxValue = 25;
            bonusFill.enabled = SceneContext.Instance.GameModel.currGameMode == PlayerState.GameMode.TIME_LIMIT_V2;
        }
        
        public void UpdateEntry(MarketUI.PlortEntry entry)
        {
            int collected = ModdedMarketUI.PlortsCollected(entry.id);
            int change = dir.GetChangeInValue(entry.id) ?? 0;
            float alpha = 0f;

            if (market.IsPlortShown(entry, collected)) alpha = market.IsPlortUnlocked(entry, collected) ? 1 : 0.5f;

            bonusFill.currValue = collected;
            
            changeIcon.sprite = market.GetChangeIcon(change, collected);
            changeIcon.enabled = changeIcon.sprite != null;
            
            changeAmountText.text = SceneContext.Instance.GameModeConfig.GetModeSettings().plortMarketDynamic ? 
                Math.Abs(change).ToString() : 
                string.Empty;

            amountText.color = amountText.color.Alpha(alpha);
            changeAmountText.color = amountText.color.Alpha(alpha);
            changeIcon.color = amountText.color.Alpha(alpha);
            coinIcon.color = amountText.color.Alpha(alpha);
            itemIcon.color = amountText.color.Alpha(alpha);
        }

        public void UpdateValue(MarketUI.PlortEntry entry)
        {
            amountText.text = dir.GetCurrValue(entry.id)?.ToString() ?? "---";
        }
    }
}