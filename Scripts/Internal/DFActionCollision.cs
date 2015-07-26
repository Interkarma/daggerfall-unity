using UnityEngine;



namespace DaggerfallWorkshop
{

/// <summary>
/// Detects collisions between player object and whatever object this is attached to.
/// Currently some objects are bugged and won't properly trigger collisions.
/// </summary>
[RequireComponent(typeof(Rigidbody))]

    public class DFActionCollision : MonoBehaviour 
    {
        //public DFAction.TriggerType triggerType = DFAction.TriggerType.none;
        private DaggerfallAction thisAction = null;
        private Rigidbody rigBody = null;
        private bool readyToPlayAgain = true;
  
    
        public void Awake()
        {
            rigBody = this.GetComponent<Rigidbody>();
            rigBody.isKinematic = true;
            rigBody.useGravity = false;
        }


        public void Start()
        {
            if (!thisAction)
                thisAction = GetComponent<DaggerfallAction>();
        }

        void OnTriggerEnter(Collider col)
        {

            if (thisAction == null)
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