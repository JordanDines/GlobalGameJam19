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

    private PlayerController player;

    int itemPlaceCount = 0;



    GameObject currentItem;

    public int ItemPlaceCount
    {
        get
        {
            return itemPlaceCount;
        }
    }

    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        PostProcessVolume volume = cam.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings<ColorGrading>(out colorGradingLayer);
        crosshair = GameObject.FindGameObjectWithTag("Canvas").GetComponentInChildren<Crosshair>();
        Debug.Log(colorGradingLayer);
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.State == PlayerController.PlayerState.Default)
        {
            if (currentItem != null) //dropping
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
                            player.SetInteracting(1.5f);
                            currentItem.GetComponent<SfxPickUpDropOff>().PlayDropOffSFX();
                            GameObject temp = new GameObject();
                            currentItem.transform.parent = null;
                            temp.transform.position = hit.point + Vector3.up * ((currentItem.transform.localScale.y / 2) + currentItem.transform.localScale.y / 4);
                            StartCoroutine(MoveObjectAToB(currentItem, temp, travelTime, true));
                            currentItem.transform.tag = "Untagged";
                            currentItem = null;
                            itemPlaceCount++;
                            player.ItemPlaced();
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
            else      //pickingup      
            {

                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2, LayerMask.GetMask("Pickupables")))
                {
                    if (hit.transform.tag == "Pickupable")
                    {
                        crosshair.Resizing = true;
                        if (Input.GetMouseButtonDown(0))
                        {
                            currentItem = hit.transform.gameObject;
                            currentItem.transform.parent = hit.transform.GetComponent<ItemInfo>().targetTrasform.transform;
                            StartCoroutine(MoveObjectAToB(hit.transform.gameObject, itemHoldPos, travelTime, false, true));
                            ArmsAC.SetBool("isHolding", true);
                            player.SetInteracting(2.5f);
                            hit.transform.GetComponent<SfxPickUpDropOff>().PlayPickUpSFX();
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

    IEnumerator MoveObjectAToB(GameObject go1, GameObject finalPos, float amoutOfTime, bool destroyGO = false, bool useLocalPos = false)
    {
        float currentTime = 0;
        float timeAsPercent = 0;

        Vector3 startValue = go1.transform.position;
        Vector3 finalPosition = Vector3.zero;
        Quaternion startRot = go1.transform.rotation;
        Quaternion finalRot = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        while (timeAsPercent <= 1)
        {
            currentTime += Time.deltaTime;
            timeAsPercent = currentTime / amoutOfTime;

            if (useLocalPos)
            {
                go1.transform.localPosition = Vector3.Lerp(startValue, finalPosition, timeAsPercent);
                go1.transform.localRotation = Quaternion.Slerp(startRot, finalRot, timeAsPercent);
            }
            else
            {
                go1.transform.rotation = Quaternion.Slerp(startRot, finalRot, timeAsPercent);
                go1.transform.position = Vector3.Lerp(startValue, finalPos.transform.position, timeAsPercent);
            }
            yield return null;
        }

        if (destroyGO)
        {
            Destroy(finalPos);
        }

    }

}
