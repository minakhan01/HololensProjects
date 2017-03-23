/***********************************************************************
/*      Copyright Niugnep Software, LLC 2014 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NGraphSimpleDataExample : MonoBehaviour
{
   NGraph mGraph;
   NGraphDataSeriesXy mSeries1;
   
   void Awake ()
   {
      mGraph = gameObject.GetComponent<NGraph>();
      if(mGraph == null)
      {
         Debug.LogWarning("NGraph component not found.  Aborting.");
         return;
      }
      mGraph.setRanges(-10, 10, -8, 8);
      
      List<Vector2> data = new List<Vector2>();
      
      data.Add (new Vector2 (-10, -3));
      data.Add (new Vector2 (-8, 6));
      data.Add (new Vector2 (-6, 2));
      data.Add (new Vector2 (-4, -1));
      data.Add (new Vector2 (-3, -7));
      data.Add (new Vector2 (-2, -4));
      data.Add (new Vector2 (-0, 3));

      mSeries1 = mGraph.addDataSeries<NGraphDataSeriesXy>("Simple Plot 1", Color.green);
      mSeries1.PlotStyle = NGraphDataSeriesXy.Style.Line;
      mSeries1.PlotThickness = 3f;
      mSeries1.Data = data;
      
      mSeries1 = mGraph.addDataSeries<NGraphDataSeriesXy>("Simple Plot 2", Color.red);
      mSeries1.PlotStyle = NGraphDataSeriesXy.Style.Bar;
      mSeries1.PlotThickness = 10f;
      mSeries1.Data = data;
   }
}
