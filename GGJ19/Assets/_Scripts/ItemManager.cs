using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{

    Camera cam;
    RaycastHit hit;
    Transform itemHoldPos;


    GameObject currentItem;
    bool holdingItem = false;


    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }


    // Update is called once per frame
    void Update()
    {
        //dropping
        if (currentItem != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10))
                {
                    if (Input.GetMouseButton(0))
                    {
                        MoveObjectAToB(currentItem, hit.transform.position + (Vector3.up * (hit.transform.localScale.y / 2)), 0.1f);
                        currentItem = null;
                    }
                }
            }
        }
        //pickingup
        else
        {
            if (Physics.SphereCast(cam.transform.position, 2, cam.transform.forward, out hit, 10))
            {
                if (hit.transform.tag == "Pickupable")
                {
                    if (Input.GetMouseButton(0))
                    {
                        currentItem = hit.transform.gameObject;
                        MoveObjectAToB(currentItem, itemHoldPos.position, 0.1f);
                    }
                }
            }
        }

    }

    IEnumerable MoveObjectAToB(GameObject go1, Vector3 finalPos, float amoutOfTime)
    {
        float length = Vector3.Distance(go1.transform.position, finalPos);
        Vector3 startPostion = go1.transform.position;
        Vector3 direction = (itemHoldPos.position - startPostion).normalized;
        float speed = length / amoutOfTime;
        float remainingTime = amoutOfTime;
        while (true)
        {

            if (remainingTime < Time.deltaTime)
            {
                go1.transform.position = finalPos;
                break;

            }
            go1.transform.position += direction * speed * Time.deltaTime;
            remainingTime -= Time.deltaTime;
            return null;

        }
        return null;
    }

}
