using UnityEngine;
using UnityEditor;
using DaggerfallWorkshop.Utility;

namespace Game.Pet
{
    [CustomEditor(typeof(PetConfigurer))]
    public class SetupDemoEnemyEditor : Editor //todo delete
    {
        private PetConfigurer PetConfigurer => target as PetConfigurer;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();
            if (GUILayout.Button("Apply Enemy Type"))
            {
                PetConfigurer.ApplySettings(PetConfigurer.petType, PetConfigurer.reaction,
                    PetConfigurer.gender, PetConfigurer.classicSpawnDistanceType);
                EditorUtility.SetDirty(PetConfigurer);
                EditorUtility.SetDirty(PetConfigurer.GetMobileBillboardChild());
            }

            if (GUILayout.Button("Align To Ground"))
            {
                CharacterController controller = PetConfigurer.GetComponent<CharacterController>();
                if (controller != null)
                {
                    GameObjectHelper.AlignControllerToGround(controller);
                }
            }

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}