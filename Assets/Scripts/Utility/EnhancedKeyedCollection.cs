// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FullSerializer;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Hastable based collection which can be accessed by index or key. Does not support serialization.
    /// </summary>
    public class ImmediateKeyedCollection<T1, T2> : EnhancedKeyedCollection<T1, T2>
        where T2 : class
    {
        protected readonly Func<T2, T1> getKeyCallback;

        /// <summary>
        /// Make a new collection whose keys are embedded in the values.
        /// </summary>
        /// <param name="getKeyCallback">Lambda expression which gets key from value.</param>
        /// <param name="dictionaryCreationThreshold">Lookup dictionary is created when Count is this number.</param>
        public ImmediateKeyedCollection(Func<T2, T1> getKeyCallback, int dictionaryCreationThreshold = 0)
            :base(null, dictionaryCreationThreshold)
        {
            this.getKeyCallback = getKeyCallback;
        }

        protected override T1 GetKeyForItem(T2 item)
        {
            return getKeyCallback(item);
        }
    }

    /// <summary>
    /// Hastable based collection which can be accessed by index or key.
    /// Derive and override GetKeyForItem() to use. If serialization is required use KeyedCollectionConverter.
    /// </summary>
    public abstract class EnhancedKeyedCollection<T1, T2> : KeyedCollection<T1, T2>
        where T2 : class
    {
        /// <summary>
        /// Make a new collection whose keys are embedded in the values. Lookup dictionary is created immediately.
        /// </summary>
        public EnhancedKeyedCollection()
            :base()
        {
        }

        /// <summary>
        /// Make a new collection whose keys are embedded in the values. Lookup dictionary is created immediately.
        /// </summary>
        /// <param name="comparer">Equality comparer (null to use default).</param>
        public EnhancedKeyedCollection(IEqualityComparer<T1> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Make a new collection whose keys are embedded in the values.
        /// </summary>
        /// <param name="comparer">Equality comparer (null to used default).</param>
        /// <param name="dictionaryCreationThreshold">Lookup dictionary is created when Count is this number.</param>
        public EnhancedKeyedCollection(IEqualityComparer<T1> comparer, int dictionaryCreationThreshold)
            :base(comparer, dictionaryCreationThreshold)
        {
        }

        /// <summary>
        /// Seeks a value from the collection. This is O(1) if lookup dictionary is created, otherwise is O(n).
        /// </summary>
        public virtual bool TryGetValue(T1 key, out T2 item)
        {
            if (Dictionary != null)
                return Dictionary.TryGetValue(key, out item);

            return (item = Items.FirstOrDefault(x => Equals(GetKeyForItem(x), key))) != null;
        }

        /// <summary>
        /// Gets a value from the collection or null. This is O(1) if lookup dictionary is created, otherwise is O(n).
        /// </summary>
        public virtual T2 GetValueOrDefault(T1 key)
        {
            T2 item;
            return TryGetValue(key, out item) ? item : null;
        }

        /// <summary>
        /// Changes the key associated with the specified element in the lookup dictionary.
        /// </summary>
        internal void ChangeKey(T2 item, T1 newKey)
        {
            base.ChangeItemKey(item, newKey);
        }

        protected override void SetItem(int index, T2 item)
        {
            var key = GetKeyForItem(item);
            if (Contains(key) && !Equals(GetKeyForItem(Items[index]), key))
            {
                // Swap if found at different position to try avoid exceptions on IList,
                // which allows same item at different positions, while hastable requires unique keys.
                Items[IndexOf(item)] = Items[index];
                Items[index] = item;
                Dictionary[key] = item;
                return;
            }

            base.SetItem(index, item);
        }
    }

    /// <summary>
    /// Serialize a class derived from KeyedCollection as a list.
    /// </summary>
    /// <remarks>
    /// A custom converter for KeyedCollection to avoid issue where "get_Item"/"set_Item" is expected to have int indexer.
    /// See issue https://github.com/jacobdufault/fullserializer/issues/131.
    /// </remarks>
    public class KeyedCollectionConverter<T, T1, T2> : fsDirectConverter
        where T : KeyedCollection<T1, T2>, new()
    {
        public override Type ModelType { get { return typeof(T); } }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new T();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            fsResult result = fsResult.Success;
            List<fsData> items = new List<fsData>();

            foreach (var item in (T)instance)
            {
                fsData itemData;
                fsResult itemResult;

                if ((itemResult = Serializer.TrySerialize(item, out itemData)).Succeeded)
                    items.Add(itemData);
                result.Merge(itemResult);
            }

            serialized = new fsData(items);
            return result;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            fsResult result = fsResult.Success;
            var collection = new T();

            foreach (fsData serializedItem in data.AsList)
            {
                var item = default(T2);
                fsResult itemResult;

                if ((itemResult = Serializer.TryDeserialize(serializedItem, ref item)).Succeeded)
                    collection.Add(item);
                result.Merge(itemResult);
            }

            instance = collection;
            return result;
        }
    }
}
