using UnityEngine;

namespace Assets.Scripts
{
    public class MenuDisc : MonoBehaviour
    {
        public Disc disc;
        public DiscMould mould;

        public GameObject putterModelPrefab;
        public GameObject midRangeModelPrefab;
        public GameObject driverModelPrefab;
        public Renderer discRenderer;
        public GameObject stamp;
        public GameObject discModel;

        //public TrailRenderer speedLine;
        //public Transform blobShadow;

        ////Audio//
        //[SerializeField] private AudioSource chainsHit;
        //[SerializeField] private AudioSource basketHit;
        //[SerializeField] private AudioSource treeHit;
        //[SerializeField] private AudioSource groundHit;
        //[SerializeField] private AudioSource throwSwish;
        public DiscData discData;

        //public float verticalSpeed;          //CurrentdDisc speed (magnitude) on the vertical axes 

        //public Vector3 flightDirection;     //Current vector of disc flight direction, normalized. 
        //public float discAngle;          //Current disc fade/turn angle.

        //public bool isThrown = false;
        //private Vector3 startPos;

        //public Rigidbody rb;
        //public BoxCollider cl;
        
        //public bool hitGround = false;
        //private bool hitChains = false;
        //private bool hitBasket = false;

        //public bool completedThrow = false;
        //public float launchSpeed = 0;
        //public float throwDistance = 0;

        public GameObject DiscModelPrefab
        {
            get
            {
                switch (mould.discType)
                {
                    case DiscMould.DiscType.Putter:
                        return putterModelPrefab;
                    case DiscMould.DiscType.Midrange:
                        return midRangeModelPrefab;
                    case DiscMould.DiscType.Fairway:
                        return driverModelPrefab;
                    default:
                        return midRangeModelPrefab;
                }
            }
        }

        //public float posX;
        //public float torqueStability;

        public PuttingBehavior puttingBehavior;

        public void PopulateDisc(Disc disc)
        {
            this.disc = disc;
            mould = discData.GetMould(disc.mouldName);

            var colors = discData.GetColor(disc);

            discModel = Instantiate(DiscModelPrefab, this.transform);
            
            var discMat = discModel.GetComponent<Renderer>().material;
            discMat.SetColor("_Color", colors.baseColor);
            discMat.SetTexture("_Stamp", mould.mouldStamp.texture);
            discMat.SetColor("_StampColor", colors.stampColor);
            discMat.SetColor("_PatternColor", new Color(0, 0, 0, 0));

            if (colors is DiscColorPattern)
            {
                var pattern = colors as DiscColorPattern;
                discMat.SetTexture("_Pattern", pattern.pattern.texture);
                discMat.SetColor("_PatternColor", pattern.patternColor);
            }

        }

        //void OnCollisionEnter(Collision collision)
        //{
        //    var hitSpeed = collision.relativeVelocity.magnitude / 20;

        //    if (collision.gameObject.tag == "Ground")
        //    {
        //        groundHit.volume = Mathf.Lerp(0.1f, 0.5f, hitSpeed);
        //        groundHit.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        //        groundHit.Play();

        //        hitGround = true;
        //    }
        //    else if (collision.gameObject.tag == "Chains")
        //    {
        //        if (!hitChains)
        //        {
        //            blobShadow.GetComponent<Projector>().enabled = false;
        //            chainsHit.pitch = UnityEngine.Random.Range(0.7f, 1f);
        //            chainsHit.volume = Mathf.Lerp(0.01f, 0.1f, hitSpeed);
        //            chainsHit.Play();
        //        }
        //        hitGround = true;
        //        hitChains = true;
        //    }
        //    else if (collision.gameObject.tag == "Basket")
        //    {
        //        throwDistance = Vector3.Distance(startPos, transform.position);
        //        if (!hitBasket)
        //        {
        //            basketHit.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        //            basketHit.volume = Mathf.Lerp(0.01f, 1f, hitSpeed);
        //            basketHit.Play();
        //            blobShadow.GetComponent<Projector>().enabled = false;
        //        }
        //        hitGround = true;
        //        hitBasket = true;
        //    }
        //}

        //void FixedUpdate()
        //{
        //    verticalSpeed = rb.velocity.magnitude;
        //    blobShadow.position = this.transform.position;
        //    flightDirection = rb.velocity.normalized;
        //   // var pos = new Vector3(transform.position.x, startPos.y, transform.position.z);

        //    if (isThrown && verticalSpeed < 10)
        //    {
        //        speedLine.time *= 0.9f;
        //    }

        //    if (isThrown && !hitGround)
        //    {
        //        //recording the speed of the disc as it leaves the hand.
        //        if (launchSpeed == 0)
        //        {
        //            launchSpeed = rb.velocity.magnitude;
        //        }

        //        rb.AddForce(Drag());

        //        var turnFade = Vector3.Lerp(Turn(), Fade(), 0.5f);
        //        var forcePosition = transform.position + (transform.forward * posX) + (turnFade * 0.1f);
        //        rb.AddForceAtPosition(Vector3.Lerp(Lift(), (turnFade * -1), 0.5f), forcePosition, ForceMode.Force);
        //        discModel.transform.Rotate(Vector3.forward, Time.deltaTime * verticalSpeed * 1000);

        //        if (flightDirection.magnitude > 0)
        //        {
        //            transform.rotation = Quaternion.LookRotation(flightDirection);
        //        }
        //    }
        //    else if (isThrown && hitGround && !completedThrow)
        //    {
        //        if (rb.velocity.magnitude < 0.1)
        //        {
        //            completedThrow = true;
        //            // rb.isKinematic = false;
        //            rb.velocity = Vector3.zero;
        //            rb.angularVelocity = Vector3.zero;
        //        }
        //    }
        //}

        //private Vector3 Drag()
        //{
        //    var dragFactor = (mould.drag / 1000) * -0.5f;

        //    var drag = (dragFactor) * rb.velocity.normalized * rb.velocity.sqrMagnitude;

        //    return drag;
        //}

        //private Vector3 Lift()
        //{
        //    var liftFactor = (mould.lift / 1000);
        //    liftFactor = (liftFactor * 2);
        //    //var windLiftAngle = Vector3.Angle(player.windDirection, transform.forward)/180;
        //    //windLiftFactor = 1+Mathf.Lerp(-0.5f, +0.5f, windLiftFactor)*windEffectMagnitude;

        //    var windAdjustedSpeed = rb.velocity.magnitude;

        //    var liftForce = transform.up * liftFactor * Mathf.Pow(windAdjustedSpeed, 2f);
        //   // var noWindLiftForce = transform.up * liftFactor * Mathf.Pow(verticalSpeed, 2f);

        //    //var angleModifier = 1 - Mathf.Abs(discAngle);            

        //    return liftForce; //* angleModifier;
        //}

        //private Vector3 Fade()
        //{
        //    //speedfactor = discSpeed / verticalSpeed;
        //    //speedfactor = speedfactor > 1 ? 1 : speedfactor;           
        //    var fade = transform.right * mould.fade * -1 /** Mathf.Pow(speedfactor,2)*/;
        //    return fade;
        //}

        //private Vector3 Turn()
        //{
        //    var turnFactor = (mould.turn / 1000);
        //    var turn = transform.right * turnFactor * Mathf.Pow(verticalSpeed, 2);

        //    return turn;
        //}

        //public void Throw(float power)
        //{
        //    isThrown = true;
        //    rb.useGravity = true;
        //    var throwDirection = transform.forward;
        //    rb.AddForce(throwDirection * power, ForceMode.Impulse);
        //    cl.isTrigger = false;
        //    speedLine.enabled = true;

        //    throwSwish.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        //    throwSwish.volume = (0.5f);
        //    throwSwish.Play();
        //}

        //internal void HitWater()
        //{
        //    rb.drag = 10f;
        //    hitGround = true;
        //}

        //internal void RemoveDisc()
        //{
        //    Destroy(blobShadow.gameObject);
        //    Destroy(this.gameObject);
        //}
    }
}