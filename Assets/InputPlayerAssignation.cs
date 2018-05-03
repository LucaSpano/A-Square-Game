//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Rewired;
//using UnityEngine;
//
//public class InputPlayerAssignation : MonoBehaviour
//{
//	void Start()
//	{
//		ReInput.ControllerConnectedEvent += OnControllerConnected;
//		AssignKeyboardToNextOpenPlayer();
//	}
//
//	void OnControllerConnected(ControllerStatusChangedEventArgs args)
//	{
//		if (args.controllerType != ControllerType.Joystick) return;
//		
//		// Get the Joystick from ReInput
//		Joystick joystick = ReInput.controllers.GetJoystick(args.controllerId);
//
//		if (joystick != null) {
//			// Assign Joystick to first Player that doesn't have any assigned
//			ReInput.controllers.AutoAssignJoystick(joystick);
//		}
//
//		AssignKeyboardToNextOpenPlayer();
//	}
//
//	void AssignKeyboardToNextOpenPlayer()
//	{
//		int openKeyboardLayout = 0;
//		foreach(Player p in ReInput.players.Players) {
//			var hasJoystick = p.controllers.joystickCount > 0;
//			p.controllers.hasKeyboard = !hasJoystick;
//			
//			var layout = ReInput.mapping.KeyboardLayouts.ElementAt(openKeyboardLayout);
//			print(layout.name);
//			openKeyboardLayout++;
//			p.controllers.maps.AddMap(p.controllers.Keyboard);
//		}
//	}
//}
