// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using DaggerfallConnect.Arena2;
using UnityEditor;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public abstract class Placeholder : MonoBehaviour
    {
        protected Action onCancel;
        protected bool snapToSurface;
        protected bool mouseHasDragged;
        protected bool crtlPressed;
        protected Vector3 positionOffset;
        protected float yRotationDelta;
        protected float groundOffset;

        public void CreateObject(bool snapToSurface, Vector3 positionOffset, Action onCancel)
        {
            this.snapToSurface = snapToSurface;
            this.onCancel = onCancel;
            this.positionOffset = new Vector3(positionOffset.x, -positionOffset.y, positionOffset.z) *
                                  MeshReader.GlobalScale;
            mouseHasDragged = false;
            yRotationDelta = 0;
            groundOffset = GetOffset();

            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            var meshSubComponents = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (var meshSubComponent in meshSubComponents)
            {
                meshSubComponent.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }

            // Change the parent of the placeholder GameObject to be the RmbBlockObject
            var rmbBlockObject = FindObjectOfType<RmbBlockObject>();
            gameObject.transform.parent = rmbBlockObject.gameObject.transform;
        }

        public void Start()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        protected void OnSceneGUI(SceneView sceneView)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive)); // disable scene selection

            var mouseUp = Event.current.type == EventType.MouseUp;
            var mouseDrag = Event.current.type == EventType.MouseDrag;

            var leftUp = Event.current.button == 0 && Event.current.isMouse && mouseUp;
            var rightUp = Event.current.button == 1 && Event.current.isMouse && mouseUp;

            crtlPressed = Event.current.control;

            if (rightUp)
            {
                OnRightClick();
                return;
            }

            if (mouseDrag)
            {
                mouseHasDragged = true;
                OnLeftDrag();
                return;
            }

            if (mouseUp && mouseHasDragged)
            {
                mouseHasDragged = false;
                return;
            }

            Vector3 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) && !mouseHasDragged)
            {
                OnRaycastHit(hit);
            }

            if (hit.collider != null && leftUp && !mouseHasDragged)
            {
                OnLeftClick();
            }
        }

        protected float GetOffset()
        {
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                //Buildings don't have a MeshFilter
                meshFilter = GetComponentInChildren<MeshFilter>();
            }

            var mesh = meshFilter.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            var lowestVertexY = 0f;
            foreach (var vertex in vertices)
            {
                if (lowestVertexY > vertex.y)
                {
                    lowestVertexY = vertex.y;
                }
            }

            return -lowestVertexY * transform.localScale.y;
        }

        protected void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        protected abstract void OnLeftClick();
        protected abstract void OnLeftDrag();
        protected abstract void OnRightClick();
        protected abstract void OnRaycastHit(RaycastHit hit);
    }

    public class ModelPlaceholder : Placeholder
    {
        private bool alignToSurface;
        private Vector3 meshScale;
        private float scaleDelta;
        private Action<Vector3, Vector3, Vector3> onAdd;

        public void CreateModelPlaceholder(bool snapToSurface, bool alignToSurface, Vector3 positionOffset,
            Vector3 rotationOffset,
            Action<Vector3, Vector3, Vector3> onAdd,
            Action onCancel)
        {
            CreateObject(snapToSurface, positionOffset, onCancel);
            meshScale = transform.localScale;
            this.snapToSurface = snapToSurface;
            this.alignToSurface = alignToSurface;
            this.onAdd = onAdd;
            scaleDelta = 0;

            var rotation = new Vector3(rotationOffset.x, -rotationOffset.y, rotationOffset.z) /
                           BlocksFile.RotationDivisor;
            yRotationDelta = rotationOffset.y;
            transform.rotation = Quaternion.Euler(rotation);
        }

        protected override void OnRightClick()
        {
            onCancel();
        }

        protected override void OnLeftClick()
        {
            var rotation = transform.rotation.eulerAngles;
            var xRotation = -rotation.x * BlocksFile.RotationDivisor;
            var yRotation = -rotation.y * BlocksFile.RotationDivisor;
            var zRotation = -rotation.z * BlocksFile.RotationDivisor;
            var newRotation = new Vector3(xRotation, yRotation, zRotation);

            var newPosition = new Vector3(transform.position.x, -transform.position.y, transform.position.z) /
                              MeshReader.GlobalScale;
            onAdd(newPosition, newRotation,
                new Vector3(
                    transform.localScale.x / meshScale.x,
                    transform.localScale.y / meshScale.y,
                    transform.localScale.z / meshScale.z)
                );
        }

        protected override void OnLeftDrag()
        {
            var delta = Event.current.delta.x;
            if (crtlPressed)
            {
                scaleDelta += delta / 10 * meshScale.x;
                if (scaleDelta < -0.9)
                {
                    scaleDelta = -0.9f;
                }

                transform.localScale = new Vector3(meshScale.x + scaleDelta, meshScale.y + scaleDelta,
                    meshScale.z + scaleDelta);

                // Recalculate the offset, based on the new scale
                groundOffset = GetOffset();
            }
            else
            {
                yRotationDelta -= delta;
                transform.Rotate(Vector3.up, -delta);
            }
        }

        protected override void OnRaycastHit(RaycastHit hit)
        {
            var groundOffsetVector = Vector3.up * groundOffset;
            if (alignToSurface)
            {
                transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                transform.Rotate(Vector3.up, yRotationDelta);
            }

            transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            transform.Translate(positionOffset);

            if (snapToSurface)
            {
                transform.Translate(groundOffsetVector);
            }
        }
    }

    public class FlatPlaceholder : Placeholder
    {
        private Action<Vector3> onAdd;

        public void CreateFlatPlaceholder(bool snapToSurface, Vector3 positionOffset,
            Action<Vector3> onAdd,
            Action onCancel)
        {
            CreateObject(snapToSurface, positionOffset, onCancel);

            this.snapToSurface = snapToSurface;
            this.onAdd = onAdd;
        }

        protected override void OnRightClick()
        {
            onCancel();
        }

        protected override void OnLeftClick()
        {
            // The offset needs to be removed for flats
            var newPosition =
                new Vector3(transform.position.x, -transform.position.y + groundOffset, transform.position.z) /
                MeshReader.GlobalScale;
            onAdd(newPosition);
        }

        protected override void OnLeftDrag()
        {
        }

        protected override void OnRaycastHit(RaycastHit hit)
        {
            var yPos = snapToSurface ? hit.point.y + groundOffset : hit.point.y;
            transform.position = new Vector3(hit.point.x, yPos, hit.point.z) + positionOffset;

            // Rotate to face the main camera
            transform.LookAt(SceneView.lastActiveSceneView.camera.transform);
        }
    }

    public class BuildingPlaceholder : Placeholder
    {
        private Action<Vector3, Vector3> onAdd;

        public void CreateBuildingPlaceholder(bool snapToSurface, Vector3 positionOffset,
            Vector3 rotationOffset,
            Action<Vector3, Vector3> onAdd,
            Action onCancel)
        {
            CreateObject(snapToSurface, positionOffset, onCancel);

            this.snapToSurface = snapToSurface;
            this.onAdd = onAdd;

            var rotation = new Vector3(rotationOffset.x, -rotationOffset.y, rotationOffset.z) /
                           BlocksFile.RotationDivisor;
            transform.rotation = Quaternion.Euler(rotation);
        }

        protected override void OnRightClick()
        {
            onCancel();
        }

        protected override void OnLeftClick()
        {
            var rotation = transform.rotation.eulerAngles;
            var xRotation = rotation.x * BlocksFile.RotationDivisor;
            var yRotation = -rotation.y * BlocksFile.RotationDivisor;
            var zRotation = rotation.z * BlocksFile.RotationDivisor;
            var newRotation = new Vector3(xRotation, yRotation, zRotation);

            var newPosition = new Vector3(transform.position.x, -transform.position.y, -transform.position.z) /
                              MeshReader.GlobalScale;
            onAdd(newPosition, newRotation);
        }

        protected override void OnLeftDrag()
        {
            var delta = Event.current.delta.x;
            yRotationDelta -= delta;
            transform.Rotate(Vector3.up, -delta);
        }

        protected override void OnRaycastHit(RaycastHit hit)
        {
            var yPos = snapToSurface ? hit.point.y + groundOffset : hit.point.y;
            transform.position = new Vector3(hit.point.x, yPos, hit.point.z) + positionOffset;
        }
    }

    public class SceneryPlaceholder : Placeholder
    {
        private Action<int, int> onAdd;
        private int x;
        private int z;

        public void CreateFlatPlaceholder(Action<int, int> onAdd, Action onCancel)
        {
            CreateObject(false, Vector3.zero, onCancel);
            this.onAdd = onAdd;
        }

        protected override void OnRightClick()
        {
            onCancel();
        }

        protected override void OnLeftClick()
        {
            onAdd(x, -z);
        }

        protected override void OnLeftDrag()
        {
        }

        protected override void OnRaycastHit(RaycastHit hit)
        {
            var y = hit.point.y + groundOffset;
            x = (int)Math.Floor(hit.point.x / 6.4);
            z = (int)Math.Ceiling(hit.point.z / 6.4);
            transform.position = new Vector3(x * 6.4f, y, z * 6.4f);

            // Rotate to face the main camera
            transform.LookAt(SceneView.lastActiveSceneView.camera.transform);
        }
    }
#endif
}