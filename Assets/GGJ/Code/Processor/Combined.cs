namespace GGJ.Code.Processor
{
    class Combined<A, B, C> : IProcessor<A, C>
    {
        readonly IProcessor<A, B> _first;
        readonly IProcessor<B, C> _second;

        public Combined(IProcessor<A, B> first, IProcessor<B, C> second)
        {
            _first = first;
            _second = second;
        }

        public C Process(A input) => _second.Process(_first.Process(input));
    }
}