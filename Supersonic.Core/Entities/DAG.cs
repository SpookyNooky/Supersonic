namespace Supersonic.Core.Entities
{
    public class DAG
    {
        private readonly Dictionary<string, Transaction> _transactions;

        public DAG()
        {
            _transactions = new Dictionary<string, Transaction>();
        }

        public bool AddTransaction(Transaction transaction, List<string> parentIds)
        {
            if (_transactions.ContainsKey(transaction.Id) || !transaction.Validate())
            {
                Console.WriteLine("Transaction invalid or already exists.");
                return false;
            }

            // Validate and add parents
            foreach (var parentId in parentIds)
            {
                if (!_transactions.ContainsKey(parentId))
                {
                    Console.WriteLine($"Parent transaction {parentId} not found.");
                    return false;
                }

                var parentTransaction = _transactions[parentId];
                transaction.Parents.Add(parentTransaction);

                if (DetectCycle(transaction))
                {
                    Console.WriteLine("Adding this transaction would create a cycle.");
                    return false;
                }
            }

            _transactions[transaction.Id] = transaction;
            return true;
        }

        private bool DetectCycle(Transaction startTransaction)
        {
            var visited = new HashSet<string>();
            var stack = new Stack<Transaction>();
            stack.Push(startTransaction);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (!visited.Add(current.Id))
                {
                    return true; // Cycle detected
                }

                foreach (var parent in current.Parents)
                {
                    stack.Push(parent);
                }
            }

            return false;
        }

        public Transaction GetTransaction(string id)
        {
            _transactions.TryGetValue(id, out var transaction);
            return transaction;
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return _transactions.Values;
        }
    }
}
