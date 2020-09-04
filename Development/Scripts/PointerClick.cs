using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Phedg1Studios {
    namespace ItemDropList {
        public class PointerClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
            public RoR2.UI.MPEventSystem eventSystem;
            public UnityEvent onLeftUnderTwo = new UnityEvent();
            public UnityEvent onLeftUnderFour = new UnityEvent();
            public UnityEvent onLeftOverFour = new UnityEvent();
            private bool leftHeld = false;
            private int leftClick = 14;
            private float pressStart = 0;

            public void OnPointerDown(PointerEventData eventData) {
                if (eventData.button == PointerEventData.InputButton.Left) {
                    PointerDown();
                }
            }


            public void OnPointerUp(PointerEventData eventData) {
                if (eventData.button == PointerEventData.InputButton.Left) {
                    PointerUp();
                }
            }

            public void PointerDown() {
                pressStart = Time.time;
            }
            
            public void PointerUp() {
                float deltaTime = Time.time - pressStart;
                if (deltaTime < 2) {
                    onLeftUnderTwo.Invoke();
                } else if (deltaTime < 4) {
                    onLeftUnderFour.Invoke();
                } else {
                    onLeftOverFour.Invoke();
                }
            }

            void Update() {
                if (!(bool)(UnityEngine.Object)eventSystem || eventSystem.player == null) {
                    return;
                }
                if (leftHeld && !eventSystem.player.GetButtonDown(leftClick)) {
                    PointerUp();
                    leftHeld = false;
                }
                if ((UnityEngine.Object)eventSystem.currentSelectedGameObject == (UnityEngine.Object)gameObject) {
                    if (eventSystem.player.GetButtonDown(leftClick) && !leftHeld) {
                        PointerDown();
                        leftHeld = true;
                    }
                }
            }
        }
    }
}
