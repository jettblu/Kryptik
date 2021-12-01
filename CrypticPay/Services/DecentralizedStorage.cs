using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Pinata.Client;

namespace CrypticPay.Services
{
    public class DecentralizedStorage
    {
        private Config _config { get; set; }

        public DecentralizedStorage(string apiKey, string apiSecret)
        {
            _config = new Config
            {
                ApiKey = apiKey,
                ApiSecret = apiSecret
            };
        }
        // uploads given bytes to IPFS via Pinata
        public async void UploadFile(string strFileName, byte[] fileData)
        {
            var client = new PinataClient(_config);
            var response = await client.Pinning.PinFileToIpfsAsync(content =>
            {
                var file = new ByteArrayContent(fileData);

                content.AddPinataFile(file, strFileName);
            });

            if (response.IsSuccess)
            {
                //File uploaded to Pinata Cloud. Can be accessed on IPFS!
                var hash = response.IpfsHash; 
            }
            
        }
    }
}
