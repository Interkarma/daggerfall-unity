// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
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
        public SongFiles[] DaySongs = _daySongs;
        public SongFiles[] WeatherRainSongs = _weatherRainSongs;
        public SongFiles[] WeatherSnowSongs = _weatherSnowSongs;
        public SongFiles[] TempleSongs = _templeSongs;
        public SongFiles[] TavernSongs = _tavernSongs;
        public SongFiles[] NightSongs = _nightSongs;
        public SongFiles[] ShopSongs = _shopSongs;
        public SongFiles[] MagesGuildSongs = _magesGuildSongs;
        public SongFiles[] InteriorSongs = _interiorSongs;
        public SongFiles[] PalaceSongs = _palaceSongs;
        public SongFiles[] CastleSongs = _castleSongs;

        DaggerfallUnity dfUnity;
        DaggerfallSongPlayer songPlayer;
        PlayerEnterExit playerEnterExit;
        PlayerWeather playerWeather;

        PlayerMusicEnvironment currentPlayerMusicEnvironment;
        PlayerMusicEnvironment lastPlayerMusicEnvironment;
        PlayerMusicWeather currentPlayerMusicWeather;
        PlayerMusicWeather lastPlayerMusicWeather;
        PlayerMusicTime currentPlayerMusicTime;
        PlayerMusicTime lastPlayerMusicTime;

        SongFiles[] currentPlaylist;
        SongFiles[] lastPlaylist;
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
            Normal,
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

            // Get streaming world if not set
            if (StreamingWorld == null)
                StreamingWorld = GameManager.Instance.StreamingWorld;

            // Get required player components
            if (LocalPlayerGPS)
            {
                playerEnterExit = LocalPlayerGPS.GetComponent<PlayerEnterExit>();
                playerWeather = LocalPlayerGPS.GetComponent<PlayerWeather>();
            }

            // Shuffle song on load or fast travel
            StreamingWorld.OnInitWorld += StreamingWorld_OnInitWorld;
        }

        void Update()
        {
            if (!songPlayer)
                return;

            // If streaming world is set, we can ignore track changes before init complete
            // This helps prevent music starting during first load or on wrong playlist
            if (StreamingWorld)
            {
                if (StreamingWorld.IsInit)
                    return;
            }

            // Update context
            UpdatePlayerMusicEnvironment();
            UpdatePlayerMusicWeather();
            UpdatePlayerMusicTime();

            // Update current playlist if context changed
            bool overrideSong = false;
            if (currentPlayerMusicEnvironment != lastPlayerMusicEnvironment || 
                currentPlayerMusicWeather != lastPlayerMusicWeather ||
                currentPlayerMusicTime != lastPlayerMusicTime ||
                (!songPlayer.IsPlaying && playSong))
            {
                lastPlayerMusicEnvironment = currentPlayerMusicEnvironment;
                lastPlayerMusicWeather = currentPlayerMusicWeather;
                lastPlayerMusicTime = currentPlayerMusicTime;
                lastPlaylist = currentPlaylist;

                // Get playlist for current context
                AssignPlaylist();

                // If current playlist is different from last playlist, pick a song from the current playlist
                if (currentPlaylist != lastPlaylist)
                {
                    SelectCurrentSong();
                    overrideSong = true;
                }
            }

            // Play song if no song was playing or if playlist changed
            // Switch to another random song to prevent fatigue of hearing same song repeatedly
            if (!songPlayer.IsPlaying || overrideSong)
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

        /// <summary>
        /// Plays a random song from the current playlist.
        /// </summary>
        public void PlayRandomSong()
        {
            SelectCurrentSong();
            PlayCurrentSong();
        }

        #endregion

        #region Event Handlers

        private void StreamingWorld_OnInitWorld()
        {
            PlayRandomSong();
        }

        #endregion

        #region Private Methods

        void SelectCurrentSong()
        {
            if (currentPlaylist == null)
                return;

            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            int index = UnityEngine.Random.Range(0, currentPlaylist.Length);
            currentSong = currentPlaylist[index];
            currentSongIndex = index;
        }

        void PlayCurrentSong(bool forcePlay = false)
        {
            // Do nothing if already playing this song or play disabled
            if (((songPlayer.Song == currentSong && songPlayer.IsPlaying) || !playSong) && !forcePlay)
                return;

            songPlayer.Song = currentSong;
            songPlayer.Play();
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
                            currentPlayerMusicEnvironment = PlayerMusicEnvironment.DungeonExterior;
                            break;
                        case DFRegion.LocationTypes.Graveyard:
                            currentPlayerMusicEnvironment = PlayerMusicEnvironment.Graveyard;
                            break;
                        case DFRegion.LocationTypes.HomeFarms:
                        case DFRegion.LocationTypes.HomeWealthy:
                        case DFRegion.LocationTypes.Tavern:
                        case DFRegion.LocationTypes.TownCity:
                        case DFRegion.LocationTypes.TownHamlet:
                        case DFRegion.LocationTypes.TownVillage:
                        case DFRegion.LocationTypes.ReligionTemple:
                            currentPlayerMusicEnvironment = PlayerMusicEnvironment.City;
                            break;
                        default:
                            currentPlayerMusicEnvironment = PlayerMusicEnvironment.Wilderness;
                            break;
                    }
                }
                else
                {
                    currentPlayerMusicEnvironment = PlayerMusicEnvironment.Wilderness;
                }

                return;
            }

            // Dungeons
            if (playerEnterExit.IsPlayerInsideDungeon)
            {
                if (playerEnterExit.IsPlayerInsideDungeonCastle)
                    currentPlayerMusicEnvironment = PlayerMusicEnvironment.Castle;
                else
                    currentPlayerMusicEnvironment = PlayerMusicEnvironment.DungeonInterior;

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
                        currentPlayerMusicEnvironment = PlayerMusicEnvironment.Shop;
                        break;
                    case DFLocation.BuildingTypes.Tavern:
                        currentPlayerMusicEnvironment = PlayerMusicEnvironment.Tavern;
                        break;
                    case DFLocation.BuildingTypes.GuildHall:
                        if (playerEnterExit.FactionID == 40)
                        {
                            currentPlayerMusicEnvironment = PlayerMusicEnvironment.MagesGuild;
                        }
                        else
                        {
                            currentPlayerMusicEnvironment = PlayerMusicEnvironment.Interior;
                        }
                        break;
                    case DFLocation.BuildingTypes.Palace:
                        currentPlayerMusicEnvironment = PlayerMusicEnvironment.Palace;
                        break;
                    case DFLocation.BuildingTypes.Temple:
                        currentPlayerMusicEnvironment = PlayerMusicEnvironment.Temple;
                        break;
                    default:
                        currentPlayerMusicEnvironment = PlayerMusicEnvironment.Interior;
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
                case WeatherType.Overcast:
                case WeatherType.Fog:
                case WeatherType.Rain:
                case WeatherType.Thunder:
                    currentPlayerMusicWeather = PlayerMusicWeather.Rain;
                    break;
                case WeatherType.Snow:
                    currentPlayerMusicWeather = PlayerMusicWeather.Snow;
                    break;
                default:
                    currentPlayerMusicWeather = PlayerMusicWeather.Normal;
                    break;
            }
        }

        void UpdatePlayerMusicTime()
        {
            if (DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.IsDay)
                currentPlayerMusicTime = PlayerMusicTime.Day;
            else
                currentPlayerMusicTime = PlayerMusicTime.Night;
        }

        void AssignPlaylist()
        {
            // Weather music in cities and wilderness at day
            if (!dfUnity.WorldTime.Now.IsNight &&
                currentPlayerMusicWeather != PlayerMusicWeather.Normal &&
                (currentPlayerMusicEnvironment == PlayerMusicEnvironment.City || currentPlayerMusicEnvironment == PlayerMusicEnvironment.Wilderness))
            {
                switch (currentPlayerMusicWeather)
                {
                    case PlayerMusicWeather.Rain:
                        currentPlaylist = WeatherRainSongs;
                        break;
                    case PlayerMusicWeather.Snow:
                        currentPlaylist = WeatherSnowSongs;
                        break;
                }
                return;
            }

            // Cities
            if (currentPlayerMusicEnvironment == PlayerMusicEnvironment.City || currentPlayerMusicEnvironment == PlayerMusicEnvironment.Wilderness)
            {
                // Day/night
                if (!dfUnity.WorldTime.Now.IsNight)
                    currentPlaylist = DaySongs;
                else
                    currentPlaylist = NightSongs;

                return;
            }

            // Environment
            switch (currentPlayerMusicEnvironment)
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

        // Day
        static SongFiles[] _daySongs = new SongFiles[]
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
            SongFiles.song_13,
            SongFiles.song_gpalac,
        };

        // Weather - Raining
        static SongFiles[] _weatherRainSongs = new SongFiles[]
        {
            SongFiles.song_overcast,
            SongFiles.song_overlong,        // Long version of overcast
            SongFiles.song_raining,
            SongFiles.song_08,
        };

        // Weather - Snowing
        static SongFiles[] _weatherSnowSongs = new SongFiles[]
        {
            SongFiles.song_20,
            SongFiles.song_gsnow__b,
            SongFiles.song_oversnow,
            SongFiles.song_snowing,
        };

        // Sneaking? These are next to each other in FALL.EXE but seem to be unused in-game
        /*static SongFiles[] _sneakingSongs = new SongFiles[]
        {
            SongFiles.song_gsneak2,
            SongFiles.song_sneaking,
            SongFiles.song_sneakng2,        // Used in Arena when trespassing in homes
            SongFiles.song_16,
            SongFiles.song_09,
            SongFiles.song_25,
            SongFiles.song_30,
        };*/

        // Temple
        static SongFiles[] _templeSongs = new SongFiles[]
        {
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
            SongFiles.song_21,          // Missing in Daggerfall classic. Only the FM version is used there.
        };

        // Dungeon FM version
        /*static SongFiles[] _dungeonSongsFM = new SongFiles[]
        {
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

        // Sneaking? FM version. These are next to each other in FALL.EXE but seem to be unused in-game.
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

        // Unused dungeon music?
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

        // Unused dungeon music? FM version
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
        };*/

        // Shop
        static SongFiles[] _shopSongs = new SongFiles[]
        {
            SongFiles.song_gshop,
        };

        // Shop FM version
        /*static SongFiles[] _shopSongsFM = new SongFiles[]
        {
            SongFiles.song_fm_sqr_2,
        };*/

        // Mages Guild
        static SongFiles[] _magesGuildSongs = new SongFiles[]
        {
            SongFiles.song_gmage_3,
            SongFiles.song_magic_2,
        };

        // Mages Guild FM version
        /*static SongFiles[] _magesGuildSongsFM = new SongFiles[]
        {
            SongFiles.song_fm_nite3,
        };*/

        // Interior
        static SongFiles[] _interiorSongs = new SongFiles[]
        {
            SongFiles.song_23,
        };

        // Interior FM version
        /*static SongFiles[] _interiorSongsFM = new SongFiles[]
        {
            SongFiles.song_23fm,
        };*/

        // Unknown. Doesn't seem to be used in final product
        /*static SongFiles[] _unknownSong = new SongFiles[]
        {  
            SongFiles.song_17,
        };

        // Unknown. Doesn't seem to be used in final product
        static SongFiles[] _unknownSongFM = new SongFiles[]
        {
            SongFiles.song_17fm,
        };*/

        // Palace
        static SongFiles[] _palaceSongs = new SongFiles[]
        {
            SongFiles.song_06,
        };

        // Palace FM version
        /*static SongFiles[] _palaceSongsFM = new SongFiles[]
        {
            SongFiles.song_06fm,
        };*/

        // Castle
        static SongFiles[] _castleSongs = new SongFiles[]
        {
            SongFiles.song_gpalac,
        };

        #endregion
    }
}