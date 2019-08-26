using UnityEngine;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Very simple class to store and destroy a temporary Unity object, e.g. a Texture2D.
    /// Ensures existing object is destroyed when replacing reference with another object.
    /// Provides a flag to protect assets (e.g. modded textures) as these do not need to be destroyed this way.
    /// </summary>
    public class TempObject
    {
        Object obj = null;
        bool asset = false;

        // Gets or sets object
        public Object Target
        {
            get { return obj; }
            set { SetObject(value); }
        }

        // Gets or sets object as texture
        public Texture2D Texture
        {
            get { return obj as Texture2D; }
            set { SetObject(value); }
        }

        // Gets or sets asset flag
        // Cannot call Object.Destroy() assets
        public bool IsAsset
        {
            get { return asset; }
            set { asset = value; }
        }

        // Destroy object if exists and not an asset
        public void Clear()
        {
            if (obj && !asset)
            {
                Object.Destroy(obj);
                obj = null;
            }
        }

        // Set current object and clear existing object first
        void SetObject(Object newObj)
        {
            Clear();
            obj = newObj;
        }
    }
}