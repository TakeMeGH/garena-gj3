using System;
using System.Collections.Generic;
using GGJ.Code.SM.Predicate;
using GGJ.Code.SM.State;
using GGJ.Code.SM.Transition;

namespace GGJ.Code.SM
{
    public class StateMachine {
        StateNode _current;
        readonly Dictionary<Type, StateNode> _nodes = new();
        readonly HashSet<ITransition> _anyTransitions = new();

        public void Update() {
            ITransition transition = GetTransition();
            if (transition != null) 
                ChangeState(transition.To);
            
            _current.State?.Update();
        }
        
        public void FixedUpdate() {
            _current.State?.FixedUpdate();
        }

        public void SetState(IState state) {
            if (!_nodes.ContainsKey(state.GetType()))
            {
                GetOrAddNode(state);
            }
            _current = _nodes[state.GetType()];
            _current.State?.OnEnter();
        }

        void ChangeState(IState state) {
            if (state == _current.State) return;
            
            IState previousState = _current.State;
            IState nextState = _nodes[state.GetType()].State;
            
            previousState?.OnExit();
            nextState?.OnEnter();
            _current = _nodes[state.GetType()];
        }

        ITransition GetTransition() {
            foreach (ITransition transition in _anyTransitions)
                if (transition.Condition.Evaluate())
                    return transition;
            
            foreach (ITransition transition in _current.Transitions)
                if (transition.Condition.Evaluate())
                    return transition;
            
            return null;
        }

        public void AddTransition(IState from, IState to, IPredicate condition) {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }
        
        public void AddAnyTransition(IState to, IPredicate condition) {
            _anyTransitions.Add(new Transition.Transition(GetOrAddNode(to).State, condition));
        }

        StateNode GetOrAddNode(IState state) {
            StateNode node = _nodes.GetValueOrDefault(state.GetType());

            if (node != null) return node;
            
            node = new StateNode(state);
            _nodes.Add(state.GetType(), node);

            return node;
        }

        class StateNode {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }
            
            public StateNode(IState state) {
                State = state;
                Transitions = new HashSet<ITransition>();
            }
            
            public void AddTransition(IState to, IPredicate condition) {
                Transitions.Add(new Transition.Transition(to, condition));
            }
        }
    }}