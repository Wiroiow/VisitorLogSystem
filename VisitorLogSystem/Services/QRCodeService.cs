using QRCoder;
using System;
using VisitorLogSystem.Interfaces;

namespace VisitorLogSystem.Services
{

    public class QRCodeService : IQRCodeService
    {
        
        public string GenerateUniqueQRValue(int preRegistrationId)
        {
            var timestamp = DateTime.Now.Ticks;
            return $"PREREG-{preRegistrationId}-{timestamp}";
        }

      
        public byte[] GenerateQRCodeImage(string qrValue)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(qrValue, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new PngByteQRCode(qrCodeData))
                {
                    // pixelsPerModule: 20 creates a good-sized QR code
                    return qrCode.GetGraphic(20);
                }
            }
        }

        
        public string GenerateQRCodeBase64(string qrValue)
        {
            var imageBytes = GenerateQRCodeImage(qrValue);
            return Convert.ToBase64String(imageBytes);
        }
    }
}