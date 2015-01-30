// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Combines Daggerfall model data.
    /// </summary>
    public class ModelCombiner
    {
        #region Fields

        bool isSealed = false;
        int vertexCount = 0;

        // Builder dictionary used for adding models.
        Dictionary<int, BatchData> builderDictionary;

        // Combined model data produced after Apply().
        CombinedModel combinedModel;

        #endregion

        #region Structures

        /// <summary>
        /// Combined model data.
        /// </summary>
        public struct CombinedModel
        {
            public Vector3[] Vertices;
            public Vector3[] Normals;
            public Vector2[] UVs;
            public int[] Indices;
            public SubMesh[] SubMeshes;
        }

        /// <summary>
        /// Submesh of a combined model.
        /// </summary>
        public struct SubMesh
        {
            public int StartIndex;
            public int PrimitiveCount;
            public int TextureArchive;
            public int TextureRecord;
        }

        /// <summary>
        /// Describes a batch of combined geometry during the build process.
        /// </summary>
        private struct BatchData
        {
            public List<Vector3> Vertices;
            public List<Vector3> Normals;
            public List<Vector2> UVs;
            public List<int> Indices;
        }

        #endregion

        #region Properties

        /// <summary>
        /// A sealed builder cannot be added to.
        /// </summary>
        public bool IsSealed
        {
            get { return isSealed; }
        }

        /// <summary>
        /// Gets current vertex count of builder.
        /// </summary>
        public int VertexCount
        {
            get { return vertexCount; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ModelCombiner()
        {
            // Start new builder
            NewCombiner();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts a new combiner.
        /// </summary>
        public void NewCombiner()
        {
            // Create empty dictionaries
            builderDictionary = new Dictionary<int, BatchData>();
            vertexCount = 0;
            isSealed = false;
        }

        /// <summary>
        /// Adds model data to combiner.
        /// </summary>
        /// <param name="key">Key to batch against.</param> 
        /// <param name="modelData">Model data to add.</param>
        /// <param name="matrix">Transform to apply.</param>
        public void Add(ref ModelData modelData, Matrix4x4 matrix)
        {
            // Do nothing if sealed
            if (isSealed)
                return;

            // Iterate submeshes
            foreach (var sm in modelData.SubMeshes)
            {
                // Start new batch data for this submesh
                BatchData batchData = new BatchData();
                batchData.Vertices = new List<Vector3>();
                batchData.Normals = new List<Vector3>();
                batchData.UVs = new List<Vector2>();
                batchData.Indices = new List<int>();

                int counter = 0;
                int index = sm.StartIndex;
                for (int tri = 0; tri < sm.PrimitiveCount; tri++)
                {
                    // Get indices
                    int i1 = modelData.Indices[index++];
                    int i2 = modelData.Indices[index++];
                    int i3 = modelData.Indices[index++];

                    // Get vertices
                    Vector3 vert1 = modelData.Vertices[i1];
                    Vector3 vert2 = modelData.Vertices[i2];
                    Vector3 vert3 = modelData.Vertices[i3];

                    // Get normals
                    Vector3 norm1 = modelData.Normals[i1];
                    Vector3 norm2 = modelData.Normals[i2];
                    Vector3 norm3 = modelData.Normals[i3];

                    // Get UVs
                    Vector3 uv1 = modelData.UVs[i1];
                    Vector3 uv2 = modelData.UVs[i2];
                    Vector3 uv3 = modelData.UVs[i3];

                    // Add vertices
                    batchData.Vertices.Add(vert1);
                    batchData.Vertices.Add(vert2);
                    batchData.Vertices.Add(vert3);

                    // Add normals
                    batchData.Normals.Add(norm1);
                    batchData.Normals.Add(norm2);
                    batchData.Normals.Add(norm3);

                    // Add UVs
                    batchData.UVs.Add(uv1);
                    batchData.UVs.Add(uv2);
                    batchData.UVs.Add(uv3);

                    // Add indices
                    batchData.Indices.Add(counter++);
                    batchData.Indices.Add(counter++);
                    batchData.Indices.Add(counter++);
                }

                // Add to builder
                int key = MaterialReader.MakeTextureKey((short)sm.TextureArchive, (byte)sm.TextureRecord, (byte)0);
                AddData(key, ref batchData, matrix);
            }
        }

        /// <summary>
        /// Adds another ModelCombiner to this one.
        /// </summary>
        /// <param name="other">Other ModelCombiner.</param>
        /// <param name="matrix">Transform to apply</param>
        public void Add(ModelCombiner other, Matrix4x4 matrix)
        {
            // Do nothing if sealed
            if (isSealed || other.isSealed)
                return;

            // Add other items to this builder
            foreach (var item in other.builderDictionary)
            {
                BatchData batchData = item.Value;
                AddData(item.Key, ref batchData, matrix);
            }
        }

        /// <summary>
        /// Apply all model combines and seal builder.
        /// </summary>
        public void Apply()
        {
            // Do nothing if sealed
            if (isSealed)
                return;

            // Count total vertices and indices
            int totalVertices = 0;
            int totalIndices = 0;
            foreach (var item in builderDictionary)
            {
                totalVertices += item.Value.Vertices.Count;
                totalIndices += item.Value.Indices.Count;
            }

            // Create combined data
            combinedModel = new CombinedModel();
            combinedModel.Vertices = new Vector3[totalVertices];
            combinedModel.Normals = new Vector3[totalVertices];
            combinedModel.UVs = new Vector2[totalVertices];
            combinedModel.Indices = new int[totalIndices];
            combinedModel.SubMeshes = new SubMesh[builderDictionary.Count];

            // Populate static arrays
            int currentVertex = 0;
            int currentIndex = 0;
            int subMeshIndex = 0;
            foreach (var item in builderDictionary)
            {
                // Save current highest vertex and index
                int highestVertex = currentVertex;
                int highestIndex = currentIndex;

                // Copy vertex data
                for (int i = 0; i < item.Value.Vertices.Count; i++)
                {
                    combinedModel.Vertices[currentVertex] = item.Value.Vertices[i];
                    combinedModel.Normals[currentVertex] = item.Value.Normals[i];
                    combinedModel.UVs[currentVertex] = item.Value.UVs[i];
                    currentVertex++;
                }

                // Copy index data
                for (int i = 0; i < item.Value.Indices.Count; i++)
                {
                    combinedModel.Indices[currentIndex++] = highestVertex + i;
                }

                // Add submesh
                int frame;
                SubMesh sm = new SubMesh();
                sm.StartIndex = highestIndex;
                sm.PrimitiveCount = item.Value.Indices.Count / 3;
                MaterialReader.ReverseTextureKey(item.Key, out sm.TextureArchive, out sm.TextureRecord, out frame);
                combinedModel.SubMeshes[subMeshIndex++] = sm;
            }

            Seal();
        }

        /// <summary>
        /// Gets combined model data created by Apply().
        /// </summary>
        /// <param name="combinedModelOut">Combined model data out.</param>
        /// <returns>True if successful.</returns>
        public bool GetCombinedModel(out CombinedModel combinedModelOut)
        {
            // Must be sealed
            if (!isSealed)
            {
                combinedModelOut = new CombinedModel();
                return false;
            }

            combinedModelOut = combinedModel;

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds batch data to combiner.
        /// </summary>
        /// <param name="key">Key to combine against.</param>
        /// <param name="batchData">Data to add.</param>
        /// <param name="matrix">Transform to apply</param>
        private void AddData(int key, ref BatchData batchData, Matrix4x4 matrix)
        {
            // Do nothing if sealed
            if (isSealed)
                return;

            BatchData builder;
            if (builderDictionary.ContainsKey(key))
            {
                // Get current batch data
                builder = builderDictionary[key];
            }
            else
            {
                // Start a new batch
                builder.Vertices = new List<Vector3>();
                builder.Normals = new List<Vector3>();
                builder.UVs = new List<Vector2>();
                builder.Indices = new List<int>();
                builderDictionary.Add(key, builder);
            }

            // Transform vertices and normals by matrix
            for (int i = 0; i < batchData.Vertices.Count; i++)
            {
                Vector3 position = matrix.MultiplyPoint3x4(batchData.Vertices[i]);
                Vector3 normal = matrix.MultiplyVector(batchData.Normals[i]);
                batchData.Vertices[i] = position;
                batchData.Normals[i] = normal;
            }

            // Add new vertices to builder
            int currentVertex = builder.Vertices.Count;
            builder.Vertices.AddRange(batchData.Vertices);
            builder.Normals.AddRange(batchData.Normals);
            builder.UVs.AddRange(batchData.UVs);

            // Update vertex count
            vertexCount += batchData.Vertices.Count;

            // Update indices to new vertex base
            for (int i = 0; i < batchData.Indices.Count; i++)
            {
                batchData.Indices[i] += currentVertex;
            }

            // Add indices to builder
            builder.Indices.AddRange(batchData.Indices);

            // Update dictionary
            builderDictionary[key] = builder;
        }

        /// <summary>
        /// Seal the builder once all static geometry has been completed.
        ///  This removes the builder dictionary and just keeps the final vertex and index buffers.
        ///  You cannot add to a sealed builder.
        /// </summary>
        private void Seal()
        {
            if (builderDictionary != null)
            {
                // Dispose of builder dictionary
                builderDictionary.Clear();
                builderDictionary = null;
            }

            // Set flag
            this.isSealed = true;
        }

        #endregion
    }
}