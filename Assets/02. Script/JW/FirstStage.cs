using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStage : MonoBehaviour
{
    public static FirstStage instance;
    public GameObject player;
    public GameObject[] stones;
    public GameObject stonePrefebs;
    public int stoneNum;
    public int lastStoneNum;
    public bool isHave;
    public float distance;

    void Start()
    {
        instance = this;
        stoneNum = -1;
    }

    void Update()
    {
        if (stoneNum >= 0 && !isHave && Input.GetKeyDown("e"))
        {
            lastStoneNum = stoneNum;
            Destroy(stones[stoneNum]);
            isHave = true;
        }else if(isHave && Input.GetKeyDown("e"))
        {
            Vector3 stonePosition = player.transform.position + player.transform.forward * distance;
            GameObject stone = Instantiate(stonePrefebs, stonePosition, Quaternion.identity);
            stones[lastStoneNum] = stone;
            stone.GetComponent<Stone>().stoneNum = lastStoneNum;
            isHave = false;
        }
    }
}
