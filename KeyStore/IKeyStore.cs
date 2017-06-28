using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyStore
{
    public interface IKeyStore : IService
    {
        Task AddOrUpdateAsync(string key, string value);
        Task RemoveAsync(string key);
        Task<string> GetAsync(string key);
    }
}
