using System.Collections.Generic;
using UnityEngine;

namespace mesh
{
    public class TKMesh
    {
        public List<TKVert> Verts { get; } = new List<TKVert>();
        public List<TKEdge> Edges { get; } = new List<TKEdge>();
        public List<TKFace> Faces { get; } = new List<TKFace>();
        public List<TKLoop> Loops { get; } = new List<TKLoop>();

        public TKMesh()
        {

        }

        public TKMesh(Mesh mesh)
        {
            TKVert[] tkVertex = new TKVert[mesh.vertices.Length];
            for (var i = 0; i < mesh.vertices.Length; i++)
            {
                tkVertex[i] = new TKVert
                {
                    pos = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z)
                };
            }

            for (int i = 0; i < mesh.triangles.Length - 3; i++)
            {
                int tr1 = mesh.triangles[i];
                int tr2 = mesh.triangles[i + 1];
                int tr3 = mesh.triangles[i + 2];
            }
        }

        private TKLoop _createInitialLoop(TKVert v, TKEdge e, out TKFace face)
        {
            face = new TKFace();
            TKLoop loop = _createLoop(v,e,face);
            Faces.Add(face);
            loop.AttachRadialLoopEdge(e);
            face.LoopFirst = loop;
            return loop;
        }

        private TKLoop _createLoop(TKVert v, TKEdge e, TKFace f)
        {
            TKLoop loop = new TKLoop(e,v,f);
            Loops.Add(loop);

            return loop;
        }


        public TKFace CreateFace(TKVert[] verts, TKEdge[] edges, Vector3 normal)
        {
            TKLoop startLoop, lastLoop;
            startLoop = lastLoop = _createInitialLoop(verts[0], edges[0], out var f);

            f.LoopFirst = startLoop;

            for (int i = 1; i < verts.Length; i++)
            {
                var l = _createLoop(verts[i], edges[i], f);

                l.AttachRadialLoopEdge(edges[i]);

                l.RadialPrev = lastLoop;
                lastLoop.RadialNext = l;
                lastLoop = l;
            }

            startLoop.RadialPrev = lastLoop;
            lastLoop.RadialNext = startLoop;
            f.Length = verts.Length;
            return f;
        }

        public TKFace CreateFace(TKVert[] verts, bool createEdges)
        {
            TKEdge[] edges = new TKEdge[verts.Length];
            if (createEdges)
            {
                EdgesFromVertEnsure(edges, verts);
            }
            else
            {
                if (EdgesFromVerts(edges, verts) == false)
                {
                    return null;
                }
            }

            return CreateFace(verts, edges, new Vector3());
        }

        public bool EdgesFromVerts(TKEdge[] edges, TKVert[] verts)
        {
            int i, i_prev = verts.Length - 1;
            for (i = 0; i < verts.Length; i++)
            {
                edges[i_prev] = TKEdge.EdgeExists(verts[i], verts[i_prev]);
                if (edges[i_prev] == null)
                {
                    return false;
                }

                i_prev = i;
            }

            return true;
        }

        /// <summary>
        /// Fill in edges if verts are not filled
        /// </summary>
        public void EdgesFromVertEnsure(TKEdge[] edges, TKVert[] verts)
        {
            int i_prev = verts.Length - 1;
            for (int i = 0; i < verts.Length; i++)
            {
                edges[i_prev] = CreateEdge(verts[i_prev], verts[i], false);
                i_prev = i;
            }
        }

        public TKEdge CreateEdge(TKVert v1, TKVert v2, bool allowDouble)
        {
            TKEdge e;
            if (!allowDouble && (e = TKEdge.EdgeExists(v1, v2)) != null)
            {
                return e;
            }

            e = new TKEdge(v1, v2);
            Edges.Add(e);
            return e;
        }


        public TKVert CreateVert(Vector3 pos) {
            TKVert vert = new TKVert();
            Verts.Add(vert);
            return vert;
        }

        public class TKFace
        {
            public int Length { get; set; }
            public Vector3 Normal { get; set; }
            public TKLoop LoopFirst { get; set; }
        }

        public class TKLoop
        {
            public TKLoop(TKEdge e, TKVert v, TKFace f)
            {
                Edge = e;
                Vert = v;
                Face = f;
            }

            public TKEdge Edge { get; set; }
            public TKVert Vert { get; set; }
            public TKFace Face { get; set; }

            public TKLoop RadialNext { get; set; }
            public TKLoop RadialPrev { get; set; }

            internal void AttachRadialLoopEdge(TKEdge edge)
            {
                if (edge.Loop == null)
                {
                    edge.Loop = this;
                    RadialNext = RadialPrev = this;
                }
                else
                {
                    RadialPrev = edge.Loop;
                    RadialNext = edge.Loop.RadialNext;

                    edge.Loop.RadialNext.RadialPrev = this;
                    edge.Loop.RadialNext = this;

                    edge.Loop = this;
                }
                Edge = edge;
            }

            internal void DetachRadialLoopEdge(TKEdge edge)
            {
                if (RadialNext != this)
                {
                    if (this == edge.Loop)
                    {
                        edge.Loop = RadialNext;
                    }

                    RadialNext.RadialPrev = RadialPrev;
                    RadialPrev.RadialNext = RadialNext;
                }
                else
                {
                    if (this == edge.Loop)
                    {
                        edge.Loop = null;
                    }
                }

            }

        }

        public class TKVert
        {
            public Vector3 pos { get; set; }
            public TKEdge Edge { get; set; }

            public int EdgeCount()
            {
                int count = 0;
                if (Edge != null)
                {
                    TKEdge first, iter;
                    first = iter = Edge;
                    do
                    {
                        count++;
                    } while ((iter = iter.NextEdge(this)) != first);
                }
                return count;
            }

        }

        public class TKEdgeDisk
        {
            public TKEdge Prev { get; set; }
            public TKEdge Next { get; set; }
        }

        public class TKEdge
        {
            public int Group { get; set; }
            public int SupportType { get; set; }

            public TKLoop Loop { get; set; }

            public TKVert V1 { get; set; }

            public TKVert V2 { get; set; }
            public TKEdgeDisk DiskV1Link { get;  } = new TKEdgeDisk();
            public TKEdgeDisk DiskV2Link { get; } = new TKEdgeDisk();

            TKEdge()
            {

            }

            public TKEdge(TKVert v1, TKVert v2)
            {
                V1 = v1;
                V2 = v2;
                AttachToDiskEdge(v1);
                AttachToDiskEdge(v2);
            }

            public static TKEdge EdgeExists(TKVert v1, TKVert v2)
            {
                TKEdge e_a, e_b;
                if ((e_a = v1.Edge) != null && (e_b = v2.Edge) != null)
                {
                    TKEdge e_a_iter = e_a, e_b_iter = e_b;
                    do
                    {
                        if (e_a_iter.IsVertInEdge(v2))
                        {
                            return e_a_iter;
                        }

                        if (e_b_iter.IsVertInEdge(v1))
                        {
                            return e_b_iter;
                        }

                    } while ((e_a_iter = e_a_iter.NextEdge(v1)) != e_a && (e_b_iter = e_b_iter.NextEdge(v2)) != e_b);
                }

                return null;
            }

            public bool IsVertInEdge(TKVert v1)
            {
                return V1 == v1 || V2 == v1;
            }

            public bool IsVertInEdge(TKVert v1, TKVert v2)
            {
                return (V1 == v1 && V2 == v2) ||
                       (V1 == v2 && V2 == v1);
            }

            public static TKEdge FindEdge(TKVert v1, TKVert v2)
            {
                TKEdge first, iter;
                if (v1.Edge != null)
                {
                    first = iter = v1.Edge;
                    do
                    {
                        if (iter.IsVertInEdge(v1, v2))
                        {
                            return iter;
                        }
                    } while ((iter = iter.NextEdge(v1)) != first);
                }

                return null;
            }



            public TKEdge NextEdge(TKVert v)
            {
                return GetDiskLinkFromVert(v).Next;
            }

            public TKEdge PrevEdge(TKVert v)
            {
                return GetDiskLinkFromVert(v).Next;
            }

            protected void AttachToDiskEdge(TKVert v)
            {
                if (v.Edge == null)
                {
                    TKEdgeDisk dl1 = GetDiskLinkFromVert(v);
                    v.Edge = this;
                    dl1.Next = dl1.Prev = this;
                }
                else
                {
                    TKEdgeDisk dl1 = GetDiskLinkFromVert(v);
                    TKEdgeDisk dl2 = v.Edge.GetDiskLinkFromVert(v);
                    TKEdgeDisk dl3 = dl2.Prev?.GetDiskLinkFromVert(v);

                    dl1.Next = v.Edge;
                    dl1.Prev = dl2.Prev;

                    dl2.Prev = this;
                    if (dl3 != null)
                    {
                        dl3.Next = this;
                    }
                }
            }

            protected void DetachDiskEdge(TKVert v)
            {
                TKEdgeDisk dl1, dl2;
                dl1 = GetDiskLinkFromVert(v);
                if (dl1.Prev != null)
                {
                    dl2 = dl1.Prev.GetDiskLinkFromVert(v);
                    dl2.Next = dl1.Next;
                }

                if (dl1.Next != null)
                {
                    dl2 = dl1.Next.GetDiskLinkFromVert(v);
                    dl2.Prev = dl1.Prev;
                }

                if (v.Edge == this)
                {
                    v.Edge = (this != dl1.Next) ? dl1.Next : null;
                }
                dl1.Next = dl1.Next = null;
            }

            protected bool SwapDiskVert(TKVert dest, TKVert src)
            {
                if (V1 == src)
                {
                    V1 = dest;
                    DiskV1Link.Next = DiskV1Link.Prev = null;
                    return true;
                }
                if (V2 == src)
                {
                    V2 = dest;
                    DiskV2Link.Next = DiskV2Link.Prev = null;
                    return true;
                }
                return false;
            }


            protected void DiskVertReplace(TKVert dest, TKVert src)
            {
                DetachDiskEdge(src);
                SwapDiskVert(dest, src);
                AttachToDiskEdge(dest);
            }

            protected void EdgeSwapVert(TKVert dest, TKVert src)
            {
                if (Loop != null)
                {
                    TKLoop iter, first;
                    iter = first = Loop;
                    do
                    {
                        if (iter.Vert == src)
                        {
                            iter.Vert = dest;
                        }
                        else if (iter.RadialNext.Vert == src)
                        {
                            iter.RadialNext.Vert = dest;
                        }
                        else
                        {
                            if(iter.RadialPrev.Vert != src)
                                Debug.Log("Previous vert is equal to src");
                        }

                    } while ((iter = iter.RadialNext) != first);
                }

                DiskVertReplace(dest, src);
            }


            private TKEdgeDisk GetDiskLinkFromVert(TKVert vert)
            {
                if (vert == V2)
                {
                    return DiskV2Link;
                }
                return DiskV1Link;
            }

        }

    }
}
