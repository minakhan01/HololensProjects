/***********************************************************************
/*      Copyright Niugnep Software, LLC 2014 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// This MonoBehavior expects to be attached to the same game object
// that a UINgraph is attached to.  This object is called "graph" if
// you created it with the NGraph Graph Creation Wizard.
public class InteractiveExampleUnityGraphScript : MonoBehaviour
{
   public Slider Plot1ThicknessSlider;
   public Slider Plot1MarkerWeightSlider;
   public Slider Plot1RevealTimeSlider;
   public Toggle Plot1Enabled;
   public Button Plot1Style;
   public Button Plot1MarkerStyle;

   public Slider Plot2MarkerWeightSlider;
   public Slider Plot2ThicknessSlider;
   public Slider Plot2RevealTimeSlider;
   public Toggle Plot2Enabled;
   public Button Plot2Style;
   public Button Plot2MarkerStyle;

   public Slider AxisThicknessSlider;
   public Toggle AxisLabelsXEnabled;
   public Toggle AxisLabelsYEnabled;
   
   UIUnityGraph mGraph;
   NGraphDataSeriesXy mSeries1;
   NGraphDataSeriesXy mSeries2;
   
   List<Vector2> data1 = new List<Vector2>();
   List<Vector2> data2 = new List<Vector2>();

   void Awake ()
   {
      // Capture the graph
      captureGraph();

      // Setup the graph
      mGraph.setRanges(0, 500, 0, 1000);
      
      // Create the data we want to plot
      data1.Add(new Vector2(-50, -100));
      data1.Add(new Vector2(0, 0));
      data1.Add(new Vector2(50, 200));
      data1.Add(new Vector2(100, 200));
      data1.Add(new Vector2(150, 350));
      data1.Add(new Vector2(200, 400));
      data1.Add(new Vector2(250, 690));
      data1.Add(new Vector2(300, 800));
      data1.Add(new Vector2(350, 620));
      data1.Add(new Vector2(400, 800));
      data1.Add(new Vector2(450, 860));
      data1.Add(new Vector2(500, 1000));
      data1.Add(new Vector2(550, 1120));
      
      // Add a X/Y Plot the the graph and capture the plot and color it blue
      createPlot1();
      
      // Create a different set of data for the second plot
      data2.Add(new Vector2(-50, -100));
      data2.Add(new Vector2(0, 250));
      data2.Add(new Vector2(50, 75));
      data2.Add(new Vector2(100, 322));
      data2.Add(new Vector2(150, 200));
      data2.Add(new Vector2(200, 360));
      data2.Add(new Vector2(250, 210));
      data2.Add(new Vector2(300, 55));
      data2.Add(new Vector2(350, 175));
      data2.Add(new Vector2(400, 322));
      data2.Add(new Vector2(450, 260));
      data2.Add(new Vector2(500, 200));
      data2.Add(new Vector2(550, -50));
      
      // Add a second X/Y Plot the the graph and capture the plot and color it red
      createPlot2();
   }
   
   // Update is called once per frame
   void Update ()
   {
   }

   void captureGraph()
   {
      mGraph = gameObject.GetComponent<UIUnityGraph>();
   }

   public void createPlot1()
   {
      captureGraph();
      mSeries1 = mGraph.addDataSeries<NGraphDataSeriesXy>("1", Color.blue);
      mSeries1.PlotThickness = Plot1ThicknessSlider.value;
      mSeries1.MarkerWeight = Plot1MarkerWeightSlider.value;
      mSeries1.Reveal = Plot1RevealTimeSlider.value;
      Plot1PlotStlyeChange();
      Plot1MarkerStlyeChange();

      // Apply our data to the plot.
      mSeries1.Data = data1;
   }

   public void createPlot2()
   {
      captureGraph();
      mSeries2 = mGraph.addDataSeries<NGraphDataSeriesXy>("2", Color.red);
      mSeries2.PlotThickness = Plot1ThicknessSlider.value;
      mSeries2.MarkerWeight = Plot1MarkerWeightSlider.value;
      mSeries2.Reveal = Plot2RevealTimeSlider.value;
      Plot2PlotStlyeChange();
      Plot2MarkerStlyeChange();

      // Apply our data to the plot.
      mSeries2.Data = data2;
   }
   
   public void ShowPlot1()
   {
      captureGraph();
      if(mSeries1 != null)
      {
         mGraph.removeDataSeries(mSeries1);
         mSeries1 = null;
      }
      else
      {
         createPlot1();
      }
   }
   public void Plot1ThicknessChange()
   {
      if(mSeries1 == null)
         return;
      mSeries1.PlotThickness = Plot1ThicknessSlider.value;
   }
   public void Plot1MarkerWeightChange()
   {
      if(mSeries1 == null)
         return;
      mSeries1.MarkerWeight = Plot1MarkerWeightSlider.value;
   }
   public void Plot1RevealTimeChange()
   {
      if(mSeries1 == null)
         return;
      mSeries1.Reveal = Plot1RevealTimeSlider.value;
   }
   
   public void Plot1PlotStlyeChange()
   {
      if(mSeries1 == null)
         return;
      Text pText = Plot1Style.GetComponentInChildren<Text>();
      switch(pText.text)
      {
      case "Line": pText.text = "Bar"; break;
      case "Bar": pText.text = "Line"; break;
      }

      switch(pText.text)
      {
      case "Line": mSeries1.PlotStyle = NGraphDataSeriesXy.Style.Line; break;
      case "Bar": mSeries1.PlotStyle = NGraphDataSeriesXy.Style.Bar; break;
      }
      Plot1ThicknessChange();
   }
   
   public void Plot1MarkerStlyeChange()
   {
      if(mSeries1 == null)
         return;

      Text pText = Plot1MarkerStyle.GetComponentInChildren<Text>();
      switch(pText.text)
      {
      case "None": pText.text = "Box"; break;
      case "Box": pText.text = "Triangle"; break;
      case "Triangle": pText.text = "None"; break;
      }
      
      switch(pText.text)
      {
      case "None": mSeries1.MarkersStyle = NGraphDataSeriesXy.MarkerStyle.None; break;
      case "Box": mSeries1.MarkersStyle = NGraphDataSeriesXy.MarkerStyle.Box; break;
      case "Triangle": mSeries1.MarkersStyle = NGraphDataSeriesXy.MarkerStyle.Triangle; break;
      }

      captureGraph();
      mGraph.RedrawNow = true;
   }
   
   public void ShowPlot2()
   {
      captureGraph();
      if(mSeries2 != null)
      {
         mGraph.removeDataSeries(mSeries2);
         mSeries2 = null;
      }
      else
      {
         createPlot2();
      }
   }
   public void Plot2ThicknessChange()
   {
      if(mSeries2 == null)
         return;
      mSeries2.PlotThickness = Plot2ThicknessSlider.value;
   }
   public void Plot2MarkerWeightChange()
   {
      if(mSeries2 == null)
         return;
      mSeries2.MarkerWeight = Plot2MarkerWeightSlider.value;
   }
   public void Plot2RevealTimeChange()
   {
      if(mSeries2 == null)
         return;
      mSeries2.Reveal = Plot2RevealTimeSlider.value;
   }
   
   public void Plot2PlotStlyeChange()
   {
      if(mSeries2 == null)
         return;
      Text pText = Plot2Style.GetComponentInChildren<Text>();
      switch(pText.text)
      {
      case "Line": pText.text = "Bar"; break;
      case "Bar": pText.text = "Line"; break;
      }
      
      switch(pText.text)
      {
      case "Line": mSeries2.PlotStyle = NGraphDataSeriesXy.Style.Line; break;
      case "Bar": mSeries2.PlotStyle = NGraphDataSeriesXy.Style.Bar; break;
      }
      Plot2ThicknessChange();
   }
   
   public void Plot2MarkerStlyeChange()
   {
      if(mSeries2 == null)
         return;

      Text pText = Plot2MarkerStyle.GetComponentInChildren<Text>();
      switch(pText.text)
      {
      case "None": pText.text = "Box"; break;
      case "Box": pText.text = "Triangle"; break;
      case "Triangle": pText.text = "None"; break;
      }
      
      switch(pText.text)
      {
      case "None": mSeries2.MarkersStyle = NGraphDataSeriesXy.MarkerStyle.None; break;
      case "Box": mSeries2.MarkersStyle = NGraphDataSeriesXy.MarkerStyle.Box; break;
      case "Triangle": mSeries2.MarkersStyle = NGraphDataSeriesXy.MarkerStyle.Triangle; break;
      }

      captureGraph();
      mGraph.RedrawNow = true;
   }
   
   public void ShowXLables()
   {
      if(mGraph == null)
         return;

      captureGraph();
      mGraph.DrawXLabel = AxisLabelsXEnabled.isOn;
      mGraph.RedrawNow = true;
   }
   public void ShowYLables()
   {
      if(mGraph == null)
         return;

      captureGraph();
      mGraph.DrawYLabel = AxisLabelsYEnabled.isOn;
      mGraph.RedrawNow = true;
   }
   
   public void AxisThicknessChange()
   {
      if(mGraph == null)
         return;

      captureGraph();
      mGraph.AxesThickness = AxisThicknessSlider.value;
      mGraph.RedrawNow = true;
   }
   
   public void XAxisStyleChange()
   {
      if(mGraph == null)
         return;

      captureGraph();
      /*
      switch(UIPopupList.current.value)
      {
      case "Even": mGraph.XTickStyle = NGraph.TickStyle.EvenSpace; break;
      case "Even - Low": mGraph.XTickStyle = NGraph.TickStyle.EvenSpaceLow; break;
      case "Even - High": mGraph.XTickStyle = NGraph.TickStyle.EvenSpaceHigh; break;
      case "Even - High And Low": mGraph.XTickStyle = NGraph.TickStyle.EvenSpaceLowAndHigh; break;
      }
      */
      mGraph.RedrawNow = true;
   }
   public void YAxisStyleChange()
   {
      if(mGraph == null)
         return;

      captureGraph();
      /*
      switch(UIPopupList.current.value)
      {
      case "Even": mGraph.YTickStyle = NGraph.TickStyle.EvenSpace; break;
      case "Even - Low": mGraph.YTickStyle = NGraph.TickStyle.EvenSpaceLow; break;
      case "Even - High": mGraph.YTickStyle = NGraph.TickStyle.EvenSpaceHigh; break;
      case "Even - High And Low": mGraph.YTickStyle = NGraph.TickStyle.EvenSpaceLowAndHigh; break;
      }
      */
      mGraph.RedrawNow = true;
   }
}
