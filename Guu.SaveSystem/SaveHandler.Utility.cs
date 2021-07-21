using System;
using System.Collections.Generic;
using System.IO;
using Guu.API;

namespace Guu.SaveSystem
{
	/// <summary>Handles all save related actions done by Guu, provides an interface for Addons to add content to the save process</summary>
	public static partial class SaveHandler
	{
		//+ UTILITY FUNCTIONS
		//? Strip Functions
		private static void StripByMatch<K, V>(Dictionary<K, V> original, Dictionary<K, V> modded, Predicate<KeyValuePair<K, V>> predicate)
		{
			modded.AddAll(original, predicate);
			original.RemoveAll(predicate);
		}
		
		private static void StripByMatch<T>(List<T> original, List<T> modded, Predicate<T> predicate)
		{
			modded.AddAll(original, predicate);
			original.RemoveAll(predicate);
		}

		//? Check Functions
		private static bool IsGuuID(long toTest) => IDRegistry.IsActorRegistered(toTest);
		private static bool IsGuuID<V>(KeyValuePair<long, V> pair) => IDRegistry.IsActorRegistered(pair.Key);
		private static bool IsGuuID(string toTest) => toTest.StartsWith("<guu>");
		private static bool IsGuuID<V>(KeyValuePair<string, V> pair) => pair.Key.StartsWith("<guu>");
		
		private static bool IsGuuIdentifiable<V>(KeyValuePair<Identifiable.Id, V> pair) => IdentifiableRegistry.IsIdentifiableRegistered(pair.Key);
		private static bool IsGuuIdentifiable(Identifiable.Id id) => IdentifiableRegistry.IsIdentifiableRegistered(id);
		
		private static bool IsGuuGadget<V>(KeyValuePair<Gadget.Id, V> pair) => GadgetRegistry.IsGadgetRegistered(pair.Key);
		private static bool IsGuuGadget(Gadget.Id id) => GadgetRegistry.IsGadgetRegistered(id);
		
		private static bool IsGuuUpgrade<V>(KeyValuePair<PlayerState.Upgrade, V> pair) => UpgradeRegistry.IsPlayerUpgradeRegistered(pair.Key);
		private static bool IsGuuUpgrade(PlayerState.Upgrade id) => UpgradeRegistry.IsPlayerUpgradeRegistered(id);

		//? Stream Manipulation
		private static void CopyStream(Stream from, Stream to)
		{
			byte[] buffer = new byte[1024];
			int count;
			do
			{
				count = from.Read(buffer, 0, buffer.Length);
				to.Write(buffer, 0, count);
			}
			while (count >= buffer.Length);
		}
	}
}