// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System;
using System.Collections;
using DaggerfallWorkshop;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Demo
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

        public SongFiles[] CityDaySongs = _cityDaySongs;
        public SongFiles[] CityNightSongs = _cityNightSongs;
        public SongFiles[] DungeonExteriorSongs = _dungeonExteriorSongs;
        public SongFiles[] DungeonInteriorSongs = _dungeonInteriorSongs;
        public SongFiles[] GraveyardSongs = _graveyardSongs;
        public SongFiles[] GuildSongs = _guildSongs;
        public SongFiles[] InteriorSongs = _interiorSongs;
        public SongFiles[] PalaceSongs = _palaceSongs;
        public SongFiles[] ShopSongs = _shopSongs;
        public SongFiles[] TavernSongs = _tavernSongs;
        public SongFiles[] WeatherRainSongs = _weatherRainSongs;
        public SongFiles[] WeatherSnowSongs = _weatherSnowSongs;
        public SongFiles[] WildernessSongs = _wildernessSongs;

        DaggerfallUnity dfUnity;
        DaggerfallSongPlayer songPlayer;
        PlayerEnterExit playerEnterExit;
        PlayerWeather playerWeather;

        PlayerMusicEnvironment currentPlayerMusicEnvironment;
        PlayerMusicEnvironment lastPlayerMusicEnvironment;
        PlayerMusicWeather currentPlayerMusicWeather;
        PlayerMusicWeather lastPlayerMusicWeather;

        SongFiles[] currentPlaylist;
        SongFiles currentSong;
        int currentSongIndex = 0;
        bool playSong = true;

        #endregion

        #region Enumerations

        enum PlayerMusicEnvironment
        {
            City,
            DungeonExterior,
            DungeonInterior,
            Graveyard,
            Guild,
            Interior,
            Palace,
            Shop,
            Tavern,
            Wilderness,
        }

        enum PlayerMusicWeather
        {
            Normal,
            Rain,
            Snow,
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

            // Try to find local player GPS if not set
            if (LocalPlayerGPS == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player)
                {
                    LocalPlayerGPS = player.GetComponent<PlayerGPS>();
                }
            }

            // Get required player components
            if (LocalPlayerGPS)
            {
                playerEnterExit = LocalPlayerGPS.GetComponent<PlayerEnterExit>();
                playerWeather = LocalPlayerGPS.GetComponent<PlayerWeather>();
            }
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

            // Switch playlists if context changes or if not playing then select a new song
            bool overrideSong = false;
            if (currentPlayerMusicEnvironment != lastPlayerMusicEnvironment || 
                currentPlayerMusicWeather != lastPlayerMusicWeather ||
                (!songPlayer.IsPlaying && playSong))
            {
                // Keep song if playing the same weather, but not when entering dungeons
                if (currentPlayerMusicWeather != PlayerMusicWeather.Normal &&
                    currentPlayerMusicWeather == lastPlayerMusicWeather &&
                    currentPlayerMusicEnvironment != PlayerMusicEnvironment.DungeonInterior)
                {
                    return;
                }

                // Change song
                lastPlayerMusicEnvironment = currentPlayerMusicEnvironment;
                lastPlayerMusicWeather = currentPlayerMusicWeather;
                AssignPlaylist();
                SelectCurrentSong();
                overrideSong = true;
            }

            // Play song
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

        #region Private Methods

        void SelectCurrentSong()
        {
            if (currentPlaylist == null)
                return;

            UnityEngine.Random.seed = System.DateTime.Now.Millisecond;
            int index = UnityEngine.Random.Range(0, currentPlaylist.Length);
            currentSong = currentPlaylist[index];
            currentSongIndex = index;
        }

        void PlayCurrentSong(bool forcePlay = false)
        {
            // Do nothing if already playing this song or play disabled
            if ((songPlayer.Song == currentSong || !playSong) && !forcePlay)
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
                            currentPlayerMusicEnvironment = PlayerMusicEnvironment.DungeonExterior;
                            break;
                        case DFRegion.LocationTypes.GraveyardCommon:
                        case DFRegion.LocationTypes.GraveyardForgotten:
                            currentPlayerMusicEnvironment = PlayerMusicEnvironment.Graveyard;
                            break;
                        case DFRegion.LocationTypes.HomeFarms:
                        case DFRegion.LocationTypes.HomePoor:
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
                if (playerEnterExit.IsPlayerInsideDungeonPalace)
                    currentPlayerMusicEnvironment = PlayerMusicEnvironment.Palace;
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
                        currentPlayerMusicEnvironment = PlayerMusicEnvironment.Guild;
                        break;
                    case DFLocation.BuildingTypes.Palace:
                        currentPlayerMusicEnvironment = PlayerMusicEnvironment.Palace;
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
                case PlayerWeather.WeatherTypes.Rain_Normal:
                    currentPlayerMusicWeather = PlayerMusicWeather.Rain;
                    break;
                case PlayerWeather.WeatherTypes.Snow_Normal:
                    currentPlayerMusicWeather = PlayerMusicWeather.Snow;
                    break;
                default:
                    currentPlayerMusicWeather = PlayerMusicWeather.Normal;
                    break;
            }
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
            if (currentPlayerMusicEnvironment == PlayerMusicEnvironment.City)
            {
                // Day/night
                if (!dfUnity.WorldTime.Now.IsNight)
                    currentPlaylist = CityDaySongs;
                else
                    currentPlaylist = CityNightSongs;

                return;
            }

            // Environment
            switch (currentPlayerMusicEnvironment)
            {
                case PlayerMusicEnvironment.DungeonExterior:
                    currentPlaylist = DungeonExteriorSongs;
                    break;
                case PlayerMusicEnvironment.DungeonInterior:
                    currentPlaylist = DungeonInteriorSongs;
                    break;
                case PlayerMusicEnvironment.Graveyard:
                    currentPlaylist = GraveyardSongs;
                    break;
                case PlayerMusicEnvironment.Guild:
                    currentPlaylist = GuildSongs;
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
                case PlayerMusicEnvironment.Wilderness:
                    currentPlaylist = WildernessSongs;
                    break;
            }
        }

        #endregion

        #region Song Playlists

        // City - Day
        static SongFiles[] _cityDaySongs = new SongFiles[]
        {
            SongFiles.song_02,
            SongFiles.song_03,
            SongFiles.song_06,
            SongFiles.song_12,
            SongFiles.song_15,
            SongFiles.song_20,
            SongFiles.song_22,
            SongFiles.song_29,
            SongFiles.song_gday___d,
            SongFiles.song_ggood,
            SongFiles.song_sunnyday,
            SongFiles.song_gsunny2,
            SongFiles.song_swimming,
        };

        // City - night
        static SongFiles[] _cityNightSongs = new SongFiles[]
        {
            SongFiles.song_07,
            SongFiles.song_10,
            SongFiles.song_11,
            SongFiles.song_13,
            SongFiles.song_16,
            SongFiles.song_21,
            SongFiles.song_25,
        };

        // Dungeon - Exterior
        static SongFiles[] _dungeonExteriorSongs = new SongFiles[]
        {
            SongFiles.song_04,
            SongFiles.song_07,
            SongFiles.song_10,
            SongFiles.song_13,
            SongFiles.song_18,
            SongFiles.song_21,
            SongFiles.song_gcurse,
            SongFiles.song_gruins,
        };

        // Dungeon - Interior
        static SongFiles[] _dungeonInteriorSongs = new SongFiles[]
        {
            SongFiles.song_d1,
            SongFiles.song_d10,
            SongFiles.song_d2,
            SongFiles.song_d3,
            SongFiles.song_d4,
            SongFiles.song_d5,
            SongFiles.song_d6,
            SongFiles.song_d7,
            SongFiles.song_d8,
            SongFiles.song_d9,
            SongFiles.song_dungeon,
            SongFiles.song_gdngn10,
            SongFiles.song_gdngn11,
            SongFiles.song_gdungn4,
            SongFiles.song_gdungn9,
        };

        // Graveyards
        static SongFiles[] _graveyardSongs = new SongFiles[]
        {
            SongFiles.song_04,
            SongFiles.song_07,
            SongFiles.song_10,
            SongFiles.song_13,
            SongFiles.song_21,
            SongFiles.song_geerie,
            SongFiles.song_gruins,
        };

        // Guilds
        static SongFiles[] _guildSongs = new SongFiles[]
        {
            SongFiles.song_gmage_3,
            SongFiles.song_magic_2,
        };

        // Interiors
        static SongFiles[] _interiorSongs = new SongFiles[]
        {
            SongFiles.song_23,
            SongFiles.song_gneut,
        };

        // Palace
        static SongFiles[] _palaceSongs = new SongFiles[]
        {
            SongFiles.song_gpalac,
        };

        // Shops
        static SongFiles[] _shopSongs = new SongFiles[]
        {
            SongFiles.song_gshop,
            SongFiles.song_square_2,
        };

        // Taverns
        static SongFiles[] _tavernSongs = new SongFiles[]
        {
            SongFiles.song_folk1,
            SongFiles.song_folk2,
            SongFiles.song_folk3,
            SongFiles.song_tavern,
        };

        // Weather - Raining
        static SongFiles[] _weatherRainSongs = new SongFiles[]
        {
            SongFiles.song_12,
            SongFiles.song_gbad,
            SongFiles.song_overcast,
            SongFiles.song_overlong,        // Long version of overcast
            SongFiles.song_raining,
        };

        // Weather - Snowing
        static SongFiles[] _weatherSnowSongs = new SongFiles[]
        {
            SongFiles.song_08,
            SongFiles.song_gsnow__b,
            SongFiles.song_snowing,
            SongFiles.song_oversnow,
        };

        // Wilderness
        static SongFiles[] _wildernessSongs = new SongFiles[]
        {
            SongFiles.song_11,
            SongFiles.song_13,
            SongFiles.song_18,
            SongFiles.song_21,
            SongFiles.song_gcurse,
            SongFiles.song_gruins,
        };

        //// Uncategorized
        //static SongFiles[] UnCategorizedSongs = new SongFiles[]
        //{
        //    SongFiles.song_05,              // dungeon?
        //    SongFiles.song_09,              // unknown
        //    SongFiles.song_17,              // unknown
        //    SongFiles.song_28,              // unknown
        //    SongFiles.song_30,              // dungeon?
        //    SongFiles.song_sneaking,        // unknown
        //    SongFiles.song_gsneak2,         // unknown
        //    SongFiles.song_sneakng2,        // unknown
        //    SongFiles.song_dungeon5,        // Not sure these dungeont tracks fit the mood (or if Daggerfall uses)
        //    SongFiles.song_dungeon6,
        //    SongFiles.song_dungeon7,
        //    SongFiles.song_dungeon8,
        //    SongFiles.song_dungeon9,
        //};

        #endregion
    }
}