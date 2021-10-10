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
    public class Crypto
    {
        // splits a secret into shares based on shamir secret sharing algo.
        public static void SplitSecret(string secret, int min = 2, int max = 4)
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
        public static string ReconstructSecret(FinitePoint<BigInteger>[] shares)
        {
            var gcd = new ExtendedEuclideanAlgorithm<BigInteger>();
            var combine = new ShamirsSecretSharing<BigInteger>(gcd);
            var recoveredSecret = combine.Reconstruction(shares);
            return recoveredSecret.ToString();
        }
    }
}
