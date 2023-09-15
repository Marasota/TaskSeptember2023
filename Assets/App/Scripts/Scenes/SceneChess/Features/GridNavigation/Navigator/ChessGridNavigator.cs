using System;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        Queue<Vector2Int> CellQueue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> VisitedFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> Visited = new HashSet<Vector2Int>();
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            //напиши реализацию не меняя сигнатуру функции
            if(unit == ChessUnitType.Pon)
            {
                if (from.x != to.x) return null;
            }
            else if (unit == ChessUnitType.Bishop)
            {
                if ((from.x + from.y) % 2 != (to.x + to.y) % 2) return null;
            }
            CellQueue.Enqueue(from);
            Visited.Add(from);

            while (CellQueue.Count > 0)
            {
                var currentCell = CellQueue.Dequeue();

                if (currentCell == to) { break; }

                foreach (var nextCell in GetMoves(currentCell, unit, grid))
                {
                    VisitedFrom[nextCell] = currentCell;
                    CellQueue.Enqueue(nextCell);
                }
            }

            if (!VisitedFrom.ContainsKey(to)) return null;

            Visited.Clear();
            CellQueue.Clear();

            return CreatePath(from, to, unit);

            throw new NotImplementedException();
        }
        public bool IsWithinGrid(Vector2Int position, ChessGrid grid)
        {
            return (position.x >= 0 && position.x < grid.Size.x) && (position.y >= 0 && position.y < grid.Size.y);
        }
        public List<Vector2Int> GetDirections(ChessUnitType unit )
        {
            List<Vector2Int> Moves = new List<Vector2Int>();
            int maxDist = unit == ChessUnitType.Queen || unit == ChessUnitType.Rook || unit == ChessUnitType.Bishop ? 8 : 2;
            switch (unit)
            {
                default:
                case ChessUnitType.Pon:
                    {
                        Moves.Add(new Vector2Int(0, 1));
                        Moves.Add(new Vector2Int(0, -1));
                        break;
                    }
                case ChessUnitType.Queen:
                case ChessUnitType.King:
                    {
                        for (int i = 1; i < maxDist; i++)
                        {
                            Moves.Add(new Vector2Int(0 , 1 * i));
                            Moves.Add(new Vector2Int(1 * i, 0));
                            Moves.Add(new Vector2Int(-1 * i, 0 ));
                            Moves.Add(new Vector2Int(0, -1 * i));
                            Moves.Add(new Vector2Int(1 * i, 1 * i));
                            Moves.Add(new Vector2Int(-1 * i, 1 * i));
                            Moves.Add(new Vector2Int(-1 * i, -1 * i));
                            Moves.Add(new Vector2Int(1 * i, -1 * i));
                        }
                        break;
                    }
                case ChessUnitType.Knight:
                    {
                        Moves.Add(new Vector2Int(1, 2));
                        Moves.Add(new Vector2Int(-1, 2));
                        Moves.Add(new Vector2Int(2, 1));
                        Moves.Add(new Vector2Int(2, -1));
                        Moves.Add(new Vector2Int(-2, 1));
                        Moves.Add(new Vector2Int(-2, -1));
                        Moves.Add(new Vector2Int(-1, -2));
                        Moves.Add(new Vector2Int(1, -2));
                        break;
                    }
                case ChessUnitType.Rook:
                    {
                        for (int i = 1; i < maxDist; i++)
                        {
                            Moves.Add(new Vector2Int(0, 1 * i));
                            Moves.Add(new Vector2Int(1 * i, 0));
                            Moves.Add(new Vector2Int(-1 * i, 0));
                            Moves.Add(new Vector2Int(0, -1 * i));
                        }
                        break;
                    }
                case ChessUnitType.Bishop:
                    {
                        for (int i = 1; i < maxDist; i++)
                        {
                            Moves.Add(new Vector2Int(1 * i, 1 * i));
                            Moves.Add(new Vector2Int(-1 * i, 1 * i));
                            Moves.Add(new Vector2Int(1 * i, -1 * i));
                            Moves.Add(new Vector2Int(-1 * i, -1 * i));
                        }
                        break;
                    }
            }
            return Moves;
        }
        public bool isChessUnitOnPath(Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            if(from.x == to.x)
            {
               int inc = to.y >= from.y ? 1 : -1; 
               for(int j = from.y+inc; j != to.y; j+=inc)
                {
                    if (grid.Get(j,from.x) != null )
                        return true;
                }
            }
            else if (from.y == to.y)
            {
                int inc = to.x >= from.x ? 1 : -1;
                for (int i = from.x+inc; i != to.x; i+=inc)
                {
                    if (grid.Get(from.y,i) != null) return true;
                }
            }
            else if((from.x + from.y) == (to.x + to.y))
            {
                int inc = to.x >= from.x ? 1 : -1;
                for (int i = from.x+inc, j = from.y-inc; i != to.x; i += inc, j-= inc)
                {
                    if (grid.Get(j,i) != null) return true;
                }
            }
            else
            {
                int inc = to.x >= from.x ? 1 : -1;
                for (int i = from.x + inc, j = from.y + inc; i != to.x; i += inc, j += inc)
                {
                    if (grid.Get(j,i) != null) return true;
                }
            }
            return false;
        }
        public List<Vector2Int> GetMoves(Vector2Int current, ChessUnitType unit, ChessGrid grid)
        {
            List<Vector2Int> Moves = new List<Vector2Int>();
            foreach (var direction in GetDirections(unit))
            {
                Vector2Int nextCell = current + direction;

                if (IsWithinGrid(nextCell, grid))
                {
                    if (!CellQueue.Contains(nextCell) && !Visited.Contains(nextCell) && !isChessUnitOnPath(current,nextCell,grid))
                    {
                        Visited.Add(nextCell);
                        if (grid.Get(nextCell) == null) Moves.Add(nextCell);
                    }
                }
            }
            return Moves;
        }
        public List<Vector2Int> CreatePath(Vector2Int from, Vector2Int to, ChessUnitType unit)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            var temp = to;
            while (temp != from)
            {
                path.Add(temp);
                temp = VisitedFrom[temp];
            }

            path.Reverse();

            VisitedFrom.Clear();
            return path;
        }
    }
}