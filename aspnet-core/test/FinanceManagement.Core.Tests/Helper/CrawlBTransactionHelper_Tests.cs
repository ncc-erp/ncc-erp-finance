using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FinanceManagement.Core.Tests.Helper
{
    public class CrawlBTransactionHelper_Tests
    {

         [Fact]
        public void ExtractBTransaction_USD()
        {

            // Arrange
            string input = @"TK 18123456789026 So tien GD:+USD 1,123.00 So du:USD 1,107,321.16 FT56543456788888 LT45454545544 1/xxxOGB2LXXX. NGAN HANG NUOC NGOAI THU PHI USD0 ..";

            // Act
            var result = CrawlBTransactionHelper.ExtractBTransaction(input);

          
            // Assert
            Assert.Equal("18123456789026", result.AccountNumber);
            Assert.Equal(1123, result.TransactionAmount);
            Assert.Equal(1107321.16, result.Balance);

        }
        
        [Fact]
        public void ExtractBTransaction_Should_Parse_Transaction_Amount_With_Decimal()
        {

            // Arrange
            string input = @"TK 19132608123456\nSo tien GD:+10,951.96\nSo du:2,527.39\n0202000433000 UNIVERSAL COMMUNIC / //URI/Invoice 85.. NGAN HANG NUOC N GOAI THU PHI USD15.00 ..";

            // Act
            var result = CrawlBTransactionHelper.ExtractBTransaction(input);

            // Assert
            Assert.Equal("19132608123456", result.AccountNumber);
            Assert.Equal(10951.96, result.TransactionAmount);
            Assert.Equal(2527.39, result.Balance);

            input = @"TK 19034753333339\nSo tien GD:-10,000,000\nSo du:644,706,229\ntam ung mua vat tu dien";
            // Act
            result = CrawlBTransactionHelper.ExtractBTransaction(input);

            // Assert
            Assert.Equal("19034753333339", result.AccountNumber);
            Assert.Equal(-10000000, result.TransactionAmount);
            Assert.Equal(644706229, result.Balance);


            input = @"TK 19132608555555\nSo tien GD:+353,182\nSo du:378,680,952\n(B/O NGAN HANG TMCP QUAN DOI) HOAN TRA LCC 10005950NGAY 08092025 LY DOSAI TK DV THU HUONG FT2525170330";
            // Act
            result = CrawlBTransactionHelper.ExtractBTransaction(input);

            // Assert
            Assert.Equal("19132608555555", result.AccountNumber);
            Assert.Equal(353182, result.TransactionAmount);
            Assert.Equal(378680952, result.Balance);


            input = @"TK 19132608555555\nSo tien GD:+2,639,500,000\nSo du:3,650,789,407\n1742365796702Y06C6DL NH MUA CUA KH KHACH HANG 32608283 100000 USD TG 26395 Tieu dung trong cong ty";
            // Act
            result = CrawlBTransactionHelper.ExtractBTransaction(input);

            // Assert
            Assert.Equal("19132608555555", result.AccountNumber);
            Assert.Equal(2639500000, result.TransactionAmount);
            Assert.Equal(3650789407, result.Balance);

            input = @"- VND 10,000,000\nTai khoan Thanh toan: 19034753333339\nSo du: VND 942,390,864\nncc ck. Ma tham chieu 970425";
            result = CrawlBTransactionHelper.ExtractBTransaction(input);

            // Assert
            Assert.Equal("19034753333339", result.AccountNumber);
            Assert.Equal(-10000000, result.TransactionAmount);
            Assert.Equal(942390864, result.Balance);

            input = @"+ VND 70,592,223\nTai khoan Thanh toan: 19034753333339\nSo du: VND 955,190,940\nDUONG vC dcm 7 2025";
            result = CrawlBTransactionHelper.ExtractBTransaction(input);

            // Assert
            Assert.Equal("19034753333339", result.AccountNumber);
            Assert.Equal(70592223, result.TransactionAmount);
            Assert.Equal(955190940, result.Balance);
        }
    }
}
