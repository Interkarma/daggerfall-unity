// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;

namespace DaggerfallWorkshop
{
    [RequireComponent(typeof(MeshCollider))]
    public class DaggerfallActionCollision : MonoBehaviour
    {
        [SerializeField]
        private bool readyToPlayAgain = true;
        private DaggerfallAction thisAction = null;
        private Rigidbody rigBody = null;
        public bool isFlat = false;

        // Use this for initialization
        void Start()
        {
            if (!thisAction)
                thisAction = GetComponent<DaggerfallAction>();
            if (!rigBody)
                rigBody = this.GetComponent<Rigidbody>();

            SetupCollision();
        }


        public void SetupCollision()
        {
            if (isFlat)
            {
                try
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    Physics.IgnoreCollision(this.GetComponent<MeshCollider>(), player.GetComponent<CharacterController>());
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
            else
            {
                if (!rigBody)
                    rigBody = gameObject.AddComponent<Rigidbody>();
                rigBody.isKinematic = true;
                rigBody.useGravity = false;
                rigBody.hideFlags = HideFlags.NotEditable;
            }

        }

        void OnTriggerEnter(Collider col)
        {

            if (thisAction == null || isFlat)
                return;

            if (!thisAction.IsPlaying() && readyToPlayAgain)
            {
                readyToPlayAgain = false;
                thisAction.Receive(col.gameObject, false);
            }

        }

        void OnTriggerExit(Collider col)
        {
            readyToPlayAgain = true;

        }


    }

}