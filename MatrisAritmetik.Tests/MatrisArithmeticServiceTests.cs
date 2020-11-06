using System;
using System.Collections.Generic;
using System.Text;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Services;
using MatrisAritmetik.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MatrisAritmetik.Tests
{
    [TestClass]
    public class MatrisArithmeticServiceTests
    {

        /// <summary>
        /// Service for matrix algebric-arithmetic and special matrices
        /// </summary>
        private readonly IMatrisArithmeticService<dynamic> matrisArithmeticService = new MatrisArithmeticService<dynamic>();
        private readonly ISpecialMatricesService specialMatricesService = new SpecialMatricesService();

        /// <summary>
        /// Example matrices to use for tests
        /// A = { {1, 2}, {3, 4} }
        /// B = { {1, -1}, {-1, 1} }
        /// </summary>
        public MatrisBase<dynamic> A =
            new MatrisBase<dynamic>(new List<List<dynamic>>()
                {
                    new List<dynamic>(){ 1, 2 },
                    new List<dynamic>(){ 3, 4 }
                }
            );
        public MatrisBase<dynamic> B =
            new MatrisBase<dynamic>(new List<List<dynamic>>()
                {
                    new List<dynamic>(){ 1, -1 },
                    new List<dynamic>(){ -1, 1 }
                }
            );

        [TestMethod]
        public void MatrisMul()
        {
            MatrisBase<dynamic> matmul_A_B = 
                new MatrisBase<dynamic>(new List<List<dynamic>>()
                {
                    new List<dynamic>(){ -1, 1 },
                    new List<dynamic>(){ -1, 1 }
                }
            );
            MatrisBase<dynamic> matmul_B_A =
                new MatrisBase<dynamic>(new List<List<dynamic>>()
                {
                    new List<dynamic>(){ -2, -2 },
                    new List<dynamic>(){ 2, 2 }
                }
            );

            MatrisBase<dynamic> matmul_A_B_Result = matrisArithmeticService.MatrisMul(A, B);
            Assert.AreEqual(matmul_A_B_Result.ToString(),
                            matmul_A_B.ToString(),
                            "\nMatris çarpımı hatalı! \nBeklenen:\n" + matmul_A_B.ToString() + "\nAlınan:\n" + matmul_A_B_Result.ToString());

            MatrisBase<dynamic> matmul_B_A_Result = matrisArithmeticService.MatrisMul(B, A);
            Assert.AreEqual(matmul_B_A_Result.ToString(),
                            matmul_B_A.ToString(),
                            "\nMatris çarpımı hatalı! \nBeklenen:\n" + matmul_B_A.ToString() + "\nAlınan:\n" + matmul_B_A_Result.ToString());

            MatrisBase<dynamic> matmul_A_ID = matrisArithmeticService.MatrisMul(A, specialMatricesService.Identity(2));
            Assert.AreEqual(A.ToString(),
                            matmul_A_ID.ToString(),
                            "\nMatris çarpımı hatalı! \nBeklenen:\n" + A.ToString() + "\nAlınan:\n" + matmul_A_ID.ToString());
           
            MatrisBase<dynamic> matmul_ID_A = matrisArithmeticService.MatrisMul(specialMatricesService.Identity(2),A);
            Assert.AreEqual(A.ToString(),
                            matmul_ID_A.ToString(),
                            "\nMatris çarpımı hatalı! \nBeklenen:\n" + A.ToString() + "\nAlınan:\n" + matmul_ID_A.ToString());

        }

        [TestMethod]
        public void Transpose()
        {
            MatrisBase<dynamic> A_T =
                new MatrisBase<dynamic>(new List<List<dynamic>>()
                {
                    new List<dynamic>(){ 1, 3 },
                    new List<dynamic>(){ 2, 4 }
                }
            );
            MatrisBase<dynamic> B_T =
                new MatrisBase<dynamic>(new List<List<dynamic>>()
                {
                    new List<dynamic>(){ 1, -1 },
                    new List<dynamic>(){ -1, 1 }
                }
            );

            MatrisBase<dynamic> transpose_A = matrisArithmeticService.Transpose(A);
            Assert.AreEqual(transpose_A.ToString(),
                            A_T.ToString(),
                            "\nMatris transpozu hatalı! \nBeklenen:\n" + A_T.ToString() + "\nAlınan:\n" + transpose_A.ToString());

            MatrisBase<dynamic> transpose_B = matrisArithmeticService.Transpose(B);
            Assert.AreEqual(transpose_B.ToString(),
                            B_T.ToString(),
                            "\nMatris transpozu hatalı! \nBeklenen:\n" + B_T.ToString() + "\nAlınan:\n" + transpose_B.ToString());

        }
    }
}
