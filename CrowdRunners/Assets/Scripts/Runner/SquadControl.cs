using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SquadControl : MonoBehaviour
{
    [SerializeField] SquadFormation squadFormation;

    [SerializeField] float speed;

    Transform target;

    bool stop;
    bool attack;
    bool stairsPart,finishLine;
    bool windowTriggered;

    Tweener camTween;

    GameManager gameManager;

    private void Awake() 
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start() 
    {
        speed = gameManager.minRunnerSpeed + ((PlayerPrefs.GetInt("level") / gameManager.increaseSpeedPerLevel)*gameManager.increaseSpeed);
        if(speed > gameManager.maxRunnerSpeed){ speed = gameManager.maxRunnerSpeed; }
    }

    void Update() 
    {
        if(attack && stop){ Attack(); }
        if(squadFormation.transform.childCount <= 0 && !gameManager.gameover && stairsPart){ camTween.Kill(); stop = true; gameManager.Win();}
        else if(squadFormation.transform.childCount <= 0 && !stairsPart && !gameManager.gameover){ stop = true; gameManager.Lose(); }
    }
    
    void FixedUpdate()
    {
        if(gameManager.readyforMove && !stop){ Moving(); }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Window" && !windowTriggered)
        {
            squadFormation.AddRunners(other.gameObject.GetComponent<Window>().Value,false);
            windowTriggered = true;
        }
        if(other.tag == "FinishLine")
        {
            Camera.main.transform.DOLocalMove(new Vector3(0.75f,0.75f,-0.6f),1f);
            Camera.main.transform.DOLocalRotate(new Vector3(25,-45,0),1f);
            Camera.main.DOFieldOfView(80,1f);
            
            StartCoroutine(makePyramid());

            transform.DOLocalMoveX(0,1f);
            speed = speed/1.25f;

            finishLine = true;
        }
        if(other.tag == "Stair")
        {
            camTween = Camera.main.transform.DOLocalMoveY(1,0.5f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            stairsPart = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Window")
        {
            windowTriggered = false;
        }
    }

    public void Moving()
    {
        float NewX = 0;
        float touchXDelta = 0;

        #if UNITY_ANDROID && !UNITY_EDITOR
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && !finishLine)
        {
            touchXDelta = 20*Input.GetTouch(0).deltaPosition.x / Screen.width;
            if(touchXDelta < 0)
            {
                squadFormation.Turn(-1);
            }
            else if(touchXDelta != 0)
            {
                squadFormation.Turn(1);
            }
            else
            {
                squadFormation.Turn(0);
            }
        }
        else
        {
            squadFormation.Turn(0);
        }
        #endif

        #if UNITY_STANDALONE || UNITY_EDITOR
        if(Input.GetMouseButton(0) && !finishLine)
        {
            touchXDelta = 10*Input.GetAxis("Mouse X");
            if(touchXDelta < 0)
            {
                squadFormation.Turn(-1);
            }
            else if(touchXDelta != 0)
            {
                squadFormation.Turn(1);
            }
            else
            {
                squadFormation.Turn(0);
            }
        }
        else
        {
            squadFormation.Turn(0);
        }
        #endif

        NewX = transform.position.x + touchXDelta * 1 * Time.deltaTime;
        NewX = Mathf.Clamp(NewX ,-0.45f,0.45f);

        Vector3 newPos = new Vector3(NewX,transform.position.y,transform.position.z + speed * Time.deltaTime);
        transform.position = newPos;
    }

    public void Attack()
    {
        if(target)
        {
            Vector3 v = new Vector3(target.position.x,transform.position.y,target.position.z);
            transform.position = Vector3.MoveTowards(transform.position, v, .5f * Time.deltaTime);
            squadFormation.Turn(0);
        }
        else
        {
            target = null;
            stop = false;
            attack = false;
        }
    }

    public void ReadyAttack(Transform _target)
    {
        target = _target;
        stop = true;
        attack = true;

        squadFormation.setFormation = false;
    }

    public void Move()
    {
        stop = false;
        for (int i = 0; i < squadFormation.transform.childCount; i++)
        {
            if(squadFormation.transform.GetChild(i).gameObject.tag == "Stickman")
            { 
                squadFormation.transform.GetChild(i).GetComponent<Animator>().SetBool("Run",true);
            }
        }
    }

    IEnumerator makePyramid()
    {
        yield return new WaitForSeconds(1f);
        List<GameObject> runnersList = new List<GameObject>();
        List<GameObject> layerRunnersList = new List<GameObject>();

        int max = 0;
        int Process = 0;
        for (int a = 0; a < squadFormation.transform.childCount; a++)
        {
            if(squadFormation.transform.GetChild(a).gameObject.tag == "Stickman")
            {
                runnersList.Add(squadFormation.transform.GetChild(a).gameObject);
            }

            Process++;
            if(Process == max + 1)
            {
                max++;
                Process = 0;
            }
        }

        int maxRunner = max;
        int Process2 = 0;
        int Layer = 0;
        for(int b = 0; b < runnersList.Count; b++)
        {
            runnersList[b].GetComponent<Rigidbody>().isKinematic = true;
            runnersList[b].transform.DOScale(new Vector3(0,0,0) , 0.25f);
            runnersList[b].transform.DOLocalMove(new Vector3(((max - 1) * -0.025f)+0.025f * (Layer)+(Process2 * 0.05f),0.05f+(0.15f * Layer),0) , 0.5f);
            layerRunnersList.Add(runnersList[b].gameObject);

            Process2++;
            if(Process2 == maxRunner)
            {
                yield return new WaitForSeconds(0.15f);
                maxRunner--;
                Process2 = 0;
                b = 0;
                Layer++;

                for(int z = 0; z < layerRunnersList.Count; z++)
                {
                    layerRunnersList[z].transform.DOScale(new Vector3(0.75f,0.75f,0.75f) , 0.25f);
                    runnersList.Remove(layerRunnersList[z]);
                }

                layerRunnersList.Clear();
            }
        }
        
        for(int c = 0; c < runnersList.Count; c++)
        {
            Destroy(runnersList[c] , 1f);
            runnersList[c].transform.DOScale(new Vector3(0,0,0) , .5f);
        }
        runnersList.Clear();
    }

}
