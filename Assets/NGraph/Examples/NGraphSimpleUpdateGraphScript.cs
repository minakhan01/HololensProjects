/***********************************************************************
/*      Copyright Niugnep Software, LLC 2013 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

using UnityEngine;
using System.Collections;

public class NGraphSimpleUpdateGraphScript : MonoBehaviour
{
   public NGraphSimplePlayerScript mPlayer;
   Rigidbody mPlayerRigidbody;
   
   NGraph mGraph;
   
   // The live updating plot for the X axis
   NGraphDataSeriesXyLiveTransient mPlotX;
   // The live updating plot for the Z axis
   NGraphDataSeriesXyLiveTransient mPlotY;
   // The live updating plot for the Z axis
   NGraphDataSeriesXyLiveTransient mPlotZ;
   
   void Awake()
   {
      // Setup the graph
      mGraph = GetComponent<NGraph>();
      mGraph.setRanges(0, 10, -5, 5);
      
      // Capture the player's transform
      mPlayerRigidbody = mPlayer.GetComponent<Rigidbody>();
      
      // Make the two plots
      mPlotX = mGraph.addDataSeries<NGraphDataSeriesXyLiveTransient>("X", Color.green);
      mPlotY = mGraph.addDataSeries<NGraphDataSeriesXyLiveTransient>("Y", Color.yellow);
      mPlotZ = mGraph.addDataSeries<NGraphDataSeriesXyLiveTransient>("Z", Color.magenta);
      
      // Change thier update rates to be quicker
      mPlotX.UpdateRate = 0.01f;
      mPlotY.UpdateRate = 0.01f;
      mPlotZ.UpdateRate = 0.01f;
   }
   
   // Update is called once per frame
   void Update ()
   {
      Vector3 mPlayerVelocity = mPlayerRigidbody.velocity;
      mPlotX.UpdateValue = mPlayerVelocity.x;
      mPlotY.UpdateValue = mPlayerVelocity.y;
      mPlotZ.UpdateValue = mPlayer.gameObject.transform.position.y;
   }
}
