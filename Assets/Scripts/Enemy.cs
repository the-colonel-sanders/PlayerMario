using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int maxHP;
    public int currentHP;
    public int atk;
    public int def;
    public float x;
    public float y;

    //Graphics
    RectTransform rectTransform;


    public void Initialize(string name, int maxHP, int atk, int def, float x, float y)
    {
        this.enemyName = name;
        this.maxHP = maxHP;
        this.atk = atk;
        this.def = def;
        this.currentHP = maxHP;
        this.x = x;
        this.y = y;

        rectTransform = GetComponent<RectTransform>();
    }

    public void DrawEnemy()
    {
        rectTransform.position = new Vector2(x, y);
    }

}
