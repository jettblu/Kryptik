using CrypticPay.Areas.Identity.Data;
using CrypticPay.Services.DataTypes;
using SecretSharingDotNet.Cryptography;
using System.Numerics;

namespace CrypticPay.Services
{
    public interface ICrypto
    {
        ClientCryptoPack GetClientCrypto(CrypticPayUser user);
        string ReconstructSecret(FinitePoint<BigInteger>[] shares);
        void SplitSecret(string secret, int min = 2, int max = 4);
        byte[] GetUserMsgKey(CrypticPayUser user);
    }
}