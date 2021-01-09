namespace Guu.API.Upgrades
{
	/// <summary>
	/// This is the base class for all upgrade items
	/// </summary>
	public abstract class UpgradeItem : RegistryItem<UpgradeItem>
	{
		/// <summary>The type of upgrade</summary>
		protected abstract UpgradeType Type { get; }

		/// <summary>Registers the item into it's registry</summary>
		public override UpgradeItem Register()
		{
			Build();

			return this;
		}

		public enum UpgradeType
		{
			PLOT_UPGRADE = 0,
			PLAYER_UPGRADE = 1,
			CUSTOM = -1
		}
	}
}
