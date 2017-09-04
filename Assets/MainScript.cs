using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour {
    
    public Transform monster;
    //public Transform boundary;
    public UnityEngine.UI.Text messageText;
    public UnityEngine.UI.Text resultText;
    public UnityEngine.UI.Button restartButton;

    private List<Transform> monsters = new List<Transform>();
    private const float INTERVAL = 5.0f;
    private const int MAX_INTERVAL_COUNT = 20;
    private const int MAX_MONSTERS = 100;
    private const float MONSTER_SPEED = 5.0f;
    private int intervalCount = 0;
    private int monsterSpawnCount = 1;
    private int monsterSpawnedTotalCount = 0;
    private int monsterBrokenTotalCount = 0;

    private bool isGameActive = false;

	// Use this for initialization
	void Start () {
        ResetAndStart();
	}

    public void ResetAndStart() {
        restartButton.gameObject.SetActive(false);
        isGameActive = false;
        intervalCount = 0;
        monsterSpawnCount = 1;
        monsterSpawnedTotalCount = 0;
        monsterBrokenTotalCount = 0;
        foreach(var m in monsters) {
            Destroy(m.gameObject);
        }
        monsters.Clear();
        Update();

        monster.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        StartCoroutine(DisplayStartInMessage());
        InvokeRepeating("SpawnCrate", 4.0f, INTERVAL);
    }

	IEnumerator DisplayStartInMessage()
	{
        resultText.gameObject.SetActive(true);
        resultText.text = "Start In 3 seconds";
		yield return new WaitForSeconds(1);
		resultText.text = "Start In 2 seconds";
		yield return new WaitForSeconds(1);
		resultText.text = "Start In 1 second";
		yield return new WaitForSeconds(1);
		resultText.text = "Start!";
        yield return new WaitForSeconds(1);
        resultText.gameObject.SetActive(false);
        isGameActive = true;
		yield break;
	}

    //void createBoundaries() {
    //    boundary.localScale = new Vector3(50.0f, 0.0f, 50.0f);
    //    Instantiate(boundary, new Vector3(0, 0, 25), Quaternion.Euler(90, 0, 0));
    //    Instantiate(boundary, new Vector3(0, 0, -25), Quaternion.Euler(90, 0, 0));
    //    Instantiate(boundary, new Vector3(25, 0, 0), Quaternion.Euler(0, 0, 90));
    //    Instantiate(boundary, new Vector3(-25, 0, 0), Quaternion.Euler(0, 0, 90));
    //    Instantiate(boundary, new Vector3(0, 25, 0), Quaternion.Euler(0, 0, 0));
    //    Instantiate(boundary, new Vector3(0, -25, 0), Quaternion.Euler(0, 0, 0));
    //}

	void SpawnCrate()
	{
        intervalCount++;

        monsterSpawnCount = intervalCount / (int)INTERVAL + 1;

        for (int i = 0; i < monsterSpawnCount; i++)
        {
            var x = Random.Range(30.0f, 50.0f);
            var z = Random.Range(30.0f, 50.0f);
            var xn = Random.Range(-1f, 1f);
            if(xn < 0) {
                x *= -1;
            }
            var zn = Random.Range(-1f, 1f);
            if(zn < 0) {
                z *= -1;
            }
            var c = Instantiate(monster, new Vector3(x, 0, z), Quaternion.identity);
            //var c = Instantiate(monster, new Vector3(0, 0, 50), Quaternion.identity);
            c.LookAt(transform);
            monsters.Add(c);
        }
	}

    // Update is called once per frame
    void Update()
    {
		messageText.text = "Monsters\nCurrent: " + monsters.Count
			+ "\nDead: " + monsterBrokenTotalCount
			+ "\nLeft: " + (MAX_MONSTERS - monsterBrokenTotalCount);
        
        if (isGameActive)
        {
            monsterBrokenTotalCount += monsters.RemoveAll(crate => crate == null);

            float step = MONSTER_SPEED * Time.deltaTime;
            foreach (var c in monsters)
            {
                if (c != null)
                {
                    var animator = c.gameObject.GetComponent<Animator>();
                    if(animator.GetCurrentAnimatorStateInfo(0).IsName("walkforward")) {
                        c.position = Vector3.MoveTowards(c.position, 
                                                         new Vector3(transform.position.x, 0, transform.position.z), step);
                    }
                }
            }

            if (monsterBrokenTotalCount == MAX_MONSTERS)
            {
                resultText.gameObject.SetActive(true);
                resultText.text = "You win!";
                End();
            }
        }

	}

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.root.gameObject.tag.Equals("Monster"))
        {
			foreach (var c in monsters)
			{
                if (c != null)
				{
					var animator = c.gameObject.GetComponent<Animator>();
                    if(c.gameObject.Equals(other.gameObject)) {
                        animator.Play("attack");
                    }
                    else {
						animator.Play("roar");
					}
				}
			}

            resultText.gameObject.SetActive(true);
            resultText.text = "You Lose!";
            isGameActive = false;
            End();
        }
    }

    void End() {
        CancelInvoke("SpawnCrate");
        restartButton.gameObject.SetActive(true);
    }
}
