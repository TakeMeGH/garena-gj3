using GGJ.Code.SM.Predicate;
using GGJ.Code.SM.State;

namespace GGJ.Code.SM.Transition
{
    public interface ITransition {
        IState To { get; }
        IPredicate Condition { get; }
    }}