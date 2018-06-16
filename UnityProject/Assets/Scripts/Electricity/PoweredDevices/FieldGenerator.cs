﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PlayGroups.Input;

namespace Electricity
{
	public class FieldGenerator : InputTrigger
	{
		public PoweredDevice poweredDevice;

		public bool isOn = false;
		public bool connectedToOther = false;
		private bool spriteAnimRunning = false;

		public Sprite offSprite;
		public Sprite onSprite;
		public Sprite[] searchingSprites;
		public Sprite[] connectedSprites;

		public SpriteRenderer spriteRend;

		List<Sprite> animSprites = new List<Sprite>();


		public override void OnStartClient()
		{
			base.OnStartClient();
			poweredDevice.OnSupplyChange.AddListener(SupplyUpdate);
		}

		private void OnDisable()
		{
			poweredDevice.OnSupplyChange.RemoveListener(SupplyUpdate);
		}

		public override void Interact(GameObject originator, Vector3 position, string hand)
		{
			//TODO Network everythng (currently WIP test mode)

			//Testing:
			isOn = !isOn;
			CheckState();
		}

		//Power supply updates
		void SupplyUpdate(){
			CheckState();
		}

		//Check the operational state
		void CheckState(){
			if(isOn){
				if(poweredDevice.suppliedElectricity.current == 0){
					StopCoroutine(SpriteAnimator());
					spriteAnimRunning = false;
					spriteRend.sprite = onSprite;
				}
				if(poweredDevice.suppliedElectricity.current > 15){
					if(!connectedToOther){
						animSprites = new List<Sprite>(searchingSprites);
						if (!spriteAnimRunning) {
							StartCoroutine(SpriteAnimator());
						}
					} else {
						animSprites = new List<Sprite>(connectedSprites);
						if(!spriteAnimRunning){
							StartCoroutine(SpriteAnimator());
						}
					}
				}
			} else {
				StopCoroutine(SpriteAnimator());
				spriteAnimRunning = false;
				spriteRend.sprite = offSprite;
			}
		}

		IEnumerator SpriteAnimator(){
			spriteAnimRunning = true;
			int index = 0;
			while(spriteAnimRunning){
				if(index >= animSprites.Count){
					index = 0;
				}
				spriteRend.sprite = animSprites[index];
				index++;
				yield return new WaitForSeconds(0.3f);
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
