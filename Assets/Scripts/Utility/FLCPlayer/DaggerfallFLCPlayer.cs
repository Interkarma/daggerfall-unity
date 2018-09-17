using UnityEngine;
using System.Collections;
using DaggerfallConnect.Arena2;
using System.IO;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop;


public class DaggerfallFLCPlayer : MonoBehaviour
{

    //FLCFile flcFile;
    DaggerfallFLCReader flcReader;
    Texture2D FLCTexture;
    GameObject Target;

    public float speedMod;
    public bool Loop = true;
    public bool IsPlaying = false;

    public DaggerfallFLCReader FLCReader { get; private set; }



    public void Open(string name)
    {
        Setup();

        string path = Path.Combine(DaggerfallWorkshop.DaggerfallUnity.Instance.Arena2Path, name);
        if (!FLCReader.Open(path))
            return;

        FLCTexture = TextureReader.CreateFromSolidColor(FLCReader.header.Width, FLCReader.header.Height, Color.black, false, false);
    }

    protected void Setup()
    {
        FLCReader = new DaggerfallFLCReader();

    }

        
    public void PlayHelper()
    {
        StartCoroutine(Play());

    }

    public IEnumerator Play()
    {

        if (!FLCReader.ReadyToPlay || IsPlaying)
            yield break;

        IsPlaying = true;
       
     
        while(IsPlaying)
        {
                
            // Update Texture
            FLCTexture.SetPixels32(FLCReader.FrameBuffer);
            FLCTexture.Apply(false);
                
            //buffer next frame async
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Next), FLCReader); //need to lock buffer or causes threading issues
            FLCReader.BufferNextFrame();  //< without threading

            Draw();

            //calculate delay
            float timer = (speedMod != 0) ? FLCReader.FrameDelay * speedMod : FLCReader.FrameDelay;


            //stop playing on last frame if not looping
            if (FLCReader.CurrentFrame >= FLCReader.header.NumOfFrames && !Loop)
                IsPlaying = false;


            yield return new WaitForSeconds(timer);
        }

        yield break;

    }



    //Async buffering
    public void Next(object obj)
    {

        DaggerfallFLCReader reader = (DaggerfallFLCReader)obj;
        if (reader == null)
            return;

        //should be locked for thread safety
        reader.BufferNextFrame();
        
       
        return;
    }

    //draw next frame
    public void Draw()
    {
        //base.Draw();
        if (Target == null)
            Target = GameObject.CreatePrimitive(PrimitiveType.Plane);

        Target.GetComponent<Renderer>().material.mainTexture = FLCTexture;
    }



}
