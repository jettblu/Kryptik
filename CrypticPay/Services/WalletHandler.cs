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
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.HdWallet;
using Nethereum.RPC.Eth.DTOs;
using NBitcoin.Altcoins;

namespace CrypticPay.Services
{
    public class WalletHandler
    {

        private readonly ITatumClient _tatumClient;
        private readonly IBitcoinClient _bitcoinClient;
        private readonly IBitcoinCashClient _bitcoinCashClient;
        private readonly IEthereumClient _ethereumClient;
        private readonly ILitecoinClient _litecoinClient;
        private readonly string _encryptKeyPub;
        private readonly string _encryptKeyPriv;
        private readonly string _mnem;

        public WalletHandler(string baseUrl, string apiKey, string encryptKeyPub, string encryptKeyPriv, string mnemonic)
        {
            // general tatum client for customer data
            _tatumClient = TatumClient.Create(baseUrl, apiKey);
            // add clients for onchain data and transactions
            _bitcoinClient = BitcoinClient.Create(baseUrl, apiKey);
            _bitcoinCashClient = BitcoinCashClient.Create(baseUrl, apiKey);
            _ethereumClient = EthereumClient.Create(baseUrl, apiKey);
            _litecoinClient = LitecoinClient.Create(baseUrl, apiKey);
            // keys for enecryption
            _encryptKeyPriv = encryptKeyPriv;
            _encryptKeyPub = encryptKeyPub;
            _mnem = mnemonic;
        }



        /*        public Key GenerateKey(string currency)
                {
                    var words = DecryptMnemonic(user);
                    var mnemo = new Mnemonic(words,
                        Wordlist.English);
                    var hdRoot = mnemo.DeriveExtKey("my password");
                }*/

        public class TransactionAndMeta
        {
            public Tatum.Model.Responses.Transaction Transaction { get; set; }
            public CrypticPayCoins Currency { get; set; }
            public Network NetworkOnChain { get; set; }
        }
        

        
        // UPDATE so userWallet is passed in... no need to duplicate initial query
        // UPDATE sp onchain data is pulled for each tx.
        // retrieves incoming transactions for a given user and currency
        public async Task<List<TransactionAndMeta>> GetTransactionsLedger(string userId, CrypticPayContext contextUsers, CrypticPayCoins coin)
        {
            var user = GetUserandWallet(userId, contextUsers);
            var currencyWallet = GetCurrencyWallet(coin, user);
            // visit https://tatum.io/apidoc#operation/getTransactionsByAccountId for more info on construction
            var filterTrans = new Tatum.Model.Requests.TransactionFilterAccount() {
                Currency = coin.Ticker,
                Id = currencyWallet.AccountId,
                TransactionType = "CREDIT_DEPOSIT"
            };

            // base ledger transactions from Tatum
            var transactions = await _tatumClient.GetTransactionsForAccount(filterTrans);
            List<TransactionAndMeta> txResult = new List<TransactionAndMeta>();
            foreach (var tx in transactions)
            {
                txResult.Add(new TransactionAndMeta()
                {
                    Transaction = tx,
                    Currency = coin,
                    NetworkOnChain = GetNetwork(coin, isTestnet: false)
                });

            }
            return txResult;
        }

        public async Task<List<TransactionOutput>> GetTransactionOnChain(string userId, CrypticPayContext contextUsers, CrypticPayCoins coin)
        {
            var user = GetUserandWallet(userId, contextUsers);
            var currencyWallet = GetCurrencyWallet(coin, user);
            var resultOutputs = new List<TransactionOutput>();
            TransactionOutput outputToAdd = new TransactionOutput();
            switch (coin.Ticker)
            {
                case "BTC":
                    var txBtc = await _bitcoinClient.GetTxForAccount(currencyWallet.AddressOnChain.Address);
                    break;

                case "LTC":
                    var txLtc = await _litecoinClient.GetTxForAccount(currencyWallet.AddressOnChain.Address);
                    foreach (var tx in txLtc)
                    {
                        outputToAdd.TransactionHash = StringToUint256(tx.Hash);
                        outputToAdd.TransactionId = StringToUint256(tx.Hash);
                        outputToAdd.Fee = tx.Fee;
                        uint opIndex = 0;
                        foreach (var op in tx.Outputs)
                        {
                            if(op.Address == currencyWallet.AddressOnChain.Address)
                            {
                                outputToAdd.OutputIndex = opIndex;
                                outputToAdd.AddressTo = op.Address;
                                
                                resultOutputs.Add(outputToAdd);
                            }
                        }
                    }
                    break;

                case "BCH":
                    var txBCh = await _bitcoinCashClient.GetTxForAccount(currencyWallet.AddressOnChain.Address);
                    break;
                case "ETH":
                    var txEth = await _ethereumClient.GetAccountTransactions(currencyWallet.AddressOnChain.Address);
                    break;

                default:
                    throw new Exception("Coin type not found. Currency client could not be established.");
            }
            return resultOutputs;
        }


        /*public async Task<List<OutPoint>> TransactionsToOutpoints()
        {
            
        }*/


        // returns network given coin
        public Network GetNetwork(CrypticPay.Data.CrypticPayCoins coin, bool isTestnet)
        {
            Network network;
            if (isTestnet)
            {
                switch (coin.Ticker)
                {
                    case "BTC":
                        network = Network.TestNet;
                        break;

                    case "LTC":
                        network = NBitcoin.Altcoins.Litecoin.Instance.Testnet;
                        break;

                    case "BCH":
                        network = NBitcoin.Altcoins.BCash.Instance.Testnet;
                        break;

                    default:
                        throw new Exception("Coin type not found. Network could not be specified.");
                }
            }
            else
            {

                switch (coin.Ticker)
                {
                    case "BTC":
                        network = Network.Main;
                        break;

                    case "LTC":
                        network = NBitcoin.Altcoins.Litecoin.Instance.Mainnet;
                        break;

                    case "BCH":
                        network = NBitcoin.Altcoins.BCash.Instance.Mainnet;
                        break;

                    default:
                        throw new Exception("Coin type not found. Network could not be specified.");
                }

            }
            return network;
        }

        public class WalletandCoins
        {
            public CrypticPayUser User { get; set; }
            public List<CrypticPayCoins> Coins {get; set;}
            public bool ShowData { get; set; }
        }

        private uint256 StringToUint256(string input)
        {
            return new uint256(input);
        }

        // generates transaction data to be signed by client, before broadcast
        // toString should be either a user id or a blockchain address
        public async Task<Globals.Status> ConstructTransaction(string toString, Data.Transaction tx, CrypticPayContext contextUsers, CrypticPayCoinContext contextCoins)
        {
            var coin = Utils.FindCryptoByID(contextCoins, tx.CoinId);
            var userFrom = GetUserandWallet(tx.SenderId, contextUsers);
            var currWallet = GetCurrencyWallet(coin, userFrom);
            try
            {
                var publicToAddress = GetBlockChainAddress(toString, coin, contextUsers, tx);
                // UPDATE so coin doesn't need to be requeried throughout transaction flow
                if (coin.Ticker == "ETH")
                {
                    var transaction = new TransactionInput()
                    {
                        From = currWallet.AddressOnChain.Address,
                        To = publicToAddress
                    };
                }
                else
                {
                    var transaction = NBitcoin.Transaction.Create(NBitcoin.Network.Main);
                    var transactions = await GetTransactionOnChain(userFrom.Id, contextUsers, coin);
                    List<Coin> coinsToSpend = new List<Coin>();

                    foreach (var inputTx in transactions)
                    {
                        var outPointToSpend = new OutPoint()
                        {
                            Hash = inputTx.TransactionHash
                        };
                        
                        transaction.Inputs.Add(new TxIn()
                        {
                            PrevOut = outPointToSpend
                        });
                    }
                    // 2x check if amount unit is correct
                    // calculate miner fees
                    // outamount = in - fees
                    // serialize and return

                }

                return Globals.Status.Success;
            }
            catch
            {
                return Globals.Status.Failure;
            }
            
        }

        public CurrencyWallet GetCurrencyWallet(CrypticPayCoins coin, CrypticPayUser user)
        {
            var wallet = user.WalletKryptik.CurrencyWallets.Where(w => w.CoinId == coin.Id).FirstOrDefault();
            return wallet;
        }


        //ADD HANDLING FOR PHONE NUMBER HERE OR.... IN CALLER
        // gets blockchain address from input
        public string GetBlockChainAddress(string inputTo, CrypticPayCoins coin, CrypticPayContext contextUsers, Data.Transaction tx)
        {
            string publicSendAddress;
            // first check to see if the input refers to a user or to a valid public address
            try
            {
                var userTo = GetUserandWallet(inputTo, contextUsers);
                tx.SenderId = userTo.Id;
                publicSendAddress = userTo.WalletKryptik.CurrencyWallets.Find(c => c.CoinId == coin.Id).AddressOnChain.Address;  
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
            var currUser = contextUsers.Users.Include(us => us.WalletKryptik).ThenInclude(w => w.CurrencyWallets).ThenInclude(w => w.AddressOnChain).Where(us => us.Id == userId || us.UserName == userId).FirstOrDefault();
            return currUser;
        }

        // signs crypto transaction on server before broadcasting to network
        public string SignTransactionLocally(string pubAddress, Data.Transaction tx)
        {   
            var transactionString = "";
            var transactionBuilder = Network.Main.CreateTransactionBuilder();
            //https://github.com/NicolasDorier/NBitcoin.Docs/blob/master/WalletDesign.md
            //transaction.BuildTransaction(true);
            return transactionString;
        }

        public BlockchainAddress CreateAddress(CurrencyWallet currWallet, CrypticPayCoins coin, string extPub, bool isTestnet=false, bool isOnChain=true)
        {
            // increment index, so we can generate new address from extended key
            int indexAddy = 1;
            var masterPubKey = ExtPubKey.Parse(extPub, Network.Main);
            
            
            var key = new NBitcoin.Key();
            
            string words;


            if(coin.Ticker == "ETH")
            {
                // we do not need to case for testnet here as eth address is cross network compatible
                var ethWallet = new PublicWallet(masterPubKey);
                string ethAddy;
                // generate P2PKH for onchhain tx.
                if (isOnChain)
                {
                    // ethereum addy is not case sensitive. use lower case to be consistent w/ tatum.
                    ethAddy = ethWallet.GetAddress(indexAddy).ToLower();
                }
                else
                {
                    // ADD LOGIC FOR ETHEREUM OFFCHAIN TX.
                    ethAddy = ethWallet.GetAddress(indexAddy);
                }
                return new BlockchainAddress()
                {
                    Address = ethAddy,
                    Index = indexAddy
                };


            }

            
            // Generate public key k from xpub
            ExtPubKey pubKeyK = masterPubKey.Derive((uint)indexAddy);


            Network network;
            bool isSegwit = true;

            network = GetNetwork(coin, isTestnet);

            // MOVE CONDITIONAL for segwit into different area to make more robust
            if(coin.Ticker == "BCH")
            {
                // BCH does NOT support segregated witness addresses as of 9-17-21
                isSegwit = false;
            }

            ScriptPubKeyType keyType;
            if (isSegwit)
            {
                keyType = ScriptPubKeyType.Segwit;
            }
            else
            {
                keyType = ScriptPubKeyType.Legacy;
            }

            string addy;

            // generate P2PKH addy for onchain sending
            if (isOnChain)
            {
                addy = pubKeyK.PubKey.GetAddress(keyType, network).ToString();
            }
            // generate P2SH for offchain sending
            else
            {
                var kryptikMnem = new Mnemonic(_mnem);
                var kryptikKeyRing = kryptikMnem.DeriveExtKey().Neuter();
                Script redeemScript = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { pubKeyK.PubKey, kryptikKeyRing.PubKey});
                addy = redeemScript.Hash.GetAddress(network).ToString();
            }
            

            return new BlockchainAddress()
            {   

                Address = addy,
                Index = indexAddy
            };

        }

        public bool IsValidResponse(Tatum.Model.Responses.Address responseAddy, BlockchainAddress localAddy)
        {
            return localAddy.Address == responseAddy.BlockchainAddress;
        }



        public async Task<Globals.Status> CreateWallet(CrypticPayUser user, string xpub, string remoteShare, CrypticPayCoinContext contextCoins, bool isTestNet = false)
        {

            
            // create blockchain data for each wallet
            var btc = Utils.FindCryptoByTicker("BTC", contextCoins);
            var bch = Utils.FindCryptoByTicker("BCH", contextCoins);
            var eth = Utils.FindCryptoByTicker("ETH", contextCoins);
            var ltc = Utils.FindCryptoByTicker("LTC", contextCoins);

            // currency wallet is null, as kryptik wallet has yet to be created
            var chainDataBtc = CreateAddress(null, btc, xpub, isTestNet);
            var chainDataBch = CreateAddress(null, bch, xpub, isTestNet);
            var chainDataEth = CreateAddress(null, eth, xpub, isTestNet);
            var chainDataLtc = CreateAddress(null, ltc, xpub, isTestNet);
                                                        
            // create customer which will be connection between seperate tatum accounts
            var customer = new Tatum.Model.Requests.CreateCustomer()
            {
                AccountingCurrency = "USD",
                ExternalId = user.Id
            };

            // uncomment below for batch account creation

            /* var accountsToCreate = new List<Tatum.Model.Requests.CreateAccount>();

             accountsToCreate.Add(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "BTC", Customer = customer });
             accountsToCreate.Add(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "BCH", Customer = customer });
             accountsToCreate.Add(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "ETH", Customer = customer });
             accountsToCreate.Add(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "LTC", Customer = customer });


             var accountsCreated = await _tatumClient.CreateAccounts(accountsToCreate);*/

            /* var btcAccount = accountsCreated[0];
            var bchAccount = accountsCreated[1];
            var ethAccount = accountsCreated[2];
            var ltcAccount = accountsCreated[3];*/

            var btcAccount = await _tatumClient.CreateAccount(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "BTC", Customer = customer });

            var bchAccount = await _tatumClient.CreateAccount(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "BCH", Customer = customer });
            var ethAccount = await _tatumClient.CreateAccount(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "ETH", Customer = customer });
            var ltcAccount = await _tatumClient.CreateAccount(new Tatum.Model.Requests.CreateAccount() { AccountingCurrency = "USD", Compliant = true, Currency = "LTC", Customer = customer });

           


            user.WalletKryptik = new Data.Wallet();
            // set client generated values
            user.WalletKryptik.Xpub = xpub;
            user.WalletKryptik.SeedShare = remoteShare;

            // comment below is format for generating local addy with tatum
            // var testAddress = Tatum.Wallet.GenerateAddress(Tatum.Model.Currency.BTC, wallBtc.XPub, rand.Next(1, 1000000), testnet: isTestNet);

            
            var respBtcAddy = await _tatumClient.AssignDepositAddress(btcAccount.Id, chainDataBtc.Address);
            // ensure deposit address is same on Tatum 
            if (!IsValidResponse(respBtcAddy, chainDataBtc)){
                throw new Exception("Error: Remote and local deposit addresses are not the same.");
            };  
            var btcAddress = chainDataBtc.Address;
            var btcQr = Utils.QrForWebGenerator(btcAddress);
            var currencyWalletBtc = new CurrencyWallet()
            {
                AccountId = btcAccount.Id,
                CoinId = btc.Id,
                AddressOnChain = chainDataBtc,
                DepositQrBlockchain = btcQr,
                WalletKryptik = user.WalletKryptik,
                CreationTime = DateTime.Now
            };

            var respBchAddy = await _tatumClient.AssignDepositAddress(bchAccount.Id, chainDataBch.Address);
            // ensure deposit address is same on Tatum 
            if (!IsValidResponse(respBchAddy, chainDataBch))
            {
                throw new Exception("Error: Remote and local deposit addresses are not the same.");
            };
            var bchAddress = chainDataBch.Address;
            var bchQr = Utils.QrForWebGenerator(bchAddress);
            var currencyWalletBch = new CurrencyWallet()
            {
                AccountId = bchAccount.Id,
                CoinId = bch.Id,
                AddressOnChain = chainDataBch,
                DepositQrBlockchain = bchQr,
                WalletKryptik = user.WalletKryptik,
                CreationTime = DateTime.Now
            };

            var respEthAddy = await _tatumClient.AssignDepositAddress(ethAccount.Id, chainDataEth.Address);
            // ensure deposit address is same on Tatum 
            if (!IsValidResponse(respEthAddy, chainDataEth))
            {
                throw new Exception("Error: Remote and local deposit addresses are not the same.");
            };
            var ethAddress = chainDataEth.Address;
            var ethQr = Utils.QrForWebGenerator(ethAddress);
            var currencyWalletEth = new CurrencyWallet()
            {
                AccountId = ethAccount.Id,
                CoinId = eth.Id,
                AddressOnChain = chainDataEth,
                DepositQrBlockchain = ethQr,
                WalletKryptik = user.WalletKryptik,
                CreationTime = DateTime.Now
            };

            var respLtcAddy = await _tatumClient.AssignDepositAddress(ltcAccount.Id, chainDataLtc.Address);
            // ensure deposit address is same on Tatum 
            if (!IsValidResponse(respLtcAddy, chainDataLtc))
            {
                throw new Exception("Error: Remote and local deposit addresses are not the same.");
            };
            var ltcAddress = chainDataLtc.Address;
            var ltcQr = Utils.QrForWebGenerator(ltcAddress);

            var currencyWalletLtc = new CurrencyWallet()
            {
                AccountId = ltcAccount.Id,
                CoinId = ltc.Id,
                AddressOnChain = chainDataLtc,
                DepositQrBlockchain = ltcQr,
                WalletKryptik = user.WalletKryptik,
                CreationTime = DateTime.Now
            };

           
            // wallet starts with kryptik managed key
            user.WalletKryptik.IsOnChain = false;
            user.WalletKryptik.CurrencyWallets = new List<CurrencyWallet>();

            // set currency wallets for container wallet
            user.WalletKryptik.CurrencyWallets.Add(currencyWalletBtc);
            user.WalletKryptik.CurrencyWallets.Add(currencyWalletBch);
            user.WalletKryptik.CurrencyWallets.Add(currencyWalletEth);
            user.WalletKryptik.CurrencyWallets.Add(currencyWalletLtc);
            user.WalletKryptik.CrypticPayUserKey = user.Id;
            

            user.WalletKryptik.CreationTime = DateTime.Now;
            user.WalletKryptik.Owner = user;
            user.WalletKryptikExists = true;

            return Globals.Status.Done;
            
        }

        // generate 12 word mnemonic
        public string GenerateMnemonic()
        {
            var phraseObj = new Mnemonic(Wordlist.English, WordCount.Twelve);
            string mnemonic = string.Join(" ", phraseObj.Words);
            return mnemonic;
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
                // balance should be denominated in crypto units
                var responseBal = await _tatumClient.GetAccountBalance(account.AccountId);
                
               
                var balance = Convert.ToDouble(responseBal.Balance);
                // retrie
                var coin = walletAndCoins.Coins[index];
                var coinExchangeRate = coinExchangeRates[coin.ApiTag];
                account.AccountBalanceFiat = balance * coinExchangeRate;
                // convert fiat rate to crypto rate
                account.AccountBalanceCrypto = balance;
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

            plaintext = plaintext.Replace("\0", "");

            return plaintext;
        }

    }
}
