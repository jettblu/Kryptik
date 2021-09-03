using CrypticPay.Areas.Identity.Data;
using CrypticPay.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tatum;
using Tatum.Clients;

namespace CrypticPay.Services
{
    public class WalletHandler
    {

        private readonly ITatumClient _tatumClient;
        private readonly IBitcoinClient _bitcoinClient;
        private readonly string _encryptKeyPub;
        private readonly string _encryptKeyPriv;


        public WalletHandler(string baseUrl, string apiKey, string encryptKeyPub, string encryptKeyPriv)
        {
            _tatumClient = TatumClient.Create(baseUrl, apiKey);
            _encryptKeyPriv = encryptKeyPriv;
            _encryptKeyPub = encryptKeyPub;
            
        }

        public class WalletandCoins
        {
            public CrypticPayUser User { get; set; }
            public List<CrypticPayCoins> Coins {get; set;}
            public bool ShowData { get; set; }
        }


        public Globals.Status ConstructTransaction(string toString, CrypticPayCoins coin, CrypticPayContext contextUsers)
        {
            try
            {
                var publicToAddress = GetBlockChainAddress(toString, coin, contextUsers);
                var signed = SignTransactionLocally(publicToAddress);
                return Globals.Status.Success;
            }
            catch
            {
                return Globals.Status.Failure;
            }
            
            
        }


        //ADD HANDLING FOR PHONE NUMBER HERE OR.... IN CALLER
        // gets blockchain address from input
        public string GetBlockChainAddress(string inputTo, CrypticPayCoins coin, CrypticPayContext contextUsers)
        {
            string publicSendAddress;
            // first check to see if the input refers to a user or to a valid public address
            try
            {
                
                var userTo = GetUserandWallet(inputTo, contextUsers);
                publicSendAddress = userTo.WalletKryptik.CurrencyWallets.Find(c => c.CoinId == coin.Id).DepositAddress;
            }
            catch
            {   
                // add handling for invalid blockchain addresses to protect user from mistakes
                publicSendAddress = inputTo;
            }
            return publicSendAddress;
        }

        public CrypticPayUser GetUserandWallet(string userId, CrypticPayContext contextUsers)
        {
            // load relational data for user
            var currUser = contextUsers.Users.Include(us => us.WalletKryptik).ThenInclude(w => w.CurrencyWallets).Where(us => us.Id == userId || us.UserName == userId).FirstOrDefault();
            return currUser;
        }

        // signs crypto transaction on server before broadcasting to network
        public string SignTransactionLocally(string pubAddress)
        {   
            var transactionString = "";
            var transactionBuilder = Network.Main.CreateTransactionBuilder();
            //https://github.com/NicolasDorier/NBitcoin.Docs/blob/master/WalletDesign.md
            //transaction.BuildTransaction(true);
            return transactionString;
        }


        public async Task<Globals.Status> CreateWallet(CrypticPayUser user, CrypticPayCoinContext contextCoins)
        {
            var mnemonic = GenerateMnemonic();

            var wallBtc = Tatum.Wallet.Create(Tatum.Model.Currency.BTC, mnemonic, testnet: true);
            var wallBch = Tatum.Wallet.Create(Tatum.Model.Currency.BCH, mnemonic, testnet: true);
            var wallEth = Tatum.Wallet.Create(Tatum.Model.Currency.ETH, mnemonic, testnet: true);
            var wallLtc = Tatum.Wallet.Create(Tatum.Model.Currency.LTC, mnemonic, testnet: true);
            

            var customer = new Tatum.Model.Requests.CreateCustomer()
            {
                AccountingCurrency = "USD",
                ExternalId = user.Id
            };

            // uncomment below for batch account creation

/*            var accountsToCreate = new List<Tatum.Model.Requests.CreateAccount>();

            accountsToCreate.Add(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "BTC", Xpub = wallBtc.XPub, Customer = customer });
            accountsToCreate.Add(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "BCH", Xpub = wallBch.XPub, Customer = customer });
            accountsToCreate.Add(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "ETH", Xpub = wallEth.XPub, Customer = customer });
            accountsToCreate.Add(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "LTC", Xpub = wallLtc.XPub, Customer = customer });

        
            var accountsCreated = await _tatumClient.CreateAccounts(accountsToCreate);*/

            var btcAccount = await _tatumClient.CreateAccount(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "BTC", Xpub = wallBtc.XPub, Customer = customer });
            var bchAccount = await _tatumClient.CreateAccount(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "BCH", Xpub = wallBch.XPub, Customer = customer });
            var ethAccount = await _tatumClient.CreateAccount(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "ETH", Xpub = wallEth.XPub, Customer = customer });
            var ltcAccount = await _tatumClient.CreateAccount(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "LTC", Xpub = wallLtc.XPub, Customer = customer });


            var rand = new Random();

            user.WalletKryptik = new Data.Wallet();


            var btcAddress = Tatum.Wallet.GenerateAddress(Tatum.Model.Currency.BTC, wallBtc.XPub, rand.Next(1, 1000000), testnet: true);
            var btcQr = Utils.QrForWebGenerator(btcAddress);
            var currencyWalletBtc = new CurrencyWallet()
            {
                AccountId = btcAccount.Id,
                CoinId = Utils.FindCryptoIdByName("Bitcoin", contextCoins),
                Xpub = wallBtc.XPub,
                DepositAddress = btcAddress,
                DepositQrBlockchain = btcQr,
                WalletKryptik = user.WalletKryptik,
                CreationTime = DateTime.Now
            };

            var bchAddress = Tatum.Wallet.GenerateAddress(Tatum.Model.Currency.BCH, wallBch.XPub, rand.Next(1, 1000000), testnet: true);
            var bchQr = Utils.QrForWebGenerator(bchAddress);
            var currencyWalletBch = new CurrencyWallet()
            {
                AccountId = bchAccount.Id,
                CoinId = Utils.FindCryptoIdByName("Bitcoin Cash", contextCoins),
                Xpub = wallBch.XPub,
                DepositAddress = bchAddress,
                DepositQrBlockchain = bchQr,
                WalletKryptik = user.WalletKryptik,
                CreationTime = DateTime.Now
            };

            var ethAddress = Tatum.Wallet.GenerateAddress(Tatum.Model.Currency.ETH, wallEth.XPub, rand.Next(1, 1000000), testnet: true);
            var ethQr = Utils.QrForWebGenerator(ethAddress);
            var currencyWalletEth = new CurrencyWallet()
            {
                AccountId = ethAccount.Id,
                CoinId = Utils.FindCryptoIdByName("Ethereum", contextCoins),
                Xpub = wallEth.XPub,
                DepositAddress = ethAddress,
                DepositQrBlockchain = ethQr,
                WalletKryptik = user.WalletKryptik,
                CreationTime = DateTime.Now
            };

            var ltcAddress = Tatum.Wallet.GenerateAddress(Tatum.Model.Currency.LTC, wallLtc.XPub, rand.Next(1, 1000000), testnet: true);
            var ltcQr = Utils.QrForWebGenerator(ltcAddress);

            var currencyWalletLtc = new CurrencyWallet()
            {
                AccountId = ltcAccount.Id,
                CoinId = Utils.FindCryptoIdByName("Litecoin", contextCoins),
                Xpub = wallLtc.XPub,
                DepositAddress = ltcAddress,
                DepositQrBlockchain = ltcQr,
                WalletKryptik = user.WalletKryptik,
                CreationTime = DateTime.Now
            };

           
            // wallet starts with kryptik managed key
            user.WalletKryptik.IsCustodial = true;
            user.WalletKryptik.CurrencyWallets = new List<CurrencyWallet>();

            // set currency wallets for container wallet
            user.WalletKryptik.CurrencyWallets.Add(currencyWalletBtc);
            user.WalletKryptik.CurrencyWallets.Add(currencyWalletBch);
            user.WalletKryptik.CurrencyWallets.Add(currencyWalletEth);
            user.WalletKryptik.CurrencyWallets.Add(currencyWalletLtc);
            user.WalletKryptik.CrypticPayUserKey = user.Id;
            

            user.WalletKryptik.Phrase = EncryptMnemonic(mnemonic, user, null);
            user.WalletKryptik.CreationTime = DateTime.Now;
            user.WalletKryptik.Owner = user;

            return Globals.Status.Done;
            
        }

        public string GenerateMnemonic()
        {
            var phraseObj = new Mnemonic(Wordlist.English);
            string mnemonic = string.Join(" ", phraseObj.Words);
            return mnemonic;
        }

        private byte [] EncryptMnemonic(string mnemonic, CrypticPayUser user, string key)
        {
            byte [] result;

            Aes aes = Aes.Create();
            aes.GenerateIV();
            aes.GenerateKey();


            // use user provided key to encrypt
            if (!user.WalletKryptik.IsCustodial)
            {
                aes.Key = ASCIIEncoding.ASCII.GetBytes(key.ToCharArray());
            }


            EncryptKey(aes, user);

            // Encrypt the string to an array of bytes.
            result = EncryptStringToBytes_Aes(mnemonic, aes);


            return result;
        }

        public void EncryptKey(Aes aes, CrypticPayUser user)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_encryptKeyPriv);
            if (user.WalletKryptik.IsCustodial)
            {
                var encryptedKey = rsa.Encrypt(aes.Key, fOAEP: false);
                user.WalletKryptik.Decrypter = encryptedKey;
            }
            
            var encryptedIv = rsa.Encrypt(aes.IV, fOAEP: false);
            user.WalletKryptik.Iv = encryptedIv;

        }


        public async Task<Tatum.Model.Responses.AccountBalance> GetCurrencyWalletHistory(CurrencyWallet currencyWallet)
        {
            var result = await _tatumClient.GetAccountBalance(currencyWallet.AccountId.ToString());
            return result;
        }

        public async Task<Globals.Status> UpdateBalances(WalletandCoins walletAndCoins)
        {
            double totalBalance = 0;

            // get exchange rates, so coin value can be displayed
            var coinExchangeRates = CoinData.GetCoinsExchangeRates(walletAndCoins.Coins);

            int index = 0;
            foreach (var account in walletAndCoins.User.WalletKryptik.CurrencyWallets)
            {
                var responseBal = await _tatumClient.GetAccountBalance(account.AccountId);
               
                var balance = Convert.ToDouble(responseBal.Balance);
                account.AccountBalanceFiat = balance;
                // convert fiat rate to crypto rate
                account.AccountBalanceCrypto = balance / coinExchangeRates[index];
                totalBalance += balance;
                index += 1;
            }

            walletAndCoins.User.WalletKryptik.BalanceTotal = totalBalance;

            return Globals.Status.Done;
        }

        private byte[] EncryptStringToBytes_Aes(string plainText, Aes aesAlg)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (aesAlg.Key == null || aesAlg.Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (aesAlg.IV == null || aesAlg.IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (aesAlg)
            {
                var scopeing = ASCIIEncoding.ASCII.GetString(aesAlg.Key);
                aesAlg.Padding = PaddingMode.Zeros;
                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }


        public void ChangeCustodialType(CrypticPayUser user, string key, CrypticPayContext contextUser)
        {   
            var mnemonic = DecryptMnemonic(user, key);

            var aes = DecryptKeys(user, key);

            if (user.WalletKryptik.IsCustodial)
            {
                aes.Key = ASCIIEncoding.ASCII.GetBytes(key.ToCharArray());
            }
            else
            {
                aes.GenerateKey();
            }

            byte[] mnemonicEncrypted;
            using (aes)
            {
                // Encrypt the string to an array of bytes.
                mnemonicEncrypted = EncryptStringToBytes_Aes(mnemonic, aes);

                // Decrypt the bytes to a string.
                // string roundtrip = DecryptStringFromBytes_Aes(encrypted, aes.Key, aes.IV);
            }


            user.WalletKryptik.Phrase = mnemonicEncrypted;
            // flip custodial type
            user.WalletKryptik.IsCustodial = !user.WalletKryptik.IsCustodial;

            EncryptKey(aes, user);

            contextUser.Users.Update(user);
            contextUser.SaveChanges();

        }

        public string DecryptMnemonic(CrypticPayUser user, string key=null)
        {
            var aes = DecryptKeys(user, key);

            return DecryptStringFromBytes_Aes(user.WalletKryptik.Phrase, aes.Key, aes.IV);

        }


        private Aes DecryptKeys(CrypticPayUser user, string key)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            // set rsa with private key
            rsa.FromXmlString(_encryptKeyPriv);

            Aes aes = Aes.Create();

            if (user.WalletKryptik.IsCustodial)
            {
                var poop = rsa.Decrypt(user.WalletKryptik.Decrypter, false);
                aes.Key = poop;
            }
            else
            {
                aes.Key = ASCIIEncoding.ASCII.GetBytes(key.ToCharArray());
            }

            aes.IV = rsa.Decrypt(user.WalletKryptik.Iv, false);


            return aes;
        }


        private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Padding = PaddingMode.Zeros;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

    }
}
