//using System;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class DiscBehavior : MonoBehaviour
    {
        public Disc disc;
        public DiscMould mould;

        public GameObject putterModelPrefab;
        public GameObject midRangeModelPrefab;
        public GameObject driverModelPrefab;
        public Material discMat;
        public GameObject stamp;
        public DiscColor colors;
        public GameObject discModel;
        public TrailRenderer speedLine;
        public Transform blobShadow;

        internal void HitTreeBranches()
        {
            var velocityChange = UnityEngine.Random.Range(2f, 5f);
            var currentDirection = rb.velocity.normalized;
            var currentSpeed = rb.velocity.magnitude;
            var newSpeed = rb.velocity.magnitude - velocityChange;
            var newSpeedPositive = newSpeed < 1 ? 1 : newSpeed;

            Debug.Log("hit speed: " + currentSpeed + " speed after " + newSpeedPositive);
            rb.velocity = currentDirection * newSpeedPositive;

            // rb.AddForce(UnityEngine.Random.insideUnitSphere*5f);
        }

        [SerializeField] private SpriteRenderer[] puttingGuideArrows;

        //Audio//
        [SerializeField] private AudioSource chainsHit;

        internal void PuttingCollider()
        {
            Cl.enabled = false;
            boxCl.isTrigger = false;
        }

        [SerializeField] private AudioSource basketHit;
        [SerializeField] private AudioSource treeHit;
        [SerializeField] private AudioSource groundHit;
        [SerializeField] public AudioSource throwSwish;
        public Player player;
        public DiscData discData;

        public float verticalSpeed;          //CurrentdDisc speed (magnitude) on the vertical axes 

        public Vector3 flightDirection;     //Current vector of disc flight direction, normalized. 
        public float discAngle;          //Current disc fade/turn angle.

        public bool isThrown = false;
        private Vector3 startPos;

        public Rigidbody rb;
        public BoxCollider boxCl;
        //public SphereCollider sphereCl;
        public MeshCollider Cl;

        public bool hitGround = false;
        private bool hitChains = false;
        private bool hitBasket = false;

        public bool completedThrow = false;
        public float launchSpeed = 0;
        public float throwDistance = 0;

        //Disc Property Modifyers
        public float dragMod = 1.0f;
        public float bounceMod = 1.0f;
        public float accuracyMod = 1.0f;
        internal float fadeMod = 1.0f;
        internal float windBreak = 1.0f;

        public PhysicMaterial bouncyMaterial;
        public PhysicMaterial stickyMaterial;
        public PhysicMaterial putterMaterial;

        private bool inboundsFlag;
        private bool OBFlag;

        public Vector3 windDirection;

        public float windAngle;
        public float windlocalAngle;
        public float windPulseCounter = 1f;
        public bool windPulsealternator = true;
        public float windEffectMagnitude;

        public bool discDebug;
        public DiscDebug discDebugScript;

        private Vector3 totalWind;
        private float completeThrowTimer;

        private float ShotTypeModifyer
        {
            get
            {
                int mod = 1;
                if (isDiscLauncher)
                {
                    return 1;
                }
                if (!isMenu)
                {
                    if (player.throwUI.shotSelect.SelectedShot == ShotSelector.ShotType.ForeHand)
                    {
                        mod *= -1;
                    }
                }

                if (Menu.PlayerSave.playerGet && player.gameState.playerSave.playerSettings.settingsData.leftHandedMode)
                {
                    mod *= -1;
                }
                return mod;
            }
        }

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

        public float ForeHandMod
        {
            get
            {
                if (!isMenu && player.throwUI.shotSelect.SelectedShot == ShotSelector.ShotType.ForeHand)
                {
                    return 0.95f;
                }
                else
                {
                    return 1f;
                }
            }
        }

        public float posX;
        public float torqueStability;

        public PuttingBehavior puttingBehavior;
        private bool flashDisc = false;
        private float flashCounter;
        private Color brightColor;
        private bool isReplay;
        public bool isMenu;
        public bool isDiscLauncher;
        private bool discScored;
        public bool isDiscTester;
        private float groundActionIterator;

        public void PopulateReplayDisc(Disc disc)
        {
            this.disc = disc;
            mould = discData.GetMould(disc.mouldName);
            rb.useGravity = false;
            blobShadow.parent = null;                           //we don't want the blobshadow parented, it shouldn't rotate with the disc.
            PopulateAppearance(disc);
            isReplay = true;
        }

        public void PopulateMenuDisc(Disc disc, bool isLauncher)
        {
            this.disc = disc;
            mould = discData.MenuDisc();
            rb.useGravity = false;
            rb.mass = 175f * 0.001f;
            blobShadow.parent = null;                           //we don't want the blobshadow parented, it shouldn't rotate with the disc.
            PopulateAppearance(disc);
            isMenu = true;
            isDiscLauncher = isLauncher;
            startPos = transform.position;

        }

        public void PopulateDisc(Disc disc)
        {
            this.disc = disc;
            mould = discData.GetMould(disc.mouldName);
            rb.mass = 175f * 0.001f;
            Cl.isTrigger = true;                               //setting the collider to trigger only when the player is holding it in hand. prevents it from being knocked away by trees.
            blobShadow.parent = null;                           //we don't want the blobshadow parented, it shouldn't rotate with the disc.
            if (mould.discType == DiscMould.DiscType.Putter)
            {
                Cl.material = putterMaterial;
                boxCl.material = putterMaterial;
            }
            foreach (DiscProperty.PropertyType prop in disc.discProperties)
            {
                discData.GetProperty(prop).ApplyProp(this);
            }
            rb.useGravity = false;
            startPos = transform.position;
            PopulateAppearance(disc);

            var terrainHeight = player.sceneManager.Terrain.terrain.SampleHeight(transform.position);

            if (!player.outOfBounds && terrainHeight < (player.sceneManager.WaterLevel.y - 0.1f))
            {
                OBFlag = true;
                player.outOfBoundsWater = true;
                // player.outOfBoundsPosition = player.transform.position;
                Debug.Log("OB water true");

            }
            else
            {
                OBFlag = false;
                player.outOfBoundsWater = false;
                Debug.Log("OB water false");
            }

            if (mould.discType == DiscMould.DiscType.Putter)
            {
                windBreak = 0.75f;
            }
        }

        public void PopulateAppearance(Disc disc)
        {
            colors = discData.GetColor(disc);

            discModel = Instantiate(DiscModelPrefab, this.transform);

            discMat = discModel.GetComponent<Renderer>().material;
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

        void OnCollisionEnter(Collision collision)
        {
            var hitSpeed = collision.relativeVelocity.magnitude / 20;
            // Debug.Log("Collision " + collision.gameObject.tag + hitSpeed);


            if (collision.gameObject.tag == "Ground")
            {
                // throwDistance = Vector3.Distance(startPos, transform.position);

                var terrainHeight = 0f;
                if (!isMenu) terrainHeight = player.sceneManager.Terrain.terrain.SampleHeight(transform.position);

                if (transform.position.y > terrainHeight + 1)
                {
                    treeHit.volume = Mathf.Lerp(0.1f, 0.8f, hitSpeed);
                    treeHit.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
                    treeHit.Play();
                }
                else if (hitSpeed > 0.1f)
                {
                    groundHit.volume = Mathf.Lerp(0.1f, 0.5f, hitSpeed);
                    groundHit.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
                    groundHit.Play();
                }
                hitGround = true;
            }
            else if (collision.gameObject.tag == "Chains")
            {
                // throwDistance = Vector3.Distance(startPos, transform.position);
                if (!hitChains)
                {
                    blobShadow.GetComponent<Projector>().enabled = false;
                    chainsHit.pitch = UnityEngine.Random.Range(0.7f, 1f);
                    chainsHit.volume = Mathf.Lerp(0.1f, 0.3f, hitSpeed);
                    chainsHit.Play();
                }
                hitGround = true;
                hitChains = true;
            }
            else if (collision.gameObject.tag == "Basket")
            {
                // throwDistance = Vector3.Distance(startPos, transform.position);
                if (!hitBasket)
                {
                    blobShadow.GetComponent<Projector>().enabled = false;
                    basketHit.pitch = UnityEngine.Random.Range(0.8f, 1f);
                    basketHit.volume = Mathf.Lerp(0.01f, 1f, hitSpeed);
                    basketHit.Play();
                }
                hitGround = true;
                hitBasket = true;
            }
        }

        void FixedUpdate()
        {
            blobShadow.position = this.transform.position + (Vector3.up * -.1f);
            verticalSpeed = rb.velocity.magnitude;

            if (isThrown && verticalSpeed < 10)
            {
                speedLine.time *= 0.9f;
            }

            if (isReplay)
            {
                return;
            }

            flightDirection = rb.velocity.normalized;            
            var pos = new Vector3(transform.position.x, startPos.y, transform.position.z);
            rb.angularDrag = verticalSpeed;

            if (flashDisc)
            {
                FlashDisc();
            }

            //if (!isMenu && !player.sceneManager.Terrain.terrain.terrainData.bounds.Contains(pos))
            if (!isMenu && !player.sceneManager.Terrain.TerrainBounds().Contains(pos))
            {
                Debug.Log("out of terrain bounds");
                hitGround = true;
                rb.velocity = Vector3.zero;
            }

            var terrainHeight = 0f;
            if (!isMenu) terrainHeight = player.sceneManager.Terrain.terrain.SampleHeight(transform.position);
                        
            if (!isMenu && !player.outOfBounds && terrainHeight < (player.sceneManager.WaterLevel.y - 0.1f))
            {
                if (!OBFlag)
                {
                    OBFlag = true;
                    player.outOfBoundsWater = true;
                    player.outOfBoundsPosition = transform.position;
                    Debug.Log("OB water true");
                }
            }
            else if (!isMenu)
            {
                if (OBFlag)
                {
                    OBFlag = false;
                    player.outOfBoundsWater = false;
                    Debug.Log("OB water false");
                }
            }

            if (isThrown && !hitGround)
            {
                //recording the speed of the disc as it leaves the hand.
                if (launchSpeed == 0)
                {
                    launchSpeed = rb.velocity.magnitude;
                }

                if (!isMenu) rb.AddForce(Wind());

                var turnFade = Vector3.Lerp(Turn(), Fade(), 0.5f);

                var forcePosition = transform.position + (transform.forward * posX) + (turnFade * 1f);

                rb.AddForceAtPosition(turnFade * -0.8f, forcePosition, ForceMode.Force);
                rb.AddTorque((transform.forward * Turn().magnitude * player.rotMod * -1) * ShotTypeModifyer);
                rb.AddTorque((transform.forward * Fade().magnitude * player.rotMod) * ShotTypeModifyer);

                rb.AddForce(Lift() * 0.5f, ForceMode.Force);

                rb.AddForce(Drag());

                discModel.transform.Rotate(Vector3.forward, Time.deltaTime * verticalSpeed * 1000);

                //Debug.Log("Terrain height: " + terrainHeight);
                //Debug.Log("Water Level: " + player.sceneManager.WaterLevel.y);
            }
            else if (isThrown && hitGround && !completedThrow)
            {
                completeThrowTimer += Time.deltaTime;
                rb.angularDrag = 1f;

                if(transform.position.y - terrainHeight < 0.1f)
                {
                    DiscGroundAction();
                }

                if (rb.velocity.magnitude < 0.01 || (completeThrowTimer > 3 && rb.velocity.magnitude < 1))
                {
                    //boxCl.enabled = true;
                    //boxCl.isTrigger = false;

                    //sphereCl.enabled = false;

                    completedThrow = true;
                    Invoke("CompletedThrow", 1f);
                }
            }
        }

        private void DiscGroundAction()
        {
            if (groundActionIterator > 1f)
            {
                Debug.Log("boop");
                // rb.AddForceAtPosition(UnityEngine.Random.insideUnitSphere, UnityEngine.Random.insideUnitSphere);
                //var randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 1f, UnityEngine.Random.Range(-1f, 1f));
                var randomDirection = UnityEngine.Random.insideUnitSphere;
                var randomPosition = UnityEngine.Random.insideUnitSphere*0.1f;
                rb.AddForceAtPosition(transform.up+randomDirection*verticalSpeed, transform.position + randomPosition);

                Debug.Log(randomPosition.ToString());
                //rb.AddForce(UnityEngine.Random.insideUnitSphere.normalized*10f);
                groundActionIterator = 0f;
            }
            groundActionIterator += UnityEngine.Random.Range(0.05f, 0.1f);
        }

        private void FlashDisc()
        {
            var t = Mathf.PingPong(flashCounter, 1f);

            discMat.color = Color.Lerp(colors.baseColor, brightColor, t);

            foreach (SpriteRenderer sprite in puttingGuideArrows)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, t * 0.5f);
            }

            flashCounter += 0.02f;
        }

        internal void ShowPuttingGuide(bool v)
        {
            puttingBehavior.EnableCollider(v);
            brightColor = new Color(colors.baseColor.r * 1.5f, colors.baseColor.g * 1.5f, colors.baseColor.b * 1.5f);
            foreach (SpriteRenderer sprite in puttingGuideArrows)
            {
                sprite.gameObject.SetActive(v);
            }
            flashDisc = v;
            if (v)
            {
                flashCounter = 0.0f;
            }
            else
            {
                discMat.color = colors.baseColor;
            }
        }

        private void CompletedThrow()
        {
            throwDistance = Vector3.Distance(startPos, transform.position);
            if (!isMenu)
            {
                if (!discScored)
                {
                    player.action.CompletedThrow(throwDistance, transform.position);
                    discScored = true;
                }
            }
            else if (isDiscLauncher)
            {

            }
            else if (isDiscTester)
            {
                player.action.CompletedThrow(throwDistance, transform.position);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private Vector3 Drag()
        {
            var dragFactor = (mould.drag / 1000) * -0.5f;

            var drag = (dragFactor * dragMod) * rb.velocity.normalized * rb.velocity.sqrMagnitude;

            return drag;
        }

        private Vector3 Lift()
        {
            var liftFactor = (mould.lift / 1000);
            liftFactor = (liftFactor * 2);
            //var windLiftAngle = Vector3.Angle(player.windDirection, transform.forward)/180;
            //windLiftFactor = 1+Mathf.Lerp(-0.5f, +0.5f, windLiftFactor)*windEffectMagnitude;

            var windAdjustedSpeed = (rb.velocity - (totalWind * 5f)).sqrMagnitude;

            var direction = Vector3.Cross(rb.velocity.normalized, transform.right);

            //var liftForce = Vector3.up * liftFactor * Mathf.Pow(windAdjustedSpeed, 2f);
            var liftForce = direction * liftFactor * windAdjustedSpeed;


            return liftForce; //* angleModifier;
        }

        private Vector3 Fade()
        {
            //speedfactor = discSpeed / verticalSpeed;
            //speedfactor = speedfactor > 1 ? 1 : speedfactor;           
            var direction = Vector3.Cross(rb.velocity.normalized, transform.up);

            var fade = direction * mould.fade * -1f * ShotTypeModifyer * fadeMod/** Mathf.Pow(speedfactor,2)*/;

            return fade;
        }

        private Vector3 Turn()
        {
            var turnFactor = (mould.turn / 1000);
            var direction = Vector3.Cross(rb.velocity.normalized, transform.up);

            var turn = direction * turnFactor * Mathf.Pow(verticalSpeed, 2) * ShotTypeModifyer;

            return turn;
        }

        private Vector3 Wind()
        {
            Vector3 windFactor = player.windManager.windDirection * (player.windManager.currentWindSpeed * 10 * windBreak);

            var windX = Vector3.Reflect(windFactor, transform.up);
            windAngle = Vector3.Angle(windFactor, windX);
            var windAngleFactor = windAngle / 180;

            windEffectMagnitude = Mathf.Lerp(0.5f, 1f, windAngleFactor);

            var wind = windFactor * windEffectMagnitude * 0.15f;
            totalWind = wind;

            if (player.throwUI.shotSelect.SelectedShot != ShotSelector.ShotType.Putt && windPulseCounter > 1)
            {
                windPulseCounter = 0f;
                var bounceSpeed = UnityEngine.Random.Range(0.03f, 0.04f) * player.windManager.currentWindSpeed * verticalSpeed;
                if (!windPulsealternator)
                {
                    bounceSpeed *= -1f;
                }
                windPulsealternator = !windPulsealternator;
                rb.AddForce(transform.up * bounceSpeed, ForceMode.Impulse);
                // Debug.Log("windBounce " + bounceSpeed.ToString());
            }
            windPulseCounter += UnityEngine.Random.Range(0.01f, 0.015f);

            return wind;
        }

        public void Throw(float power, float accuracyMod)
        {
            isThrown = true;
            rb.useGravity = true;
            RandomRotation(accuracyMod);
            var factor = Mathf.Pow(power, 0.5f);
            var adjustedPower = (factor / Mathf.Pow(1f, 0.5f)) * ForeHandMod;
            var throwDirection = new Vector3(0, 0, 5f) * adjustedPower;
            rb.AddRelativeForce(throwDirection, ForceMode.Impulse);
            // Cl.isTrigger = false;
            Invoke("EnableDiscCollider", 0.1f);
            completeThrowTimer = 0f;
            transform.parent = null;
            speedLine.enabled = true;

            throwSwish.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
            throwSwish.volume = Mathf.Lerp(0.5f, 1f, adjustedPower);
            throwSwish.Play();
        }

        private void RandomRotation(float accuracyMod)
        {
            var dir = UnityEngine.Random.Range(accuracyMod * -1, accuracyMod);
            transform.Rotate(transform.up, dir);
        }

        public void Putt()
        {
            if (isThrown)
            {
                return;
            }
            transform.parent = null;
            player.sceneManager.Basket.HideBasketIcon();
            player.sceneManager.MiniMap.trackFlight = true;
            Invoke("EnableDiscCollider", 0.1f);
            player.action.StartReplayRecord();
            isThrown = true;
            rb.useGravity = true;
            // Cl.isTrigger = false;
            if (Menu.PlayerSave.playerGet)
            {
                if (!player.gameState.playerSave.GetTutorialCheck().seenFirstPutt)
                {
                    player.UI.Tutorial().PuttComplete();
                }
            }
        }

        private void EnableDiscCollider()
        {
            Cl.isTrigger = false;
        }

        internal void HitWater()
        {
            rb.drag = 10f;
            hitGround = true;
        }

        internal void RemoveDisc()
        {
            Destroy(blobShadow.gameObject);
            Destroy(this.gameObject);
        }

        internal void MenuPutt()
        {
            if (isThrown)
            {
                return;
            }
            //Debug.Log("putt");
            isThrown = true;
            rb.useGravity = true;
            Cl.isTrigger = false;
            transform.parent = null;
            player.action.ThrowAgain();
        }
    }
}