using System;

namespace VIPSLib.Maths
{
	public class Matrix
	{
		private double[][] A;
		private int m;
		private int n;
		

		/** Construct an m-by-n matrix of zeros.
	   @param m    Number of rows.
	   @param n    Number of colums.
		 */

		public Matrix (int m, int n) {
			this.m = m;
			this.n = n;
			A = new double[m][];
			for (int i=0;i<m;i++)
				A[i] = new double[n];
		}

		/** Construct an m-by-n constant matrix.
	   @param m    Number of rows.
	   @param n    Number of colums.
	   @param s    Fill the matrix with this scalar value.
		 */

		public Matrix (int m, int n, double s) {
			this.m = m;
			this.n = n;
			A = new double[m][];
			for (int i = 0; i < m; i++)
			{
				A[i] = new double[n];
				for (int j = 0; j < n; j++) {
					A[i][j] = s;
				}
			}
		}

		/** Construct a matrix from a 2-D array.
	   @param A    Two-dimensional array of doubles.
	   @exception  Exception All rows must have the same length
	   @see        #constructWithCopy
		 */

		public Matrix (double[][] A) {
			m = A.Length;
			n = A[0].Length;
			for (int i = 0; i < m; i++) {
				if (A[i].Length != n) {
					throw new Exception("All rows must have the same length.");
				}
			}
			this.A = A;
		}

		/** Construct a matrix quickly without checking arguments.
	   @param A    Two-dimensional array of doubles.
	   @param m    Number of rows.
	   @param n    Number of colums.
		 */

		public Matrix (double[][] A, int m, int n) {
			this.A = A;
			this.m = m;
			this.n = n;
		}

		/** Construct a matrix from a one-dimensional packed array
	   @param vals One-dimensional array of doubles, packed by columns (ala Fortran).
	   @param m    Number of rows.
	   @exception  Exception Array length must be a multiple of m.
		 */

		public Matrix (double[] vals, int m) {
			this.m = m;
			n = (m != 0 ? vals.Length/m : 0);
			if (m*n != vals.Length) {
				throw new Exception("Array length must be a multiple of m.");
			}
			A = new double[m][];
			for (int i = 0; i < m; i++)
			{
				A[i] = new double[n];
				for (int j = 0; j < n; j++) {
					A[i][j] = vals[i+j*m];
				}
			}
		}
		
		
		public static Matrix ConstructWithCopy(double[][] A) {
			int m = A.Length;
			int n = A[0].Length;
			Matrix X = new Matrix(m,n);
			double[][] C = X.GetArray();
			for (int i = 0; i < m; i++) {
				if (A[i].Length != n) {
					throw new Exception
						("All rows must have the same length.");
				}
				for (int j = 0; j < n; j++) {
					C[i][j] = A[i][j];
				}
			}
			return X;
		}

		/** Make a deep copy of a matrix
		 */

		public Matrix Copy () {
			Matrix X = new Matrix(m,n);
			double[][] C = X.GetArray();
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					C[i][j] = A[i][j];
				}
			}
			return X;
		}

		/** Clone the Matrix object.
		 */

		public Object Clone () {
			return this.Copy();
		}

		/** Access the internal two-dimensional array.
   		@return     Pointer to the two-dimensional array of matrix elements.
		 */

		public double[][] GetArray () {
			return A;
		}

		/** Copy the internal two-dimensional array.
  		 @return     Two-dimensional array copy of matrix elements.
		 */

		public double[][] GetArrayCopy () {
			double[][] C = new double[m][];
			for (int i = 0; i < m; i++)
			{
				C[i] = new double[n];
				for (int j = 0; j < n; j++) {
					C[i][j] = A[i][j];
				}
			}
			return C;
		}

		/** Make a one-dimensional column packed copy of the internal array.
   		@return     Matrix elements packed in a one-dimensional array by columns.
		 */

		public double[] GetColumnPackedCopy () {
			double[] vals = new double[m*n];
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					vals[i+j*m] = A[i][j];
				}
			}
			return vals;
		}

		/** Make a one-dimensional row packed copy of the internal array.
   		@return     Matrix elements packed in a one-dimensional array by rows.
		 */

		public double[] GetRowPackedCopy () {
			double[] vals = new double[m*n];
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					vals[i*n+j] = A[i][j];
				}
			}
			return vals;
		}

		/** Get row dimension.
   		@return     m, the number of rows.
		 */

		public int GetRowDimension () {
			return m;
		}

		/** Get column dimension.
   		@return     n, the number of columns.
		 */

		public int GetColumnDimension () {
			return n;
		}

		/** Get a single element.
	   @param i    Row index.
	   @param j    Column index.
	   @return     A(i,j)
	   @exception  Exception
		 */

		public double[] this[int i]
		{
			get { return A[i]; }
			set { A[i] = value; }
		}
		
		public double Get (int i, int j) {
			return A[i][j];
		}

		/** Get a submatrix.
	   @param i0   Initial row index
	   @param i1   Final row index
	   @param j0   Initial column index
	   @param j1   Final column index
	   @return     A(i0:i1,j0:j1)
	   @exception  Exception Submatrix indices
		 */

		public Matrix GetMatrix (int i0, int i1, int j0, int j1) {
			Matrix X = new Matrix(i1-i0+1,j1-j0+1);
			double[][] B = X.GetArray();
			try {
				for (int i = i0; i <= i1; i++) {
					for (int j = j0; j <= j1; j++) {
						B[i-i0][j-j0] = A[i][j];
					}
				}
			} catch(Exception e) {
				throw new Exception("Submatrix indices");
			}
			return X;
		}

		/** Get a submatrix.
	   @param r    Array of row indices.
	   @param c    Array of column indices.
	   @return     A(r(:),c(:))
	   @exception  Exception Submatrix indices
		 */

		public Matrix GetMatrix (int[] r, int[] c) {
			Matrix X = new Matrix(r.Length,c.Length);
			double[][] B = X.GetArray();
			try {
				for (int i = 0; i < r.Length; i++) {
					for (int j = 0; j < c.Length; j++) {
						B[i][j] = A[r[i]][c[j]];
					}
				}
			} catch(Exception e) {
				throw new Exception("Submatrix indices");
			}
			return X;
		}

		/** Get a submatrix.
	   @param i0   Initial row index
	   @param i1   Final row index
	   @param c    Array of column indices.
	   @return     A(i0:i1,c(:))
	   @exception  Exception Submatrix indices
		 */

		public Matrix GetMatrix (int i0, int i1, int[] c) {
			Matrix X = new Matrix(i1-i0+1,c.Length);
			double[][] B = X.GetArray();
			try {
				for (int i = i0; i <= i1; i++) {
					for (int j = 0; j < c.Length; j++) {
						B[i-i0][j] = A[i][c[j]];
					}
				}
			} catch(Exception e) {
				throw new Exception("Submatrix indices");
			}
			return X;
		}

		/** Get a submatrix.
	   @param r    Array of row indices.
	   @param j0   Initial column index
	   @param j1   Final column index
	   @return     A(r(:),j0:j1)
	   @exception  Exception Submatrix indices
		 */

		public Matrix GetMatrix (int[] r, int j0, int j1) {
			Matrix X = new Matrix(r.Length,j1-j0+1);
			double[][] B = X.GetArray();
			try {
				for (int i = 0; i < r.Length; i++) {
					for (int j = j0; j <= j1; j++) {
						B[i][j-j0] = A[r[i]][j];
					}
				}
			} catch(Exception e) {
				throw new Exception("Submatrix indices");
			}
			return X;
		}

		/** Set a single element.
	   @param i    Row index.
	   @param j    Column index.
	   @param s    A(i,j).
	   @exception  Exception
		 */

		public void Set (int i, int j, double s) {
			A[i][j] = s;
		}

		/** Set a submatrix.
	   @param i0   Initial row index
	   @param i1   Final row index
	   @param j0   Initial column index
	   @param j1   Final column index
	   @param X    A(i0:i1,j0:j1)
	   @exception  Exception Submatrix indices
		 */

		public void SetMatrix (int i0, int i1, int j0, int j1, Matrix X) {
			try {
				for (int i = i0; i <= i1; i++) {
					for (int j = j0; j <= j1; j++) {
						A[i][j] = X.Get(i-i0,j-j0);
					}
				}
			} catch(Exception e) {
				throw new Exception("Submatrix indices");
			}
		}

		/** Set a submatrix.
	   @param r    Array of row indices.
	   @param c    Array of column indices.
	   @param X    A(r(:),c(:))
	   @exception  Exception Submatrix indices
		 */

		public void SetMatrix (int[] r, int[] c, Matrix X) {
			try {
				for (int i = 0; i < r.Length; i++) {
					for (int j = 0; j < c.Length; j++) {
						A[r[i]][c[j]] = X.Get(i,j);
					}
				}
			} catch(Exception e) {
				throw new Exception("Submatrix indices");
			}
		}

		/** Set a submatrix.
	   @param r    Array of row indices.
	   @param j0   Initial column index
	   @param j1   Final column index
	   @param X    A(r(:),j0:j1)
	   @exception  Exception Submatrix indices
		 */

		public void SetMatrix (int[] r, int j0, int j1, Matrix X) {
			try {
				for (int i = 0; i < r.Length; i++) {
					for (int j = j0; j <= j1; j++) {
						A[r[i]][j] = X.Get(i,j-j0);
					}
				}
			} catch(Exception e) {
				throw new Exception("Submatrix indices");
			}
		}

		/** Set a submatrix.
	   @param i0   Initial row index
	   @param i1   Final row index
	   @param c    Array of column indices.
	   @param X    A(i0:i1,c(:))
	   @exception  Exception Submatrix indices
		 */

		public void SetMatrix (int i0, int i1, int[] c, Matrix X) {
			try {
				for (int i = i0; i <= i1; i++) {
					for (int j = 0; j < c.Length; j++) {
						A[i][c[j]] = X.Get(i-i0,j);
					}
				}
			} catch(Exception e) {
				throw new Exception("Submatrix indices");
			}
		}

		/** Matrix transpose.
   		@return    A'
		 */

		public Matrix Transpose () {
			Matrix X = new Matrix(n,m);
			double[][] C = X.GetArray();
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					C[j][i] = A[i][j];
				}
			}
			return X;
		}

		/** One norm
   		@return    maximum column sum.
		 */

		public double Norm1 () {
			double f = 0;
			for (int j = 0; j < n; j++) {
				double s = 0;
				for (int i = 0; i < m; i++) {
					s += Math.Abs(A[i][j]);
				}
				f = Math.Max(f,s);
			}
			return f;
		}

		/** Two norm
   		@return    maximum singular value.
		 */

		public double Norm2 () {
			throw new Exception("Not implement yet");
		}

		/** Infinity norm
   		@return    maximum row sum.
		 */

		public double NormInf () {
			double f = 0;
			for (int i = 0; i < m; i++) {
				double s = 0;
				for (int j = 0; j < n; j++) {
					s += Math.Abs(A[i][j]);
				}
				f = Math.Max(f,s);
			}
			return f;
		}

		/** Frobenius norm
   		@return    sqrt of sum of squares of all elements.
		 */

		public double NormF () {
			double f = 0;
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					f = Hypot(f,A[i][j]);
				}
			}
			return f;
		}

		/**  Unary minus
  		 @return    -A
		 */

		public Matrix Uminus () {
			Matrix X = new Matrix(m,n);
			double[][] C = X.GetArray();
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					C[i][j] = -A[i][j];
				}
			}
			return X;
		}

		/** C = A + B
	   @param B    another matrix
	   @return     A + B
		 */

		public Matrix Plus (Matrix B) {
			CheckMatrixDimensions(B);
			Matrix X = new Matrix(m,n);
			double[][] C = X.GetArray();
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					C[i][j] = A[i][j] + B.A[i][j];
				}
			}
			return X;
		}

		/** A = A + B
	   @param B    another matrix
	   @return     A + B
		 */

		public Matrix PlusEquals (Matrix B) {
			CheckMatrixDimensions(B);
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					A[i][j] = A[i][j] + B.A[i][j];
				}
			}
			return this;
		}

		/** C = A - B
	   @param B    another matrix
	   @return     A - B
		 */

		public Matrix Minus (Matrix B) {
			CheckMatrixDimensions(B);
			Matrix X = new Matrix(m,n);
			double[][] C = X.GetArray();
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					C[i][j] = A[i][j] - B.A[i][j];
				}
			}
			return X;
		}

		/** A = A - B
	   @param B    another matrix
	   @return     A - B
		 */

		public Matrix MinusEquals (Matrix B) {
			CheckMatrixDimensions(B);
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					A[i][j] = A[i][j] - B.A[i][j];
				}
			}
			return this;
		}

		/** Element-by-element multiplication, C = A.*B
	   @param B    another matrix
	   @return     A.*B
		 */

		public Matrix ArrayTimes (Matrix B) {
			CheckMatrixDimensions(B);
			Matrix X = new Matrix(m,n);
			double[][] C = X.GetArray();
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					C[i][j] = A[i][j] * B.A[i][j];
				}
			}
			return X;
		}

		/** Element-by-element multiplication in place, A = A.*B
	   @param B    another matrix
	   @return     A.*B
		 */

		public Matrix ArrayTimesEquals (Matrix B) {
			CheckMatrixDimensions(B);
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					A[i][j] = A[i][j] * B.A[i][j];
				}
			}
			return this;
		}

		/** Element-by-element right division, C = A./B
	   @param B    another matrix
	   @return     A./B
		 */

		public Matrix ArrayRightDivide (Matrix B) {
			CheckMatrixDimensions(B);
			Matrix X = new Matrix(m,n);
			double[][] C = X.GetArray();
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					C[i][j] = A[i][j] / B.A[i][j];
				}
			}
			return X;
		}

		/** Element-by-element right division in place, A = A./B
	   @param B    another matrix
	   @return     A./B
		 */

		public Matrix ArrayRightDivideEquals (Matrix B) {
			CheckMatrixDimensions(B);
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					A[i][j] = A[i][j] / B.A[i][j];
				}
			}
			return this;
		}

		/** Element-by-element left division, C = A.\B
	   @param B    another matrix
	   @return     A.\B
		 */

		public Matrix ArrayLeftDivide (Matrix B) {
			CheckMatrixDimensions(B);
			Matrix X = new Matrix(m,n);
			double[][] C = X.GetArray();
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					C[i][j] = B.A[i][j] / A[i][j];
				}
			}
			return X;
		}

		/** Element-by-element left division in place, A = A.\B
	   @param B    another matrix
	   @return     A.\B
		 */

		public Matrix ArrayLeftDivideEquals (Matrix B) {
			CheckMatrixDimensions(B);
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					A[i][j] = B.A[i][j] / A[i][j];
				}
			}
			return this;
		}

		/** Multiply a matrix by a scalar, C = s*A
	   @param s    scalar
	   @return     s*A
		 */

		public Matrix Times (double s) {
			Matrix X = new Matrix(m,n);
			double[][] C = X.GetArray();
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					C[i][j] = s*A[i][j];
				}
			}
			return X;
		}

		/** Multiply a matrix by a scalar in place, A = s*A
	   @param s    scalar
	   @return     replace A by s*A
		 */

		public Matrix TimesEquals (double s) {
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					A[i][j] = s*A[i][j];
				}
			}
			return this;
		}

		/** Linear algebraic matrix multiplication, A * B
	   @param B    another matrix
	   @return     Matrix product, A * B
	   @exception  Exception Matrix inner dimensions must agree.
		 */

		public Matrix Times (Matrix B) {
			if (B.m != n) {
				throw new Exception("Matrix inner dimensions must agree.");
			}
			Matrix X = new Matrix(m,B.n);
			double[][] C = X.GetArray();
			double[] Bcolj = new double[n];
			for (int j = 0; j < B.n; j++) {
				for (int k = 0; k < n; k++) {
					Bcolj[k] = B.A[k][j];
				}
				for (int i = 0; i < m; i++) {
					double[] Arowi = A[i];
					double s = 0;
					for (int k = 0; k < n; k++) {
						s += Arowi[k]*Bcolj[k];
					}
					C[i][j] = s;
				}
			}
			return X;
		}


		/**
		 * Linear algebraic matrix multiplication, A * B
		 * B being a triangular matrix
		 * <b>Note:</b>
		 * Actually the matrix should be a <b>column orienten, upper triangular
		 * matrix</b> but use the <b>row oriented, lower triangular matrix</b>
		 * instead (transposed), because this is faster due to the easyer array
		 * access.
		 *
		 * @param B    another matrix
		 * @return     Matrix product, A * B
		 *
		 * @exception  Exception Matrix inner dimensions must agree.
		 */
		public Matrix TimesTriangular (Matrix B)
		{
			if (B.m != n)
				throw new Exception("Matrix inner dimensions must agree.");

			Matrix X = new Matrix(m,B.n);
			double[][] c = X.GetArray();
			double[][] b;
			double s = 0;
			double[] Arowi;
			double[] Browj;

			b = B.GetArray();
			//multiply with each row of A
			for (int i = 0; i < m; i++)
			{
				Arowi = A[i];

				//for all columns of B
				for (int j = 0; j < B.n; j++)
				{
					s = 0;
					Browj = b[j];
					//since B being triangular, this loop uses k <= j
					for (int k = 0; k <= j; k++)
					{
						s += Arowi[k] * Browj[k];
					}
					c[i][j] = s;
				}
			}
			return X;
		}


		/**
		 * X.diffEquals() calculates differences between adjacent columns of this
		 * matrix. Consequently the size of the matrix is reduced by one. The result
		 * is stored in this matrix object again.
		 */
		public void DiffEquals()
		{
			double[] col = null;
			for(int i = 0; i < A.Length; i++)
			{
				col = new double[A[i].Length - 1];

				for(int j = 1; j < A[i].Length; j++)
					col[j-1] = Math.Abs(A[i][j] - A[i][j - 1]);

				A[i] = col;
			}
			n--;
		}

		/**
		 * X.logEquals() calculates the natural logarithem of each element of the
		 * matrix. The result is stored in this matrix object again.
		 */
		public void LogEquals()
		{
			for(int i = 0; i < A.Length; i++)
				for(int j = 0; j < A[i].Length; j++)
					A[i][j] = Math.Log(A[i][j]);
		}

		/**
		 * X.powEquals() calculates the power of each element of the matrix. The
		 * result is stored in this matrix object again.
		 */
		public void PowEquals(double exp)
		{
			for(int i = 0; i < A.Length; i++)
				for(int j = 0; j < A[i].Length; j++)
					A[i][j] = Math.Pow(A[i][j], exp);
		}

		/**
		 * X.powEquals() calculates the power of each element of the matrix.
		 *
		 * @return Matrix
		 */
		public Matrix Pow(double exp)
		{
			Matrix X = new Matrix(m,n);
			double[][] C = X.GetArray();

			for (int i = 0; i < m; i++)
				for (int j = 0; j < n; j++)
					C[i][j] = Math.Pow(A[i][j], exp);

			return X;
		}





		/**
		 * X.thrunkAtLowerBoundariy(). All values smaller than the given one are set
		 * to this lower boundary.
		 */
		public void ThrunkAtLowerBoundary(double value)
		{
			for(int i = 0; i < A.Length; i++)
				for(int j = 0; j < A[i].Length; j++)
			{
				if(A[i][j] < value)
					A[i][j] = value;
			}
		}

		/** Matrix trace.
  		 @return     sum of the diagonal elements.
		 */

		public double Trace () {
			double t = 0;
			for (int i = 0; i < Math.Min(m,n); i++) {
				t += A[i][i];
			}
			return t;
		}

		/** Generate matrix with random elements
	   @param m    Number of rows.
	   @param n    Number of colums.
	   @return     An m-by-n matrix with uniformly distributed random elements.
		 */

		public static Matrix Random (int m, int n) {
			Random rand = new Random();
			Matrix A = new Matrix(m,n);
			double[][] X = A.GetArray();
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					X[i][j] = rand.NextDouble();
				}
			}
			return A;
		}

		/** Generate identity matrix
	   @param m    Number of rows.
	   @param n    Number of colums.
	   @return     An m-by-n matrix with ones on the diagonal and zeros elsewhere.
		 */

		public static Matrix Identity (int m, int n) {
			Matrix A = new Matrix(m,n);
			double[][] X = A.GetArray();
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					X[i][j] = (i == j ? 1.0 : 0.0);
				}
			}
			return A;
		}
		/** Check if size(A) == size(B) **/

		private void CheckMatrixDimensions (Matrix B) {
			if (B.m != m || B.n != n) {
				throw new Exception("Matrix dimensions must agree.");
			}
		}
		/**
		 * Returns the mean values along the specified dimension.
		 *
		 * @param dim
		 *            If 1, then the mean of each column is returned in a row
		 *            vector. If 2, then the mean of each row is returned in a
		 *            column vector.
		 * @return A vector containing the mean values along the specified
		 *         dimension.
		 */
		public Matrix Mean(int dim) {
			Matrix result;
			switch (dim) {
				case 1:
					result = new Matrix(1, n);
					for (int currN = 0; currN < n; currN++) {
						for (int currM = 0; currM < m; currM++)
							result.A[0][currN] += A[currM][currN];
						result.A[0][currN] /= m;
					}
					return result;
				case 2:
					result = new Matrix(m, 1);
					for (int currM = 0; currM < m; currM++) {
						for (int currN = 0; currN < n; currN++) {
							result.A[currM][0] += A[currM][currN];
						}
						result.A[currM][0] /= n;
					}
					return result;
				default:
					throw new Exception("dim must be either 1 or 2, and not: " + dim);
					return null;
			}
		}

		/**
		 * Calculate the full covariance matrix.
		 * @return the covariance matrix
		 */
		public Matrix Cov() {
			Matrix transe = this.Transpose();
			Matrix result = new Matrix(transe.m, transe.m);
			for(int currM = 0; currM < transe.m; currM++){
				for(int currN = currM; currN < transe.m; currN++){
					double covMN = Cov(transe.A[currM], transe.A[currN]);
					result.A[currM][currN] = covMN;
					result.A[currN][currM] = covMN;
				}
			}
			return result;
		}

		/**
		 * Calculate the covariance between the two vectors.
		 * @param vec1
		 * @param vec2
		 * @return the covariance between the two vectors.
		 */
		private double Cov(double[] vec1, double[] vec2){
			double result = 0;
			int dim = vec1.Length;
			if(vec2.Length != dim)
				throw new Exception("vectors are not of same length");
			double meanVec1 = Mean(vec1), meanVec2 = Mean(vec2);
			for(int i=0; i<dim; i++){
				result += (vec1[i]-meanVec1)*(vec2[i]-meanVec2);
			}
			return result / Math.Max(1, dim-1);
//		int dim = vec1.Length;
//		if(vec2.Length != dim)
//			(new Exception("vectors are not of same length")).printStackTrace();
//		double[] times = new double[dim];
//		for(int i=0; i<dim; i++)
//			times[i] += vec1[i]*vec2[i];
//		return mean(times) - mean(vec1)*mean(vec2);
		}

		/**
		 * the mean of the values in the double array
		 * @param vec	double values
		 * @return	the mean of the values in vec
		 */
		private double Mean(double[] vec){
			double result = 0;
			for(int i=0; i<vec.Length; i++)
				result += vec[i];
			return result / vec.Length;
		}
		
		/**
		 * Returns the sum of the component of the matrix.
		 * @return the sum
		 */
		public double Sum(){
			double result = 0;
			foreach(double[] dArr in A)
				foreach(double d in dArr)
					result += d;
			return result;
		}

		/**
		 * returns a new Matrix object, where each value is set to the absolute value
		 * @return a new Matrix with all values being positive
		 */
		public Matrix Abs() {
			Matrix result = new Matrix(m, n); // don't use clone(), as the values are assigned in the loop.
			for(int i=0; i<result.A.Length; i++){
				for(int j=0; j<result.A[i].Length; j++)
					result.A[i][j] = Math.Abs(A[i][j]);
			}
			return result;
		}
		
		/** sqrt(a^2 + b^2) without under/overflow. **/
		public static double Hypot(double a, double b)
		{
			double r;
			if (Math.Abs(a) > Math.Abs(b)) {
				r = b/a;
				r = Math.Abs(a)*Math.Sqrt(1+r*r);
			} else if (b != 0) {
				r = a/b;
				r = Math.Abs(b)*Math.Sqrt(1+r*r);
			} else {
				r = 0.0;
			}
			return r;
		}
	}
}
