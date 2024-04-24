using UnityEngine.Networking;

public class BypassCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Always return true to bypass certificate checks
        return true;
    }
}
