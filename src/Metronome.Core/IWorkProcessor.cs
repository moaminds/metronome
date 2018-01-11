using System.Threading.Tasks;

namespace Metronome.Core
{
    public interface IWorkProcessor
    {
        Task ProcessAsync(WorkDefinition work);
    }
}