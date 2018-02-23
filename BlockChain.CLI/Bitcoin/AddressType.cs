using BlockChain.CLI.Core.Interfaces;

namespace BlockChain.CLI.Bitcoin
{
    public enum AddressType
    {
        Unknown = -1,
        PublicKey,
        PrivateKey,
        Script,
        Bip32PublicKey,
        Bip32PrivateKey,
        WitnessPublicKey,
        WitnessScript
    }
}
