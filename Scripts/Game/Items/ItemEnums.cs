// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: LypyL (lypyl@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;

namespace DaggerfallWorkshop.Game.Items.ItemEnums
{
    public enum ItemType
    {
        None = -1,
        Drug = 0,
        Useless_item,
        Armor,
        Weapon,
        Magic_item,
        Artifact,
        Mens_clothing,
        Book,
        Error,  //Not sure what this is for - from Dagged.
        Useless_items2,
        Relgious_items,
        Maps,
        Womens_Clothing,
        Paintings,
        Crystals,
        Plant_ingredients1,
        Plant_ingredients2,
        Creature_ingredients1,
        Creature_ingredients2,
        Creature_ingredients3,
        Miscellaneous_ingredients,
        Mineral_ingredients,
        Miscellaneous_ingredients2,
        Transportation,
        Deeds_to_houses,
        Jewelry,
        Quest_items,
        Misc_Items,
        Gold
    }


    public enum Drugs //checked
    {
        Indulcet,
        Sursam,
        Quaesto_Vil,
        Aegrotat,
    }

    public enum UselessItems //checked
    {
        Glass_Jar,
        Glass_Bottle,
        Decanter,
        Clay_Jar,
        Small_Sack,
        Large_Sack,
        Quiver,
        Backpack,
        Small_Chest,
        Wine_Rack,
        Large_Chest,
    }

    public enum Armor   //checked
    {
        None = -1,
        Cuirass = 0,
        Gauntlets,
        Grieves,
        Left_Pauldron,
        Right_Pauldron,
        Helmet,
        Boots,
        Buckler,
        Round_Shield,
        Kite_Shield,
        Tower_Shield,
    }

    public enum Weapons  //checked
    {
        None = -1,
        Dagger,
        Tanto,
        Staff,
        Short_Sword,
        Wakizashi,
        Broard_Sword,
        Sabre,
        Long_Sword,
        Katana,
        Claymore,
        Dai_Katana,
        Mace,
        Flail,
        Warhammer,
        Battle_Axe,
        War_Axe,
        Short_Bow,
        Long_Bow,
        Arrow,
    }


    public enum MagicItemSubTypes
    {
        None = -1,
        A_Magic_Item,
    }

    public enum ArtifactsSubTypes //not sure how these work, but DEFEDIT sets these as follows
    {
        None = -1,
        Masque_of_Clavicus = 0,
        Mehrunes_Razor = 1,
        Mace_of_Molag_Bal = 2,
        Hircine_Ring = 3,
        Sanguine_Rose = 4,
        Oghma_Infinium = 5,
        Wabbajack = 6,
        Ring_of_Namira = 7,
        Skull_of_Curroption = 8,
        Azuras_Star = 9,
        Volendrung = 10,
        Warlocks_Ring = 11,
        Auriels_Bow = 12,
        Necromancers_Amulet = 13,
        Chrysamere = 14,
        Lords_Mail = 15,
        Staff_of_Magnus = 16,
        Ring_of_Khajiit = 17,
        Ebony_Mail = 18,
        Auriels_Shield = 19,
        Spell_Breaker = 20,
        Skeleton_Key = 21,
        Ebony_Blade = 22,
        Daves_Blade = 23,

    }

    public enum MensClothing  //check
    {
        None = -1,
        Straps,
        Armbands,
        Kimono,
        Fancy_Armbands,
        Sash,
        Eodoric,
        Shoes,
        Tall_Boots,
        Boots,
        Sandals,
        Casual_Pants,
        Breeches,
        Short_skirt,
        Casual_cloak,
        Formal_cloak,
        Khajit_suit,
        Dwynnen_surcoat,
        Short_tunic,
        Formal_tunic,
        Toga,
        Reversible_tunic,
        Loincloth,
        Plain_Robes,
        Priest_Robes,
        Short_Shirt,
        Short_shirt_with_belt,
        Long_shirt,
        Long_shirt_with_belt,
        Short_shirt_closed_top,
        Short_shirt_closed_top2,
        Long_shirt_closed_top,
        Long_shirt_closed_top2,
        Open_Tunic,
        Wrap,
        Long_Skirt,
        Anticlere_Surcoat,
        Challenger_Straps,
        Short_shirt_unchangeable,
        Long_shirt_unchangeable,
        Vest,
        Champion_straps,
    }

    public enum Books  //checked
    {
        None = -1,
        a_Book,
    }

    public enum ERROR           //?
    {
        None = -1,
        ERROR,

    }

    public enum UselessItems2 //checked
    {
        None = -1,
        Torch,
        Lantern,
        Bandage,
        Oil,
        Candle,
        Parchment,
    }

    public enum ReligiousItems  //checked
    {
        None = -1,
        Prayer_beads,
        Rare_symbol,
        Common_symbol,
        Belt,
        Holy_water,
        Tailsman,
        Religious_item,
        Small_statue,
        Icon,
        Scarab,
        Holy_candle,
        Holy_dagger,
        Holy_tome,


    }

    public enum Maps //checked
    {
        None = -1,
        a_Map
    }

    public enum Womens_Clothing  //checked
    {
        None = -1,
        Brassier,
        Formal_brassier,
        Peasant_blouse,
        Eodoric,
        Shoes,
        Tall_boots,
        Boots,
        Sandals,
        Casual_pants,
        Casual_cloak,
        Formal_cloak,
        Khajitt_suit,
        Formal_eodoric,
        Evening_gown,
        Day_gown,
        Casual_dress,
        Strapless_dress,
        Loincloth,
        Plain_robes,
        Priestess_robes,
        Short_shirt,
        Short_shirt_belt,
        Long_shirt,
        Long_shirt_belt,
        Short_shirt_closed,
        Short_shirt_closed_belt,
        Long_shirt_closed,
        Long_shirt_closed_belt,
        Open_tunic,
        Wrap,
        Long_skirt,
        Tights,
        Short_skirt_unchangeable,
        Long_skirt_unchangeable,
        Vest,
    }

    public enum Paintings //DEFEDIT sets subgroup to 255? ... correct group # though
    {
        None = -1,
        a_Painting,
    }

    public enum Crystals  //checked
    {
        None = -1,
        Ruby,
        Emerald,
        Sapphire,
        Diamond,
        Jade,
        Turquiose,
        Malachite,
        Amber,

    }

    public enum PlantIngredients1 //checked
    {
        None = -1,
        Twigs,
        Green_leaves,
        Red_flowers,
        Yellow_flowers,
        Root_tendrils,
        Root_bulb,
        Pine_branch,
        Green_berries,
        Red_berries,
        Yellow_berries,
        Clover,
        Red_rose,
        Yellow_rose,
        Red_poppy,
        Yellow_poppy,

    }

    public enum PlantIngredients2  //checked
    {
        None = -1,
        Twigs,
        Green_leaves,
        Red_flowers,
        Yellow_flowers,
        Root_tendrils,
        Root_bulb,
        Green_berries,
        Red_berries,
        Yellow_berries,
        Black_rose,
        White_rose,
        Black_poppy,
        White_poppy,
        Ginkgo_leaves,
        Bamboo,
        Palm,
        Aloe,
        Fig,
        Cactus,
    }

    public enum CreatureIngredients1  //checked
    {
        None = -1,
        Werewolfs_blood,
        Fairy_dragon_scales,
        Wraith_essence,
        Ectoplasm,
        Ghouls_tongue,
        Spider_venom,
        Troll_blood,
        Snake_venom,
        Grogon_snake,
        Lich_dust,
        Giant_blood,
        Basilisk_eye,
        Daedra_heart,
        Saint_hair,
        Orc_blood
    }

    public enum CreatureIngredients2  //checked
    {
        None = -1,
        Dragon_scales,
        Giant_scorpion_stinger,
        Small_scorpion_stinger,
        Mummy_wrappings,
        Gryphon_Feather,

    }

    public enum CreatureIngredients3 //checked
    {
        None = -1,
        Wearboar_tusk,
        Nymph_hair,
        Unicorn_horn,
    }

    public enum MiscIngredients1  //checked
    {
        None = -1,
        Holy_relic,
        Big_tooth,
        Medium_tooth,
        Small_tooth,
        Pure_water,
        Rain_water,
        Elixir_vitae,
        Nectar,
        Ichor,
    }

    public enum MineralIngredients  //Checked
    {
        None = -1,
        Mercury,
        Tin,
        Brass,
        Lodestone,
        Sulphur,
        Lead,
        Iron,
        Copper,
        Silver,
        Gold,
        Platinum,
    }

    public enum MiscIngredients2 //checked
    {
        None = -1,
        Ivory,
        Pearl,
    }

    public enum Transportation  //Checked
    {
        None = -1,
        Small_cart,
        Horse,
        Rowboat,
        Large_boat,
        Small_ship,
        Large_Galley

    }

    public enum Deeds  //checked
    {
        None = -1,
        Deed_to_townhouse,
        Deed_to_house,
        Deed_to_manor,

    }

    public enum Jewlry  //checked
    {
        None = -1,
        Amulet,
        Bracer,
        Ring,
        Bracelet,
        Mark,
        Torc,
        Cloth_amulet,
        Wand,
    }

    public enum Quest_Items  //checked
    {
        None = -1,
        Telescope,
        Scales,
        Globe,
        Skeleton,
        Totem,
        Dead_body,
        Mantella,
        Finger,
    }

    public enum Misc_Items  //checked
    {
        None = -1,
        Spellbook,
        Soul_trap,
        Letter_of_credit,
        Ruby,
        Recipe,
        Dead_Body,
        Deed_For_House, //checked
        Ship_Deed,
        a_Map,
    }

    public enum Gold  //checked
    {
        None = -1,
        Gold,
    }
}
