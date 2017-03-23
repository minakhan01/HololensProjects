/***********************************************************************
/*      Copyright Niugnep Software, LLC 2013 All Rights Reserved.      *
/*                                                                     *
/*     THIS WORK CONTAINS TRADE SECRET AND PROPRIETARY INFORMATION     *
/*     WHICH IS THE PROPERTY OF NIUGNEP SOFTWARE, LLC OR ITS           *
/*             LICENSORS AND IS SUBJECT TO LICENSE TERMS.              *
/**********************************************************************/

using UnityEngine;
using System.Collections;

public class NGraphSimplePlayerScript : MonoBehaviour
{
   const float MOVEMENT_RATE = 8f;
   
   Transform mTransform;
   public void Awake()
   {
      mTransform = transform;
   }
   
	public void Update()
	{
      // Move the player
      Vector3 pPosition = mTransform.position;
      if(Input.GetKey(KeyCode.LeftArrow))
         pPosition.x -= MOVEMENT_RATE * Time.deltaTime;
      if(Input.GetKey(KeyCode.RightArrow))
         pPosition.x += MOVEMENT_RATE * Time.deltaTime;
      if(Input.GetKey(KeyCode.DownArrow))
         pPosition.z -= MOVEMENT_RATE * Time.deltaTime;
      if(Input.GetKey(KeyCode.UpArrow))
         pPosition.z += MOVEMENT_RATE * Time.deltaTime;
      
      // Don't let the player run off the screen
      pPosition.x = Mathf.Clamp(pPosition.x, -10, 10);
      pPosition.z = Mathf.Clamp(pPosition.z, -10, 10);
      
      // Set the new position
      mTransform.position = pPosition;
	}
}
