/***********************************************************************
/*      Copyright Niugnep Software, LLC 2014 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NGraphUnityEquationExample : MonoBehaviour
{
   public InputField Equation;
   public InputField XMax;
   public InputField XMin;
   public InputField YMax;
   public InputField YMin;
   public InputField InputResolution;
   public Text       ErrorLabel;

   NGraph mGraph;
   NGraphDataSeriesXyEquation mEquationPlot1;

   // Use this for initialization
   void Start ()
   {
      mGraph = gameObject.GetComponent<NGraph>();
      if(mGraph == null)
      {
         Debug.LogWarning("NGraph component not found.  Aborting.");
         return;
      }
      
      // Setup the graph
      mGraph.setRangeX(-5, 5);
      mGraph.setRangeY(-15, 15);

      mEquationPlot1 = mGraph.addDataSeries<NGraphDataSeriesXyEquation>("Simple Equation", Color.blue);
      mEquationPlot1.Resolution = 0.05f;
      mEquationPlot1.Equation = "(x + 3) * (x + 1) * (x - 2)";

      ErrorLabel.text = "";

      Equation.text = mEquationPlot1.Equation;
      InputResolution.text = mEquationPlot1.Resolution.ToString();
      XMin.text = mGraph.XRange.x.ToString();
      XMax.text = mGraph.XRange.y.ToString();
      YMin.text = mGraph.YRange.x.ToString();
      YMax.text = mGraph.YRange.y.ToString();
      mEquationPlot1.Reveal = 2.0f;

   }

   public void EvaluatePressed()
   {
      ErrorLabel.text = "";
      try
      {
         mEquationPlot1.Equation = Equation.text;
      }
      catch(Exception ex)
      {
         ErrorLabel.text = ex.Message;
      }
      mEquationPlot1.Reveal = 2.0f;
      RangePressed ();
   }
   
   public void RangePressed()
   {
      float res = float.Parse(InputResolution.text);
      mEquationPlot1.Resolution = res;

      float xMax = float.Parse(XMax.text);
      float xMin = float.Parse(XMin.text);
      float yMax = float.Parse(YMax.text);
      float yMin = float.Parse(YMin.text);
      mGraph.setRanges (xMin, xMax, yMin, yMax);
      mEquationPlot1.Reveal = 2.0f;
   }
}
