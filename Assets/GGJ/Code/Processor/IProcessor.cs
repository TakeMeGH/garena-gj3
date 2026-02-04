namespace GGJ.Code.Processor
{
    public interface IProcessor<in TIn, out TOut>
    {
        TOut Process(TIn input);
    }

    public delegate TOut ProcessorDelegate<in TIn, out TOut>(TIn input);
}