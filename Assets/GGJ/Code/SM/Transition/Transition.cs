using GGJ.Code.SM.Predicate;
using GGJ.Code.SM.State;

namespace GGJ.Code.SM.Transition
{
    public class Transition : ITransition {
        public IState To { get; }
        public IPredicate Condition { get; }

        public Transition(IState to, IPredicate condition) {
            To = to;
            Condition = condition;
        }
    }}