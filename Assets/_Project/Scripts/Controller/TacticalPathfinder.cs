using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Manager;
using _Tools.DataStructures;
using UnityEngine;

namespace _Project.Scripts.Controller
{
    public class TacticalPathfinder : MonoBehaviour
    {
        public GridManager gridManager;
        public float dangerPenalty = 20.0f;

        public List<Node> FindTacticalPath(Node startNode, Node endNode)
        {
            var openList = new PriorityQueue<Node, float>();
            var closedSet = new HashSet<Vector3>();
            var cameFrom = new Dictionary<Vector3, Vector3>();
            var costSoFar = new Dictionary<Vector3, float> { [startNode.position] = 0 };
            var exposureSoFar = new Dictionary<Vector3, float> { [startNode.position] = 0 };
            
            openList.Enqueue(startNode, 0);

            while (openList.Count > 0)
            {
                Node currentNode = openList.Dequeue();
                if (currentNode.position == endNode.position)
                    return ReconstructPath(cameFrom, startNode, endNode);
                
                closedSet.Add(currentNode.position);

                foreach (Node neighbor in GetNeighbors(currentNode))
                {
                    if (closedSet.Contains(neighbor.position)) continue;

                    float exposure = exposureSoFar[currentNode.position];
                    int dangerScore = gridManager.CalculateNodeDanger(gridManager.GetSectors(neighbor.position));

                    float tempExposure = exposure;
                    float newCost = costSoFar[currentNode.position] +
                                    TacticalCostNodeToNode(currentNode, neighbor, ref tempExposure, dangerScore);
                    if (costSoFar.TryGetValue(neighbor.position, out float existingCost) &&
                        newCost >= existingCost) continue;
                    
                    costSoFar[neighbor.position] = newCost;
                    exposureSoFar[neighbor.position] = tempExposure;
                    cameFrom[neighbor.position] = currentNode.position;

                    float priority = newCost + Heuristic(neighbor, endNode);
                    openList.Enqueue(neighbor, priority);
                }
            }
            
            Debug.LogWarning("No Path found!");
            return null;
        }

        private List<Node> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Node startNode, Node endNode)
        {
            var path = new List<Node>();
            Vector3 current = endNode.position;

            while (current != startNode.position)
            {
                path.Add(new Node(current));
                current = cameFrom[current];
            }
            
            path.Add(startNode);
            path.Reverse();
            return SimplifyPath(path);
        }
        
        private List<Node> SimplifyPath(List<Node> fullPath) {
            if (fullPath == null || fullPath.Count < 2) return fullPath;

            var simplifiedPath = new List<Node> { fullPath[0] };

            for (int i = 1; i < fullPath.Count - 1; i++) {
                if (gridManager.CalculateNodeDanger(gridManager.GetSectors(fullPath[i].position)) == 0) {
                    simplifiedPath.Add(fullPath[i]);
                }
            }

            simplifiedPath.Add(fullPath[^1]);
            return simplifiedPath;
        }

        private float Heuristic(Node fromNode, Node toNode)
        {
            float baseHeuristic = (fromNode.position - toNode.position).sqrMagnitude;
            int dangerScore = gridManager.CalculateNodeDanger(gridManager.GetSectors(toNode.position));
            float heuristicPenalty = dangerScore * dangerPenalty;
            return baseHeuristic + heuristicPenalty;
        }

        private float TacticalCostNodeToNode(Node startNode, Node toNode, ref float exposure, int dangerScore)
        {
            float travelTime = Vector3.Distance(startNode.position, toNode.position);

            if (dangerScore > 0) {
                exposure += dangerScore * dangerPenalty;
            }

            float exposureRefund = 0f;
            if (dangerScore == 0) {
                exposureRefund = -exposure;
                exposure = 0;
            }

            float penalty = Mathf.Pow(3, dangerScore);
            return travelTime + penalty * penalty + exposureRefund;
        }

        IEnumerable<Node> GetNeighbors(Node node)
        {
            Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
            return directions
                .Select(direction => gridManager.GetNodeFromWorldPosition(node.position + direction * gridManager.cellSize))
                .Where(neighbor => neighbor.isWalkable);
        }
    }
}