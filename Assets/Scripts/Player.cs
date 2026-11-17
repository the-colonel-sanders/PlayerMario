using UnityEngine;

public class Player : MonoBehaviour
{

    public string playerName;
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
        this.playerName = name;
        this.maxHP = maxHP;
        this.atk = atk;
        this.def = def;
        this.currentHP = maxHP;
        this.x = x;
        this.y = y;

        rectTransform = GetComponent<RectTransform>();
    }

    public void DrawPlayer()
    {
        rectTransform.position = new Vector2(x, y);
    }

}
