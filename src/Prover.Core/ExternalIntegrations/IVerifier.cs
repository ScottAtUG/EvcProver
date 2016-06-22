using System.Threading.Tasks;

namespace Prover.Core.ExternalIntegrations
{
    public interface IVerifier
    {
        Task<object> Verify();
    }
}