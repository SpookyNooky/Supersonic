using Supersonic.Core.Entities;
using Supersonic.Core.Interfaces;

namespace Supersonic.Application.Consensus
{
    public class DAGConsensus : IConsensusMechanism
    {
        private readonly DAG _dag = new();

        public void AddTransaction(Transaction transaction, List<string> parentIds)
        {
            _dag.AddTransaction(transaction, parentIds);
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return _dag.GetAllTransactions();
        }
    }
}
