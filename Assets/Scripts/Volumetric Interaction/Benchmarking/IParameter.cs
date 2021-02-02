namespace VolumetricInteraction.Benchmarking
{
    public interface IParameter
    {
        void Increment();
        void Reset();
        bool Finished();
    }
}