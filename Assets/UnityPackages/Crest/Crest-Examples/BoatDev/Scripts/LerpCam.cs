﻿// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

using Crest;
using System;
using UnityEngine;

public class LerpCam : MonoBehaviour
{
    [SerializeField] float _lerpAlpha = 0.1f;
    [SerializeField] Transform _targetPos = null;
    [SerializeField] Transform _targetLookatPos = null;
    [SerializeField] float _lookatOffset = 5f;
    [SerializeField] float _minHeightAboveWater = 0.5f;



    SampleHeightHelper _sampleHeightHelper = new SampleHeightHelper();

    void Update()
    {
        if (OceanRenderer.Instance == null) return;

      //  _sampleHeightHelper.Init(transform.position, 0f);
       // float h = 0f;
     //   _sampleHeightHelper.Sample(ref h);

        var targetPos = _targetPos.position;
    //    targetPos.y = Mathf.Max(targetPos.y, h + _minHeightAboveWater);

        transform.position = Vector3.Lerp(transform.position, targetPos, _lerpAlpha * OceanRenderer.Instance.DeltaTime * 60f);


        transform.LookAt(_targetLookatPos.position + _lookatOffset * Vector3.up);

       
    }
}
