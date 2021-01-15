using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    public Player player;
    public bool cameraFollows;
    public CameraBounds cameraBounds;

    public LifeBar enemyLifeBar;

    public LevelData currentLevelData;
    private BattleEvent currentBattleEvent;
    private int nextEventIndex;
    public bool hasRemainingEvents;

    public List<GameObject> activeEnemies;

    public Transform[] spawnPositions;
    public GameObject currentLevelBackground;
    public GameObject robotPrefab;

    public Transform walkInStartTarget;
    public Transform walkInTarget;

    public Transform walkOutTarget;

    public LevelData[] levels;
    public static int currentLevel = 0;


    void Start()
    {

        cameraBounds.SetXPosition(cameraBounds.minVisibleX);
        nextEventIndex = 0;
        StartCoroutine(LoadLevelData(levels[currentLevel]));
        currentLevelData = levels[currentLevel];
    }



    private void Update()
    {
        if (currentBattleEvent == null && hasRemainingEvents)
        {
            if (Mathf.Abs(currentLevelData.battleData[nextEventIndex].column -
          cameraBounds.activeCamera.transform.position.x) < 0.2f)
            {
                PlayBattleEvent(currentLevelData.battleData[nextEventIndex]);
            }
        }
        
        if (currentBattleEvent != null)
        {
            //has event, check if enemies are alive!
            if (Robot.TotalEnemies == 0)
            {
                //no more enemies;
                CompleteCurrentEvent();
            }
        }
        if (cameraFollows)
        {
            cameraBounds.SetXPosition(player.transform.position.x);
        }
    }

    private void PlayBattleEvent(BattleEvent battleEventData)
    {

        currentBattleEvent = battleEventData;
        nextEventIndex++;
        cameraFollows = false;
       
        cameraBounds.SetXPosition(battleEventData.column);

        foreach (GameObject enemy in activeEnemies)
        {
            Destroy(enemy);
        }
        activeEnemies.Clear();
        Enemy.TotalEnemies = 0;

        //we take all the data of enemies and spawn
        foreach (EnemyData enemyData in currentBattleEvent.enemies)
        {
            activeEnemies.Add(SpawnEnemy(enemyData));
        }
    }

    private GameObject SpawnEnemy(EnemyData data)
    {
        //Spawn the enemy
        GameObject enemyObj = Instantiate(robotPrefab);
        Vector3 position = spawnPositions[data.row].position;
        position.x = cameraBounds.activeCamera.transform.position.x +
      (data.offset * (cameraBounds.cameraHalfWidth + 1));
        enemyObj.transform.position = position;
        if (data.type == EnemyType.Robot)
        {
            enemyObj.GetComponent<Robot>().SetColor(data.color);
        }

        enemyObj.GetComponent<Enemy>().RegisterEnemy();
        return enemyObj;
    }
    private void CompleteCurrentEvent()
    {
        //clean the currentBatlleEvent, let camera follow and check if there's more events
        currentBattleEvent = null;
        cameraFollows = true;
        cameraBounds.CalculateOffset(player.transform.position.x);

        hasRemainingEvents = currentLevelData.battleData.Count >
      nextEventIndex;
        enemyLifeBar.EnableLifeBar(false);
        if (!hasRemainingEvents)
        {
            StartCoroutine(HeroWalkout());
        }
    }

    private void DidFinishIntro()
    {
        player.UseAutopilot(false);
        player.controllable = true;
        player.walker.enabled = false;

        cameraBounds.EnableBounds(true);
    }

    private IEnumerator LoadLevelData(LevelData data)
    {
        //EnemyData and BattleEvent need to be loaded into the game before they can be used.
        cameraFollows = false;
        currentLevelData = data;
        
        hasRemainingEvents = currentLevelData.battleData.Count > 0;
        activeEnemies = new List<GameObject>();
        
        yield return null; //stop the script so CameraBounds can perform its Starrt method
        cameraBounds.SetXPosition(cameraBounds.minVisibleX);

        currentLevelBackground = Instantiate(currentLevelData.levelPrefab);
      
        if (currentLevelBackground != null)
        {
            //Destroy(currentLevelBackground);
        }
        cameraBounds.EnableBounds(false);
        player.transform.position = walkInStartTarget.transform.position;
        yield return new WaitForSeconds(0.1f);//let everything to load
        player.UseAutopilot(true);
        player.AnimateTo(walkInTarget.transform.position, false, DidFinishIntro);

        cameraFollows = true;
    }

    private IEnumerator HeroWalkout()
    {
        cameraBounds.EnableBounds(false);
        cameraFollows = false;
        player.walker.enabled = true;
        player.UseAutopilot(true);
        player.controllable = false;
        player.AnimateTo(walkOutTarget.transform.position, true,
      DidFinishWalkout);
        yield return null;
    }
  
    private void DidFinishWalkout()
    {
        currentLevel++;
        if (currentLevel >= levels.Length)
        {
            Debug.Log("Game Completed!");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            StartCoroutine(AnimateNextLevel());
        }
        cameraBounds.EnableBounds(true);
        cameraFollows = false;
        player.UseAutopilot(false);
        player.controllable = false;
    }

    private IEnumerator AnimateNextLevel()
    {
        yield return null;
        SceneManager.LoadScene("Game");
    }
}