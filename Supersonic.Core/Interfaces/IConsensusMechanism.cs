using Supersonic.Core.Entities;

namespace Supersonic.Core.Interfaces
{
    public interface IConsensusMechanism
    {
        // Define methods for the consensus mechanism
        void AddTransaction(Transaction transaction, List<string> nodes);
    }
}
