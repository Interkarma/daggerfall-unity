using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Hosts quests and manages their execution during play.
    /// Quests are instantiated from a source text template.
    /// It's possible to have the same quest multiple times (e.g. same fetch quest from two different mage guildhalls).
    /// Running quests can perform actions in the world (e.g. spawn enemies and play sounds).
    /// Or they can provide data to external systems like the NPC dialog interface (e.g. 'tell me about' and 'rumors').
    /// Quest support is considered to be in very early prototype stages and may change at any time.
    /// </summary>
    public class QuestMachine : MonoBehaviour
    {
        public string TestQuest = "_BRISIEN";       // TEMP: Test quest to parse at startup

        void Start()
        {
            // Load test quest is specified
            if (!string.IsNullOrEmpty(TestQuest))
            {
                // Attempt to parse quest source
                TextAsset source = Resources.Load<TextAsset>(Path.Combine("Quests", TestQuest));
                Parser.Parse(source.text.Split('\n'));
            }
        }
    }
}