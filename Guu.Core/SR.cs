using JetBrains.Annotations;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

namespace Guu
{
	/// <summary>Holds references to all base classes in the game. You can find all directors here</summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public static class SR
	{
		//+ CONDITIONS
		/// <summary>Is the game context loaded?</summary>
		public static bool GameContextLoaded => GameContext.Instance != null;

		/// <summary>Is the scene context loaded?</summary>
		public static bool SceneContextLoaded => SceneContext.Instance != null;

		//+ GAME CONTEXT
		/// <summary>Direct call to the <see cref="GameContext.Instance"/> in game context</summary>
		public static GameContext Game => GameContext.Instance;
		
		/// <summary>Direct call to the <see cref="GameContext.SlimeDefinitions"/> in game context</summary>
		public static SlimeDefinitions SlimeDefs => GameContext.Instance.SlimeDefinitions;
		
		/// <summary>Direct call to the <see cref="GameContext.LookupDirector"/> in game context</summary>
		public static LookupDirector LookupDir => GameContext.Instance.LookupDirector;
		
		/// <summary>Direct call to the <see cref="GameContext.AutoSaveDirector"/> in game context</summary>
		public static AutoSaveDirector AutoSaveDir => GameContext.Instance.AutoSaveDirector;
		
		/// <summary>Direct call to the <see cref="GameContext.SlimeShaders"/> in game context</summary>
		public static SlimeShaders SlimeShaders => GameContext.Instance.SlimeShaders;
		
		/// <summary>Direct call to the <see cref="GameContext.MessageDirector"/> in game context</summary>
		public static MessageDirector MessageDir => GameContext.Instance.MessageDirector;
		
		/// <summary>Direct call to the <see cref="GameContext.UITemplates"/> in game context</summary>
		public static UITemplates UITemplates => GameContext.Instance.UITemplates;
		
		/// <summary>Direct call to the <see cref="GameContext.InputDirector"/> in game context</summary>
		public static InputDirector InputDir => GameContext.Instance.InputDirector;
		
		/// <summary>Direct call to the <see cref="GameContext.SlimeDefinitions"/> in game context</summary>
		public static MusicDirector MusicDir => GameContext.Instance.MusicDirector;
		
		/// <summary>Direct call to the <see cref="GameContext.OptionsDirector"/> in game context</summary>
		public static OptionsDirector OptionsDir => GameContext.Instance.OptionsDirector;
		
		/// <summary>Direct call to the <see cref="GameContext.GifRecorder"/> in game context</summary>
		public static GifRecorder GifRec => GameContext.Instance.GifRecorder;
		
		/// <summary>Direct call to the <see cref="GameContext.PerformanceTracker"/> in game context</summary>
		public static PerformanceTracker PerfTracker => GameContext.Instance.PerformanceTracker;
		
		/// <summary>Direct call to the <see cref="GameContext.RichPresenceDirector"/> in game context</summary>
		public static RichPresence.Director RPDir => GameContext.Instance.RichPresenceDirector;
		
		/// <summary>Direct call to the <see cref="GameContext.ToyDirector"/> in game context</summary>
		public static ToyDirector ToyDir => GameContext.Instance.ToyDirector;
		
		/// <summary>Direct call to the <see cref="GameContext.DLCDirector"/> in game context</summary>
		public static DLCDirector DLCDir => GameContext.Instance.DLCDirector;
		
		//+ SCENE CONTEXT
		/// <summary>Direct call to the <see cref="SceneContext.Instance"/> in scene context</summary>
		public static SceneContext Scene => SceneContext.Instance;
		
		/// <summary>Direct call to the <see cref="SceneContext.SlimeAppearanceDirector"/> in scene context</summary>
		public static SlimeAppearanceDirector SlimeAppDir => SceneContext.Instance.SlimeAppearanceDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.PlayerState"/> in scene context</summary>
		public static PlayerState PlayerState => SceneContext.Instance.PlayerState;
		
		/// <summary>Direct call to the <see cref="SceneContext.EconomyDirector"/> in scene context</summary>
		public static EconomyDirector EcoDir => SceneContext.Instance.EconomyDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.ExchangeDirector"/> in scene context</summary>
		public static ExchangeDirector ExchangeDir => SceneContext.Instance.ExchangeDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.PediaDirector"/> in scene context</summary>
		public static PediaDirector PediaDir => SceneContext.Instance.PediaDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.TutorialDirector"/> in scene context</summary>
		public static TutorialDirector TutorialDir => SceneContext.Instance.TutorialDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.MailDirector"/> in scene context</summary>
		public static MailDirector MailDir => SceneContext.Instance.MailDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.ModDirector"/> in scene context</summary>
		public static ModDirector ModDir => SceneContext.Instance.ModDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.ProgressDirector"/> in scene context</summary>
		public static ProgressDirector ProgressDir => SceneContext.Instance.ProgressDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.AchievementsDirector"/> in scene context</summary>
		public static AchievementsDirector AchieveDir => SceneContext.Instance.AchievementsDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.TimeDirector"/> in scene context</summary>
		public static TimeDirector TimeDir => SceneContext.Instance.TimeDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.AmbianceDirector"/> in scene context</summary>
		public static AmbianceDirector AmbianceDir => SceneContext.Instance.AmbianceDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.GadgetDirector"/> in scene context</summary>
		public static GadgetDirector GadgetDir => SceneContext.Instance.GadgetDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.PopupDirector"/> in scene context</summary>
		public static PopupDirector PopupDir => SceneContext.Instance.PopupDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.TeleportNetwork"/> in scene context</summary>
		public static TeleportNetwork TeleportNet  => SceneContext.Instance.TeleportNetwork;
		
		/// <summary>Direct call to the <see cref="SceneContext.SceneParticleDirector"/> in scene context</summary>
		public static SceneParticleDirector ScenePartDir => SceneContext.Instance.SceneParticleDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.MetadataDirector"/> in scene context</summary>
		public static MetadataDirector MetaDir => SceneContext.Instance.MetadataDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.ActorRegistry"/> in scene context</summary>
		public static ActorRegistry ActorReg => SceneContext.Instance.ActorRegistry;
		
		/// <summary>Direct call to the <see cref="SceneContext.RegionRegistry"/> in scene context</summary>
		public static RegionRegistry RegionReg => SceneContext.Instance.RegionRegistry;
		
		/// <summary>Direct call to the <see cref="SceneContext.GameModel"/> in scene context</summary>
		public static GameModel GameModel => SceneContext.Instance.GameModel;
		
		/// <summary>Direct call to the <see cref="SceneContext.RanchDirector"/> in scene context</summary>
		public static RanchDirector RanchDir => SceneContext.Instance.RanchDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.HolidayDirector"/> in scene context</summary>
		public static HolidayDirector HolidayDir => SceneContext.Instance.HolidayDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.InstrumentDirector"/> in scene context</summary>
		public static InstrumentDirector InstrDir => SceneContext.Instance.InstrumentDirector;
		
		/// <summary>Direct call to the <see cref="SceneContext.GameModeConfig"/> in scene context</summary>
		public static GameModeConfig GMConfig => SceneContext.Instance.GameModeConfig;
		
		/// <summary>Direct call to the <see cref="SceneContext.Player"/> in scene context</summary>
		public static GameObject Player => SceneContext.Instance.Player;
		
		/// <summary>Direct call to the <see cref="SceneContext.PlayerZoneTracker"/> in scene context</summary>
		public static PlayerZoneTracker PZoneTracker => SceneContext.Instance.PlayerZoneTracker;
		
		//+ METHODS
		/// <summary>Checks if an argument is present on the command line arguments</summary>
		public static bool IsCmdArgumentPresent(string arg) => GuuCore.CMD_ARGS.Contains(arg);
	}
}