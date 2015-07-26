using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Sets up enemy using demo components.
    /// </summary>
    [RequireComponent(typeof(EnemyMotor))]
    public class SetupDemoEnemy : MonoBehaviour
    {
        public MobileTypes EnemyType = MobileTypes.SkeletalWarrior;
        public MobileReactions EnemyReaction = MobileReactions.Hostile;

        void Start()
        {
            // Disable this game object if missing mobile setup
            DaggerfallMobileUnit dfMobile = GetMobileBillboardChild();
            if (dfMobile == null)
                this.gameObject.SetActive(false);
            if (!dfMobile.Summary.IsSetup)
                this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets up enemy based on current settings.
        /// </summary>
        public void ApplyEnemySettings()
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            Dictionary<int, MobileEnemy> enemyDict = GameObjectHelper.EnemyDict;

            // Find mobile unit in children
            DaggerfallMobileUnit dfMobile = GetMobileBillboardChild();
            if (dfMobile != null)
            {
                // Setup mobile billboard
                Vector2 size = Vector2.one;
                dfMobile.SetEnemy(dfUnity, enemyDict[(int)EnemyType], EnemyReaction);

                // Setup controller
                CharacterController controller = GetComponent<CharacterController>();
                if (controller)
                {
                    // Set base height from sprite
                    size = dfMobile.Summary.RecordSizes[0];
                    controller.height = size.y;

                    // Reduce height of flying creatures as their wing animation makes them taller than desired
                    // This helps them get through doors while aiming for player eye height
                    if (dfMobile.Summary.Enemy.Behaviour == MobileBehaviour.Flying)
                        controller.height /= 2f;

                    // Uncomment below lines to limit maximum controller height
                    // Some particularly tall sprites (e.g. giants) require this hack to get through doors
                    // However they will appear sunken into ground as a result
                    //if (controller.height > 1.9f)
                    //    controller.height = 1.9f;
                }

                // Setup sounds
                EnemySounds enemySounds = GetComponent<Demo.EnemySounds>();
                if (enemySounds)
                {
                    enemySounds.MoveSound = (SoundClips)dfMobile.Summary.Enemy.MoveSound;
                    enemySounds.BarkSound = (SoundClips)dfMobile.Summary.Enemy.BarkSound;
                    enemySounds.AttackSound = (SoundClips)dfMobile.Summary.Enemy.AttackSound;
                }
            }
        }

        /// <summary>
        /// Change enemy settings and configure in a single call.
        /// </summary>
        /// <param name="enemyType">Enemy type.</param>
        public void ApplyEnemySettings(MobileTypes enemyType, MobileReactions enemyReaction)
        {
            EnemyType = enemyType;
            EnemyReaction = enemyReaction;
            ApplyEnemySettings();
        }

        /// <summary>
        /// Finds mobile billboard in children.
        /// </summary>
        /// <returns>DaggerfallMobileUnit.</returns>
        public DaggerfallMobileUnit GetMobileBillboardChild()
        {
            return GetComponentInChildren<DaggerfallMobileUnit>();
        }
    }
}