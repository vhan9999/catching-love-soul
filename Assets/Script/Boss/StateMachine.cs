using System.Collections.Generic;
using System;
using UnityEngine;
namespace GameCore.Basic
{
    public class StateMachine:IStateMachineContext
    {
        Dictionary<string,IState> _stateMap;
        IState _currentState;
        IState _defaultState;
        IState _nextState;
        public StateMachine(){
            _stateMap = new Dictionary<string, IState>();
        }
        public void SetState(string state){
            _currentState = _stateMap[state];
        }
        public void SetNextState(string state){
            _nextState = _stateMap[state];
        }
        public void UnSetNextState(){
            _nextState = null;
        }
        public void SetDefaultState(string state){
            _defaultState = _stateMap[state];
        }
        public void Start(){
            if(_currentState == null)
                _currentState = _defaultState;
            _currentState.OnEnter();
        }
        public void ChangeState(string state,Action onComplete = null){
            IState newState = _stateMap[state];
            if(_currentState.Equals(_stateMap[state]))
                return;
            IState lastState = _currentState;
            _currentState.OnLeave();
            newState.CallOnLeave(onComplete);
            _currentState = newState;
            newState.OnEnter();
            lastState.OnComplete();
            Debug.Log($"ChangeTo:{state}");
        }
        public void AddState(string stateName,IState state){
            _stateMap.Add(stateName,state);
        }
        
        public void OnUpdate(){
            _currentState.OnUpdate();
        }
        public void OnFixUpdate(){
            _currentState.OnFixUpdate();
        }
        public void MoveNextState(){
            IState lastState = _currentState;
            if(_nextState!=null){
                _currentState = _nextState;
                _nextState = null;
            }
            else
                _currentState = _defaultState;
            lastState.OnLeave();
            _currentState.OnEnter();
            Debug.Log($"ChangeTo:{GetCurrentState()}");
            lastState.OnComplete();
        }
        public string GetCurrentState(){
            return _currentState.GetType().ToString();
        }
    }
    public interface IState{
        void OnEnter();
        void OnLeave();
        void OnUpdate();
        void OnFixUpdate();
        void CallOnLeave(Action func);
        void OnComplete();
    }
    public interface IStateMachineContext{
        void MoveNextState();
    }
    
}