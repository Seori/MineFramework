using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using ThoughtWorks.QRCode.Codec;

namespace MineFramework
{
    public class QrCls
    {
        public static string _lastErrorMessage = string.Empty;

        /// <summary>
        /// 생성자함수
        /// </summary>
        public QrCls()
        {

        }

        /// <summary>
        /// QRCode를 생성하고 저장합니다.
        /// </summary>
        /// <param name="_qrdata">QR Data</param>
        /// <param name="_qrpath">저장대상경로</param>
        /// <param name="_qrname">이미지명</param>
        /// <param name="_qrimg">Image Out</param>
        /// <returns></returns>
        public static int fnMakeQrCode(string _qrdata, string _qrpath, string _qrname, out Bitmap _qrimg)
        {
            QRCodeEncoder encode = new QRCodeEncoder();

            // 단위체크 (byte, numeric, alpha_numeric)
            encode.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            // QR코드 크기
            encode.QRCodeScale = 3;
            // byte, 문자, 숫자갯수에 따라 Version체크가능 (https://www.qrcode.com/ko/about/version.html 참조)
            encode.QRCodeVersion = 4;
            // 오류복원능력 (L:CodeWords 7% 복원가능, M:CodeWords 15% 복원가능, Q:CodeWords 25% 복원가능, H:CodeWords 30% 복원가능)
            encode.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            try
            {
                // URL QR이미지 생성
                _qrimg = encode.Encode(_qrdata);

                if (fnSaveQrCode(_qrpath, _qrname, _qrimg))
                {
                    // 저장까지 처리완료된경우
                    return 0;
                }
                else
                {
                    // 저장은하지 않고 QR까지만 생성된경우
                    return 1;
                }
                
            }
            catch (Exception ex)
            {
                _lastErrorMessage = ex.Message;
                _qrimg = null;
                return -9;
            }
        }

        /// <summary>
        /// QR Image를 생성합니다.
        /// </summary>
        /// <param name="_gd">Request Info</param>
        /// qrData : QR Data
        /// qrMakeImg : QR Image 생성여부 (Y or N)
        /// qrPath : QR Image 생성경로
        /// qrName : QR Image 파일명
        /// qrScale : QR 코드 크기
        /// qrVersion : QR Version
        /// <param name="_qrimg">Output : QR Image</param>
        /// <returns></returns>
        public static int fnMakeQrCode(GeneralDictionary _gd, out Bitmap _qrimg)
        {
            try
            {
                QRCodeEncoder encode = new QRCodeEncoder();

                // 기본값 셋팅
                int iqrScale = 3;
                int iqrVersion = 4;

                // QR Data
                string qrdata = _gd["qrData"].ToString();
                // QR Image 생성여부 (Y or N)
                string qrmakeimg = _gd["qrMakeImg"].ToString();
                // QR Image 생성경로
                string qrpath = _gd["qrPath"].ToString();
                // QR Image 파일명
                string qrname = _gd["qrName"].ToString();
                // QR 코드 크기
                string qrcodescale = _gd["qrScale"].ToString();
                // QR Version
                string qrversion = _gd["qrVersion"].ToString();

                if (string.IsNullOrEmpty(qrdata))
                {
                    _qrimg = null;
                    // QR을 만들 Data가 없는경우 Return
                    return -7;
                }

                if (!string.IsNullOrEmpty(qrcodescale))
                {
                    iqrScale = int.Parse(qrcodescale);
                }

                if (!string.IsNullOrEmpty(qrversion))
                {
                    iqrVersion = int.Parse(qrversion);
                }

                // 단위체크 (byte, numeric, alpha_numeric)
                encode.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                // QR코드 크기
                encode.QRCodeScale = iqrScale;
                // byte, 문자, 숫자갯수에 따라 Version체크가능 (https://www.qrcode.com/ko/about/version.html 참조)
                encode.QRCodeVersion = iqrVersion;
                // 오류복원능력 (L:CodeWords 7% 복원가능, M:CodeWords 15% 복원가능, Q:CodeWords 25% 복원가능, H:CodeWords 30% 복원가능)
                encode.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            
                // URL QR이미지 생성
                _qrimg = encode.Encode(qrdata);

                if (qrmakeimg == "Y")
                {
                    if (fnSaveQrCode(qrpath, qrname, _qrimg))
                    {
                        // 저장까지 처리완료된경우
                        return 0;
                    }
                    else
                    {
                        // 저장실패시
                        return -8;
                    }
                }
                else
                {
                    // 저장은 하지 않고 QR까지만 생성된경우
                    return 1;
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = ex.Message;
                _qrimg = null;
                return -9;
            }
        }

        /// <summary>
        /// QRCode Image를 실제 이미지파일로 생성합니다.
        /// </summary>
        /// <param name="_qrpath">생성할경로</param>
        /// <param name="_qrfile">생성할파일명</param>
        /// <param name="_qrimg">실제 QRImage (Bitmap)</param>
        /// <returns></returns>
        public static bool fnSaveQrCode(string _qrpath, string _qrfile, Bitmap _qrimg)
        {
            try
            {
                Bitmap saveImg = _qrimg;
                if (saveImg != null)
                {
                    // 해당경로에 파일저장
                    saveImg.Save(_qrpath + "\\" + _qrfile, System.Drawing.Imaging.ImageFormat.Png);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;
            }            
        }
    }
}
