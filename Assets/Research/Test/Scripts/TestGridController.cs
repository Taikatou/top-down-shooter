using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Research.Test.Scripts
{
    public class TestGridController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public Transform movePoint;

        public float moveThreshold = 0.2f;

        private float Speed => moveSpeed * Time.deltaTime;

        public LayerMask obstructMovement;

        private void Start()
        {
            movePoint.parent = null;
        }

        private void Update()
        {
            transform.position =
                Vector3.MoveTowards(transform.position, movePoint.position, Speed);

            var moveDistance = Vector3.Distance(transform.position, movePoint.position);
            if (moveDistance <= 0.05f)
            {
                UpdateInput();   
            }
        }

        protected virtual void UpdateInput()
        {
            var horizontal = Input.GetAxis("Horizontal");
            ApplyMovement(horizontal, new Vector3(1, 0));
            
            var vertical = Input.GetAxis("Vertical");
            ApplyMovement(vertical, new Vector3(0, 1));
        }

        private void ApplyMovement(float axis, Vector3 moveDirection)
        {
            var pastThreshold = Mathf.Abs(axis) > moveThreshold;
            if (pastThreshold)
            {
                var direction = axis > 0 ? 1 : -1;
                
                var newPosition = movePoint.position + moveDirection * direction;
                var squareValid = Physics2D.OverlapCircle(newPosition, 0.2f, obstructMovement);
                if (!squareValid)
                {
                    movePoint.position = newPosition;
                }
            }
        }
    }
}
