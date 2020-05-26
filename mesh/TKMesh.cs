using System.Collections.Generic;
using UnityEngine;

namespace mesh
{
    public class TKMesh
    {
        private List<TKVertex> _vertices = new List<TKVertex>();

        struct TKFace {
            private int Num { get; set; }
        }

        public class TKVertex
        {
            public Vector3 pos { get; set; } = new Vector3();
            public List<TKEdge> Edges { get; }= new List<TKEdge>();
        }

        public class TKDisk
        {
            public TKVertex prev { get; set; }
            public TKVertex next { get; set; }
        }

        public class TKEdge
        {
            public TKVertex V1 { get; set; }
            public TKVertex V2 { get; set; }

            public TKDisk DiskV1Link { get; set; } = null;
            public TKDisk DiskV2Link { get; set; } = null;
        }

    }
}
