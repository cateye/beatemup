using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    public Image fillImage;
    public Image avatarImage;
    public Sprite[] fillSprites;


    // Start is called before the first frame update
    void Start()
    {
        
        FillHpBar(1f);

        //EnableLifeBar(true);
    }
    /*
     private void FixedUpdate()
    {
        FillHpBar();
    SetAvatar(avatar,avatarColor);
    } */
    public void SetAvatar(Sprite avatar, Color color)
    {
        avatarImage.sprite = avatar;
        avatarImage.color = color;

    }

    private Sprite GetHPSprite(float progress)
    {
        if (progress >= 0.5f)
        {
            return fillSprites[0];
        }
        else if (progress >= 0.25f)
        {
            return fillSprites[2];
        }
        else
        {
            return fillSprites[1];
        }
    }

    public void FillHpBar(float progress)
    {
        fillImage.fillAmount = progress;
        fillImage.sprite = GetHPSprite(progress);
        // Update is called once per frame

    }

    public void EnableLifeBar(bool enabled)
    {
        foreach (Transform tr in transform)
        {
            tr.gameObject.SetActive(enabled);
        }

    }
        
}
