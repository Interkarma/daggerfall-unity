// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Bookshelves in building interiors which allow reading books.
    /// </summary>
    public class DaggerfallBookshelf : MonoBehaviour
    {
        private List<int> books = null;

        void Start()
        {
            if (books == null)
            {
                // Populate bookshelf with up to 10 random books.
                books = new List<int>();
                for (int i=0; i<10; i++)
                {
                    int bookNum = Random.Range(0, 111);
                    string bookName = DaggerfallUnity.Instance.ItemHelper.getBookNameByID(bookNum, string.Empty);
                    if (bookName != string.Empty)
                        books.Add(bookNum);
                }
            }
        }

        public void ReadBook()
        {
            // Check permission to access bookshelf if inside a guild or temple
            PlayerGPS.DiscoveredBuilding buildingData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;
            // TODO: Faction membership check here when implemented, just check building type for now..
            int factionID = buildingData.factionID;
            Debug.Log("Faction ID = " + factionID);
            if (buildingData.buildingType == DFLocation.BuildingTypes.GuildHall ||
                buildingData.buildingType == DFLocation.BuildingTypes.Temple)
            {
                DaggerfallUI.MessageBox(HardStrings.serviceDenied);
            }
            else
            {
                // Show book picker loaded with list of books on this shelf
                IUserInterfaceManager uiManager = DaggerfallUI.UIManager;
                DaggerfallListPickerWindow bookPicker = new DaggerfallListPickerWindow(uiManager, uiManager.TopWindow);
                bookPicker.OnItemPicked += BookShelf_OnItemPicked;

                foreach (int bookNum in books)
                {
                    string bookName = DaggerfallUnity.Instance.ItemHelper.getBookNameByID(bookNum, string.Empty);
                    if (bookName != string.Empty)
                        bookPicker.ListBox.AddItem(bookName);
                }

                uiManager.PushWindow(bookPicker);
            }
        }

        public void BookShelf_OnItemPicked(int index, string bookName)
        {
            DaggerfallUnity.Instance.TextProvider.OpenBook(books[index]);
            DaggerfallUI.UIManager.PopWindow();
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenBookReaderWindow);
        }
    }
}