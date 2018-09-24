using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {


   /** @TODO

        1. Punkte für jeden Block
        2. Minuspunkte für verlorenen Block
        3. Belohnung für gutes Bauen (nicht verlorene Blöcke am Stück)
        4. Wettrennen
        5. Bestrafung für zuviele Minuspunkte

   **/
    
    public static GameManager instance;


	public List<Block> Prefabs = new List<Block> ();
    internal List<Block> StapledBlocks = new List<Block>();
    public List<GameObject> PreviewImages = new List<GameObject>(); 

    //BLOCK CONFIGS
    internal Block CurrentBlock = null;
    internal Block NextBlock = null;
	internal Rigidbody2D currentRB;
    public int createdBlocks = 0;
    public int lostBlocks = 0;
    public int groundedBlocks = 0;
    public int StapledMass;
    public float StapledDrag;

    //SPAWNPOINT & DROPSPEED
	public Transform SpawnPoint;
    public float DropSpeed;
    public float gravity;
    public int Range;

    //SCALE OF OBJECTS & MOVEMENTSCALING
    public float ObjectScale = 0.5f;
    public float StepSize = 0.25f;

    //GOAL CONFIG
    public float Stage = 5;
    public float StageStep = 5;

    //POINTS PARAMETERS
    int currentPoints = 0;
    

    //CameraShaker  
    internal CameraShake camShake;
    public float ShakeDuration;
    public float LimitToShake;

    int currentPreviewIndex;

    float goalCounter = 0;
    public float goalTimeLimit = 3;
    bool reachedGoal = false;
    public float currentHeight = 0;
    //Input Parameters
    internal bool isSwiping = false;
    bool isTapped;
    public float maxDropSpeed;


    //DEBUG TEXT
    public Text DEBUG;

    
    void Awake()
    {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }


    int getRandomIndex(){
		return (int) Mathf.Round(Random.value * (Prefabs.Count -0.55f));
	}

	float getRandomXPosition(){
		float Position = 0;
		int Limit = (int) (Random.value * (Range * 4));
		for (int i = 0; i < Limit; i++) {
			Position += ObjectScale * StepSize;
		}

        if (Random.value > 0.5f) {
            Position = -Position; 
        }
		return SpawnPoint.position.x + Position;
	}

  

	// Use this for initialization
	void Start () {

        camShake = GetComponent<CameraShake>();

		Physics2D.gravity = new Vector2 (0, -9);
        
        currentPreviewIndex = getRandomIndex();
        StartNewBlock(CreateNewBlock(currentPreviewIndex));
    }

    private Block CreateNewBlock(int index)
    {
        return Instantiate(Prefabs[index], new Vector2(getRandomXPosition(), SpawnPoint.position.y), Quaternion.identity);
    }

    //Instantiates and Configs New Block
    private void StartNewBlock(Block NewBlock)
    {
        CurrentBlock = NewBlock;
        CurrentBlock.gameObject.SetActive(true);
        currentRB = CurrentBlock.GetComponent<Rigidbody2D>();
        currentRB.velocity = new Vector2(0, -DropSpeed);
        CurrentBlock.toggleActive();
        createdBlocks += 1;
    }

    internal void StopBlock()
    {
        currentRB.velocity = new Vector2(currentRB.velocity.x, currentRB.velocity.y / 100);
    }

    internal void shakeCam()
    {
        camShake.enabled = true;
        camShake.activateCamShake(ShakeDuration);
    }

    // Update is called once per frame
    void FixedUpdate () {
        currentHeight = getMaxiumHeight();
        //1.Es wird ein neues GameObject generiert.
        //2.Der Spieler bekommt Kontrolle über Gameobject
        //3.Wurde GameObject aufgesetzt? -> 1.
        if (CurrentBlock == null) {
            StartNewBlock(NextBlock);
            NextBlock = null;
		} else {

            if(NextBlock == null)
            {
                int randomIndex = getRandomIndex();
                NextBlock = CreateNewBlock(randomIndex);
                //currentPreviewImage = Instantiate(PreviewImages[randomIndex], new Vector2(3f, SpawnPoint.position.y - 2), Quaternion.identity);
                PreviewImages[currentPreviewIndex].SetActive(false);
                currentPreviewIndex = randomIndex;
                PreviewImages[currentPreviewIndex].SetActive(true);
                
            }

            if(!CurrentBlock.isActive()) {
                CurrentBlock.groundBlock();
            }
            else {
                if(!MobileControls())
                {
                    currentRB.velocity = new Vector2(currentRB.velocity.x, Mathf.Lerp(currentRB.velocity.y, -DropSpeed, 0.35f));
                }
            }

            currentHeight = getMaxiumHeight();
            handleReachingGoals();
        }
    }

    public void handleReachingGoals()
    {
        //CurrentHeight Bigger than Stage Value ?
        if (!reachedGoal && currentHeight >= Stage)
        {
            reachedGoal = true; //Ziel erreicht -> Timer starten
            goalCounter = 0; // Timer auf 0 setzen
        }

        if (reachedGoal) // Ziel erreicht?
        {
            //Ziel immernoch erreicht ?
            if (currentHeight < Stage)
            {
                reachedGoal = false;
            }
            else // wenn ja 
            {
                //Zeit hochzählen 
                goalCounter += Time.deltaTime;

                //Zeit über Limit ?
                if (goalCounter >= goalTimeLimit)
                {
                    //wenn ja Blöcke einfrieren
                    FreezeBlocks();
                }
            }
        }
    }



    internal void addStapledBlock(Block block)
    {
        StapledBlocks.Add(block);
    }

    internal void removeStapledBlock(Block block)
    {
        StapledBlocks.Remove(block);
    }

    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    [SerializeField]
    private bool detectSwipeOnlyAfterRelease = false;
    [SerializeField]
    private float minDistanceForVerticalSwipe = 30f;
    [SerializeField]
    private float minDistanceForHorizontalSwipe = 15;

    float tapDelay = 0;

    bool MobileControls()
    {
        //Debug.DrawRay(CurrentBlock.transform.position, Vector2.down * 0.5f, Color.blue);

        Debug.Log(Input.touchCount);

        if (Input.touchCount != 0) {
        //foreach(Touch touch in Input.touches) { 
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }

            if ((Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled))
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
                Debug.Log("ENDED");
            }

            return true;
        }
        else
        {
            if (isSwiping)
            {
                isSwiping = false;
            }

            if(isTapped)
            {
                if(tapDelay > 0.1f)
                {
                    tapDelay = 0;
                    isTapped = false;
                }
                tapDelay += Time.deltaTime;
            }

        }
        return false;
    }


    public void Swipe(SwipeDirection direction, float distance)
    {

        //Vector2 StartPosition = fingerDownPosition;
        //Vector2 EndPosition = fingerUpPosition;
        
        switch(direction)
        {
            case SwipeDirection.Tap:
                {
                    if (!isTapped)
                    {
                        isTapped = true;
                        currentRB.rotation += 90;
                        CurrentBlock.ToggleShadow();
                    }
                }
                break;

            case SwipeDirection.Up :
                {
                    //NOT USED
                }
            break;

            case SwipeDirection.Down:
                {
                    //CurrentBlock.transform.position = new Vector2(CurrentBlock.transform.position.x, CurrentBlock.transform.position.y - Mathf.Lerp(0, distance/10, 0.075f));
                    currentRB.velocity = new Vector2(currentRB.velocity.x, Mathf.Lerp(currentRB.velocity.y, -maxDropSpeed, 0.25f));
                }
                break;

            case SwipeDirection.Left:
                {
                    CurrentBlock.transform.position = new Vector2(CurrentBlock.transform.position.x - (ObjectScale * StepSize), CurrentBlock.transform.position.y);
                }
                break;

            case SwipeDirection.Right:
                {
                    CurrentBlock.transform.position = new Vector2(CurrentBlock.transform.position.x + (ObjectScale * StepSize), CurrentBlock.transform.position.y);
                }
                break;
            
        }


        Debug.Log("Direction: " + direction + " | Distance: " + distance + " | StartPosition: " + fingerDownPosition + " |  EndPosition: " + fingerUpPosition);

    }


    private void DetectSwipe()
    {
        //if (SwipeDistanceCheckMet())
        //{

        if (IsVerticalSwipe())
        {
            var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
            Swipe(direction, VerticalMovementDistance());
            fingerUpPosition = fingerDownPosition;
            isSwiping = true;

            return;
        }

        if (IsHorizontalSwipe())
        {
            var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            Swipe(direction, HorizontalMovementDistance());
            fingerUpPosition = fingerDownPosition;
            isSwiping = true;
            return;
        }

            

        if (!isSwiping && !isTapped && (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled))
        {
            //TAP --> ROTATION
            Swipe(SwipeDirection.Tap, 0);
        }
        
    }

    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > minDistanceForVerticalSwipe;
    }

    private bool IsHorizontalSwipe()
    {
        return HorizontalMovementDistance() > minDistanceForHorizontalSwipe;
    }


    private bool SwipeDistanceCheckMet()
    {

        return VerticalMovementDistance() > minDistanceForVerticalSwipe || HorizontalMovementDistance() > minDistanceForHorizontalSwipe;
    }



    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }


    public enum SwipeDirection
    {
        Tap,
        Up,
        Down,
        Left,
        Right
    }


    public float getMaxiumHeight()
    {
        float topHeight = 0;
        groundedBlocks = StapledBlocks.Count;
        foreach(Block b in StapledBlocks)
        {
            if(b.transform.position.y >= topHeight)
            {
                topHeight = b.transform.position.y;
            } 
        }
        currentHeight = topHeight;
        return topHeight;

    }

    //Friert Blöcke bei erreichendes Ziels ein, setzt neues Ziel und resetet reachGoal zu false
    public void FreezeBlocks()
    {

        Stage += StageStep;
            foreach (Block b in StapledBlocks)
            {
                if (!b.isStatic())
                {
                    b.GetComponent<SpriteRenderer>().color = Color.grey;
                    b.setStatic();
                }
            }

        SpawnPoint.position = new Vector3(SpawnPoint.position.x, currentHeight + 10, SpawnPoint.position.z);
    }

    private void LateUpdate()
    {
        //SpawnPoint.position = new Vector3(SpawnPoint.position.x, currentHeight + 7.5f, SpawnPoint.position.z);
    }




}
