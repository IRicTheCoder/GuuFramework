using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Guu.API;
using MonomiPark.SlimeRancher.Persist;
using UnityEngine;

namespace Guu.SaveSystem
{
	/// <summary>Handles all save related actions done by Guu, provides an interface for Addons to add content to the save process</summary>
	public static partial class SaveHandler
	{
		//+ EVENTS
		/// <summary>Triggers after the Save Handler loads the game. The GameV12 provided is only reference, changing it doesn't alter the game</summary>
		public static event Action<GameV12, string> OnLoad;
		
		//+ CONSTANTS
		internal const string GUU_EXTENSION = "_guu.sav";
		private const string TEMP_EXTENSION = "_guu.tmp";
		private const string PROFILE_FILENAME = "guuprofile.prf";
		private const string SETTINGS_FILENAME = "guusettings.cfg";

		private const Identifiable.Id FALLBACK_ICON = Identifiable.Id.GOLD_PLORT;
		
		//+ VARIABLES
		private static string saveName;
		private static GameV12 gameState;

		//+ PROPERTIES
		internal static string SavePath => Application.persistentDataPath.Replace("unity.Monomi Park.Slime Rancher", Path.Combine("Monomi Park", "Slime Rancher"));

		//+ SAVE HANDLING
		//! Patch: FileStorageProvider.StoreGameData
		internal static void SaveFile(string name)
		{
			string realPath = Path.Combine(SavePath, name + GUU_EXTENSION);
			string tempPath = Path.Combine(SavePath, name + TEMP_EXTENSION);
			using (MemoryStream dataStream = new MemoryStream())
			{
				gameState.Write(dataStream);
				dataStream.Seek(0, SeekOrigin.Begin);
				using (FileStream fileStream = File.Create(tempPath))
					CopyStream(dataStream, fileStream);
			}
			
			File.Copy(tempPath, realPath, true);
			try
			{
				File.Delete(tempPath);
			}
			catch (Exception e)
			{
				GuuCore.LOGGER.LogError("Failed to delete temporary modded save file");
				GuuCore.LOGGER.LogError(e.ParseTrace());
			}
			
			GuuCore.LOGGER.Log($"Save modded save file {name}");
		}

		//! Patch: SavedGame.Save
		internal static void Save(GameV12 original)
		{
			GuuCore.LOGGER.Log("Attempting to save modded save file");
			gameState = new GameV12();
			
			//& Strip Summary
			if (IdentifiableRegistry.IsIdentifiableRegistered(original.summary.iconId))
			{
				gameState.summary.iconId = original.summary.iconId;
				original.summary.iconId = FALLBACK_ICON;
			}
			
			//# Strip Game Mode
			
			//& Strip World
			StripByMatch(original.world.lastOfferRancherIds, gameState.world.lastOfferRancherIds, IsGuuID);
			StripByMatch(original.world.pendingOfferRancherIds, gameState.world.pendingOfferRancherIds, IsGuuID);
			//# Strip Weather
			
			StripByMatch(original.world.offers, gameState.world.offers, pair => IsGuuID(pair.Value.rancherId) || IsGuuID(pair.Value.offerId) || ExchangeRegistry.IsOfferTypeRegistered(pair.Key));
			foreach (ExchangeDirector.OfferType type in original.world.offers.Keys)
			{
				ExchangeOfferV04 ori = original.world.offers[type];
				ExchangeOfferV04 mod = new ExchangeOfferV04();
				
				StripByMatch(ori.requests, mod.requests, request => IsGuuIdentifiable(request.id) || ExchangeRegistry.IsNonIdenRegistered(request.nonIdentReward));
				StripByMatch(ori.rewards, mod.rewards, reward => IsGuuIdentifiable(reward.id) || ExchangeRegistry.IsNonIdenRegistered(reward.nonIdentReward));

				gameState.world.offers.Add(type, mod);
			}

			StripByMatch(original.world.econSaturations, gameState.world.econSaturations, IsGuuIdentifiable);
			StripByMatch(original.world.teleportNodeActivations, gameState.world.teleportNodeActivations, IsGuuID);
			StripByMatch(original.world.liquidSourceUnits, gameState.world.liquidSourceUnits, IsGuuID);
			
			StripByMatch(original.world.gordos, gameState.world.gordos, IsGuuID);
			foreach (KeyValuePair<string, GordoV01> gordo in original.world.gordos.Where(pair => pair.Value.fashions.Exists(IsGuuIdentifiable)))
			{
				gameState.world.gordos.Add(gordo.Key, new GordoV01());
				StripByMatch(gordo.Value.fashions, gameState.world.gordos[gordo.Key].fashions, IsGuuIdentifiable);
			}
			
			StripByMatch(original.world.placedGadgets, gameState.world.placedGadgets, IsGuuID);
			foreach (string id in original.world.placedGadgets.Keys)
			{
				PlacedGadgetV08 ori = original.world.placedGadgets[id];
				PlacedGadgetV08 mod = new PlacedGadgetV08();

				if (IsGuuGadget(ori.gadgetId))
				{
					mod.gadgetId = ori.gadgetId;
					ori.gadgetId = Gadget.Id.NONE;
				}

				foreach (AmmoDataV02 ammo in ori.ammo)
				{
					AmmoDataV02 mAmmo = new AmmoDataV02();

					if (IsGuuIdentifiable(ammo.id))
					{
						mAmmo.id = ammo.id;
						ammo.id = Identifiable.Id.NONE;
					}
					
					//# Strip Emotion Data
				}

				if (IsGuuIdentifiable(ori.baitTypeId))
				{
					mod.baitTypeId = ori.baitTypeId;
					ori.baitTypeId = Identifiable.Id.NONE;
				}
				
				if (IsGuuIdentifiable(ori.gordoTypeId))
				{
					mod.gordoTypeId = ori.gordoTypeId;
					ori.gordoTypeId = Identifiable.Id.NONE;
				}
				
				StripByMatch(ori.fashions, mod.fashions, IsGuuIdentifiable);

				if (ori.drone != null)
				{
					mod.drone = new DroneGadgetV01
					{
						drone = new DroneV05()
					};

					AmmoDataV02 ammo = ori.drone.drone.ammo;
					AmmoDataV02 mAmmo = mod.drone.drone.ammo;
					
					if (IsGuuIdentifiable(ammo.id))
					{
						mAmmo.id = ammo.id;
						ammo.id = Identifiable.Id.NONE;
					}
					
					//# Strip Emotion Data
					
					StripByMatch(ori.drone.drone.fashions, mod.drone.drone.fashions, IsGuuIdentifiable);
				}
				
				gameState.world.placedGadgets.Add(id, mod);
			}
			
			StripByMatch(original.world.treasurePods, gameState.world.treasurePods, IsGuuID);
			foreach (KeyValuePair<string, TreasurePodV01> pod in original.world.treasurePods.Where(pair => pair.Value.spawnQueue.Exists(IsGuuIdentifiable)))
			{
				gameState.world.treasurePods.Add(pod.Key, new TreasurePodV01());
				StripByMatch(pod.Value.spawnQueue, gameState.world.treasurePods[pod.Key].spawnQueue, IsGuuIdentifiable);
			}
			
			StripByMatch(original.world.switches, gameState.world.switches, IsGuuID);
			StripByMatch(original.world.puzzleSlotsFilled, gameState.world.puzzleSlotsFilled, IsGuuID);
			StripByMatch(original.world.occupiedPhaseSites, gameState.world.occupiedPhaseSites, IsGuuID);
			StripByMatch(original.world.quicksilverEnergyGenerators, gameState.world.quicksilverEnergyGenerators, IsGuuID);
			StripByMatch(original.world.oasisStates, gameState.world.oasisStates, IsGuuID);
			StripByMatch(original.world.activeGingerPatches, gameState.world.activeGingerPatches, IsGuuID);
			StripByMatch(original.world.echoNoteGordos, gameState.world.echoNoteGordos, IsGuuID);
			
			//? Slimeulation
			StripByMatch(original.world.glitch.teleporters, gameState.world.glitch.teleporters, IsGuuID);
			StripByMatch(original.world.glitch.nodes, gameState.world.glitch.nodes, IsGuuID);
			StripByMatch(original.world.glitch.impostoDirectors, gameState.world.glitch.impostoDirectors, IsGuuID);
			StripByMatch(original.world.glitch.impostos, gameState.world.glitch.impostos, IsGuuID);
			StripByMatch(original.world.glitch.slimes, gameState.world.glitch.slimes, IsGuuID);
			StripByMatch(original.world.glitch.storage, gameState.world.glitch.storage, IsGuuID);
			StripByMatch(original.world.glitch.storage, gameState.world.glitch.storage, pair => IsGuuIdentifiable(pair.Value.id));
			
			//& Strip Player
			//# Strip Game Mode
			
			if (IsGuuIdentifiable(original.player.gameIconId))
			{
				gameState.player.gameIconId = original.player.gameIconId;
				original.player.gameIconId = FALLBACK_ICON;
			}
			
			StripByMatch(original.player.upgrades, gameState.player.upgrades, IsGuuUpgrade);
			
			StripByMatch(original.player.ammo, gameState.player.ammo, pair => AmmoRegistry.IsAmmoModeRegistered(pair.Key));
			foreach (KeyValuePair<PlayerState.AmmoMode,List<AmmoDataV02>> ammoList in original.player.ammo)
			{
				List<AmmoDataV02> moddedList = new List<AmmoDataV02>();
				foreach (AmmoDataV02 ammo in ammoList.Value)
				{
					AmmoDataV02 data = new AmmoDataV02();

					if (IsGuuIdentifiable(ammo.id))
					{
						data.id = ammo.id;
						ammo.id = Identifiable.Id.NONE;
					}
					
					//# Strip Emotions

					moddedList.Add(data);
				}

				if (moddedList.Count > 0) gameState.player.ammo[ammoList.Key] = moddedList;
			}
			
			foreach (MailV02 mail in original.player.mail)
			{
				MailV02 mod = new MailV02
				{
					messageKey = string.Empty,
					mailType = MailDirector.Type.UPGRADE
				};

				if (MailRegistry.IsTypeRegistered(mail.mailType))
				{
					mod.mailType = mail.mailType;
					mail.mailType = MailDirector.Type.PERSONAL;
				}

				if (MailRegistry.IsMailRegistered(mail.messageKey))
				{
					mod.messageKey = mail.messageKey;
					if (mod.mailType == MailDirector.Type.UPGRADE) mod.mailType = mail.mailType;
					mail.mailType = MailDirector.Type.UPGRADE;
				}

				gameState.player.mail.Add(mod);
			}
			
			StripByMatch(original.player.availUpgrades, gameState.player.availUpgrades, IsGuuUpgrade);
			StripByMatch(original.player.upgradeLocks, gameState.player.upgradeLocks, IsGuuUpgrade);
			StripByMatch(original.player.progress, gameState.player.progress, pair => ProgressRegistry.IsTypeRegistered(pair.Key));
			StripByMatch(original.player.delayedProgress, gameState.player.delayedProgress, pair => ProgressRegistry.IsTrackerRegistered(pair.Key));
			StripByMatch(original.player.blueprints, gameState.player.blueprints, IsGuuGadget);
			StripByMatch(original.player.availBlueprints, gameState.player.availBlueprints, IsGuuGadget);
			StripByMatch(original.player.blueprintLocks, gameState.player.blueprintLocks, IsGuuGadget);
			StripByMatch(original.player.gadgets, gameState.player.gadgets, IsGuuGadget);
			StripByMatch(original.player.craftMatCounts, gameState.player.craftMatCounts, IsGuuIdentifiable);
			
			//# Strip Region Set ID && Player Pos
			//# Strip Unlocked Zone
			
			//? Decorizer
			StripByMatch(original.player.decorizer.contents, gameState.player.decorizer.contents, IsGuuIdentifiable);
			StripByMatch(original.player.decorizer.settings, gameState.player.decorizer.settings, IsGuuID);
			StripByMatch(original.player.decorizer.settings, gameState.player.decorizer.settings, pair => IsGuuIdentifiable(pair.Value.selected));
			
			//& Strip Ranch
			//# Strip Plots
			
			StripByMatch(original.ranch.accessDoorStates, gameState.ranch.accessDoorStates, IsGuuID);
			StripByMatch(original.ranch.palettes, gameState.ranch.palettes, pair => ChromaPackRegistry.IsTypeRegistered(pair.Key) || ChromaPackRegistry.IsPaletteRegistered(pair.Value));
			StripByMatch(original.ranch.ranchFastForward, gameState.ranch.ranchFastForward, IsGuuID);
			
			//& Strip Actors
			StripByMatch(original.actors, gameState.actors, actor => IsGuuID(actor.actorId));
			foreach (ActorDataV09 actor in original.actors)
			{
				ActorDataV09 newActor = new ActorDataV09();
				// ReSharper disable once ReplaceWithSingleAssignment.False
				bool hasData = false;
				
				//# Strip Emotions

				if (actor.fashions.Exists(IsGuuIdentifiable))
				{
					hasData = true;
					StripByMatch(actor.fashions, newActor.fashions, IsGuuIdentifiable);
				}
				
				//# Strip Region Set && Position

				if (hasData)
					gameState.actors.Add(newActor);
			}
			
			//& Strip Pedia
			//# Strip Unlocked IDs
			//# Strip Completed Tuts
			//# Strip Popup Queue
			
			//& Strip Achievements
			//# Strip Float Stats
			//# Strip Double Stats
			//# Strip Int Stats
			
			//# Strip ID Stats by Key
			foreach (KeyValuePair<AchievementsDirector.GameIdDictStat, Dictionary<Identifiable.Id, int>> idDictStat in original.achieve.gameIdDictStatDict.Where(pair => pair.Value.Count(pair2 => IsGuuIdentifiable(pair2.Key)) > 0))
			{
				gameState.achieve.gameIdDictStatDict[idDictStat.Key] = new Dictionary<Identifiable.Id, int>();
				StripByMatch(idDictStat.Value, gameState.achieve.gameIdDictStatDict[idDictStat.Key], IsGuuIdentifiable);
			}
			
			//& Strip Holidays
			StripByMatch(original.holiday.eventGordos, gameState.holiday.eventGordos, IsGuuID);
			StripByMatch(original.holiday.eventEchoNoteGordos, gameState.holiday.eventEchoNoteGordos, IsGuuID);
			
			//& Strip Appearances
			StripByMatch(original.appearances.unlocks, gameState.appearances.unlocks, IsGuuIdentifiable);
			//# Strip Unlocks based on Appearance Sets
			
			StripByMatch(original.appearances.selections, gameState.appearances.selections, IsGuuIdentifiable);
			//# Strip Selections based on Appearance Sets
			
			//& Strip Instruments
			//# Strip Unlocks
			//# Strip Selection

			// TODO: Strip content from the original
		}
		
		//! Patch: SavedProfile.SaveProfile 
		internal static void SaveProfile(ProfileV07 original)
		{
			string realPath = Path.Combine(SavePath, PROFILE_FILENAME);
			GuuCore.LOGGER.Log($"Saving modded profile file {PROFILE_FILENAME}");

			ProfileV07 profile = new ProfileV07();

			// TODO: Strip content from the original

			using (MemoryStream dataStream = new MemoryStream())
			{
				profile.Write(dataStream);
				dataStream.Seek(0, SeekOrigin.Begin);
				using (FileStream fileStream = File.Create(realPath))
				{
					try
					{
						CopyStream(dataStream, fileStream);
					}
					catch (Exception e)
					{
						GuuCore.LOGGER.LogError("Failed to save modded profile file.");
						GuuCore.LOGGER.LogError(e.ParseTrace());
					}
				}
			}
			
			GuuCore.LOGGER.Log($"Saved modded profile file {PROFILE_FILENAME}");
		}
		
		//! Patch: SavedProfile.SaveSettings 
		internal static void SaveSettings(SettingsV01 original)
		{
			string realPath = Path.Combine(SavePath, SETTINGS_FILENAME);
			GuuCore.LOGGER.Log($"Saving modded settings file {SETTINGS_FILENAME}");

			SettingsV01 settings = new SettingsV01();

			// TODO: Strip content from the original

			using (MemoryStream dataStream = new MemoryStream())
			{
				settings.Write(dataStream);
				dataStream.Seek(0, SeekOrigin.Begin);
				using (FileStream fileStream = File.Create(realPath))
				{
					try
					{
						CopyStream(dataStream, fileStream);
					}
					catch (Exception e)
					{
						GuuCore.LOGGER.LogError("Failed to save modded settings file.");
						GuuCore.LOGGER.LogError(e.ParseTrace());
					}
				}
			}
			
			GuuCore.LOGGER.Log($"Saved modded settings file {SETTINGS_FILENAME}");
		}
	}
}