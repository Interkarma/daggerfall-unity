// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public struct CatalogItem
    {
        public string ID;
        public string Label;
        public string Category;
        public string Subcategory;
        public string Tags;

        public CatalogItem(string ID)
        {
            this.ID = ID;
            this.Label = ID;
            this.Category = "";
            this.Subcategory = "";
            this.Tags = "";
        }

        public CatalogItem(string ID, string label)
        {
            this.ID = ID;
            this.Label = label;
            this.Category = "";
            this.Subcategory = "";
            this.Tags = "";
        }

        public CatalogItem(string ID, string label, string Category)
        {
            this.ID = ID;
            this.Label = label;
            this.Category = Category;
            this.Subcategory = "";
            this.Tags = "";
        }

        public CatalogItem(string ID, string Label, string Category, string Subcategory)
        {
            this.ID = ID;
            this.Label = Label;
            this.Category = Category;
            this.Subcategory = Subcategory;
            this.Tags = "";
        }

        public CatalogItem(string ID, string Label, string Category, string Subcategory, string Tags)
        {
            this.ID = ID;
            this.Label = Label;
            this.Category = Category;
            this.Subcategory = Subcategory;
            this.Tags = Tags;
        }
    }
}