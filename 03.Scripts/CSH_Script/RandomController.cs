using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomController : MonoBehaviour
{
    public bool IsPlayerTurn
    {
        get
        {
            return isPlayerTurn;
        }
        set
        {
            isPlayerTurn = value;
        }
    }
    bool isPlayerTurn;

    public int Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }
    int speed;
    public int SuccessCount
    {
        get
        {
            return successCount;
        }
        set
        {
            successCount = value;
        }
    }
    int successCount = 0;

    Image[] moveImages;
    List<int> MyNum;
    List<bool> SuccessList;
    [SerializeField] Sprite successImage;
    [SerializeField] Sprite failImage;
    [SerializeField] Sprite defaultImage;
    GameObject gameManager;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager");
        MyNum = new List<int>();
        SuccessList = new List<bool>();
        moveImages = new Image[5];

        for(int i = 0; i < moveImages.Length; i++)
            moveImages[i] = transform.GetChild(i).GetComponent<Image>();
    }

    public void OnRandomPositionNumberClick()
    {
        Debug.Log("��: " + isPlayerTurn);
        PickRandomly();
        RollTheDice();
        StopAllCoroutines();
        StartCoroutine(ShowResult());
    }

    void PickRandomly()
    {
        MyNum.Clear();

        for(int i = 0; i < speed; i++)
        {
            int rand = Random.Range(1,101);
            MyNum.Add(rand);
        }
    }

    void RollTheDice()
    {
       SuccessList.Clear();

        for(int i = 0; i < moveImages.Length; i++)
        {
            int rand = Random.Range(1,101);
            
            if(MyNum.Contains(rand))
                SuccessList.Add(true);
            else
                SuccessList.Add(false);
        }
    }

    IEnumerator ShowResult()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < moveImages.Length; i++)
            moveImages[i].GetComponent<Image>().enabled = true;
        
        for (int i = 0; i < moveImages.Length; i++)
        {
            GetComponent<AudioSource>().Play();

            if (SuccessList[i])
            {
                moveImages[i].sprite = successImage;
                if (isPlayerTurn)
                    successCount++;
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                moveImages[i].sprite = failImage;
                yield return new WaitForSeconds(0.3f);
            }
        }

        print("successCount: " + successCount);
        if (isPlayerTurn)
        {
            gameManager.GetComponent<GameManager>().SetMoveCount(successCount);
            successCount = 0;
            isPlayerTurn = false;
        }
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < moveImages.Length; i++)
        {
            moveImages[i].GetComponent<Image>().sprite = defaultImage;
            moveImages[i].GetComponent<Image>().enabled = false;
        }
    }
}
