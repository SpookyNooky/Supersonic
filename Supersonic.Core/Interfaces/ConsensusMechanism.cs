using Supersonic.Core.Entities;

namespace Supersonic.Core.Interfaces
{
    public class ConsensusMechanism : IConsensusMechanism
    {
        public void AddTransaction(Transaction transaction, List<string> nodes)
        {
            // Implement the logic to add a transaction and communicate with other nodes
        }
    }
}
