using Unity.VisualScripting;
using UnityEngine;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST };
public class BattleSystem : MonoBehaviour
{


    //Graphics
    [Serialize] public Canvas canvas;
    private float canvasScaleWidth;
    private float canvasScaleHeight;
    private const float CANVAS_SCALE_FACTOR = 500;


    //Prefabs and Transforms
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform enemies;


    public BattleState state;


    // Player and Enemy placeholders
    Player player;
    Enemy enemy;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasScaleWidth = canvasRect.rect.width / CANVAS_SCALE_FACTOR;
        canvasScaleHeight = canvasRect.rect.height / CANVAS_SCALE_FACTOR;
        Debug.Log(canvasScaleWidth + " " +  canvasScaleHeight);
        state = BattleState.START;
        SetupBattle();
    }

    void SetupBattle()
    {

        //Player and Enemy Initialization
        GameObject playerObj = Instantiate(playerPrefab);
        GameObject enemyObj = Instantiate(enemyPrefab, enemies);

        player = playerObj.GetComponent<Player>();
        enemy = enemyObj.GetComponent<Enemy>();

        player.AddComponent<RectTransform>();
        enemy.AddComponent<RectTransform>();

        player.Initialize("Player1", 100, 5, 5, -1 * canvasScaleWidth, 1 * canvasScaleHeight);
        enemy.Initialize("Die-angle", 10, 1, 0, 1 * canvasScaleWidth, 1 * canvasScaleHeight);

        
    }


    // Update is called once per frame
    void Update()
    {
        player.DrawPlayer();
        enemy.DrawEnemy();
    }
}
