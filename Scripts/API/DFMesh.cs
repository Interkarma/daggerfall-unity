// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Text;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Stores decomposed mesh data. A mesh is made up of one or more submeshes (DFSubMesh). Each submesh has a unique texture.
    ///  All faces sharing a texture have been grouped into the appropriate submesh. Each submesh contains one
    ///  or more planes (DFPlane). A plane is a collection of points (DFPoint) in a fan radiating from point 0.
    ///  Planes with 3 points can be written as triangles to your 3D engine, however planes with 4 or more points must be
    ///  written based on your needs. For example, if you wanted to write a triangle list then write the first triangle from
    ///  the first three points (indices 0, 1, 2), then increment the second two point indices (0, 2, 3, then 0, 3, 4, and so on)
    ///  until all points have been read. This structure has been chosen as a half-way point between native Daggerfall formats
    ///  and modern 3D engines. How the data is used will depend on the engine chosen for visualisation.
    /// </summary>
    [Serializable]
    public struct DFMesh
    {
        #region Structure Variables

        /// <summary>Object ID of the mesh. This ID is referenced by higher level structures such as blocks.</summary>
        public int ObjectId;

        /// <summary>Radius of this mesh from origin to farthest point.</summary>
        public float Radius;

        /// <summary>Total number of vertices across all submeshes. Helpful for allocating vertex buffers.</summary>
        public int TotalVertices;

        /// <summary>Total number of triangles across all subeshes. Helpful for allocating index buffers.</summary>
        public int TotalTriangles;

        /// <summary>Submesh array. Each submesh contains DFPlane structures grouped by texture.</summary>
        public DFSubMesh[] SubMeshes;

        #endregion

        #region Child Structures

        /// <summary>
        /// These are methods by which the UV coordinates are generated. This is for troubleshooting only and can be ignored.
        /// </summary>
        public enum UVGenerationMethods
        {
            /// <summary>An unknown method.</summary>
            Unknown,
            /// <summary>Face has three points only.</summary>
            TriangleOnly,
            /// <summary>Using FaceUVTool matrix generator.</summary>
            ModifedMatrixGenerator,
        }

        /// <summary>
        /// Stores mesh submesh data. Each submesh has a unique texture.
        /// </summary>
        [Serializable]
        public struct DFSubMesh
        {
            /// <summary>Texture archive index. Used to determine which texture file to load (e.g. TEXTURE.210).</summary>
            public int TextureArchive;

            /// <summary>Texture record index. Used to determine which texture record (index) to load from archive.</summary>
            public int TextureRecord;

            /// <summary>Total number of triangles in this submesh. Helpful for allocating index buffers.</summary>
            public int TotalTriangles;

            /// <summary>Array of faces sharing the same texture.</summary>
            public DFPlane[] Planes;
        }

        /// <summary>
        /// Stores plane data. The point array is stored in a left-handed counter-clockwise fashion.
        ///  Adjust winding and invert based on your destination engine. Planes are grouped under a submesh
        ///  based on their texture. Each plane is a triangle fan.
        /// </summary>
        [Serializable]
        public struct DFPlane
        {
            /// <summary>Array of vertices.</summary>
            public DFPoint[] Points;

            /// <summary>The UV generation method used for this plane. This is for troubleshooting only and can be ignored.</summary>
            public UVGenerationMethods UVGenerationMethod;
        }

        /// <summary>
        /// Describes a single vertex. Normals and texture coordinates have been read from native mesh format.
        /// </summary>
        [Serializable]
        public struct DFPoint
        {
            /// <summary>X position.</summary>
            public float X;

            /// <summary>Y position.</summary>
            public float Y;

            /// <summary>Z position.</summary>
            public float Z;

            /// <summary>X component of normal.</summary>
            public float NX;

            /// <summary>Y component of normal.</summary>
            public float NY;

            /// <summary>Z component of normal.</summary>
            public float NZ;

            /// <summary>U coordinate of texture.</summary>
            public float U;

            /// <summary>V coordinate of texture.</summary>
            public float V;
        }

        #endregion
    }
}
