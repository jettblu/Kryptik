using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public async Task<Data.Responses.ResponseUpload> UploadFile(string strFileName, IFormFile file)
        {
            // get bytes for arg. file
            byte[] fileData = await Utils.GetBytes(file);
            var client = new PinataClient(_config);
            var response = await client.Pinning.PinFileToIpfsAsync(content =>
            {
                var file = new ByteArrayContent(fileData);

                content.AddPinataFile(file, strFileName);
            });

            if (response.IsSuccess)
            {
                //File uploaded to Pinata Cloud. Can be accessed on IPFS!
                var cid = response.IpfsHash;
                return new Data.Responses.ResponseUpload()
                {
                    Status = Globals.Status.Success,
                    CID = cid
                };
            }

            // if we made it this far something went wrong....
            return new Data.Responses.ResponseUpload()
            {
                Status = Globals.Status.Failure,
                CID = null
            };
        }
    }
}
