using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Eden.Core.Diagnostics;
using Guu.API;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

namespace Guu.SaveSystem
{
	/// <summary>Handles all save related actions done by Guu, provides an interface for Addons to add content to the save process</summary>
	public static partial class SaveHandler
	{
		//+ LOAD HANDLING
		//! Patch: SavedGame.LoadSummary
		internal static void LoadSummary(string summarySaveName, GameData.Summary summary)
		{
			string realPath = Path.Combine(SavePath, summarySaveName + GUU_EXTENSION);
			if (!File.Exists(realPath)) return;
			GuuCore.LOGGER.Log($"Attempting to load summary for modded save file {summarySaveName}");
			
			using (FileStream fileStream = File.Open(realPath, FileMode.Open))
			{
				using (MemoryStream dataStream = new MemoryStream())
				{
					CopyStream(fileStream, dataStream);
					dataStream.Seek(0, SeekOrigin.Begin);
					try
					{
						GameV12 modded = new GameV12();
						modded.LoadSummary(dataStream);
						if (modded.summary.iconId != Identifiable.Id.NONE && IsGuuIdentifiable(modded.summary.iconId)) summary.iconId = modded.summary.iconId;
						if (modded.summary.gameMode != PlayerState.GameMode.CLASSIC) summary.gameMode = modded.summary.gameMode;
					}
					catch (Exception e)
					{
						GuuCore.LOGGER.LogError("Failed to load summary from modded save file");
						GuuCore.LOGGER.LogError(e.ParseTrace());
					}
				}
			}
			
			GuuCore.LOGGER.Log($"Loaded summary for modded save file {summarySaveName}");
		}
		
		//! Patch: FileStorageProvider.GetGameData
		internal static void LoadFile(string name)
		{
			string realPath = Path.Combine(SavePath, name + GUU_EXTENSION);
			if (!File.Exists(realPath)) return;
			GuuCore.LOGGER.Log($"Attempting to load modded save file {name}");
			
			saveName = name;
			
			using (FileStream fileStream = File.Open(realPath, FileMode.Open))
			{
				using (MemoryStream dataStream = new MemoryStream())
				{
					CopyStream(fileStream, dataStream);
					dataStream.Seek(0, SeekOrigin.Begin);
					try
					{
						gameState = new GameV12();
						gameState.Load(dataStream);
					}
					catch (Exception e)
					{
						GuuCore.LOGGER.LogError("Failed to load modded save file");
						GuuCore.LOGGER.LogError(e.ParseTrace());
					}
				}
			}
		}

		//! Patch: SavedGame.Load
		internal static void Load(GameV12 original)
		{
			//& Merge Summary
			if (gameState.summary.iconId != Identifiable.Id.NONE && IsGuuIdentifiable(gameState.summary.iconId)) 
				original.summary.iconId = gameState.summary.iconId;
			
			if (gameState.summary.gameMode != PlayerState.GameMode.CLASSIC) 
				original.summary.gameMode = gameState.summary.gameMode;
			
			//& Merge World
			original.world.lastOfferRancherIds.AddAll(gameState.world.lastOfferRancherIds, IsGuuID);
			original.world.pendingOfferRancherIds.AddAll(gameState.world.pendingOfferRancherIds, IsGuuID);
			
			if (gameState.world.weather != AmbianceDirector.Weather.NONE) original.world.weather = gameState.world.weather;

			foreach (ExchangeDirector.OfferType type in original.world.offers.Keys)
			{
				if (!gameState.world.offers.ContainsKey(type)) continue;
				ExchangeOfferV04 ori = original.world.offers[type];
				ExchangeOfferV04 mod = gameState.world.offers[type];

				ori.requests.AddAll(mod.requests, request => IsGuuIdentifiable(request.id) || ExchangeRegistry.IsNonIdenRegistered(request.nonIdentReward));
				ori.rewards.AddAll(mod.rewards, reward => IsGuuIdentifiable(reward.id) || ExchangeRegistry.IsNonIdenRegistered(reward.nonIdentReward));

				gameState.world.offers.Remove(type);
			}
			original.world.offers.AddAll(gameState.world.offers, pair => IsGuuID(pair.Value.rancherId) || IsGuuID(pair.Value.offerId) || ExchangeRegistry.IsOfferTypeRegistered(pair.Key));
			
			original.world.econSaturations.AddAll(gameState.world.econSaturations, IsGuuIdentifiable);
			original.world.teleportNodeActivations.AddAll(gameState.world.teleportNodeActivations, IsGuuID);
			original.world.liquidSourceUnits.AddAll(gameState.world.liquidSourceUnits, IsGuuID);

			foreach (string id in original.world.gordos.Keys)
			{
				if (!gameState.world.gordos.ContainsKey(id)) continue;
				original.world.gordos[id].fashions.AddAll(gameState.world.gordos[id].fashions, IsGuuIdentifiable);
				gameState.world.gordos.Remove(id);
			}
			original.world.gordos.AddRange(gameState.world.gordos);

			foreach (string id in original.world.placedGadgets.Keys)
			{
				if (!gameState.world.placedGadgets.ContainsKey(id)) continue;
				PlacedGadgetV08 ori = original.world.placedGadgets[id];
				PlacedGadgetV08 mod = gameState.world.placedGadgets[id];

				if (mod.gadgetId != Gadget.Id.NONE) ori.gadgetId = mod.gadgetId;
				
				int i = 0;
				foreach (AmmoDataV02 ammo in mod.ammo)
				{
					if (ammo.id != Identifiable.Id.NONE && IsGuuIdentifiable(ammo.id)) ori.ammo[i].id = ammo.id;
					ori.ammo[i].emotionData.emotionData.AddRange(ammo.emotionData.emotionData);
					i++;
				}

				if (mod.baitTypeId != Identifiable.Id.NONE && IsGuuIdentifiable(mod.baitTypeId)) ori.baitTypeId = mod.baitTypeId;
				if (mod.gordoTypeId != Identifiable.Id.NONE && IsGuuIdentifiable(mod.gordoTypeId)) ori.gordoTypeId = mod.gordoTypeId;
				ori.fashions.AddAll(mod.fashions, IsGuuIdentifiable);

				if (ori.drone != null)
				{
					if (mod.drone.drone.ammo.id != Identifiable.Id.NONE && IsGuuIdentifiable(mod.drone.drone.ammo.id)) 
						ori.drone.drone.ammo.id = mod.drone.drone.ammo.id;
					ori.drone.drone.ammo.emotionData.emotionData.AddRange(mod.drone.drone.ammo.emotionData.emotionData);
					ori.drone.drone.fashions.AddAll(mod.drone.drone.fashions, IsGuuIdentifiable);
				}

				gameState.world.placedGadgets.Remove(id);
			}
			original.world.placedGadgets.AddAll(gameState.world.placedGadgets, IsGuuID);
			
			foreach (string id in original.world.treasurePods.Keys)
			{
				if (!gameState.world.treasurePods.ContainsKey(id)) continue;
				original.world.treasurePods[id].spawnQueue.AddAll(gameState.world.treasurePods[id].spawnQueue, IsGuuIdentifiable);
				gameState.world.treasurePods.Remove(id);
			}
			original.world.treasurePods.AddRange(gameState.world.treasurePods);

			original.world.switches.AddAll(gameState.world.switches, IsGuuID);
			original.world.puzzleSlotsFilled.AddAll(gameState.world.puzzleSlotsFilled, IsGuuID);
			original.world.occupiedPhaseSites.AddAll(gameState.world.occupiedPhaseSites, IsGuuID);
			original.world.quicksilverEnergyGenerators.AddAll(gameState.world.quicksilverEnergyGenerators, IsGuuID);
			original.world.oasisStates.AddAll(gameState.world.oasisStates, IsGuuID);
			original.world.activeGingerPatches.AddAll(gameState.world.activeGingerPatches, IsGuuID);
			original.world.echoNoteGordos.AddAll(gameState.world.echoNoteGordos, IsGuuID);

			//? Slimeulation
			original.world.glitch.teleporters.AddAll(gameState.world.glitch.teleporters, IsGuuID);
			original.world.glitch.nodes.AddAll(gameState.world.glitch.nodes, IsGuuID);
			original.world.glitch.impostoDirectors.AddAll(gameState.world.glitch.impostoDirectors, IsGuuID);
			original.world.glitch.impostos.AddAll(gameState.world.glitch.impostos, IsGuuID);
			original.world.glitch.slimes.AddAll(gameState.world.glitch.slimes, IsGuuID);
			original.world.glitch.storage.AddAll(gameState.world.glitch.storage, pair => IsGuuID(pair.Key) || IsGuuIdentifiable(pair.Value.id));
			
			//& Merge Player
			if (gameState.player.gameMode != PlayerState.GameMode.CLASSIC) original.player.gameMode = gameState.player.gameMode;
			if (gameState.player.gameIconId != Identifiable.Id.NONE && IsGuuIdentifiable(gameState.player.gameIconId)) 
				original.player.gameIconId = gameState.player.gameIconId;
			
			original.player.upgrades.AddAll(gameState.player.upgrades, IsGuuUpgrade);
			
			foreach (PlayerState.AmmoMode mode in original.player.ammo.Keys)
			{
				if (!gameState.player.ammo.ContainsKey(mode)) continue;
				
				int i = 0;
				foreach (AmmoDataV02 ammo in gameState.player.ammo[mode])
				{
					if (ammo.id != Identifiable.Id.NONE && IsGuuIdentifiable(ammo.id)) original.player.ammo[mode][i].id = ammo.id;
					original.player.ammo[mode][i].emotionData.emotionData.AddRange(ammo.emotionData.emotionData);
					i++;
				}

				gameState.player.ammo.Remove(mode);
			}
			original.player.ammo.AddAll(gameState.player.ammo, pair => AmmoRegistry.IsAmmoModeRegistered(pair.Key));

			foreach (MailV02 mail in original.player.mail)
			{
				MailV02 mod = gameState.player.mail.Find(m => m.messageKey.Equals(mail.messageKey));
				if (mod != null) continue;

				if (!mod.messageKey.Equals(string.Empty) && MailRegistry.IsMailRegistered(mod.messageKey))
				{
					mail.messageKey = mod.messageKey;
					mail.mailType = mod.mailType;
					mod.mailType = MailDirector.Type.UPGRADE;
				}
				
				if (mod.mailType != MailDirector.Type.UPGRADE && MailRegistry.IsTypeRegistered(mod.mailType)) mail.mailType = mod.mailType;
				
				gameState.player.mail.RemoveAll(m => m.messageKey.Equals(mail.messageKey));
			}
			
			original.player.availUpgrades.AddAll(gameState.player.availUpgrades, IsGuuUpgrade);
			original.player.upgradeLocks.AddAll(gameState.player.upgradeLocks, IsGuuUpgrade);
			original.player.progress.AddAll(gameState.player.progress, pair => ProgressRegistry.IsTypeRegistered(pair.Key));
			original.player.delayedProgress.AddAll(gameState.player.delayedProgress, pair => ProgressRegistry.IsTrackerRegistered(pair.Key));
			original.player.blueprints.AddAll(gameState.player.blueprints, IsGuuGadget);
			original.player.availBlueprints.AddAll(gameState.player.availBlueprints, IsGuuGadget);
			original.player.blueprintLocks.AddAll(gameState.player.blueprintLocks, IsGuuGadget);
			original.player.gadgets.AddAll(gameState.player.gadgets, IsGuuGadget);
			original.player.craftMatCounts.AddAll(gameState.player.craftMatCounts, IsGuuIdentifiable);
			
			original.player.unlockedZoneMaps.AddRange(gameState.player.unlockedZoneMaps);

			if (gameState.player.regionSetId != RegionRegistry.RegionSetId.UNSET)
			{
				original.player.playerPos = gameState.player.playerPos;
				original.player.regionSetId = gameState.player.regionSetId;
			}
			
			//? Decorizer
			original.player.decorizer.contents.AddAll(gameState.player.decorizer.contents, IsGuuIdentifiable);
			original.player.decorizer.settings.AddAll(gameState.player.decorizer.settings, pair => IsGuuIdentifiable(pair.Value.selected));
			
			//& Merge Ranch
			foreach (LandPlotV08 plot in original.ranch.plots)
			{
				LandPlotV08 mPlot = gameState.ranch.plots.Find(p => p.id.Equals(plot.id));
				if (mPlot == null) continue;

				if (mPlot.typeId != LandPlot.Id.NONE) plot.typeId = mPlot.typeId;
				if (mPlot.attachedId != SpawnResource.Id.NONE) plot.attachedId = mPlot.attachedId;
				plot.upgrades.AddRange(plot.upgrades);
				
				foreach (SiloStorage.StorageType type in plot.siloAmmo.Keys)
				{
					if (!mPlot.siloAmmo.ContainsKey(type)) continue;
				
					int i = 0;
					foreach (AmmoDataV02 ammo in mPlot.siloAmmo[type])
					{
						if (ammo.id != Identifiable.Id.NONE && IsGuuIdentifiable(ammo.id)) plot.siloAmmo[type][i].id = ammo.id;
						plot.siloAmmo[type][i].emotionData.emotionData.AddRange(ammo.emotionData.emotionData);
						i++;
					}

					mPlot.siloAmmo.Remove(type);
				}
				plot.siloAmmo.AddAll(mPlot.siloAmmo, pair => AmmoRegistry.IsStorageTypeRegistered(pair.Key));

				gameState.ranch.plots.RemoveAll(p => p.id.Equals(plot.id));
			}
			original.ranch.plots.AddRange(gameState.ranch.plots);
			
			original.ranch.accessDoorStates.AddAll(gameState.ranch.accessDoorStates, IsGuuID);
			original.ranch.palettes.AddAll(gameState.ranch.palettes, pair => ChromaPackRegistry.IsTypeRegistered(pair.Key) || ChromaPackRegistry.IsPaletteRegistered(pair.Value));
			original.ranch.ranchFastForward.AddAll(gameState.ranch.ranchFastForward, IsGuuID);
			
			//& Merge Actors
			foreach (ActorDataV09 actor in original.actors)
			{
				ActorDataV09 mActor = gameState.actors.Find(a => a.actorId == actor.actorId);
				if (mActor == null) continue;
				
				actor.fashions.AddAll(mActor.fashions, IsGuuIdentifiable);
				actor.emotions.emotionData.AddRange(mActor.emotions.emotionData);
				if (mActor.regionSetId != RegionRegistry.RegionSetId.UNSET)
				{
					actor.pos = mActor.pos;
					actor.regionSetId = mActor.regionSetId;
				}

				gameState.actors.RemoveAll(a => a.actorId == actor.actorId);
			}
			original.actors.AddRange(gameState.actors);
			
			//& Merge Pedia
			original.pedia.unlockedIds.AddRange(gameState.pedia.unlockedIds);
			original.pedia.completedTuts.AddRange(gameState.pedia.completedTuts);
			original.pedia.popupQueue.AddRange(gameState.pedia.popupQueue);
			
			//& Merge Achievements
			original.achieve.gameFloatStatDict.AddRange(gameState.achieve.gameFloatStatDict);
			original.achieve.gameDoubleStatDict.AddRange(gameState.achieve.gameDoubleStatDict);
			original.achieve.gameIntStatDict.AddRange(gameState.achieve.gameIntStatDict);
			
			foreach (AchievementsDirector.GameIdDictStat stat in original.achieve.gameIdDictStatDict.Keys)
			{
				if (!gameState.achieve.gameIdDictStatDict.ContainsKey(stat)) continue;
				original.achieve.gameIdDictStatDict[stat].AddAll(gameState.achieve.gameIdDictStatDict[stat], IsGuuIdentifiable);
				gameState.achieve.gameIdDictStatDict.Remove(stat);
			}
			original.achieve.gameIdDictStatDict.AddRange(gameState.achieve.gameIdDictStatDict);
			
			//& Merge Holiday
			original.holiday.eventGordos.AddAll(gameState.holiday.eventGordos, IsGuuID);
			original.holiday.eventEchoNoteGordos.AddAll(gameState.holiday.eventEchoNoteGordos, IsGuuID);
			
			//& Merge Appearances
			foreach (Identifiable.Id unlKey in original.appearances.unlocks.Keys)
			{
				if (!gameState.appearances.unlocks.ContainsKey(unlKey)) continue;
				original.appearances.unlocks[unlKey].AddRange(gameState.appearances.unlocks[unlKey]);
				gameState.appearances.unlocks.Remove(unlKey);
			}
			original.appearances.unlocks.AddAll(gameState.appearances.unlocks, IsGuuIdentifiable);

			foreach (Identifiable.Id selKey in original.appearances.selections.Keys)
			{
				if (!gameState.appearances.selections.ContainsKey(selKey)) continue;
				original.appearances.selections[selKey] = gameState.appearances.selections[selKey];
				gameState.appearances.selections.Remove(selKey);
			}
			original.appearances.selections.AddAll(gameState.appearances.selections, IsGuuIdentifiable);
			
			//& Merge Instrument
			original.instrument.unlocks.AddRange(gameState.instrument.unlocks);
			if (gameState.instrument.selection != InstrumentModel.Instrument.NONE) original.instrument.selection = gameState.instrument.selection;
			
			//& Runs the loading actions registered to SaveHandler
			OnLoad?.Invoke(gameState, saveName);
			
			//& Finalize the process
			saveName = null;
			gameState = null;
			
			GuuCore.LOGGER.Log($"Loaded and injected modded save file {saveName}");
		}

		//! Patch: SavedProfile.LoadProfile
		internal static void LoadProfile(ProfileV07 original)
		{
			string realPath = Path.Combine(SavePath, PROFILE_FILENAME);
			if (!File.Exists(realPath)) return;
			GuuCore.LOGGER.Log($"Attempting to load modded profile file {PROFILE_FILENAME}");
			
			using (FileStream fileStream = File.Open(realPath, FileMode.Open))
			{
				using (MemoryStream dataStream = new MemoryStream())
				{
					CopyStream(fileStream, dataStream);
					dataStream.Seek(0, SeekOrigin.Begin);
					ProfileV07 profile = new ProfileV07();
					try
					{
						profile.Load(dataStream);
						original.achievements.progress.counts.AddRange(profile.achievements.progress.counts);
						original.achievements.progress.events.AddRange(profile.achievements.progress.events);
						original.achievements.progress.lists.AddRange(profile.achievements.progress.lists);
						original.achievements.earnedAchievements.AddRange(profile.achievements.earnedAchievements);
					}
					catch (Exception e)
					{
						GuuCore.LOGGER.LogError("Failed to load modded profile file.");
						GuuCore.LOGGER.LogError(e.ParseTrace());
					}
				}
			}
			
			GuuCore.LOGGER.Log($"Loaded modded profile file {PROFILE_FILENAME}");
		}
		
		//! Patch: SavedProfile.LoadSettings
		internal static void LoadSettings(SettingsV01 original)
		{
			string realPath = Path.Combine(SavePath, SETTINGS_FILENAME);
			if (!File.Exists(realPath)) return;
			GuuCore.LOGGER.Log($"Attempting to load modded settings file {SETTINGS_FILENAME}");
			
			using (FileStream fileStream = File.Open(realPath, FileMode.Open))
			{
				using (MemoryStream dataStream = new MemoryStream())
				{
					CopyStream(fileStream, dataStream);
					dataStream.Seek(0, SeekOrigin.Begin);
					SettingsV01 settings = new SettingsV01();
					try
					{
						settings.Load(dataStream);
						original.options.bindings.bindings.AddRange(settings.options.bindings.bindings);
					}
					catch (Exception e)
					{
						GuuCore.LOGGER.LogError("Failed to load modded settings file.");
						GuuCore.LOGGER.LogError(e.ParseTrace());
					}
				}
			}
			
			GuuCore.LOGGER.Log($"Loaded modded settings file {PROFILE_FILENAME}");
		}
	}
}