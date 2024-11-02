using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStage : MonoBehaviour
{
    public static FirstStage instance;
    public GameObject player;
    public GameObject buildCol;
    public GameObject[] stones;
    public GameObject[] stoneBridge;
    public int stoneNum;
    public bool isHave;
    public bool isBuild;
    int bridgeNum;

    void Start()
    {
        instance = this;
        bridgeNum = 0;
        stoneNum = -1;
    }

    void Update()
    {
        if(bridgeNum < 6)
        {
            if (stoneNum >= 0 && !isHave && Input.GetKeyDown("e"))
            {
                Destroy(stones[stoneNum]);
                stoneNum = -1;
                isHave = true;
            }
            else if (isHave && Input.GetKeyDown("e") && isBuild)
            {
                stoneBridge[bridgeNum].SetActive(true);
                bridgeNum++;
                isHave = false;
                isBuild = false;
                buildCol.transform.position = new Vector3(buildCol.transform.position.x, buildCol.transform.position.y, buildCol.transform.position.z + 10);
            }
        }

    }
}
