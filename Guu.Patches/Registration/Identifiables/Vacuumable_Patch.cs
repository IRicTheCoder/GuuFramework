using System.Reflection;
using Eden.Patching.Harmony;
using Guu.API;
using JetBrains.Annotations;

// ReSharper disable RedundantAssignment
// ReSharper disable InconsistentNaming
namespace Guu.Patches.Registration
{
	[EdenHarmony.Wrapper(typeof(Vacuumable))]
	[UsedImplicitly]
	internal static class Vacuumable_Patch
	{
		[UsedImplicitly]
		private static bool TryConsume_Prefix(Vacuumable @this, ref bool @return)
		{
			@return = false;
			
			FieldInfo consume = typeof(Vacuumable).GetField("consume", BindingFlags.Instance | BindingFlags.NonPublic);
			if (consume != null)
			{
				Vacuumable.Consume evtConsume = consume.GetValue(@this) as Vacuumable.Consume;
				if (evtConsume != null)
				{
					evtConsume.Invoke();
					@return = true;

					return false;
				}
			}

			PlayerState state = SceneContext.Instance.PlayerState;
			Identifiable ident = @this.GetComponent<Identifiable>();

			if (AmmoRegistry.CheckInventoryLocks(ident.id, state.GetAmmoMode()) && state.Ammo.MaybeAddToSlot(ident.id, ident))
			{
				Destroyer.DestroyActor(@this.transform.gameObject, "Vacuumable.consume");
				
				@return = true;
				return false;
			}

			return false;
		}
	}
}