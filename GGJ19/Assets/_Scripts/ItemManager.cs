using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class ItemManager : MonoBehaviour
{
    Camera cam;
    RaycastHit hit;
    RaycastHit[] hits;

    Crosshair crosshair;

    [SerializeField] Animator ArmsAC;
    [SerializeField] GameObject itemHoldPos;
    [SerializeField] float travelTime = 0.1f;
    [SerializeField] GameObject reticle;

    [Header("Saturation")]
    [SerializeField] int maxSaturation = 100;
    [SerializeField] int saturationIncreaseAmount = 25;
    [SerializeField] float saturationLerpTime = 2;
    ColorGrading colorGradingLayer;

    int itemPlaceCount = 0;


    GameObject currentItem;


    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        PostProcessVolume volume = cam.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings<ColorGrading>(out colorGradingLayer);
        crosshair = GameObject.FindGameObjectWithTag("Canvas").GetComponentInChildren<Crosshair>();
        Debug.Log(colorGradingLayer);
    }


    // Update is called once per frame
    void Update()
    {
        //dropping
        if (currentItem != null)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2))
            {
                if (hit.transform.tag == "Placeable")
                {
                    crosshair.Resizing = true;

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
                        currentItem.transform.parent = null;
                        temp.transform.position = hit.point + Vector3.up * currentItem.transform.localScale.y / 2;
                        StartCoroutine(MoveObjectAToB(currentItem, temp, travelTime, true));
                        currentItem.transform.tag = "Untagged";
                        currentItem = null;
                        itemPlaceCount++;
                    }
                }
                else
                {
                    crosshair.Resizing = false;
                }
            }
            else
            {
                crosshair.Resizing = false;
            }
        }


        //pickingup
        else
        {

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2))
            {
                if (hit.transform.tag == "Pickupable")
                {
                    crosshair.Resizing = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(MoveObjectAToB(hit.transform.gameObject, itemHoldPos, travelTime));
                        currentItem = hit.transform.gameObject;
                        currentItem.transform.parent = itemHoldPos.transform;
                        ArmsAC.SetBool("isHolding", true);
                    }
                }
                else
                {
                    crosshair.Resizing = false;
                }
            }
            else
            {
                crosshair.Resizing = false;
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
        Quaternion startRot = go1.transform.rotation;
        Quaternion finalRot = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        while (timeAsPercent <= 1)
        {
            currentTime += Time.deltaTime;
            timeAsPercent = currentTime / amoutOfTime;

            go1.transform.position = Vector3.Lerp(startValue, finalPos.transform.position, timeAsPercent);
            go1.transform.rotation = Quaternion.Slerp(startRot, finalRot, timeAsPercent);
            yield return null;
        }

        if (destroyGO)
        {
            Destroy(finalPos);
        }

    }

}
