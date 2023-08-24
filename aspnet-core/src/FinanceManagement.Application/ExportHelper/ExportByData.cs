using FinanceManagement.APIs.GetOutcomingEntries.Dto;
using FinanceManagement.Uitls;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FinanceManagement.Helper;
using System.Globalization;
using System.Threading.Tasks;
using FinanceManagement.APIs.OutcomingEntries.Dto;

namespace FinanceManagement.ExportHelper
{
    public class ExportByData
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ExportByData(IWebHostEnvironment webHostEnvironment)
        {
            this._hostingEnvironment = webHostEnvironment;
        }
		public FileExportDto ExportPhieuChiPdfSelecect(GetInformationExport data)
        {
            var now = data.TransactionDate;
            string day = now.Day < 10 ? "0" + now.Day.ToString() : now.Day.ToString();
            string month = now.Month < 10 ? "0" + now.Month.ToString() : now.Month.ToString();
            string year = now.Year.ToString();
            string reciever = data.Receiver;
            string name = data.Name;
            string currency = string.Format(new CultureInfo("vi-VN"), "{0:#,##0}", data.Value);
            string money = currency + data.CurrencyName;
            string moneyText = Helpers.NumberToText(data.Value);
            string isFile = data.IsAcceptFile == Enums.OutcomingEntryFileStatus.Confirmred ? "Có file đính kèm/hóa đơn" : "Không có file đính kèm";
            StringBuilder builder = new StringBuilder();

			        builder.Append($@"<div>
	        <table cellpadding='0' cellspacing='0' style='border-collapse: collapse'>
            <tbody>
            <tr style='height: 56.75pt'>
                <td
                style='
                    width: 272.45pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p style='margin-top: 0pt; margin-bottom: 0pt; font-size: 11pt'>
                    <strong
                    ><span style='font-family: 'Times New Roman''
                        >C&ocirc;ng ty TNHH C&ocirc;ng Nghệ NCCSOFT Việt Nam</span
                    ></strong
                    >
                </p>
                <p style='margin-top: 0pt; margin-bottom: 0pt; font-size: 11pt'>
                    <strong
                    ><span style='font-family: 'Times New Roman''
                        >Số 58 đường Tố Hữu, Nam Từ Li&ecirc;m, HN</span
                    ></strong
                    >
                </p>
                <p style='margin-top: 0pt; margin-bottom: 0pt; font-size: 11pt'>
                    <strong
                    ><span style='font-family: 'Times New Roman''
                        >MST : 0106522655</span
                    ></strong
                    >
                </p>
                </td>
                <td
                style='
                    width: 156.75pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 11pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''
                        >Mẫu số: 01 &ndash; TT</span
                    ></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 11pt;
                    '
                >
                    <em
                    ><span style='font-family: 'Times New Roman''
                        >Ban h&agrave;nh theo QĐ số 48/2006/QĐ-BTC ng&agrave;y 14/09/2006
                        của Bộ trưởng BTC)</span
                    ></em
                    >
                </p>
                </td>
            </tr>
            </tbody>
        </table>
        <p
            style='
            margin-top: 0pt;
            margin-bottom: 8pt;
            line-height: 108%;
            font-size: 12pt;
            '
        >
            <span style='font-family: 'Times New Roman''>&nbsp;</span>
        </p>
        <p
            style='
            margin-top: 0pt;
            margin-bottom: 8pt;
            margin-left: 180pt;
            line-height: 108%;
            font-size: 22pt;
            '
        >
            <strong><span style='font-family: 'Times New Roman''>PHIẾU CHI</span></strong>
        </p>
        <p
            style='
            margin-top: 0pt;
            margin-left: 108pt;
            margin-bottom: 8pt;
            text-indent: 36pt;
            line-height: 108%;
            font-size: 12pt;
            '
        >
            <strong
            ><em><span style='font-family: 'Times New Roman''>&nbsp;</span></em></strong
            ><strong
            ><em
                ><span style='font-family: 'Times New Roman''>Ng&agrave;y</span></em
            ></strong
            ><strong
            ><em
                ><span style='font-family: 'Times New Roman''>&nbsp;&nbsp;</span></em
            ></strong
            ><a name='dd'
            ><strong
                ><em
                ><span style='font-family: 'Times New Roman''>{day}</span></em
                ></strong
            ></a
            ><strong
            ><em
                ><span style='font-family: 'Times New Roman''>&nbsp;&nbsp;</span></em
            ></strong
            ><strong
            ><em
                ><span style='font-family: 'Times New Roman''>th&aacute;ng</span></em
            ></strong
            ><strong
            ><em
                ><span style='font-family: 'Times New Roman''>&nbsp;&nbsp;</span></em
            ></strong
            ><a name='mm'
            ><strong
                ><em
                ><span style='font-family: 'Times New Roman''>{month}</span></em
                ></strong
            ></a
            ><strong
            ><em
                ><span style='font-family: 'Times New Roman''
                >&nbsp;&nbsp;&nbsp;</span
                ></em
            ></strong
            ><strong
            ><em><span style='font-family: 'Times New Roman''>năm</span></em></strong
            ><strong
            ><em
                ><span style='font-family: 'Times New Roman''>&nbsp;&nbsp;</span></em
            ></strong
            ><a name='yy'
            ><strong
                ><em
                ><span style='font-family: 'Times New Roman''>{year}</span></em
                ></strong
            ></a
            ><span style='width: 20.97pt; text-indent: 0pt; display: inline-block'
            >&nbsp;</span
            ><span style='width: 36pt; text-indent: 0pt; display: inline-block'
            >&nbsp;</span
            >
        </p>
        <p
            style='
            margin-top: 0pt;
            margin-bottom: 8pt;
            line-height: 108%;
            font-size: 12pt;
            '
        >
            <span style='width: 330.75pt; display: inline-block'>&nbsp;</span
            ><span style='width: 29.25pt; display: inline-block'>&nbsp;</span>
        </p>
        <p
            style='
            margin-top: 0pt;
            margin-bottom: 8pt;
            line-height: 108%;
            font-size: 12pt;
            '
        >
            <span style='font-family: 'Times New Roman''>Người nhận tiền:</span
            ><strong><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
            ><a name='nm'
            ><strong
                ><span style='font-family: 'Times New Roman''>{reciever}</span></strong
            ></a
            ><span style='width: 215.9pt; display: inline-block'>&nbsp;</span
            ><span style='width: 29.25pt; display: inline-block'>&nbsp;</span>
        </p>
        <p
            style='
            margin-top: 0pt;
            margin-bottom: 8pt;
            line-height: 108%;
            font-size: 12pt;
            '
        >
            <span style='font-family: 'Times New Roman''>Địa chỉ:&nbsp;</span>
        </p>
        <p
            style='
            margin-top: 0pt;
            margin-bottom: 8pt;
            line-height: 108%;
            font-size: 12pt;
            '
        >
            <span style='font-family: 'Times New Roman''>L&yacute; do chi:&nbsp;</span
            ><a name='rq'
            ><span style='font-family: 'Times New Roman''>{name}</span></a
            >
        </p>
        <p
            style='
            margin-top: 0pt;
            margin-bottom: 8pt;
            line-height: 108%;
            font-size: 12pt;
            '
        >
            <span style='font-family: 'Times New Roman''>Số tiền:&nbsp;</span
            ><a name='mn'
            ><strong
                ><span style='font-family: 'Times New Roman''>{money}</span></strong
            ></a
            >
        </p>
        <p
            style='
            margin-top: 0pt;
            margin-bottom: 8pt;
            line-height: 108%;
            font-size: 12pt;
            '
        >
            <span style='font-family: 'Times New Roman''>Viết bằng chữ:&nbsp;</span
            ><a name='tm'
            ><strong
                ><em
                ><span style='font-family: 'Times New Roman''>{moneyText}</span></em
                ></strong
            ></a
            >
        </p>
        <p
            style='
            margin-top: 0pt;
            margin-bottom: 8pt;
            line-height: 108%;
            font-size: 12pt;
            '
        >
            <span style='font-family: 'Times New Roman''>K&egrave;m theo:</span
            ><strong><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
            ><a name='dc'
            ><strong
                ><span style='font-family: 'Times New Roman''>{isFile}</span></strong
            ></a
            >
        </p>
        <table
            cellpadding='0'
            cellspacing='0'
            style='width: 477pt; border-collapse: collapse'
        >
            <tbody>
            <tr>
                <td
                colspan='3'
                style='
                    width: 466.2pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: right;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><em
                        ><span style='font-family: 'Times New Roman''
                        >Ng&agrave;y</span
                        ></em
                    ></strong
                    ><strong
                    ><em
                        ><span style='font-family: 'Times New Roman''
                        >&nbsp;&nbsp;</span
                        ></em
                    ></strong
                    ><a name='dd1'
                    ><strong
                        ><em
                        ><span style='font-family: 'Times New Roman''
                            >{day}</span
                        ></em
                        ></strong
                    ></a
                    ><strong
                    ><em
                        ><span style='font-family: 'Times New Roman''
                        >&nbsp;&nbsp;</span
                        ></em
                    ></strong
                    ><strong
                    ><em
                        ><span style='font-family: 'Times New Roman''
                        >th&aacute;ng</span
                        ></em
                    ></strong
                    ><strong
                    ><em
                        ><span style='font-family: 'Times New Roman''
                        >&nbsp;&nbsp;</span
                        ></em
                    ></strong
                    ><a name='mm1'
                    ><strong
                        ><em
                        ><span style='font-family: 'Times New Roman''
                            >{month}</span
                        ></em
                        ></strong
                    ></a
                    ><strong
                    ><em
                        ><span style='font-family: 'Times New Roman''
                        >&nbsp;&nbsp;</span
                        ></em
                    ></strong
                    ><strong
                    ><em
                        ><span style='font-family: 'Times New Roman''>năm&nbsp;</span></em
                    ></strong
                    ><a name='yy1'
                    ><strong
                        ><em
                        ><span style='font-family: 'Times New Roman''
                            >{year}</span
                        ></em
                        ></strong
                    ></a
                    >
                </p>
                </td>
            </tr>
            <tr style='height: 11.55pt'>
                <td
                style='
                    width: 141.95pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''
                        >NGƯỜI NHẬN TIỀN</span
                    ></strong
                    >
                </p>
                </td>
                <td
                style='
                    width: 146.7pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''
                        >NGƯỜI LẬP PHIẾU</span
                    ></strong
                    >
                </p>
                </td>
                <td
                style='
                    width: 155.95pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''
                        >KT GI&Aacute;M ĐỐC</span
                    ></strong
                    >
                </p>
                </td>
            </tr>
            <tr>
                <td
                style='
                    width: 141.95pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <em
                    ><span style='font-family: 'Times New Roman''
                        >(K&yacute;, họ t&ecirc;n)</span
                    ></em
                    >
                </p>
                </td>
                <td
                style='
                    width: 146.7pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <em
                    ><span style='font-family: 'Times New Roman''
                        >(K&yacute;, họ t&ecirc;n)</span
                    ></em
                    >
                </p>
                </td>
                <td
                style='
                    width: 155.95pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <em
                    ><span style='font-family: 'Times New Roman''
                        >(K&yacute;, họ t&ecirc;n)</span
                    ></em
                    >
                </p>
                </td>
            </tr>
            <tr style='height: 75.25pt'>
                <td
                style='
                    width: 141.95pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p style='margin-top: 0pt; margin-bottom: 0pt; font-size: 12pt'>
                    <strong
                    ><span style='font-family: 'Times New Roman''
                        >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span
                    ></strong
                    >
                </p>
                </td>
                <td
                style='
                    width: 146.7pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                </td>
                <td
                style='
                    width: 155.95pt;
                    padding-right: 5.4pt;
                    padding-left: 5.4pt;
                    vertical-align: top;
                '
                >
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                <p
                    style='
                    margin-top: 0pt;
                    margin-bottom: 0pt;
                    text-align: center;
                    font-size: 12pt;
                    '
                >
                    <strong
                    ><span style='font-family: 'Times New Roman''>&nbsp;</span></strong
                    >
                </p>
                </td>
            </tr>
            </tbody>
        </table>
        <p
            style='
            margin-top: 0pt;
            margin-bottom: 8pt;
            text-align: center;
            line-height: 108%;
            font-size: 12pt;
            '
        >
            <span style='font-family: 'Times New Roman''>&nbsp;</span>
        </p>
        </div>");
           
            return new FileExportDto { Html = builder.ToString(), Message = "Success" };
        }
    }
}
