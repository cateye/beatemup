﻿using System.Collections;
using UnityEngine;

public class Robot : Enemy
{
    public RobotColor color;
    public SpriteRenderer smokeSprite;
    public SpriteRenderer beltSprite;

    public void SetColor(RobotColor color)
    {
        this.color = color;
        switch (color)
        {
            case RobotColor.Colorless:
                baseSprite.color = Color.white;
                smokeSprite.color = Color.white;
                beltSprite.color = Color.white;
                maxLife = 50.0f;
                normalAttack.attackDamage = 2;
                break;
            case RobotColor.Copper:
                baseSprite.color = new Color(1.0f, 0.75f, 0.62f);
                smokeSprite.color = new Color(0.38f, 0.63f, 1.0f);
                beltSprite.color = new Color(0.86f, 0.85f, 0.71f);
                maxLife = 100.0f;
                normalAttack.attackDamage = 4;
                break;
            case RobotColor.Silver:
                baseSprite.color = Color.white;
                smokeSprite.color = new Color(0.38f, 1.0f, 0.5f);
                beltSprite.color = new Color(0.5f, 0.5f, 0.5f);
                maxLife = 125.0f;
                normalAttack.attackDamage = 5;
                break;
            case RobotColor.Gold:
                baseSprite.color = new Color(0.91f, 0.7f, 0.0f);
                smokeSprite.color = new Color(0.42f, 0.15f, 0.10f);
                beltSprite.color = new Color(0.86f, 0.5f, 0.32f);
                maxLife = 150.0f;
                normalAttack.attackDamage = 6;
                break;
            case RobotColor.Random:
                baseSprite.color = new Color(Random.Range(0, 1.0f), Random.Range(0,
          1.0f), Random.Range(0, 1.0f));
                smokeSprite.color = new Color(Random.Range(0, 1.0f),
          Random.Range(0, 1.0f), Random.Range(0, 1.0f));
                beltSprite.color = new Color(Random.Range(0, 1.0f), Random.Range(0,
          1.0f), Random.Range(0, 1.0f));
                maxLife = Random.Range(100, 250);
                normalAttack.attackDamage = Random.Range(4, 10);
                break;
        }
        currentLife = maxLife;
    }

    //1
    [ContextMenu("Color: Copper")]
    void SetToCopper()
    {
        SetColor(RobotColor.Copper);
    }
    //2
    [ContextMenu("Color: Silver")]
    void SetToSilver()
    {
        SetColor(RobotColor.Silver);
    }
    //3
    [ContextMenu("Color: Gold")]
    void SetToGold()
    {
        SetColor(RobotColor.Gold);
    }
    //4
    [ContextMenu("Color: Random")]
    void SetToRandom()
    {
        SetColor(RobotColor.Random);
    }

    protected override IEnumerator KnockdownRoutine()
    {
        isKnockedOut = true;
        baseAnim.SetTrigger("Knockout");
        ai.enabled = false;
        yield return new WaitForSeconds(2.0f);
        baseAnim.SetTrigger("GetUp");
        knockdownRoutine = null;
        ai.enabled = true;

    }

}

public enum RobotColor
{
    Colorless = 0,
    Copper,
    Silver,
    Gold,
    Random

}
