using NBitcoin;
using SecretSharingDotNet.Cryptography;
using SecretSharingDotNet.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace CrypticPay.Services
{
    // service for securing secrets
    public class Crypto : ICrypto
    {
        // splits a secret into shares based on shamir secret sharing algo.
        public void SplitSecret(string secret, int min = 2, int max = 4)
        {
            var gcd = new ExtendedEuclideanAlgorithm<BigInteger>();

            // Create Shamir's Secret Sharing instance with BigInteger
            var split = new ShamirsSecretSharing<BigInteger>(gcd);

            // Secret length automatically changes security level
            var shares = split.MakeShares(min, max, secret);

            foreach (var share in shares)
            {
                Console.WriteLine(share.X);
                Console.WriteLine(share.Y);
            }

            gcd = new ExtendedEuclideanAlgorithm<BigInteger>();

            // The 'shares' instance contains the shared secrets
            var combine = new ShamirsSecretSharing<BigInteger>(gcd);
            var subSet1 = shares.Where(p => p.X.IsEven).ToList();
            var recoveredSecret1 = combine.Reconstruction(subSet1.ToArray());
            var subSet2 = shares.Where(p => !p.X.IsEven).ToList();
            var recoveredSecret2 = combine.Reconstruction(subSet2.ToArray());
            Console.WriteLine(recoveredSecret1);
            Console.WriteLine(recoveredSecret2);
        }

        // used to reconstruct secret (mnemonic usually) given two shamir shares
        public string ReconstructSecret(FinitePoint<BigInteger>[] shares)
        {
            var gcd = new ExtendedEuclideanAlgorithm<BigInteger>();
            var combine = new ShamirsSecretSharing<BigInteger>(gcd);
            var recoveredSecret = combine.Reconstruction(shares);
            return recoveredSecret.ToString();
        }

        public DataTypes.ClientCryptoPack GetClientCrypto(Areas.Identity.Data.CrypticPayUser user)
        {
            
            return new DataTypes.ClientCryptoPack()
            {
                KeyPath = "m/0/1/2/3/4",
                KeyShare = user.WalletKryptik.SeedShare,
                Xpub = user.WalletKryptik.Xpub
            };
        }

        // get public key for messaging
        public byte[] GetUserMsgKey(Areas.Identity.Data.CrypticPayUser user)
        {
            var masterPubKey = ExtPubKey.Parse(user.WalletKryptik.Xpub, Network.Main);
            var toKey = masterPubKey.Derive(new KeyPath("m/0/1/2/3/4"));
            return toKey.ToBytes();
        }

    }
}
