using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Localization
{
    /// <summary>
    /// First-pass helper used during development of localization features.
    /// Will be repurposed or replaced later in development.
    /// </summary>
    public static class RSCHelper
    {
        public static void TestGetRootKeys()
        {
           



            /*
            // Get available culture collection
            string path = LanguageRuntimeData.AvailableCulturesFilePath();
            TextAsset xml = Resources.Load<TextAsset>(path);
            SmartCultureInfoCollection collection = SmartCultureInfoCollection.Deserialize(xml);

            

            // Add a key
            rootDict.Add("TestKey", "Random string of text we need to write back to store.");

            Dictionary<string, string> changeNewRootKeys = new Dictionary<string, string>();
            Dictionary<string, string> changeNewRootValues = new Dictionary<string, string>();

            for (int i = 0; i < changedRootKeys.Count; i++)
            {
                SerializableStringPair rootKey = changedRootKeys[i];
                SerializableLocalizationObjectPair rootValue = changedRootValues[i];
                //Check for possible duplicates and rename them
                string newKeyValue = LocFileUtility.AddNewKeyPersistent(changeNewRootKeys, rootKey.originalValue, rootValue.changedValue.GetFullKey(rootKey.changedValue));

                //Check for possible duplicates and rename them(same as above)
                LocFileUtility.AddNewKeyPersistent(changeNewRootValues, newKeyValue, rootValue.changedValue.TextValue);
            }

            // Generate new key dictionary

            // Save back root dictionary
            //LanguageHandlerEditor.SaveRootLanguageFile(rootDict, rootDict, collection);
            */
        }
    }
}