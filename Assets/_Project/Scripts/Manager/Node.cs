using UnityEngine;

namespace _Project.Scripts.Manager
{
    public struct Node
    {
        public Vector3 position;
        public bool isWalkable;

        public Node(Vector3 position, bool isWalkable = false)
        {
            this.position = position;
            this.isWalkable = isWalkable;
        }
    }
}