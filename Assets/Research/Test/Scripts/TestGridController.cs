using System;
using UnityEngine;

namespace Research.Test.Scripts
{
    public sealed class TestGridController : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public Transform movePoint;

        public float moveThreshold = 0.2f;

        private float Speed => moveSpeed * Time.deltaTime;

        public LayerMask obstructMovement;

        public Vector2 Input { get; set; }

        private void Start()
        {
            movePoint.parent = null;
        }

        public void CompleteMovement()
        {
            transform.position = movePoint.position;
        }

        private void Update()
        {
            transform.position =
                Vector3.MoveTowards(transform.position, movePoint.position, Speed);
        }

        public void UpdateInput(Vector2 input)
        {
            Input = input;
            UpdateInput();
        }

        public void UpdateInput()
        {
            var horizontal = Input.x;
            ApplyMovement(horizontal, new Vector3(1, 0));

            var vertical = Input.y;
            ApplyMovement(vertical, new Vector3(0, 1));
        }

        private void ApplyMovement(float axis, Vector3 moveDirection)
        {
            var pastThreshold = Mathf.Abs(axis) > moveThreshold;
            if (pastThreshold)
            {
                var direction = axis > 0 ? 1 : -1;
                
                var newPosition = movePoint.position + moveDirection * direction;
                var colliders = Physics2D.OverlapCircleAll(newPosition, 0.5f, obstructMovement);
                var squareValid = true;
                foreach (var col in colliders)
                {
                    if (col.gameObject != gameObject)
                    {
                        squareValid = false;
                        break;
                    }
                }
                if (squareValid)
                {
                    movePoint.position = newPosition;
                }
            }
        }

        public void ResetMovePoint(Vector3 newPosition)
        {
            movePoint.transform.position = newPosition;
        }
    }
}
