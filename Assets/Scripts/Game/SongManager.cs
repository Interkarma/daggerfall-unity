// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Numidium
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using DaggerfallWorkshop;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Weather;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Peer this with DaggerfallSongPlayer to play music based on climate, location, time of day, etc.
    /// Starting lists can be edited in Inspector to add/remove items.
    /// There is some overlap in songs lists where the moods are similar.
    /// </summary>
    [RequireComponent(typeof(DaggerfallSongPlayer))]
    public class SongManager : MonoBehaviour
    {
        #region Fields

        public PlayerGPS LocalPlayerGPS;            // Must be peered with PlayerWeather and PlayerEnterExit for full support
        public StreamingWorld StreamingWorld;

        public SongFiles[] DungeonInteriorSongs = _dungeonSongs;
        public SongFiles[] SunnySongs = _sunnySongs;
        public SongFiles[] CloudySongs = _cloudySongs;
        public SongFiles[] OvercastSongs = _overcastSongs;
        public SongFiles[] RainSongs = _rainSongs;
        public SongFiles[] SnowSongs = _snowSongs;
        public SongFiles[] TempleSongs = _templeSongs;
        public SongFiles[] TavernSongs = _tavernSongs;
        public SongFiles[] NightSongs = _nightSongs;
        public SongFiles[] ShopSongs = _shopSongs;
        public SongFiles[] MagesGuildSongs = _magesGuildSongs;
        public SongFiles[] InteriorSongs = _interiorSongs;
        public SongFiles[] PalaceSongs = _palaceSongs;
        public SongFiles[] CastleSongs = _castleSongs;
        public SongFiles[] CourtSongs = _courtSongs;
        public SongFiles[] SneakingSongs = _sneakingSongs;

        DaggerfallUnity dfUnity;
        DaggerfallSongPlayer songPlayer;
        PlayerEnterExit playerEnterExit;
        PlayerWeather playerWeather;
        PlayerEntity playerEntity;

        struct PlayerMusicContext
        {
            public PlayerMusicEnvironment environment;
            public PlayerMusicWeather weather;
            public PlayerMusicTime time;
            public bool arrested;

            //minimize GC alloc of struct.Equals(object o) with this method instead
            public bool Equals(PlayerMusicContext pmc) {
                return  environment == pmc.environment
                        && weather == pmc.weather
                        && time == pmc.time
                        && arrested == pmc.arrested;
            }
        }

        PlayerMusicContext currentContext;
        PlayerMusicContext lastContext;

        SongFiles[] currentPlaylist;
        SongFiles currentSong;
        int currentSongIndex = 0;
        bool playSong = true;

        #endregion

        #region Enumerations

        enum PlayerMusicEnvironment
        {
            Castle,
            City,
            DungeonExterior,
            DungeonInterior,
            Graveyard,
            MagesGuild,
            Interior,
            Palace,
            Shop,
            Tavern,
            Temple,
            Wilderness,
        }

        enum PlayerMusicWeather
        {
            Sunny,
            Cloudy,
            Overcast,
            Rain,
            Snow,
        }

        enum PlayerMusicTime
        {
            Day,
            Night,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets song player.
        /// </summary>
        public DaggerfallSongPlayer SongPlayer
        {
            get { return songPlayer; }
        }

        #endregion

        #region Unity

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            songPlayer = GetComponent<DaggerfallSongPlayer>();

            // Get local player GPS if not set
            if (LocalPlayerGPS == null)
                LocalPlayerGPS = GameManager.Instance.PlayerGPS;

            // Get player entity
            if (playerEntity == null)
                playerEntity = GameManager.Instance.PlayerEntity;

            // Get streaming world if not set
            if (StreamingWorld == null)
                StreamingWorld = GameManager.Instance.StreamingWorld;

            // Get required player components
            if (LocalPlayerGPS)
            {
                playerEnterExit = LocalPlayerGPS.GetComponent<PlayerEnterExit>();
                playerWeather = LocalPlayerGPS.GetComponent<PlayerWeather>();
            }

            // Use alternate music if set
            if (DaggerfallUnity.Settings.AlternateMusic)
            {
                DungeonInteriorSongs = _dungeonSongsFM;
                SunnySongs = _sunnySongsFM;
                CloudySongs = _cloudySongsFM;
                OvercastSongs = _overcastSongsFM;
                RainSongs = _weatherRainSongsFM;
                SnowSongs = _weatherSnowSongsFM;
                TempleSongs = _templeSongsFM;
                TavernSongs = _tavernSongsFM;
                NightSongs = _nightSongsFM;
                ShopSongs = _shopSongsFM;
                MagesGuildSongs = _magesGuildSongsFM;
                InteriorSongs = _interiorSongsFM;
                PalaceSongs = _palaceSongsFM;
                CastleSongs = _castleSongsFM;
                CourtSongs = _courtSongsFM;
                SneakingSongs = _sneakingSongsFM;
            }

            PlayerEnterExit.OnTransitionDungeonInterior += PlayerEnterExit_OnTransitionDungeonInterior;
        }

        void Update()
        {
            UpdateSong(false);
        }

        void UpdateSong(bool forceChange = false)
        {
            if (!songPlayer)
                return;

            // Play song if no song was playing or if playlist changed
            // Switch to another random song to prevent fatigue of hearing same song repeatedly

            PlayerMusicUpdateContext();

            // Update current playlist if context changed
            if (!currentContext.Equals(lastContext) || (!songPlayer.IsPlaying && playSong) || forceChange)
            {
                lastContext = currentContext;

                SongFiles[] lastPlaylist = currentPlaylist;
                // Get playlist for current context
                AssignPlaylist();

                // If current playlist is different from last playlist, pick a song from the current playlist
                if (currentPlaylist != lastPlaylist || forceChange)
                {
                    PlayAnotherSong();
                    return;
                }
            }

            if (!songPlayer.IsPlaying)
                PlayCurrentSong();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start playing.
        /// </summary>
        public void StartPlaying()
        {
            playSong = true;
            PlayCurrentSong(true);
        }

        /// <summary>
        /// Stop playing.
        /// </summary>
        public void StopPlaying()
        {
            playSong = false;
            songPlayer.Stop();
        }

        /// <summary>
        /// Toggle play state.
        /// </summary>
        public void TogglePlay()
        {
            if (playSong)
                StopPlaying();
            else
                StartPlaying();
        }

        /// <summary>
        /// Play next song in current playlist.
        /// </summary>
        public void PlayNextSong()
        {
            if (currentPlaylist == null)
                return;

            if (++currentSongIndex >= currentPlaylist.Length)
                currentSongIndex = 0;

            currentSong = currentPlaylist[currentSongIndex];
            PlayCurrentSong();
        }

        /// <summary>
        /// Play previous song in current playlist.
        /// </summary>
        public void PlayPreviousSong()
        {
            if (currentPlaylist == null)
                return;

            if (--currentSongIndex < 0)
                currentSongIndex = currentPlaylist.Length - 1;

            currentSong = currentPlaylist[currentSongIndex];
            PlayCurrentSong();
        }

        #endregion

        #region Private Methods

        void SelectCurrentSong()
        {
            if (currentPlaylist == null || currentPlaylist.Length == 0)
                return;

            int index = 0;
            // General MIDI song selection
            {
                uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
                DFRandom.srand(gameMinutes / 1440);
                uint random = DFRandom.rand();
                if (currentPlaylist == NightSongs)
                    index = (int)(random % NightSongs.Length);
                else if (currentPlaylist == SunnySongs)
                    index = (int)(random % SunnySongs.Length);
                else if (currentPlaylist == CloudySongs)
                    index = (int)(random % CloudySongs.Length);
                else if (currentPlaylist == OvercastSongs)
                    index = (int)(random % OvercastSongs.Length);
                else if (currentPlaylist == RainSongs)
                    index = (int)(random % RainSongs.Length);
                else if (currentPlaylist == SnowSongs)
                    index = (int)(random % SnowSongs.Length);
                else if (currentPlaylist == TempleSongs && playerEnterExit)
                {
                    byte[] templeFactions = { 0x52, 0x54, 0x58, 0x5C, 0x5E, 0x62, 0x6A, 0x24 };
                    uint factionOfPlayerEnvironment = playerEnterExit.FactionID;
                    index = Array.IndexOf(templeFactions, (byte)factionOfPlayerEnvironment);
                    if (index < 0)
                    {
                        byte[] godFactions = { 0x15, 0x16, 0x18, 0x1A, 0x1B, 0x1D, 0x21, 0x23 };
                        index = Array.IndexOf(godFactions, (byte)factionOfPlayerEnvironment);
                    }
                }
                else if (currentPlaylist == TavernSongs)
                {
                    index = (int)(gameMinutes / 1440 % TavernSongs.Length);
                }
                else if (currentPlaylist == DungeonInteriorSongs)
                {
                    PlayerGPS gps = GameManager.Instance.PlayerGPS;
                    ushort unknown2 = 0;
                    int region = 0;
                    if (gps.HasCurrentLocation)
                    {
                        unknown2 = (ushort)gps.CurrentLocation.Dungeon.RecordElement.Header.Unknown2;
                        region = gps.CurrentRegionIndex;
                    }
                    DFRandom.srand(unknown2 ^ ((byte)region << 8));
                    random = DFRandom.rand();
                    index = (int)(random % DungeonInteriorSongs.Length);
                }
                else if (currentPlaylist == SneakingSongs || currentPlaylist == MagesGuildSongs)
                {
                    index = UnityEngine.Random.Range(0, currentPlaylist.Length);
                }
            }
            currentSong = currentPlaylist[index];
            currentSongIndex = index;
        }

        void PlayAnotherSong()
        {
            SelectCurrentSong();
            PlayCurrentSong();
        }

        void PlayCurrentSong(bool forcePlay = false)
        {
            // Do nothing if already playing this song or play disabled
            if (((songPlayer.Song == currentSong && songPlayer.IsPlaying) || !playSong) && !forcePlay)
                return;

            songPlayer.Song = currentSong;
            songPlayer.Play();
        }

        void PlayerMusicUpdateContext()
        {
            UpdatePlayerMusicEnvironment();
            UpdatePlayerMusicWeather();
            UpdatePlayerMusicTime();
            UpdatePlayerMusicArrested();
        }

        void UpdatePlayerMusicEnvironment()
        {
            if (!playerEnterExit || !LocalPlayerGPS || !dfUnity)
                return;

            // Exteriors
            if (!playerEnterExit.IsPlayerInside)
            {
                if (LocalPlayerGPS.IsPlayerInLocationRect)
                {
                    switch (LocalPlayerGPS.CurrentLocationType)
                    {
                        case DFRegion.LocationTypes.DungeonKeep:
                        case DFRegion.LocationTypes.DungeonLabyrinth:
                        case DFRegion.LocationTypes.DungeonRuin:
                        case DFRegion.LocationTypes.Coven:
                        case DFRegion.LocationTypes.HomePoor:
                            currentContext.environment = PlayerMusicEnvironment.DungeonExterior;
                            break;
                        case DFRegion.LocationTypes.Graveyard:
                            currentContext.environment = PlayerMusicEnvironment.Graveyard;
                            break;
                        case DFRegion.LocationTypes.HomeFarms:
                        case DFRegion.LocationTypes.HomeWealthy:
                        case DFRegion.LocationTypes.Tavern:
                        case DFRegion.LocationTypes.TownCity:
                        case DFRegion.LocationTypes.TownHamlet:
                        case DFRegion.LocationTypes.TownVillage:
                        case DFRegion.LocationTypes.ReligionTemple:
                            currentContext.environment = PlayerMusicEnvironment.City;
                            break;
                        default:
                            currentContext.environment = PlayerMusicEnvironment.Wilderness;
                            break;
                    }
                }
                else
                {
                    currentContext.environment = PlayerMusicEnvironment.Wilderness;
                }

                return;
            }

            // Dungeons
            if (playerEnterExit.IsPlayerInsideDungeon)
            {
                if (playerEnterExit.IsPlayerInsideDungeonCastle)
                    currentContext.environment = PlayerMusicEnvironment.Castle;
                else
                    currentContext.environment = PlayerMusicEnvironment.DungeonInterior;

                return;
            }

            // Interiors
            if (playerEnterExit.IsPlayerInside)
            {
                switch (playerEnterExit.BuildingType)
                {
                    case DFLocation.BuildingTypes.Alchemist:
                    case DFLocation.BuildingTypes.Armorer:
                    case DFLocation.BuildingTypes.Bank:
                    case DFLocation.BuildingTypes.Bookseller:
                    case DFLocation.BuildingTypes.ClothingStore:
                    case DFLocation.BuildingTypes.FurnitureStore:
                    case DFLocation.BuildingTypes.GemStore:
                    case DFLocation.BuildingTypes.GeneralStore:
                    case DFLocation.BuildingTypes.Library:
                    case DFLocation.BuildingTypes.PawnShop:
                    case DFLocation.BuildingTypes.WeaponSmith:
                        currentContext.environment = PlayerMusicEnvironment.Shop;
                        break;
                    case DFLocation.BuildingTypes.Tavern:
                        currentContext.environment = PlayerMusicEnvironment.Tavern;
                        break;
                    case DFLocation.BuildingTypes.GuildHall:
                        if (playerEnterExit.FactionID == (int)FactionFile.FactionIDs.The_Mages_Guild)
                        {
                            currentContext.environment = PlayerMusicEnvironment.MagesGuild;
                        }
                        else
                        {
                            currentContext.environment = PlayerMusicEnvironment.Interior;
                        }
                        break;
                    case DFLocation.BuildingTypes.Palace:
                        currentContext.environment = PlayerMusicEnvironment.Palace;
                        break;
                    case DFLocation.BuildingTypes.Temple:
                        currentContext.environment = PlayerMusicEnvironment.Temple;
                        break;
                    default:
                        currentContext.environment = PlayerMusicEnvironment.Interior;
                        break;
                }

                return;
            }
        }

        void UpdatePlayerMusicWeather()
        {
            if (!playerWeather)
                return;

            switch (playerWeather.WeatherType)
            {
                case WeatherType.Cloudy:
                    currentContext.weather = PlayerMusicWeather.Cloudy;
                    break;
                case WeatherType.Overcast:
                case WeatherType.Fog:
                    currentContext.weather = PlayerMusicWeather.Overcast;
                    break;
                case WeatherType.Rain:
                case WeatherType.Thunder:
                    currentContext.weather = PlayerMusicWeather.Rain;
                    break;
                case WeatherType.Snow:
                    currentContext.weather = PlayerMusicWeather.Snow;
                    break;
                default:
                    currentContext.weather = PlayerMusicWeather.Sunny;
                    break;
            }
        }

        void UpdatePlayerMusicTime()
        {
            if (DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.IsDay)
                currentContext.time = PlayerMusicTime.Day;
            else
                currentContext.time = PlayerMusicTime.Night;
        }

        void UpdatePlayerMusicArrested()
        {
            currentContext.arrested = playerEntity.Arrested;
        }

        void AssignPlaylist()
        {
            // Court window
            if (currentContext.arrested)
            {
                currentPlaylist = CourtSongs;
                return;
            }

            // Weather music in cities and wilderness at day
            if (!dfUnity.WorldTime.Now.IsNight &&
                (currentContext.environment == PlayerMusicEnvironment.City || currentContext.environment == PlayerMusicEnvironment.Wilderness))
            {
                switch (currentContext.weather)
                {
                    case PlayerMusicWeather.Sunny:
                        currentPlaylist = SunnySongs;
                        break;
                    case PlayerMusicWeather.Cloudy:
                        currentPlaylist = CloudySongs;
                        break;
                    case PlayerMusicWeather.Overcast:
                        currentPlaylist = OvercastSongs;
                        break;
                    case PlayerMusicWeather.Rain:
                        currentPlaylist = RainSongs;
                        break;
                    case PlayerMusicWeather.Snow:
                        currentPlaylist = SnowSongs;
                        break;
                }
                return;
            }

            // Cities and wilderness
            if (currentContext.environment == PlayerMusicEnvironment.City || currentContext.environment == PlayerMusicEnvironment.Wilderness)
            {
                // Night songs
                if (dfUnity.WorldTime.Now.IsNight)
                    currentPlaylist = NightSongs;
                return;
            }

            // Environment
            switch (currentContext.environment)
            {
                case PlayerMusicEnvironment.Castle:
                    currentPlaylist = CastleSongs;
                    break;
                case PlayerMusicEnvironment.DungeonExterior:
                    currentPlaylist = NightSongs;
                    break;
                case PlayerMusicEnvironment.DungeonInterior:
                    currentPlaylist = DungeonInteriorSongs;
                    break;
                case PlayerMusicEnvironment.Graveyard:
                    currentPlaylist = NightSongs;
                    break;
                case PlayerMusicEnvironment.MagesGuild:
                    currentPlaylist = MagesGuildSongs;
                    break;
                case PlayerMusicEnvironment.Interior:
                    currentPlaylist = InteriorSongs;
                    break;
                case PlayerMusicEnvironment.Palace:
                    currentPlaylist = PalaceSongs;
                    break;
                case PlayerMusicEnvironment.Shop:
                    currentPlaylist = ShopSongs;
                    break;
                case PlayerMusicEnvironment.Tavern:
                    currentPlaylist = TavernSongs;
                    break;
                case PlayerMusicEnvironment.Temple:
                    currentPlaylist = TempleSongs;
                    break;
            }
        }

        #endregion

        #region Events


        private void PlayerEnterExit_OnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            if (!songPlayer)
                return;

            // Dungeons have a dedicated SongManager and only one play list, so SongManager would never change song
            if (currentContext.environment == PlayerMusicEnvironment.DungeonInterior)
            {
                // Immediately change song (instead of waiting for next Update()) to avoid transitory states
                UpdateSong(true);
            }
        }

        #endregion

        #region Song Playlists

        // Dungeon
        static SongFiles[] _dungeonSongs = new SongFiles[]
        {
            SongFiles.song_dungeon,
            SongFiles.song_dungeon5,
            SongFiles.song_dungeon6,
            SongFiles.song_dungeon7,
            SongFiles.song_dungeon8,
            SongFiles.song_dungeon9,
            SongFiles.song_gdngn10,
            SongFiles.song_gdngn11,
            SongFiles.song_gdungn4,
            SongFiles.song_gdungn9,
            SongFiles.song_04,
            SongFiles.song_05,
            SongFiles.song_07,
            SongFiles.song_15,
            SongFiles.song_28,
        };

        // Sunny
        static SongFiles[] _sunnySongs = new SongFiles[]
        {
            SongFiles.song_gday___d,
            SongFiles.song_swimming,
            SongFiles.song_gsunny2,
            SongFiles.song_sunnyday,
            SongFiles.song_02,
            SongFiles.song_03,
            SongFiles.song_22,
        };

        // Sunny FM Version
        static SongFiles[] _sunnySongsFM = new SongFiles[]
        {
            SongFiles.song_fday___d,
            SongFiles.song_fm_swim2,
            SongFiles.song_fm_sunny,
            SongFiles.song_02fm,
            SongFiles.song_03fm,
            SongFiles.song_22fm,
        };

        // Cloudy
        static SongFiles[] _cloudySongs = new SongFiles[]
        {
            SongFiles.song_gday___d,
            SongFiles.song_swimming,
            SongFiles.song_gsunny2,
            SongFiles.song_sunnyday,
            SongFiles.song_02,
            SongFiles.song_03,
            SongFiles.song_22,
            SongFiles.song_29,
            SongFiles.song_12,
        };

        // Cloudy FM
        static SongFiles[] _cloudySongsFM = new SongFiles[]
{
            SongFiles.song_fday___d,
            SongFiles.song_fm_swim2,
            SongFiles.song_fm_sunny,
            SongFiles.song_02fm,
            SongFiles.song_03fm,
            SongFiles.song_22fm,
            SongFiles.song_29fm,
            SongFiles.song_12fm,
};

        // Overcast/Fog
        static SongFiles[] _overcastSongs = new SongFiles[]
        {
            SongFiles.song_29,
            SongFiles.song_12,
            SongFiles.song_13,
            SongFiles.song_gpalac,
            SongFiles.song_overcast,
        };

        // Overcast/Fog FM Version
        static SongFiles[] _overcastSongsFM = new SongFiles[]
        {
            SongFiles.song_29fm,
            SongFiles.song_12fm,
            SongFiles.song_13fm,
            SongFiles.song_fpalac,
            SongFiles.song_fmover_c,
        };

        // Rain
        static SongFiles[] _rainSongs = new SongFiles[]
        {
            SongFiles.song_overlong,        // Long version of overcast
            SongFiles.song_raining,
            SongFiles.song_08,
        };

        // Snow
        static SongFiles[] _snowSongs = new SongFiles[]
        {
            SongFiles.song_20,
            SongFiles.song_gsnow__b,
            SongFiles.song_oversnow,
            SongFiles.song_snowing,         // Not used in classic
        };

        // Sneaking - Not used in classic
        static SongFiles[] _sneakingSongs = new SongFiles[]
        {
            SongFiles.song_gsneak2,
            SongFiles.song_sneaking,
            SongFiles.song_sneakng2,
            SongFiles.song_16,
            SongFiles.song_09,
            SongFiles.song_25,
            SongFiles.song_30,
        };

        // Temple
        static SongFiles[] _templeSongs = new SongFiles[]
        {
            SongFiles.song_ggood,
            SongFiles.song_gbad,
            SongFiles.song_ggood,
            SongFiles.song_gneut,
            SongFiles.song_gbad,
            SongFiles.song_ggood,
            SongFiles.song_gbad,
            SongFiles.song_gneut,
        };

        // Tavern
        static SongFiles[] _tavernSongs = new SongFiles[]
        {
            SongFiles.song_square_2,
            SongFiles.song_tavern,
            SongFiles.song_folk1,
            SongFiles.song_folk2,
            SongFiles.song_folk3,
        };

        // Night
        static SongFiles[] _nightSongs = new SongFiles[]
        {
            SongFiles.song_10,
            SongFiles.song_11,
            SongFiles.song_gcurse,
            SongFiles.song_geerie,
            SongFiles.song_gruins,
            SongFiles.song_18,
            SongFiles.song_21,          // For general midi song_10 is duplicated here in Daggerfall classic, although song_21fm is used in FM mode.
        };

        // Dungeon FM version
        static SongFiles[] _dungeonSongsFM = new SongFiles[]
        {
            SongFiles.song_fm_dngn1,
            SongFiles.song_fm_dngn1,
            SongFiles.song_fm_dngn2,
            SongFiles.song_fm_dngn3,
            SongFiles.song_fm_dngn4,
            SongFiles.song_fm_dngn5,
            SongFiles.song_fdngn10,
            SongFiles.song_fdngn11,
            SongFiles.song_fdungn4,
            SongFiles.song_fdungn9,
            SongFiles.song_04fm,
            SongFiles.song_05fm,
            SongFiles.song_07fm,
            SongFiles.song_15fm,
            SongFiles.song_15fm,
        };

        // Day FM version
        static SongFiles[] _daySongsFM = new SongFiles[]
        {
            SongFiles.song_fday___d,
            SongFiles.song_fm_swim2,
            SongFiles.song_fm_sunny,
            SongFiles.song_02fm,
            SongFiles.song_03fm,
            SongFiles.song_22fm,
            SongFiles.song_29fm,
            SongFiles.song_12fm,
            SongFiles.song_13fm,
            SongFiles.song_fpalac,
        };

        // Weather - Raining FM version
        static SongFiles[] _weatherRainSongsFM = new SongFiles[]
        {
            SongFiles.song_fmover_c,
            SongFiles.song_fm_rain,
            SongFiles.song_08fm,
        };

        // Weather - Snowing FM version
        static SongFiles[] _weatherSnowSongsFM = new SongFiles[]
        {
            SongFiles.song_20fm,
            SongFiles.song_fsnow__b,
            SongFiles.song_fmover_s,
        };

        // Sneaking FM version
        static SongFiles[] _sneakingSongsFM = new SongFiles[]
        {
            SongFiles.song_fsneak2,
            SongFiles.song_fmsneak2,        // Used in Arena when trespassing in homes
            SongFiles.song_fsneak2,
            SongFiles.song_16fm,
            SongFiles.song_09fm,
            SongFiles.song_25fm,
            SongFiles.song_30fm,
        };

        // Temple FM version
        static SongFiles[] _templeSongsFM = new SongFiles[]
        {
            SongFiles.song_fgood,
            SongFiles.song_fbad,
            SongFiles.song_fgood,
            SongFiles.song_fneut,
            SongFiles.song_fbad,
            SongFiles.song_fgood,
            SongFiles.song_fbad,
            SongFiles.song_fneut,
        };

        // Tavern FM version
        static SongFiles[] _tavernSongsFM = new SongFiles[]
        {
            SongFiles.song_fm_sqr_2,
        };

        // Night FM version
        static SongFiles[] _nightSongsFM = new SongFiles[]
        {
            SongFiles.song_11fm,
            SongFiles.song_fcurse,
            SongFiles.song_feerie,
            SongFiles.song_fruins,
            SongFiles.song_18fm,
            SongFiles.song_21fm,
        };

        // Unused dungeon music
        static SongFiles[] _unusedDungeonSongs = new SongFiles[]
        {
            SongFiles.song_d1,
            SongFiles.song_d2,
            SongFiles.song_d3,
            SongFiles.song_d4,
            SongFiles.song_d5,
            SongFiles.song_d6,
            SongFiles.song_d7,
            SongFiles.song_d8,
            SongFiles.song_d9,
            SongFiles.song_d10,
        };

        // Unused dungeon music FM version
        static SongFiles[] _unusedDungeonSongsFM = new SongFiles[]
        {
            SongFiles.song_d1fm,
            SongFiles.song_d2fm,
            SongFiles.song_d3fm,
            SongFiles.song_d4fm,
            SongFiles.song_d5fm,
            SongFiles.song_d6fm,
            SongFiles.song_d7fm,
            SongFiles.song_d8fm,
            SongFiles.song_d9fm,
            SongFiles.song_d10fm,
        };

        // Shop
        static SongFiles[] _shopSongs = new SongFiles[]
        {
            SongFiles.song_gshop,
        };

        // Shop FM version
        static SongFiles[] _shopSongsFM = new SongFiles[]
        {
            SongFiles.song_fm_sqr_2,
        };

        // Mages Guild
        static SongFiles[] _magesGuildSongs = new SongFiles[]
        {
            SongFiles.song_gmage_3,
            SongFiles.song_magic_2,
        };

        // Mages Guild FM version
        static SongFiles[] _magesGuildSongsFM = new SongFiles[]
        {
            SongFiles.song_fm_nite3,
        };

        // Interior
        static SongFiles[] _interiorSongs = new SongFiles[]
        {
            SongFiles.song_23,
        };

        // Interior FM version
        static SongFiles[] _interiorSongsFM = new SongFiles[]
        {
            SongFiles.song_23fm,
        };

        // Not used in classic. There is unused code to play it in knightly orders
        static SongFiles[] _unusedKnightSong = new SongFiles[]
        {  
            SongFiles.song_17,
        };

        // FM version of above
        static SongFiles[] _unusedKnightSongFM = new SongFiles[]
        {
            SongFiles.song_17fm,
        };

        // Palace
        static SongFiles[] _palaceSongs = new SongFiles[]
        {
            SongFiles.song_06,
        };

        // Palace FM version
        static SongFiles[] _palaceSongsFM = new SongFiles[]
        {
            SongFiles.song_06fm,
        };

        // Castle
        static SongFiles[] _castleSongs = new SongFiles[]
        {
            SongFiles.song_gpalac,
        };

        // Castle FM Version
        static SongFiles[] _castleSongsFM = new SongFiles[]
        {
            SongFiles.song_fpalac,
        };

        // Court
        static SongFiles[] _courtSongs = new SongFiles[]
        {
            SongFiles.song_11,
        };

        // Court FM Version
        static SongFiles[] _courtSongsFM = new SongFiles[]
        {
            SongFiles.song_11fm,
        };

        #endregion
    }
}