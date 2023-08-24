using FinanceManagement.Configuration;
using FinanceManagement.Helper;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace FinanceManagement.Core.Tests.Helper
{
    public class HelperDetection_Tests : FinfastCoreTestBase
    {
        public HelperDetection_Tests() { }
        [Fact]
        public void DetectionTransactionMoney_Test()
        {
            var regex = new Regex(AppSettingProvider.RegexMoneyDetectionValue);
            var messages = new List<string>
            {
                "TK 19132608283018 So tien GD:-3,286,802 So du:112,765,887 NCCPLUS thanh toan tien dien tu ngay 13.612.07",
                "TK 19132608283018 So tien GD:+209,725,000 So du:1,314,134,841 (B/O CT CP QSOFT VN) IBBIZ.50870010070960280723.5.Qsoft chuyen tien cho CONG TY CO PHAN NCCPLUS VIET NAM",
                "TK 19132608283026 So tien GD:+11,762.50 So du:2,089,129.16 12705464 CORE DEVTEAM LTD 235 SEVEN MILE STRAIGHT NUTTS CORN CRUMLIN B T29 4YS GB.OTHR/DEV TEAM INV 40 /RO C/617168392 BNY CUST RRN  PET67596 7186./ACC//INS/BARCGB22XXX. NGAN HA NG NUOC NGOAI THU PHI",
                "TK 19132608283026 So tien GD:-30,000 So du:1,077,366.66 1688438734209JW2GeeE NH MUA CUA KH KHACH HANG 32608283 30000 USD TG 23720 9. Noi dung khac Tieu dung noi bo",
                "TK 19034753904029 So tien GD:-6,000,000 So du:443,105,757 bonus tuyen dung",
                "TK 19034753904029 So tien GD:+3,000,000 So du:174,545,590 thao nguyenthanhncc asia"
            };
            foreach(var message in messages)
            {
                var output = Helpers.DetectionMoney(regex, message);
                output.IsValid.ShouldBeTrue();
            }
        }

        [Fact]
        public void DetectionRemainMoney_Test()
        {
            var regex = new Regex(AppSettingProvider.RegexRemainMoneyDetectionValue);
            var messages = new List<string>
            {
                "TK 19132608283018 So tien GD:-3,286,802 So du:112,765,887 NCCPLUS thanh toan tien dien tu ngay 13.612.07",
                "TK 19132608283018 So tien GD:+209,725,000 So du:1,314,134,841 (B/O CT CP QSOFT VN) IBBIZ.50870010070960280723.5.Qsoft chuyen tien cho CONG TY CO PHAN NCCPLUS VIET NAM",
                "TK 19132608283026 So tien GD:+11,762.50 So du:2,089,129.16 12705464 CORE DEVTEAM LTD 235 SEVEN MILE STRAIGHT NUTTS CORN CRUMLIN B T29 4YS GB.OTHR/DEV TEAM INV 40 /RO C/617168392 BNY CUST RRN  PET67596 7186./ACC//INS/BARCGB22XXX. NGAN HA NG NUOC NGOAI THU PHI",
                "TK 19132608283026 So tien GD:-30,000 So du:1,077,366.66 1688438734209JW2GeeE NH MUA CUA KH KHACH HANG 32608283 30000 USD TG 23720 9. Noi dung khac Tieu dung noi bo",
                "TK 19034753904029 So tien GD:-6,000,000 So du:443,105,757 bonus tuyen dung",
                "TK 19034753904029 So tien GD:+3,000,000 So du:174,545,590 thao nguyenthanhncc asia"
            };
            foreach (var message in messages)
            {
                var output = Helpers.DetectionMoney(regex, message);
                output.IsValid.ShouldBeTrue();
            }
        }

        [Fact]
        public void DetectionBankNumber_Test()
        {
            var regex = new Regex(AppSettingProvider.RegexSTKDetectionValue);
            var messages = new List<string>
            {
                "TK 19132608283018 So tien GD:-3,286,802 So du:112,765,887 NCCPLUS thanh toan tien dien tu ngay 13.612.07",
                "TK 19132608283018 So tien GD:+209,725,000 So du:1,314,134,841 (B/O CT CP QSOFT VN) IBBIZ.50870010070960280723.5.Qsoft chuyen tien cho CONG TY CO PHAN NCCPLUS VIET NAM",
                "TK 19132608283026 So tien GD:+11,762.50 So du:2,089,129.16 12705464 CORE DEVTEAM LTD 235 SEVEN MILE STRAIGHT NUTTS CORN CRUMLIN B T29 4YS GB.OTHR/DEV TEAM INV 40 /RO C/617168392 BNY CUST RRN  PET67596 7186./ACC//INS/BARCGB22XXX. NGAN HA NG NUOC NGOAI THU PHI",
                "TK 19132608283026 So tien GD:-30,000 So du:1,077,366.66 1688438734209JW2GeeE NH MUA CUA KH KHACH HANG 32608283 30000 USD TG 23720 9. Noi dung khac Tieu dung noi bo",
                "TK 19034753904029 So tien GD:-6,000,000 So du:443,105,757 bonus tuyen dung",
                "TK 19034753904029 So tien GD:+3,000,000 So du:174,545,590 thao nguyenthanhncc asia"
            };
            foreach (var message in messages)
            {
                var output = Helpers.DetectionBankNumber(regex, message);
                output.IsValid.ShouldBeTrue();
            }
        }
    }
}
