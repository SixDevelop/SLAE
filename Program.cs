using System;

namespace SLAE {
    class Program {
        public static void OutputVec(double[] vec, int size) {
            for (int j = 0; j < size; j++)
                Console.Out.Write(Math.Round(vec[j], 5) + " ");
            Console.Out.Write("\n" + "\n");
        }
        public static double[,] inputMatrix(int sizeRows,int sizeCols) {
            double[,] matrix = new double[sizeRows,sizeCols];
            string[] str;
            for (int j = 0; j < sizeRows; j++) {
                str = Console.ReadLine().Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < sizeCols; i++){
                    matrix[j, i] = Double.Parse(str[i]);
                }
            }
            return matrix;
        } 
        static void Main(string[] args) {
            #region For Square Matrix
            Random rand = new Random();
            int size = rand.Next(4, 7);
            Console.WriteLine("Size of matrix: {0}", size);
            double[,] arr = new double[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++){
                    if (i == j)
                        arr[i, j] = rand.Next(50, 100);
                    else
                        arr[i, j] = rand.Next(-49, 49);
                }

            Console.WriteLine("Matrix A:");
            Matrix.Output(arr, size, size);

            Matrix matrix = new Matrix(arr);

            #region LU decomposition
            matrix.LUDecompose();
            Console.WriteLine("Matrix L:");
            Matrix.Output(matrix.matL, size,size);
            Console.WriteLine("Matrix U:");
            Matrix.Output(matrix.matU, size,size);
            Console.WriteLine("L & U matrix product:");
            Matrix.Output(Matrix.Multiply(matrix.matL, matrix.matU, size,size,size), size,size);
            Console.WriteLine("P,A & Q matrix product:");
            Matrix.Output(Matrix.Multiply(Matrix.Multiply(matrix.matP, matrix._mat, size,size,size), matrix.matQ, size,size,size), size,size);
            #endregion

            #region determinant, rank and reverse matrix
            Console.WriteLine("Determinant of matrix A:");
            Console.WriteLine(matrix.GetDeterminant());
            Console.Out.Write("\n" + "\n");

            Console.WriteLine("Rank of matrix :");
            Console.WriteLine(matrix.GetRank());
            Console.Out.Write("\n" + "\n");

            Console.WriteLine("Reverse matrix:");
            double[,] reverseM = matrix.revMatrix;
            Matrix.Output(reverseM,size, size);

            Console.WriteLine("Simple & reverse matrix product:");
            double[,] mult = Matrix.Multiply(arr, matrix.revMatrix, size,size,size);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (mult[i, j] < matrix.eps)
                        mult[i, j] = 0;
            Matrix.Output(mult, size,size);


            Console.WriteLine("Reverse & simple matrix product:");
            mult = Matrix.Multiply(matrix.revMatrix, arr, size,size,size);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (mult[i, j] < matrix.eps)
                        mult[i, j] = 0;
            Matrix.Output(mult, size,size);

            #endregion

            #region solving the SLAE through LU decomposition and the condition number

            double[] b = new double[size];
            for(int i = 0; i < size; i++) {
                b[i] = rand.Next(0, 50);
            }

            Console.WriteLine("Vector b : Ax = b:");
            OutputVec(b,size);

            Console.WriteLine("Solving SLAU:");

            double[,] sol = matrix.Solution(b);
            for (int i = 0; i < sol.GetLength(0); i++) {   
                Console.WriteLine(" Vector {0}:",i + 1);
                for (int j = 0; j < sol.GetLength(1); j++)
                   Console.WriteLine(sol[i, j]);
            }
            Console.Out.Write("\n" + "\n");

            Console.WriteLine(" Check Ax - b = 0 :");
            Console.WriteLine(" Ax : ");

            double[] x = new double[size];
            for(int i = 0;i < size;i++)
                x[i] = sol[0,i];

            double[] check = Matrix.multiplyVec(arr, x, size);
            for (int i = 0; i < size; i++)
                Console.WriteLine(check[i]);
            bool result = true;
            for (int i = 0; i < size; i++){
                check[i] -= b[i];
                if (Math.Abs(check[i]) < 1e-8)
                    check[i] = 0;
            }
            for (int i = 0; i < size; i++)
                if (check[i] != 0)
                    result = false;
            if (result)
                Console.WriteLine("Everething is allright!");
            else Console.WriteLine("Not properly!");
            Console.Out.Write("\n" + "\n");

            // the condition number
            Console.WriteLine("Conditionality:");
            Console.WriteLine(matrix.condNumber);
            Console.Out.Write("\n" + "\n");

            #endregion

            #region method QR decomposition

            matrix.QRDecompose();
            Console.WriteLine("Matrix Q :");
            Matrix.Output(matrix.Q, size,size);
            Console.WriteLine("Matrix R :");
            Matrix.Output(matrix.R, size,size);
            Console.WriteLine("Q & R matrix product:");
            Matrix.Output(Matrix.Multiply(matrix.Q.Transpose(size), matrix.R, size,size,size), size,size);

            #endregion

            #region solving the SLAE through QR decomposition

            Console.WriteLine("Solving SLAU with QR:");
            double[] x1 = matrix.QRSolution(b);
            for (int i = 0; i < size; i++)
                Console.WriteLine(x1[i]);
            Console.Out.Write("\n" + "\n");

            #endregion

            #region methods Jacoby and Seildel 

            Console.WriteLine("Enter initial value (x0):");
            double[] x0 = new double[size];
            var mX = inputMatrix(1, size);
            for (int i = 0; i < size; i++)
                x0[i] = mX[0, i];
            Console.Out.Write("\n" + "\n");
            double[] copB = new double[size];
            for (int i = 0; i < size; i++)
                copB[i] = b[i];
            double[] solutionJ = matrix.methodJacoby(b, x0);
            Console.WriteLine("Jacoby solvation:");
            for (int i = 0; i < size; i++)
                Console.WriteLine(solutionJ[i]);
            Console.WriteLine("Number of iterations:");
            Console.WriteLine(matrix.jacobyInter);
            Console.Out.Write("\n" + "\n");

            double[] solutionS = matrix.methodSeidel(copB, x0);
            Console.WriteLine("Seidel solvation:");
            for (int i = 0; i < size; i++)
                Console.WriteLine(solutionS[i]);
            Console.WriteLine("Number of iterations:");
            Console.WriteLine(matrix.seidelIter);
            Console.Out.Write("\n" + "\n");




            Console.ReadLine();
            #endregion
            #endregion

            #region For a matrix of any dimension

            
            // Random rand = new Random();
            // int sizeRows = rand.Next(2,10);
            // int sizeCols = rand.Next(2,10);

            // double[,] arr2 = new double[sizeRows, sizeCols];

            // for (int i = 0; i < sizeRows; i++)
            //    for (int j = 0; j < sizeCols; j++)
            //        arr2[i, j] = rand.Next(-100, 100);




            // Matrix.Output(arr2,sizeRows,sizeCols);
            // Matrix mat1 = new Matrix(arr2);

            // mat1.LUDecomposition();
            // Console.WriteLine("Matrix L :");
            // Matrix.Output(mat1.matrixL, mat1.matrixL.GetLength(0), mat1.matrixL.GetLength(1));
            // Console.WriteLine("Matrix U :");
            // Matrix.Output(mat1.matrixU, mat1.matrixU.GetLength(0), mat1.matrixU.GetLength(1));
            // Console.WriteLine("L & U product:");
            // Matrix.Output(Matrix.Multiply(mat1.matrixL, mat1.matrixU, mat1.matrixL.GetLength(0)
            //                               , mat1.matrixL.GetLength(1), mat1.matrixU.GetLength(1)),
            //                               mat1.matrixL.GetLength(0), mat1.matrixU.GetLength(1));
            // Console.WriteLine("P,A & Q product:");
            // Matrix.Output(Matrix.Multiply(Matrix.Multiply(mat1.matrixP, mat1._matrix, mat1.matrixP.GetLength(0)
            //                                           , mat1.matrixP.GetLength(1), mat1._matrix.GetLength(1))
            //                                           , mat1.matrixQ, mat1.sizeRows, mat1.matrixQ.GetLength(0), mat1.matrixQ.GetLength(1)), mat1.sizeRows, mat1.sizeCols);

            // Console.WriteLine("Rank of matrix A :");
            // Console.WriteLine(mat1.GetRank());

            // double[] b3 = new double[sizeRows];
            // for(int i = 0;i < sizeRows;i++)
            //     b3[i] = rand.Next(0,100);
            // Console.WriteLine("Vector b : Ax = b :");
            // OutputVec(b3,sizeRows);

            // double[,] sol = mat1.Solution(b3);
            // Console.WriteLine("Sistem solvation:");
            // for (int i = 0; i < sol.GetLength(0); i++)
            //    for (int j = 0; j < sol.GetLength(1); j++)
            //        Console.WriteLine(sol[i, j]);

            // Console.ReadLine();
            #endregion

        }
    }
}
