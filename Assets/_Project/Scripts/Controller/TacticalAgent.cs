using System;
using System.Collections.Generic;
using _Project.Scripts.Manager;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Controller
{
    public class TacticalAgent : MonoBehaviour
    {
        public GridManager gridManager;
        public TacticalPathfinder pathFinder;
        public Transform destination;

        private NavMeshAgent navMeshAgent;
        private List<Vector3> wayPoints;
        private int currentWayPointIndex;
        
        public Vector3 GetMovementVelocity() => navMeshAgent.velocity;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            SetCustomPath();
        }

        private void Update()
        {
            FollowCustomPath();
        }

        private void SetCustomPath()
        {
            Node startNode = gridManager.GetNodeFromWorldPosition(transform.position);
            Node endNode = gridManager.GetNodeFromWorldPosition(destination.position);
            List<Node> path = pathFinder.FindTacticalPath(startNode, endNode);

            if (path?.Count > 0)
            {
                wayPoints = path.ConvertAll(node => node.position);
                currentWayPointIndex = 0;
            }
        }
        
        private void FollowCustomPath()
        {
            if (wayPoints == null || currentWayPointIndex >= wayPoints.Count)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
                return;
            }
            
            navMeshAgent.SetDestination(wayPoints[currentWayPointIndex]);
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 1f) {
                currentWayPointIndex++;
            }
        }
    }
}