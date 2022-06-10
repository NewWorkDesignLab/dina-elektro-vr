using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]
public class VoltageTester : MonoBehaviour
{
    public int frequency = 50;
    public int voltage = 230;
    public bool lighting;
    public bool arrowLeft;
    public bool arrowRight;
    public bool resistance;
    public TextMeshProUGUI frequencyText;
    public TextMeshProUGUI voltageText;
    public Image lightingImage;
    public MeshRenderer indicator50;
    public MeshRenderer indicator120;
    public MeshRenderer indicator230;
    public MeshRenderer indicator400;
    public MeshRenderer indicatorLightning;
    public MeshRenderer indicatorRestance;
    public MeshRenderer indicatorLeftArrow;
    public MeshRenderer indicatorRightArrow;
    public Material quadOnMaterial;
    public Material quadOffMaterial;
    public Material dotOnMaterial;
    public Material dotOffMaterial;
    public Material arrowOnMaterial;
    public Material arrowOffMaterial;
    public Material lightningOnMaterial;
    public Material lightningOffMaterial;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        voltageText.text = voltage.ToString();
        frequencyText.text = frequency.ToString();
        indicator50.material = voltage >= 50 ? quadOnMaterial : quadOffMaterial;
        indicator120.material = voltage >= 120 ? quadOnMaterial : quadOffMaterial;
        indicator230.material = voltage >= 230 ? quadOnMaterial : quadOffMaterial;
        indicator400.material = voltage >= 400 ? quadOnMaterial : quadOffMaterial;

        lighting = voltage >= 400;

        lightingImage.enabled = lighting;
        indicatorLightning.material = lighting ? lightningOnMaterial : lightningOffMaterial;
        indicatorRestance.material = resistance ? dotOnMaterial : dotOffMaterial;
        indicatorLeftArrow.material = arrowLeft ? arrowOnMaterial : arrowOffMaterial;
        indicatorRightArrow.material = arrowRight ? arrowOnMaterial : arrowOffMaterial;
    }
}
