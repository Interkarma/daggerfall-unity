// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Text;
using DaggerfallConnect.InternalTypes;
#endregion

namespace DaggerfallConnect.Utility
{
    /// <summary>
    /// This class is based on Dave Humphrey's DF_3DSTex.CPP.
    ///  Modified for Daggerfall Connect Library by Gavin Clayton "Interkarma" (www.dfworkshop.net).
    ///  For original version download DFTo3DS at www.uesp.net.
    /// </summary>
    internal class FaceUVTool
    {
        #region Class Structures

        /// <summary>
        /// Describes a single Daggerfall-native vertex.
        /// </summary>
        internal struct DFPurePoint
        {
            public Int32 x;
            public Int32 y;
            public Int32 z;
            public Int32 nx;
            public Int32 ny;
            public Int32 nz;
            public Int32 u;
            public Int32 v;
        }

        /// <summary>
        /// Used to store a 2D point.
        /// </summary>
        private struct DF2DPoint
        {
            public Int32 x;
            public Int32 y;
        }

        /// <summary>
        /// Local type for matrix conversion parameters.
        /// </summary>
        private struct df3duvparams_lt
        {
            public float[] X;
            public float[] Y;
            public float[] Z;
            public float[] U;
            public float[] V;
        }

        /// <summary>
        /// Used to convert XYZ point coordinates to DF UV coordinates.
        /// </summary>
        private struct df3duvmatrix_t
        {
            public float UA;
            public float UB;
            public float UC;
            public float UD;
            public float VA;
            public float VB;
            public float VC;
            public float VD;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates absolute UV values for all points of a face.
        /// </summary>
        /// <param name="faceVertsIn">Source array of native point values.</param>
        /// <param name="faceVertsOut">Destination array for calculated UV values.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool ComputeFaceUVCoordinates(ref DFPurePoint[] faceVertsIn, ref DFPurePoint[] faceVertsOut)
        {
            // Get first three vertices of ngon (these three vertices are always non-collinear)
            Vector3 P0 = new Vector3(faceVertsIn[0].x, faceVertsIn[0].y, faceVertsIn[0].z);
            Vector3 P1 = new Vector3(faceVertsIn[1].x, faceVertsIn[1].y, faceVertsIn[1].z);
            Vector3 P2 = new Vector3(faceVertsIn[2].x, faceVertsIn[2].y, faceVertsIn[2].z);

            // Create coplanar vectors from p1->p0 and p2->p0
            Vector3 V0 = P1 - P0;
            Vector3 V1 = P2 - P0;

            // Orthogonalize V1
            V1 = V1 - (V0 * (V1.DotProduct(V0) / (V0.DotProduct(V0))));

            // Normalize both vectors
            V0.Normalize();
            V1.Normalize();

            // Compute first three vertices in 2D space
            DF2DPoint p0, p1, p2;
            p0.x = (Int32)P0.DotProduct(V0);
            p0.y = (Int32)P0.DotProduct(V1);
            p1.x = (Int32)P1.DotProduct(V0);
            p1.y = (Int32)P1.DotProduct(V1);
            p2.x = (Int32)P2.DotProduct(V0);
            p2.y = (Int32)P2.DotProduct(V1);

            // Initialise the params struct
            df3duvparams_lt Params = new df3duvparams_lt();
            Params.X = new float[4];
            Params.Y = new float[4];
            Params.Z = new float[4];
            Params.U = new float[4];
            Params.V = new float[4];

            // Initialise the conversion matrix
            df3duvmatrix_t Matrix = new df3duvmatrix_t();
            Matrix.UA = 1.0f;
            Matrix.UB = 0.0f;
            Matrix.UC = 0.0f;
            Matrix.UD = 0.0f;
            Matrix.VA = 0.0f;
            Matrix.VB = 1.0f;
            Matrix.VC = 0.0f;
            Matrix.UD = 0.0f;

            // Store the first 3 points of texture coordinates
            Params.U[0] = faceVertsIn[0].u;
            Params.U[1] = faceVertsIn[1].u + Params.U[0];
            Params.U[2] = faceVertsIn[2].u + Params.U[1];
            Params.V[0] = faceVertsIn[0].v;
            Params.V[1] = faceVertsIn[1].v + Params.V[0];
            Params.V[2] = faceVertsIn[2].v + Params.V[1];

            // Get and store the 1st point coordinates in face
            Params.X[0] = p0.x;
            Params.Y[0] = p0.y;
            Params.Z[0] = 0;

            // Get and store the 2nd point coordinates in face
            Params.X[1] = p1.x;
            Params.Y[1] = p1.y;
            Params.Z[1] = 0;

            // Get and store the 3rd point coordinates in face
            Params.X[2] = p2.x;
            Params.Y[2] = p2.y;
            Params.Z[2] = 0;

            // Compute the solution using an XY linear equation
            if (!l_ComputeDFUVMatrixXY(ref Matrix, ref Params))
                return false;

            // Assign matrix to all points if successful
            Int32 u = 0, v = 0;
            for (int point = 0; point < faceVertsIn.Length; point++)
            {
                if (point > 2)
                {
                    // Use generated matrix to calculate UV value from 2D point
                    DF2DPoint pn;
                    Vector3 PN = new Vector3(faceVertsIn[point].x, faceVertsIn[point].y, faceVertsIn[point].z);
                    pn.x = (Int32)PN.DotProduct(V0);
                    pn.y = (Int32)PN.DotProduct(V1);
                    u = (Int32)((pn.x * Matrix.UA) + (pn.y * Matrix.UB) + Matrix.UD);
                    v = (Int32)((pn.x * Matrix.VA) + (pn.y * Matrix.VB) + Matrix.VD);
                }
                else if (point == 0)
                {
                    // UV[0] is absolute
                    u = faceVertsIn[0].u;
                    v = faceVertsIn[0].v;
                }
                else if (point == 1)
                {
                    // UV[1] is a delta from UV[0]
                    u = faceVertsIn[0].u + faceVertsIn[1].u;
                    v = faceVertsIn[0].v + faceVertsIn[1].v;
                }
                else if (point == 2)
                {
                    // UV[2] is a delta from UV[1] + UV[0]
                    u = faceVertsIn[0].u + faceVertsIn[1].u + faceVertsIn[2].u;
                    v = faceVertsIn[0].v + faceVertsIn[1].v + faceVertsIn[2].v;
                }

                // Write outgoing point
                faceVertsOut[point].x = faceVertsIn[point].x;
                faceVertsOut[point].y = faceVertsIn[point].y;
                faceVertsOut[point].z = faceVertsIn[point].z;
                faceVertsOut[point].nx = faceVertsIn[point].nx;
                faceVertsOut[point].ny = faceVertsIn[point].ny;
                faceVertsOut[point].nz = faceVertsIn[point].nz;
                faceVertsOut[point].u = u;
                faceVertsOut[point].v = v;
            }

            return true;
        }

        #endregion

        #region Private Methods

        /*===========================================================================
        *
        * Local Function - boolean l_ComputeDFUVMatrixXY (Matrix, Params);
        *
        * Computes the UV conversion parameters from the given input based on
        * the formula:
        *			U = AX + BY + D
        *
        * Returns FALSE on any error.  For use on faces with 0 Z-coordinates.
        *
        *=========================================================================*/
        private bool l_ComputeDFUVMatrixXY(ref df3duvmatrix_t matrix, ref df3duvparams_lt param)
        {
            float determinant;
            float[] Xi = new float[3];
            float[] Yi = new float[3];
            float[] Zi = new float[3];

            /* Compute the determinant of the coefficient matrix */
            determinant = param.X[0] * param.Y[1] + param.Y[0] * param.X[2] +
            param.X[1] * param.Y[2] - param.Y[1] * param.X[2] -
            param.Y[0] * param.X[1] - param.X[0] * param.Y[2];

            /* Check for a singular matrix indicating no valid solution */
            if (determinant == 0)
            {
                return false;
            }

            /* Compute parameters of the the inverted XYZ matrix */
            Xi[0] = (param.Y[1] - param.Y[2]) / determinant;
            Xi[1] = (-param.X[1] + param.X[2]) / determinant;
            Xi[2] = (param.X[1] * param.Y[2] - param.X[2] * param.Y[1]) / determinant;

            Yi[0] = (-param.Y[0] + param.Y[2]) / determinant;
            Yi[1] = (param.X[0] - param.X[2]) / determinant;
            Yi[2] = (-param.X[0] * param.Y[2] + param.X[2] * param.Y[0]) / determinant;

            Zi[0] = (param.Y[0] - param.Y[1]) / determinant;
            Zi[1] = (-param.X[0] + param.X[1]) / determinant;
            Zi[2] = (param.X[0] * param.Y[1] - param.X[1] * param.Y[0]) / determinant;

            /* Compute the UV conversion parameters */
            matrix.UA = (param.U[0] * Xi[0] + param.U[1] * Yi[0] + param.U[2] * Zi[0]);
            matrix.UB = (param.U[0] * Xi[1] + param.U[1] * Yi[1] + param.U[2] * Zi[1]);
            matrix.UC = (float)0.0;
            matrix.UD = (param.U[0] * Xi[2] + param.U[1] * Yi[2] + param.U[2] * Zi[2]); ;

            matrix.VA = (param.V[0] * Xi[0] + param.V[1] * Yi[0] + param.V[2] * Zi[0]);
            matrix.VB = (param.V[0] * Xi[1] + param.V[1] * Yi[1] + param.V[2] * Zi[1]);
            matrix.VC = (float)0.0;
            matrix.VD = (param.V[0] * Xi[2] + param.V[1] * Yi[2] + param.V[2] * Zi[2]);

            return true;
        }
        /*===========================================================================
         *		End of Function l_ComputeDFUVMatrixXY()
         *=========================================================================*/

        #endregion
    }
}
