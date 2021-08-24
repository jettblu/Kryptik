using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay
{
    public class StorageAccountOptions
    {
        public string StorageAccountNameOption { get; set; }
        public string StorageAccountKeyOption { get; set; }
        public string StorageAccountConnectionString { get; set; }
        public string FullImagesContainerNameOption { get; set; }
        public string ScaledImagesContainerNameOption { get; set; }
    }
}
