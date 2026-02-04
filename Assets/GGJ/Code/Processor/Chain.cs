namespace GGJ.Code.Processor
{
    class Chain<TIn, TOut>
    {
        readonly IProcessor<TIn, TOut> _processor;

        Chain(IProcessor<TIn, TOut> processor)
        {
            _processor = processor;
        }

        public static Chain<TIn, TOut> Start(IProcessor<TIn, TOut> processor)
        {
            return new Chain<TIn, TOut>(processor);
        }

        public Chain<TIn, TNext> Then<TNext>(IProcessor<TOut, TNext> next)
        {
            Combined<TIn, TOut, TNext> combined = new(_processor, next);
            return new Chain<TIn, TNext>(combined);
        }

        public TOut Run(TIn input) => _processor.Process(input);

        public ProcessorDelegate<TIn, TOut> Compile() =>
            input => _processor.Process(input);
    }
}