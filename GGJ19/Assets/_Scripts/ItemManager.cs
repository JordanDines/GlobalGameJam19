using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class ItemManager : MonoBehaviour
{

    Camera cam;
    RaycastHit hit;
    [SerializeField] Animator ArmsAC;
    [SerializeField] Transform itemHoldPos;
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
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10))
        {
            if (!reticle.activeInHierarchy)
            {
                reticle.SetActive(true);
            }

            reticle.transform.position = hit.point;
        }
        else
        {
            if (reticle.activeInHierarchy)
            {
                reticle.SetActive(false);
            }
        }

        //dropping
        if (currentItem != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10))
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

                        ArmsAC.SetTrigger("action");
                        StartCoroutine(MoveObjectAToB(currentItem, hit.point + (Vector3.up * (hit.transform.localScale.y / 2)), travelTime));
                        currentItem.transform.parent = null;
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
                    if (Input.GetMouseButtonDown(0))
                    {
                        ArmsAC.SetTrigger("action");
                        currentItem = hit.transform.gameObject;
                        currentItem.transform.parent = itemHoldPos;
                        StartCoroutine(MoveObjectAToB(currentItem, itemHoldPos.position, travelTime));
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



    IEnumerator MoveObjectAToB(GameObject go1, Vector3 finalPos, float amoutOfTime)
    {
        float length = Vector3.Distance(go1.transform.position, finalPos);
        Vector3 startPostion = go1.transform.position;
        Vector3 direction = (finalPos - startPostion).normalized;
        float speed = length / amoutOfTime;
        float remainingTime = amoutOfTime;
        while (true)
        {

            if (remainingTime < Time.deltaTime)
            {
                //go1.transform.position = finalPos;
                go1.transform.position += direction * speed * remainingTime;
                break;

            }
            go1.transform.position += direction * speed * Time.deltaTime;
            remainingTime -= Time.deltaTime;
            yield return null;

        }
    }

}
