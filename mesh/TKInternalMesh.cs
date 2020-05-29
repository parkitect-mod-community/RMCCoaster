using UnityEngine;

namespace mesh
{
    internal class TKInternalMesh
    {
        public static TKMesh.TKEdgeDisk EdgeLinkFromVert(TKMesh.TKEdge edge, TKMesh.TKVert vert)
        {
            if (vert == edge.V2)
            {
                return edge.DiskV2Link;
            }
            return edge.DiskV1Link;
        }

        public static TKMesh.TKEdge TKDiskNextEdge(TKMesh.TKEdge e, TKMesh.TKVert v) {
            return EdgeLinkFromVert(e, v).Next;
        }

        public static TKMesh.TKEdge TKDiskPrevEdge(TKMesh.TKEdge e, TKMesh.TKVert v)
        {
            return EdgeLinkFromVert(e, v).Prev;
        }

        public static bool TKVertsInEdge(TKMesh.TKVert v1, TKMesh.TKVert v2, TKMesh.TKEdge e)
        {
            return (e.V1 == v1 && e.V2 == v2) || (e.V1 == v2 && e.V2 == v1);
        }

        public static void TKRadialLoopAppend(TKMesh.TKEdge e, TKMesh.TKLoop l)
        {
            if (e.Loop == null)
            {
                e.Loop = l;
                l.RadialNext = l.RadialPrev = l;
            }
            else
            {
                l.RadialPrev = e.Loop;
                l.RadialNext = e.Loop.RadialNext;

                e.Loop.RadialNext.RadialPrev = l;
                e.Loop.RadialNext = l;
                e.Loop = l;
            }

            l.Edge = e;
        }

        public static void TKDiskEdgeAppend(TKMesh.TKEdge e, TKMesh.TKVert v)
        {
            if (v.Edge == null)
            {
                TKMesh.TKEdgeDisk dl1 = EdgeLinkFromVert(e, v);
                v.Edge = e;
                dl1.Next = dl1.Prev = e;
            }
            else
            {
                TKMesh.TKEdgeDisk dl1 = EdgeLinkFromVert(e, v);
                TKMesh.TKEdgeDisk dl2 = EdgeLinkFromVert(v.Edge, v);
                TKMesh.TKEdgeDisk dl3 = dl2.Prev != null ? EdgeLinkFromVert(dl2.Prev, v) : null;

                dl1.Next = v.Edge;
                dl1.Prev = dl2.Prev;

                dl2.Prev = e;
                if (dl3 != null)
                {
                    dl3.Next = e;
                }
            }
        }

        public static void TKDiskEdgeRemove(TKMesh.TKEdge e, TKMesh.TKVert v)
        {
            TKMesh.TKEdgeDisk dl1, dl2;
            dl1 = EdgeLinkFromVert(e, v);
            if (dl1.Prev != null)
            {
                dl2 = EdgeLinkFromVert(dl1.Prev, v);
                dl2.Next = dl1.Next;
            }

            if (dl1.Next != null)
            {
                dl2 = EdgeLinkFromVert(dl1.Next, v);
                dl2.Prev = dl1.Prev;
            }

            if (v.Edge == e)
            {
                v.Edge = (e != dl1.Next) ? dl1.Next : null;
            }

            dl1.Next = dl1.Prev = null;
        }

        public static TKMesh.TKEdge TKDiskEdgeExists(TKMesh.TKVert v1, TKMesh.TKVert v2)
        {
            TKMesh.TKEdge first, iter;
            if (v1.Edge != null)
            {
                first = iter = v1.Edge;
                do
                {
                    if (TKVertsInEdge(v1, v2, iter))
                    {
                        return iter;
                    }
                } while ((iter = TKDiskNextEdge(iter, v1)) != first);
            }

            return null;
        }


        public static int TKDiskCount(TKMesh.TKVert v)
        {
            int count = 0;
            if (v.Edge != null)
            {
                TKMesh.TKEdge first, iter;
                first = iter = v.Edge;
                do
                {
                    count++;
                } while ((iter = TKDiskNextEdge(iter, v)) != first);
            }
            return count;
        }

        public static int TKDiskCount(TKMesh.TKVert v, int max)
        {
            int count = 0;
            if (v.Edge != null)
            {
                TKMesh.TKEdge first, iter;
                first = iter = v.Edge;
                do
                {
                    count++;
                    if (count == max)
                    {
                        break;
                    }
                } while ((iter = TKDiskNextEdge(iter, v)) != first);
            }

            return count;
        }

    }
}
