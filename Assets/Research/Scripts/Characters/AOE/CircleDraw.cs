﻿using UnityEngine;

namespace Research.Scripts.Characters.AOE
{
    public class CircleDraw : MonoBehaviour
    {
        public float ThetaScale = 0.01f; //Set lower to add more points
        public int Size; //Total number of points in circle
        public float Radius = 3f;
        public LineRenderer LineRenderer;

        private void Awake()
        {
            float sizeValue = (2.0f * Mathf.PI) / ThetaScale;
            Size = (int) sizeValue;
            Size++;
            LineRenderer.startWidth = 0.1f;
            LineRenderer.endWidth = 0.1f;
            LineRenderer.positionCount = Size;
        }

        private void Update()
        {
            if(Time.timeScale <= 1)
            {
                var theta = 0f;
                var addition = 2.0f * Mathf.PI * ThetaScale;
                for (var i = 0; i < Size; i++)
                {
                    theta += addition;
                    var x = Radius * Mathf.Cos(theta);
                    var y = Radius * Mathf.Sin(theta);
                    x += gameObject.transform.position.x;
                    y += gameObject.transform.position.y;
                    var pos = new Vector3(x, y + 0.3f, -1);
                    LineRenderer.SetPosition(i, pos);
                }
            }
        }
    }
}
