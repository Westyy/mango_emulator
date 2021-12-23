using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms.Instance;

namespace Mango.Rooms.Mapping.PathFinding
{
    static class PathFinder
    {
        private static bool NoDiag = false;

        public static List<Vector2D> FindPath(MappingComponent Mapping, Vector2D Start, Vector2D End)
        {
            List<Vector2D> Path = new List<Vector2D>();

            PathFinderNode Nodes = FindPathReversed(Mapping, End, Start);

            if (Nodes != null) // make sure we do have a path first
            {
                while (Nodes.Next != null)
                {
                    Path.Add(Nodes.Next.Position);
                    Nodes = Nodes.Next;
                }
            }

            // I need to change 'IsValidStep' to not count the position the user is on (the user who wants to walk) or the emulator will error..

            return Path;
        }

        private static PathFinderNode FindPathReversed(MappingComponent Mapping, Vector2D Start, Vector2D End)
        {
            MinHeap<PathFinderNode> OpenList = new MinHeap<PathFinderNode>(256);

            PathFinderNode[,] Map = new PathFinderNode[Mapping.SizeX, Mapping.SizeY];
            PathFinderNode Node;
            Vector2D Tmp;
            int Cost;
            int Diff;

            PathFinderNode Current = new PathFinderNode(Start);
            Current.Cost = 0;

            PathFinderNode Finish = new PathFinderNode(End);
            Map[Current.Position.X, Current.Position.Y] = Current;
            OpenList.Add(Current);

            while (OpenList.Count > 0)
            {
                Current = OpenList.ExtractFirst();
                Current.InClosed = true;

                for (int i = 0; NoDiag ? i < NoDiagMovePoints.Length : i < MovePoints.Length; i++)
                {
                    Tmp = Current.Position + (NoDiag ? NoDiagMovePoints[i] : MovePoints[i]);
                    bool IsFinalMove = (Tmp.X == End.X && Tmp.Y == End.Y); // are we at the final position?

                    if (Mapping.IsValidStep(new Vector2D(Current.Position.X, Current.Position.Y), Tmp, IsFinalMove, new List<RoomAvatar>[Mapping.SizeX, Mapping.SizeY])) // need to set the from positions
                    {
                        if (Map[Tmp.X, Tmp.Y] == null)
                        {
                            Node = new PathFinderNode(Tmp);
                            Map[Tmp.X, Tmp.Y] = Node;
                        }
                        else
                        {
                            Node = Map[Tmp.X, Tmp.Y];
                        }

                        if (!Node.InClosed)
                        {
                            Diff = 0;

                            if (Current.Position.X != Node.Position.X)
                            {
                                Diff += 1;
                            }

                            if (Current.Position.Y != Node.Position.Y)
                            {
                                Diff += 1;
                            }

                            Cost = Current.Cost + Diff + Node.Position.GetDistanceSquared(End);

                            if (Cost < Node.Cost)
                            {
                                Node.Cost = Cost;
                                Node.Next = Current;
                            }

                            if (!Node.InOpen)
                            {
                                if (Node.Equals(Finish))
                                {
                                    Node.Next = Current;
                                    return Node;
                                }

                                Node.InOpen = true;
                                OpenList.Add(Node);
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static Vector2D[] MovePoints = new Vector2D[]
        {
            new Vector2D(-1, -1),
            new Vector2D(0, -1),
            new Vector2D(1, -1),
            new Vector2D(1, 0),
            new Vector2D(1, 1),
            new Vector2D(0, 1),
            new Vector2D(-1, 1),
            new Vector2D(-1, 0)
        };

        private static Vector2D[] NoDiagMovePoints = new Vector2D[]
        {
            new Vector2D(0, -1),
            new Vector2D(1, 0),
            new Vector2D(0, 1),
            new Vector2D(-1, 0)
        };
    }
}
