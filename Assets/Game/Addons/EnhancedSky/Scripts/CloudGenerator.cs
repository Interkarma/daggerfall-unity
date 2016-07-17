using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using DaggerfallWorkshop;

namespace EnhancedSky
{
    public class CloudGenerator : MonoBehaviour
    {
        const int NUMOFOCTAVES  = 10;
        const int BUFFERSIZE    = 2;
        float   _persistance    = .45f;
        bool    _isWorking      = false;
        bool    _abortJob       = false;
        Color[] _colors;
        Queue<Color[]> _normalCloudBuffer = new Queue<Color[]>(BUFFERSIZE);
        Queue<Color[]> _overcastCloudBuffer = new Queue<Color[]>(BUFFERSIZE);
        SkyManager SkyMan { get { return SkyManager.instance; } }



        [Serializable]
        private class CloudParams
        {

            public int xDimension = 600;
            public int yDimension = 600;
            public int seed = -1;
            public int numOfoctaves = 9;
            public float persistance = .45f;
            public Gradient gradient;
            public bool overCast;

            public CloudParams()
            {


            }

            public CloudParams(int seed, int Dimensions, int octaves, float pers, bool overCast)
            {
                this.xDimension = Dimensions;
                this.yDimension = Dimensions;
                this.seed = seed;
                this.numOfoctaves = octaves;
                this.persistance = pers;
                this.overCast = overCast;
            }

        }



        void Start()
        {
            this.StartCoroutine(CreateTexture(SkyMan.IsOvercast, null));


        }

        void OnDestroy()
        {
            _colors             = null;
            _normalCloudBuffer  = null;
            _overcastCloudBuffer = null;


        }

        /// <summary>
        /// Creates texture from color[]
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator CreateTexture(bool usingOvercast, Action<Texture2D> callback)
        {
            Queue<Color[]> target = null;
            Texture2D texture;
            int stepCount = 0;

            if (usingOvercast)
                target = _overcastCloudBuffer;
            else
                target = _normalCloudBuffer;
            
            if(callback != null)
            {
                //if qeue empty, generate noise and make texture from that
                if (target.Count < 1)
                {
                    BuildQueue(usingOvercast);
                    while (target.Count < 1)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }

                Color[] temp = target.Dequeue();
                int dimension = (int)Math.Sqrt(temp.Length);
                texture = new Texture2D(dimension, dimension, TextureFormat.ARGB32, false);
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Trilinear;
                texture.SetPixels(temp);
                texture.Apply();
                if(texture)
                    callback(texture);
                texture = null;
            }

             //Debug.Log("Cloud Texture created...overcast: " + usingOvercast);


             //Make sure both queues are filled
             while (BUFFERSIZE - _normalCloudBuffer.Count > 0 && stepCount++ < 1000)
             {
                 if (!_isWorking)
                     BuildQueue(false);
                 //Debug.Log("step count: " + stepCount);
                 yield return new WaitForEndOfFrame();
             }

             stepCount = 0;
             while (BUFFERSIZE - _overcastCloudBuffer.Count > 0 && stepCount++ < 1000)
             {
                 if (!_isWorking)
                     BuildQueue(true);

                 //Debug.Log("step count: " + stepCount);
                 yield return new WaitForEndOfFrame();
             }
    
        }

        private bool BuildQueue(bool usingOvercast = false)
        {
            if (_isWorking)
            {
                Debug.LogWarning("BuildQueue found job already in progress - stopping");
                return false;

            }

            this._persistance = UnityEngine.Random.Range(.5f, .6f);
            CloudParams cp = new CloudParams(SkyMan.cloudSeed, SkyMan.cloudQuality, NUMOFOCTAVES, _persistance, usingOvercast);
            
            if (usingOvercast)
                cp.gradient = PresetContainer.Instance.cloudNoiseOver;
            else
                cp.gradient = PresetContainer.Instance.cloudNoiseBase;
            _isWorking = true;
            //executes on thread pool; if no threads avaliable will wait for one
            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(DoWork), cp))
            {
                Debug.LogError("Failed to queue worker");
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Generates color array used to build cloud texture
        /// </summary>
        /// <param name="e"></param>
        private void DoWork(object e)
        {

            CloudParams cp = (CloudParams)e;

            if (cp == null)
            {
                DaggerfallUnity.LogMessage("Error - cp == null", true);
                return;
            }
            if (cp.gradient == null)
            {
                DaggerfallUnity.LogMessage("Error - invalid gradient supplied", true);
                return;
            }

            int numOfOctaves = cp.numOfoctaves;

            if (cp.seed < 0)        //if seed negative, get a random seed
                cp.seed = System.Environment.TickCount;

            System.Random rand = new System.Random(cp.seed);

            Noise[] octaves = new Noise[numOfOctaves];
            float[] freq = new float[numOfOctaves];
            float[] amp = new float[numOfOctaves];

            for (int i = 0; i < numOfOctaves; i++)
            {
                octaves[i] = new Noise();
                octaves[i].Seed(rand.Next());
                freq[i] = (float)Math.Pow(2, i);
                amp[i] = (float)Math.Pow(cp.persistance, octaves.Length - i);
            }

            _colors = new Color[cp.xDimension * cp.yDimension];
            for (int i = 0; i < cp.xDimension; i++)
            {
                //if isWorking set to false from main thread, stop
                if (_abortJob)
                {
                    _abortJob = false;
                    _colors = null;
                    return;
                }

                for (int j = 0; j < cp.yDimension; j++)
                {
                    float result = 0;
                    for (int z = 0; z < numOfOctaves; z++)
                    {
                        result += octaves[z].Generate((i / freq[z]) % cp.xDimension, (j / freq[z]) % cp.yDimension) * amp[z];
                    }

                    //DaggerfallUnity.LogMessage(string.Format("res: {0} i: {1} j: {2}", result, i, j), true);
                    _colors[i + (j * cp.xDimension)] = cp.gradient.Evaluate(0.5f * (1 + result));
                }

            }

            //DaggerfallUnity.LogMessage("Finished generating cloud noise...seed: " + cp.seed, true);
            if (cp.overCast)
                _overcastCloudBuffer.Enqueue(_colors);
            else
                _normalCloudBuffer.Enqueue(_colors);
            //alert WaitForJobFinish() to create texture
            _isWorking = false;


        }

    }
}
