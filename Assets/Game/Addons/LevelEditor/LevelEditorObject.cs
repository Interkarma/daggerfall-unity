using UnityEngine;

public class LevelEditorObject : MonoBehaviour {

    public int type = -1;
    public bool isExterior = false;
    public string id;
    public byte objectType;
    public short factionID;
    public byte flags;
    public short openRotation;

    public void CreateData(DaggerfallConnect.DFBlock.RmbBlock3dObjectRecord data, bool isExterior) {
        HideComponents();
        type = 0;
        this.isExterior = isExterior;
        id = data.ModelId;
        objectType = data.ObjectType;
    }
    public void CreateData(DaggerfallConnect.DFBlock.RmbBlockFlatObjectRecord data, bool isExterior) {
        HideComponents();
        type = 1;
        this.isExterior = isExterior;
        id = data.TextureArchive + "." +data.TextureRecord;
        factionID = data.FactionID;
        flags = data.Flags;
    }
    public void CreateData(DaggerfallConnect.DFBlock.RmbBlockPeopleRecord data, bool isExterior) {
        HideComponents();
        type = 2;
        this.isExterior = isExterior;
        id = data.TextureArchive + "." + data.TextureRecord;
        factionID = data.FactionID;
        flags = data.Flags;
    }
    public void CreateData(DaggerfallConnect.DFBlock.RmbBlockDoorRecord data, bool isExterior) {
        HideComponents();
        type = 3;
        this.isExterior = isExterior;
        openRotation = data.OpenRotation;
    }

    private void HideComponents() {
        foreach (Material mat in gameObject.GetComponent<Renderer>().sharedMaterials) {
            mat.hideFlags = HideFlags.HideInInspector;
        }

        foreach (Component comp in GetComponents<Component>()) {
            comp.hideFlags = HideFlags.HideInInspector;
        }
    }

    public void UpdateVisibility(bool isExterior) {

        if (this.isExterior != isExterior) {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            gameObject.GetComponent<Renderer>().enabled = false;
        }
        else {
            gameObject.hideFlags = HideFlags.None;
            gameObject.GetComponent<Renderer>().enabled = true;
            HideComponents();
        }
    }
}
