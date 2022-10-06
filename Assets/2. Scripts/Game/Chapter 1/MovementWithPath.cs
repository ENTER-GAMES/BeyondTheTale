using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BeyondTheTale.Chapter1
{
    public class MovementWithPath : MonoBehaviour
    {
        [SerializeField]
        private Transform[] waypoints;
        [SerializeField]
        private float time;
        private int count = 0;
        private Vector3 originScale;

        public UnityEvent onMoveEnd = new UnityEvent();

        private void Start()
        {
            originScale = transform.localScale;
        }

        public void Move()
        {
            StartCoroutine(MoveToPathRoutine());
        }

        private IEnumerator MoveToPathRoutine()
        {
            while (count < waypoints.Length)
            {
                yield return MoveToPointRoutine();
                count++;
                if (count == waypoints.Length)
                {
                    onMoveEnd.Invoke();
                    break;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator MoveToPointRoutine()
        {
            float percent = 0;
            Vector3 start = transform.position;
            Vector3 end = new Vector3(waypoints[count].position.x, transform.position.y, transform.position.z);
            float endY = waypoints[count].position.y;
            Flip(start.x < end.x);
            while (percent <= 1)
            {
                percent = Mathf.InverseLerp(endY, start.y, waypoints[count].position.y);
                transform.position = Vector3.Lerp(start, end, percent);

                if (percent == 1) break;
                else yield return null;
            }
        }

        private void Flip(bool value)
        {
            if (value)
                transform.localScale = new Vector3(originScale.x * -1, originScale.y, originScale.z);
            else
                transform.localScale = originScale;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Transform prevPoint = null;
            foreach (Transform curPoint in waypoints)
            {
                if (prevPoint != null)
                    Gizmos.DrawLine(prevPoint.position, curPoint.position);

                prevPoint = curPoint;
            }
        }
    }
}