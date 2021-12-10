using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class FileUpload
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        // user specified file name
        public string Name { get; set; }
        // file description
        public string Description { get; set; }
        // IPFS file identifier
        public string CID { get; set; }
        // Kryptik user that file belongs to....
        public string  OwnerId{ get; set; }
        public DateTime CreationTime { get; set; }
    }
}
