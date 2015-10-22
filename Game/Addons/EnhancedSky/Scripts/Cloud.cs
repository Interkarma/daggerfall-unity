//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop;

namespace EnhancedSky
{

    public class Cloud : MonoBehaviour {

        SkyManager skyMan;
        public float persistance = .45f;
        public int Dimensions = 400;
        public int seed = -1;                // < 0 generates random seed
        public int numOfOctaves = 6;
        public int normalQueueSize = 3;
        public int overCastQueueSize = 1;
        
        bool isWorking = false;
        bool abortJob = false;
        bool usingOvercast = false;
        Color[] colors;
        Gradient cloudColor;
        Renderer rend;
        Texture2D texture;

        public Queue<Color[]> normalCloudBuffer = new Queue<Color[]>(2);
        public Queue<Color[]> overcastCloudBuffer = new Queue<Color[]>(1);
        

        //used to pass params
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

            public CloudParams(int seed, int Dimensions, int octaves, float pers, Gradient gradient, bool overCast)
            {
                this.xDimension = Dimensions;
                this.yDimension = Dimensions;
                this.seed = seed;
                this.numOfoctaves = octaves;
                this.persistance = pers;
                this.gradient = gradient;
                this.overCast = overCast;

            }

        }


        // Use this for initialization
        void Start()
        {

            skyMan = SkyManager.instance;
            rend = this.GetComponent<Renderer>();
            SkyManager.fastTravelEvent += Generate;
            SkyManager.toggleSkyObjectsEvent += this.ToggleState;
            //Create initial cloud texture
            Generate(skyMan.IsOvercast);
        }

        void FixedUpdate()
        {
            //Set cloud color
            rend.material.SetColor("_Color", cloudColor.Evaluate(skyMan.TimeRatio));
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            abortJob = true;
        }

        /// <summary>
        /// Handles events from SkyManager - typically from fast travel, int/ext transitions etc.
        /// </summary>
        /// <param name="on"></param>
        public void ToggleState(bool on)
        {

            this.gameObject.SetActive(on);
            if (!on)
            {
                StopAllCoroutines();
                //abortJob = true;
            }
            else if (skyMan.IsOvercast != usingOvercast || skyMan.TimeInside > skyMan.TimeInsideLimit)
            {
                abortJob = false;
                Generate(skyMan.IsOvercast);
            }
        }

        /// <summary>
        /// Generates noise data for texture, and then create new texture
        /// </summary>
        public void Generate(bool isOVerCast)
        {
            usingOvercast = isOVerCast;
            if (isOVerCast)
            {
                //gradient = PresetContainer.instance.cloudNoiseOver;
                cloudColor = PresetContainer.instance.colorOver;

            }
            else
            {
                //gradient = PresetContainer.instance.cloudNoiseBase;
                cloudColor = PresetContainer.instance.colorBase;
            }

            //generates noise & creates textures for clouds
            StartCoroutine(CreateTexture(isOVerCast));
        }


        /// <summary>
        /// Generates color array used to build cloud texture
        /// </summary>
        /// <param name="e"></param>
        private void DoWork(object e)
        {
            //System.Object lockObj = new System.Object();

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

            colors = new Color[cp.xDimension * cp.yDimension];
            for (int i = 0; i < cp.xDimension; i++)
            {
                //if isWorking set to false from main thread, stop
                if (abortJob)
                {
                    abortJob = false;
                    colors = null;
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
                    colors[i + (j * cp.xDimension)] = cp.gradient.Evaluate(0.5f * (1 + result));
                }

            }

            DaggerfallUnity.LogMessage("Finished generating cloud noise...seed: " + cp.seed, true);
            if (cp.overCast)
                overcastCloudBuffer.Enqueue(colors);
            else
                normalCloudBuffer.Enqueue(colors);
            //alert WaitForJobFinish() to create texture
            isWorking = false;


        }


        /// <summary>
        /// Creates texture from color[]
        /// </summary>
        /// <returns></returns>
        public IEnumerator CreateTexture(bool usingOvercast)
        {
            Queue<Color[]> target = null;
            if (usingOvercast)
            {
                target = overcastCloudBuffer;
                transform.localPosition = new Vector3(0, 0.08f, 0);

            }
            else
            {
                target = normalCloudBuffer;
                transform.localPosition = new Vector3(0, 0.15f, 0);
            }

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
            rend.material.mainTexture = texture;
            Debug.Log("Cloud Texture created...overcast: " + usingOvercast);


            //Make sure both queues are at min. size
            int stepCount = 0;
            while (normalQueueSize - normalCloudBuffer.Count > 0 && stepCount++ < 1000)
            {

                if (!isWorking)
                    BuildQueue(false);


                //Debug.Log("step count: " + stepCount);
                yield return new WaitForEndOfFrame();

            }

            stepCount = 0;
            while (overCastQueueSize - overcastCloudBuffer.Count > 0 && stepCount++ < 1000)
            {
                if (!isWorking)
                    BuildQueue(true);

                //Debug.Log("step count: " + stepCount);
                yield return new WaitForEndOfFrame();
            }

            yield break;

        }


        public bool BuildQueue(bool usingOvercast = false)
        {
            if (isWorking)
            {
                Debug.LogWarning("BuildQueue found job already in progress - stopping");
                return false;

            }


            CloudParams cp = new CloudParams(seed, Dimensions, numOfOctaves, persistance, usingOvercast);
            if (usingOvercast)
                cp.gradient = PresetContainer.instance.cloudNoiseOver;
            else
                cp.gradient = PresetContainer.instance.cloudNoiseBase;
            isWorking = true;
            //executes on thread pool; if no threads avaliable will wait for one
            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(DoWork), cp))
            {
                Debug.LogError("Failed to queue worker");
                return false;
            }
            else
                return true;

        }


    }
}
