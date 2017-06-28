using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace KeyStore
{
    internal sealed class KeyStore : StatefulService, IKeyStore
    {
        private Task<IReliableDictionary<string, string>> _keystore => this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>("__keystore__");

        public KeyStore(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddOrUpdateAsync(string key, string value)
        {
            var ks = await _keystore;

            using (var tx = this.StateManager.CreateTransaction())
            {
                await ks.AddOrUpdateAsync(tx, key, value, ((k, v) => value));
                await tx.CommitAsync();
            }
        }

        public async Task<string> GetAsync(string key)
        {
            var ks = await _keystore;
            string value = string.Empty;

            using (var tx = this.StateManager.CreateTransaction())
            {
                var res = await ks.TryGetValueAsync(tx, key);
                value = res.Value;
                await tx.CommitAsync();
            }

            return value;
        }

        public async Task RemoveAsync(string key)
        {
            var ks = await _keystore;

            using (var tx = this.StateManager.CreateTransaction())
            {
                await ks.TryRemoveAsync(tx, key);
                await tx.CommitAsync();
            }
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
               new ServiceReplicaListener(context =>
                   this.CreateServiceRemotingListener(context))
            };
        }
    }
}
