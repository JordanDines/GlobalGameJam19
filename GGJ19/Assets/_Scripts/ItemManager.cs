using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class ItemManager : MonoBehaviour
{

    Camera cam;
    RaycastHit hit;
    RaycastHit[] hits;

    [SerializeField] Animator ArmsAC;
    [SerializeField] GameObject itemHoldPos;
    [SerializeField] float travelTime = 0.1f;
    [SerializeField] GameObject reticle;

    [Header("Saturation")]
    [SerializeField] int maxSaturation = 100;
    [SerializeField] int saturationIncreaseAmount = 25;
    [SerializeField] float saturationLerpTime = 2;
    ColorGrading colorGradingLayer;

    GameObject currentItem;


    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        PostProcessVolume volume = cam.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings<ColorGrading>(out colorGradingLayer);

        Debug.Log(colorGradingLayer);
    }


    // Update is called once per frame
    void Update()
    {
        //dropping
        if (currentItem != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1))
                {
                    if (hit.transform.tag == "Placeable")
                    {


                        if (Input.GetMouseButtonDown(0))
                        {

                            if (colorGradingLayer.saturation.value + saturationIncreaseAmount <= maxSaturation)
                            {
                                StartCoroutine(LerpSaturationForSeconds(colorGradingLayer.saturation, colorGradingLayer.saturation.value + saturationIncreaseAmount, saturationLerpTime));
                            }
                            else
                            {
                                if (colorGradingLayer.saturation.value != maxSaturation)
                                {
                                    StartCoroutine(LerpSaturationForSeconds(colorGradingLayer.saturation, maxSaturation, saturationLerpTime));
                                }
                            }

                            ArmsAC.SetBool("isHolding", false);
                            GameObject temp = new GameObject();
                            temp.transform.position = (hit.point + (Vector3.up * (hit.transform.localScale.y / 2)));
                            StartCoroutine(MoveObjectAToB(currentItem, temp, travelTime, true));
                            currentItem.transform.parent = null;
                            currentItem = null;
                        }
                    }
                }
            }
        }

        //pickingup
        else
        {
            hits = Physics.SphereCastAll(cam.transform.position, 2, cam.transform.forward, 10);

            foreach (var currenthit in hits)
            {
                if (currenthit.transform.tag == "Pickupable")
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(MoveObjectAToB(currenthit.transform.gameObject, itemHoldPos, travelTime));
                        currentItem = currenthit.transform.gameObject;
                        Debug.Log(currentItem);
                        currentItem.transform.parent = itemHoldPos.transform;
                        ArmsAC.SetBool("isHolding", true);
                    }
                }
            }

        }

    }

    IEnumerator LerpSaturationForSeconds(FloatParameter valueToChange, float endAmount, float amoutOfTime)
    {
        float currentTime = 0;
        float timeAsPercent = 0;

        float startValue = valueToChange.value;

        while (timeAsPercent <= 1)
        {
            currentTime += Time.deltaTime;
            timeAsPercent = currentTime / amoutOfTime;

            valueToChange.value = Mathf.Lerp(startValue, endAmount, timeAsPercent);
            yield return null;
        }

    }



    IEnumerator MoveObjectAToB(GameObject go1, GameObject finalPos, float amoutOfTime, bool destroyGO = false)
    {
        float currentTime = 0;
        float timeAsPercent = 0;

        Vector3 startValue = go1.transform.position;

        while (timeAsPercent <= 1)
        {
            currentTime += Time.deltaTime;
            timeAsPercent = currentTime / amoutOfTime;

            go1.transform.position = Vector3.Lerp(startValue, finalPos.transform.position, timeAsPercent);
            yield return null;
        }

        if (destroyGO)
        {
            Destroy(finalPos);
        }


     
    }

}
