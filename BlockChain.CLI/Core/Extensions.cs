using System.Collections.Generic;
using System.Linq;

namespace BlockChain.Core
{
    public static class Extensions
    {
        public static bool Validate(this IEnumerable<Block> chain)
        {
            if (chain.First() != Chain.GenesisBlock)
            {
                return false;
            }

            using (var enumerator = chain.GetEnumerator())
            {
				var lastBlock = chain.First();

                while (enumerator.MoveNext())
                {
                    if (object.ReferenceEquals(enumerator.Current, lastBlock))
                    {
                        continue;
                    }
                        
                    if (!Block.IsValidNewBlock(lastBlock, enumerator.Current))
                    {
                        return false;
                    }

                    lastBlock = enumerator.Current;
                }
            }

            return true;
        }
    }
}
