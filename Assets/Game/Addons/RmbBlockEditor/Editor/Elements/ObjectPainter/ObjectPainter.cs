// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public abstract class ObjectPainter
    {
        public VisualElement visualElement;
        protected GameObject placeholderGo;
        protected Func<GameObject> createPlaceholder;
        protected bool snap;
        protected Vector3 paintPositionOffset;
        protected Vector3 placementPositionOffset;

        protected ObjectPainter()
        {
            visualElement = new VisualElement();
            snap = false;
            paintPositionOffset = Vector3.zero;
            placementPositionOffset = Vector3.zero;

            RenderTemplate();
            BindFields();
        }

        public void Destroy()
        {
            DestroyPlaceholder();
        }

        private void RenderTemplate()
        {
            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Elements/ObjectPainter/Template.uxml");
            visualElement.Add(tree.CloneTree());
        }

        private void BindFields()
        {
            var snapButton = visualElement.Query<Toggle>("snap").First();
            var positionOffset = visualElement.Query<Vector3Field>("paint-position-offset").First();
            var paintToggle = visualElement.Query<Toggle>("paint").First();

            var placementPosition = visualElement.Query<Vector3Field>("placement-position").First();
            var placeButton = visualElement.Query<Button>("place").First();

            snapButton.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                StopPainting();
                snap = evt.newValue;
            });

            positionOffset.RegisterCallback<ChangeEvent<Vector3>>(evt => paintPositionOffset = evt.newValue);
            placementPosition.RegisterCallback<ChangeEvent<Vector3>>(evt => placementPositionOffset = evt.newValue);

            paintToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                DestroyPlaceholder();
                if (evt.newValue)
                {
                    var info = visualElement.Query<Box>("info").First();
                    info.RemoveFromClassList("hidden");
                    CreatePlaceholder();
                }
            });

            placeButton.RegisterCallback<MouseUpEvent>(
                evt => { PlaceObject(); },
                TrickleDown.TrickleDown);
        }

        protected void StopPainting()
        {
            // Hide info box
            var info = visualElement.Query<Box>("info").First();
            info.AddToClassList("hidden");

            // Switch the toggle off
            var paintToggle = visualElement.Query<Toggle>("paint").First();
            paintToggle.value = false;

            DestroyPlaceholder();
        }

        protected void DestroyPlaceholder()
        {
            // Remove the placeholder GameObject
            if (placeholderGo != null)
            {
                GameObject.DestroyImmediate(placeholderGo);
            }
        }

        protected abstract void CreatePlaceholder();
        protected abstract void PlaceObject();
    }

    public class ModelPainter : ObjectPainter
    {
        private bool alignToSurface;
        protected Vector3 paintRotationOffset;
        protected Vector3 placementRotationOffset;
        protected float placementScaleOffset;
        protected Action<Vector3, Vector3, Vector3> placeObject;

        public ModelPainter(Func<GameObject> createPlaceholder, Action<Vector3, Vector3, Vector3> placeObject)
        {
            this.createPlaceholder = createPlaceholder;
            this.placeObject = placeObject;
            alignToSurface = false;
            paintRotationOffset = Vector3.zero;
            placementRotationOffset = Vector3.zero;
            placementScaleOffset = 1;

            ShowModelSpecificFields();
            BindModelSpecificFields();
        }

        private void ShowModelSpecificFields()
        {
            var alignButton = visualElement.Query<Toggle>("align").First();
            var paintRotation = visualElement.Query<VisualElement>("paint-rotation-vector-container").First();
            var placementRotation = visualElement.Query<VisualElement>("placement-rotation-vector-container").First();
            var placementScaleContainer =
                visualElement.Query<VisualElement>("placement-scale-vector-container").First();
            var placementScale = visualElement.Query<FloatField>("placement-scale").First();

            alignButton.RemoveFromClassList("hidden");
            paintRotation.RemoveFromClassList("hidden");
            placementRotation.RemoveFromClassList("hidden");
            placementScaleContainer.RemoveFromClassList("hidden");
            placementScale.value = 1;
        }

        private void BindModelSpecificFields()
        {
            var alignButton = visualElement.Query<Toggle>("align").First();
            var rotationOffset = visualElement.Query<Vector3Field>("paint-rotation-offset").First();
            var placementRotation = visualElement.Query<Vector3Field>("placement-rotation").First();
            var placementScale = visualElement.Query<FloatField>("placement-scale").First();
            alignButton.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                StopPainting();
                alignToSurface = evt.newValue;
            });
            rotationOffset.RegisterCallback<ChangeEvent<Vector3>>(evt => paintRotationOffset = evt.newValue);
            placementRotation.RegisterCallback<ChangeEvent<Vector3>>(evt => placementRotationOffset = evt.newValue);
            placementScale.RegisterCallback<ChangeEvent<float>>(evt => placementScaleOffset = evt.newValue);
        }

        protected override void CreatePlaceholder()
        {
            DestroyPlaceholder();
            placeholderGo = createPlaceholder();
            var placeholder = placeholderGo.AddComponent<ModelPlaceholder>();
            placeholder.CreateModelPlaceholder(snap, alignToSurface, paintPositionOffset, paintRotationOffset,
                placeObject, StopPainting);
        }

        protected override void PlaceObject()
        {
            placeObject(placementPositionOffset, placementRotationOffset,
                new Vector3(placementScaleOffset, placementScaleOffset, placementScaleOffset));
        }
    }

    public class BuildingPainter : ObjectPainter
    {
        protected Vector3 paintRotationOffset;
        protected Vector3 placementRotationOffset;
        protected Action<Vector3, Vector3> placeObject;

        public BuildingPainter(Func<GameObject> createPlaceholder, Action<Vector3, Vector3> placeObject)
        {
            this.createPlaceholder = createPlaceholder;
            this.placeObject = placeObject;
            paintRotationOffset = Vector3.zero;
            placementRotationOffset = Vector3.zero;

            ShowBuildingSpecificFields();
            BindBuildingSpecificFields();
        }

        private void ShowBuildingSpecificFields()
        {
            var paintRotation = visualElement.Query<VisualElement>("paint-rotation-y-container").First();
            var placementRotation = visualElement.Query<VisualElement>("placement-rotation-y-container").First();
            paintRotation.RemoveFromClassList("hidden");
            placementRotation.RemoveFromClassList("hidden");
        }

        private void BindBuildingSpecificFields()
        {
            var paintYRotationOffset = visualElement.Query<IntegerField>("paint-y-rotation-offset").First();
            var placementYRotation = visualElement.Query<IntegerField>("placement-y-rotation").First();
            paintYRotationOffset.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                paintRotationOffset = new Vector3(0, evt.newValue, 0);
            });
            placementYRotation.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                placementRotationOffset = new Vector3(0, evt.newValue, 0);
            });
        }

        protected override void CreatePlaceholder()
        {
            DestroyPlaceholder();
            placeholderGo = createPlaceholder();
            var placeholder = placeholderGo.AddComponent<BuildingPlaceholder>();
            placeholder.CreateBuildingPlaceholder(snap, paintPositionOffset, paintRotationOffset, placeObject,
                StopPainting);
        }

        protected override void PlaceObject()
        {
            placeObject(placementPositionOffset, placementRotationOffset);
        }
    }

    public class FlatPainter : ObjectPainter
    {
        protected Action<Vector3> placeObject;

        public FlatPainter(Func<GameObject> createPlaceholder, Action<Vector3> placeObject)
        {
            this.createPlaceholder = createPlaceholder;
            this.placeObject = placeObject;
        }

        protected override void CreatePlaceholder()
        {
            DestroyPlaceholder();
            placeholderGo = createPlaceholder();
            var placeholder = placeholderGo.AddComponent<FlatPlaceholder>();
            placeholder.CreateFlatPlaceholder(snap, paintPositionOffset, placeObject, StopPainting);
        }

        protected override void PlaceObject()
        {
            placeObject(placementPositionOffset);
        }
    }
}