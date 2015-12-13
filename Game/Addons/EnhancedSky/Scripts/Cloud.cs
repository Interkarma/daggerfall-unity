//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
///Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

using UnityEngine;
using System;
using DaggerfallWorkshop;

namespace EnhancedSky
{
   
    public class Cloud : MonoBehaviour 
    {
        
        bool            _usingOvercast = false;
        Gradient        _cloudColor;
        Renderer        _rend;
        Texture2D       _texture;
            
        Renderer        Rend        { get { return (_rend != null) ? _rend : _rend = this.GetComponent<Renderer>(); } }
        Gradient        CloudColor  { get { return (_cloudColor != null) ? _cloudColor : PresetContainer.Instance.colorBase; } set { _cloudColor = value;} }
        CloudGenerator  cloudGen    { get { return SkyManager.instance.CloudGen; } }
        Material        CloudMat    { get {return SkyManager.instance.CloudMat; } }
        SkyManager      SkyMan      { get { return SkyManager.instance; } }
        public bool     HasTexture  { get; private set; }
       
   
        void OnDestroy()
        {
            _cloudColor = null;
            Destroy(_texture);
            StopAllCoroutines();
        }

        void OnEnable()
        {
            _rend = this.GetComponent<Renderer>();
            _rend.material = CloudMat;
            _rend.material.mainTexture = this._texture;

            if (SkyMan.IsOvercast != _usingOvercast || SkyMan.TimeInside > SkyManager.TIMEINSIDELIMIT)
            {
                GetNewTexture(SkyMan.IsOvercast);
            }
            else if(!HasTexture)
                GetNewTexture(SkyMan.IsOvercast);

            SkyManager.updateSkyEvent += GetNewTexture;

        }
        
        void OnDisable()
        {

            SkyManager.updateSkyEvent -= this.GetNewTexture;
        }

        void FixedUpdate()
        {
            //Set cloud color
            if(HasTexture)
                Rend.material.SetColor("_Color", CloudColor.Evaluate(SkyMan.TimeRatio));

        }


        public void GetNewTexture(bool isOverCast)
        {
            HasTexture = false;
            _usingOvercast = isOverCast;

            if(cloudGen == null)
            {
                Debug.LogError("Cloud gen was null");
                return;
            }

            cloudGen.StartCoroutine(cloudGen.CreateTexture(_usingOvercast, this.ApplyTexture));

        }
       

        public void ApplyTexture(Texture2D texture)
        {
            
            try
            {
                if (texture == null)
                {
                    Debug.Log("Cloud texture returned null");
                    return;

                }
                if (SkyMan.IsOvercast)
                {
                    CloudColor = PresetContainer.Instance.colorOver;
                    transform.localPosition = new Vector3(0, 0.11f, 0);


                }
                else
                {
                    CloudColor = PresetContainer.Instance.colorBase;
                    transform.localPosition = new Vector3(0, 0.08f, 0);

                }
                Destroy(this._texture);
                this._texture = texture;
                Rend.material.mainTexture = this._texture;
                HasTexture = true;
            }
            catch(MissingReferenceException ex)
            {
                //if cloud is destroyed before CloudGenerator.CreateTexture Calls back, it can create an exception
                DaggerfallUnity.LogMessage(ex.Message);

            }
            catch(Exception ex)
            {
                DaggerfallUnity.LogMessage(ex.Message, true);
            }


            

        }

    }
}
