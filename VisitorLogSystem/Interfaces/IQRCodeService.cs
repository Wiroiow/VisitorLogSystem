namespace VisitorLogSystem.Interfaces
{
    public interface IQRCodeService
    {
        string GenerateQRCodeBase64(string qrValue);

        byte[] GenerateQRCodeImage(string qrValue);

        string GenerateUniqueQRValue(int preRegistrationId);


    }
}
