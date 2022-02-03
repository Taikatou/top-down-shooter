using System;
using System.Collections.Generic;
using System.Linq;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts.MLAgents;
using UnityEngine;

namespace Research.LevelDesign.Scripts.AI
{
    public class Node
    {
        public Node Parent;
        public Vector2Int Position;
        public int DistanceToTarget;
        public int Cost;
        public readonly int Weight;
        public float F
        {
            get
            {
                if (DistanceToTarget != -1 && Cost != -1)
                    return DistanceToTarget + Cost;
                else
                    return -1;
            }
        }
        public readonly bool Walkable;

        public Node(Vector2Int pos, bool walkable, int weight = 1)
        {
            Parent = null;
            Position = pos;
            DistanceToTarget = -1;
            Cost = 1;
            Weight = weight;
            Walkable = walkable;
        }
    }

    public static class Heuristic
    {
        public static int StraightLine(Vector2Int positionA, Vector2Int positionB)
        {
            return Math.Abs(positionA.x - positionB.x) + Math.Abs(positionA.y - positionB.y);
        }
    }

    public static class AStar
    {
        public static Stack<Node> FindPath(GridSpace[,] gridSpace, Vector2Int startPos, Vector2Int endPos)
        {
            var width = gridSpace.GetUpperBound(0);
            var height = gridSpace.GetUpperBound(1);
            var map = new Node[width, height];
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    map[i,j] = new Node
                    (
                        new Vector2Int(i, j),
                        gridSpace[i, j] == GridSpace.Floor
                    );
                }
            }

            var start = new Node(startPos, true);
            var end = new Node(endPos, true);

            var path = new Stack<Node>();
            var openList = new List<Node>();
            var closedList = new List<Node>();
           
            var current = start;
           
            // add start node to Open List
            openList.Add(start);

            while(openList.Count != 0 && !closedList.Exists(x => x.Position == end.Position))
            {
                current = openList[0];
                openList.Remove(current);
                closedList.Add(current);
                var adjacencies = GetAdjacentNodes(map, current);

 
                foreach(var n in adjacencies)
                {
                    if (!closedList.Contains(n) && n.Walkable)
                    {
                        if (!openList.Contains(n))
                        {
                            n.Parent = current;
                            n.DistanceToTarget = Heuristic.StraightLine(n.Position, endPos);
                            n.Cost = n.Weight + n.Parent.Cost;
                            openList.Add(n);
                            openList = openList.OrderBy(node => node.F).ToList();
                        }
                    }
                }
            }
            
            // construct path, if end was not closed return null
            if(!closedList.Exists(x => x.Position == end.Position))
            {
                return null;
            }

            // if all good, return path
            var temp = closedList[closedList.IndexOf(current)];
            if (temp == null) return null;
            do
            {
                path.Push(temp);
                temp = temp.Parent;
            }
            while (temp != start && temp != null);
            return path;
        }
		
        private static List<Node> GetAdjacentNodes(Node[,] grid, Node n)
        {
            var temp = new List<Node>();

            var row = n.Position.y;
            var col = n.Position.x;

            if(row + 1 < grid.GetUpperBound(0))
            {
                temp.Add(grid[col, row + 1]);
            }
            if(row - 1 >= 0)
            {
                temp.Add(grid[col, row - 1]);
            }
            if(col - 1 >= 0)
            {
                temp.Add(grid[col - 1, row]);
            }
            if(col + 1 < grid.GetUpperBound(1))
            {
                temp.Add(grid[col + 1, row]);
            }

            return temp;
        }
    }
}
