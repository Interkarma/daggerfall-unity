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
        [SerializeField]
        private bool readyToPlayAgain = true;
        private DaggerfallAction thisAction = null;
        private Rigidbody rigBody = null;

        public void Awake()
        {
            rigBody = this.GetComponent<Rigidbody>();
            rigBody.isKinematic = true;
            rigBody.useGravity = false;
            rigBody.hideFlags = HideFlags.NotEditable;
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