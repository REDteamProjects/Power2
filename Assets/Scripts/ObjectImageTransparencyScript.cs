﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class ObjectImageTransparencyScript : MonoBehaviour
    {
        private float _toTransparency = 1;
        private float _transparencyChanger = 0;
        private MovingFinishedDelegate _transparencyChangerCallback = null;
        public float Transparency
        {
            get { 
                var image = GetComponent<Image>();
                Color c = image.color;
                return c.a;
                }
        }

        void Update()
        {
            if (_transparencyChanger != 0)
            {
                var image = GetComponent<Image>();
                Color c = image.color;
                c.a += _transparencyChanger;
                image.color = c;
                if(c.a == _toTransparency)
                {
                    _transparencyChanger = 0;
                    if (_transparencyChangerCallback != null)
                        _transparencyChangerCallback(this, true);
                }

            }
        }


        public void SetTransparency(float value, MovingFinishedDelegate callback)
        {
            if (value < 0 || value > 1 || value == Transparency)
                return;
            if (value < _toTransparency)
            {
                _transparencyChanger = -0.1f;
                _toTransparency = value;
                _transparencyChangerCallback = callback;
            }
            else
                if (value > _toTransparency)
                { 
                    _transparencyChanger = 0.1f;
                    _toTransparency = value;
                    _transparencyChangerCallback = callback;
                }
        }
    }
}
