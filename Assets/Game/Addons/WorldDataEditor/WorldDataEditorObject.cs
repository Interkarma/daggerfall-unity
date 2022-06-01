// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Uncanny Valley
// Contributors:    Hazelnut

using DaggerfallConnect;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Utility.WorldDataEditor
{
    [ExecuteInEditMode]
    public class WorldDataEditorObject : MonoBehaviour
    {
        public enum DataType { Object3D, Flat, Person, Door, Light };

        public DataType type;
        public bool isExterior = false;
        public string id;
        public byte objectType;
        public short factionID;
        public byte flags;
        public short openRotation;
        public int rdbObjListIdx;
        public DFBlock.RdbObject rdbObj;
        public ushort radius;

        private int tempLocationX;
        private int tempLocationY;
        private int tempLocationZ;

        public void CreateData(DFBlock.RdbObject obj, int rol, DataType type)
        {
            if (type != DataType.Light)
                HideComponents();

            rdbObjListIdx = rol;
            rdbObj = obj;
            this.type = type;
            if (type == DataType.Flat)
            {
                id = obj.Resources.FlatResource.TextureArchive + "." + obj.Resources.FlatResource.TextureRecord;
                factionID = (short)obj.Resources.FlatResource.FactionOrMobileId;
            }
            else if (type == DataType.Light)
            {
                radius = obj.Resources.LightResource.Radius;
            }
        }

        public void CreateData(DFBlock.RmbBlock3dObjectRecord data, bool isExterior)
        {
            HideComponents();
            type = DataType.Object3D;
            this.isExterior = isExterior;
            id = data.ModelId;
            objectType = data.ObjectType;
        }
        public void CreateData(DFBlock.RmbBlockFlatObjectRecord data, bool isExterior)
        {
            HideComponents();
            type = DataType.Flat;
            this.isExterior = isExterior;
            id = data.TextureArchive + "." + data.TextureRecord;
            factionID = data.FactionID;
            flags = data.Flags;
        }
        public void CreateData(DFBlock.RmbBlockPeopleRecord data, bool isExterior)
        {
            HideComponents();
            type = DataType.Person;
            this.isExterior = isExterior;
            id = data.TextureArchive + "." + data.TextureRecord;
            factionID = data.FactionID;
            flags = data.Flags;
        }
        public void CreateData(DFBlock.RmbBlockDoorRecord data, bool isExterior)
        {
            HideComponents();
            type = DataType.Door;
            this.isExterior = isExterior;
            openRotation = data.OpenRotation;
        }

        private void HideComponents()
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer)
                foreach (Material mat in renderer.sharedMaterials)
                    mat.hideFlags = HideFlags.HideInInspector;

            foreach (Component comp in GetComponents<Component>())
                comp.hideFlags = HideFlags.HideInInspector;
        }

        void LateUpdate()
        {
            //Me make sure that objects always snapps to the same degree of precision in which they are stored
            tempLocationX = Mathf.RoundToInt((transform.position.x / MeshReader.GlobalScale));
            tempLocationY = Mathf.RoundToInt((transform.position.y / MeshReader.GlobalScale));
            tempLocationZ = Mathf.RoundToInt((transform.position.z / MeshReader.GlobalScale));
            transform.position = new Vector3(tempLocationX * MeshReader.GlobalScale, tempLocationY * MeshReader.GlobalScale, tempLocationZ * MeshReader.GlobalScale);

            //Reduce the precision of rotation to whole degrees, this is less than needed to be stored properly, but should be enough and makes it easier to snap interior parts together
            transform.rotation = Quaternion.Euler(Mathf.RoundToInt(transform.rotation.eulerAngles.x),
                                                Mathf.RoundToInt(transform.rotation.eulerAngles.y),
                                                Mathf.RoundToInt(transform.rotation.eulerAngles.z));

        }
    }
}