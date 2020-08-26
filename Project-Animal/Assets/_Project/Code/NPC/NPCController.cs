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
                transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * moveSpeed);
                // transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * moveSpeed);
            }
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