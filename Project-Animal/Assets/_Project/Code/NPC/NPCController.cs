using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrilathTTV
{
    public class NPCController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float targetTime;
        [SerializeField] private float targetTimeCountdown;
        [SerializeField] private float restTime;
        [SerializeField] private bool retargeting;
        [SerializeField] private float moveSpeed;
        [SerializeField] private Vector3 rotationOffset;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Start()
        {
            targetTimeCountdown = targetTime;
        }

        private void Update()
        {
            if (targetTimeCountdown > 0)
            {
                targetTimeCountdown -= Time.deltaTime;
            }
            else if (!retargeting)
            {
                StartCoroutine(RestAndRetarget());
            }
            if (target != null)
            {
                Move();
                Rotation();
            }
        }

        private void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position - new Vector3(.5f, .5f, 0), Time.deltaTime * moveSpeed);
        }

        private void Rotation()
        {
            Vector3 _lookAt = target.position - transform.position;
            float angle = Mathf.Atan2(_lookAt.y, _lookAt.x) * Mathf.Rad2Deg;
            if (angle + rotationOffset.z >= 270 || angle + rotationOffset.z <= 90)
            {
                spriteRenderer.flipY = false;
            }
            else
            {
                spriteRenderer.flipY = true;
            }
            transform.rotation = Quaternion.Euler(0 + rotationOffset.x, 0 + rotationOffset.y, angle + rotationOffset.z);
        }

        private Transform GetNewTarget()
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log($"Players found {targets.Length - 1}");
            if (targets.Length == 0) return this.transform;

            int randomTarget = Random.Range(0, targets.Length - 1);
            retargeting = false;
            targetTimeCountdown = targetTime;
            return targets[randomTarget].transform;
        }

        private IEnumerator RestAndRetarget()
        {
            target = null;
            retargeting = true;
            yield return new WaitForSeconds(restTime);
            target = GetNewTarget();
        }
    }
}