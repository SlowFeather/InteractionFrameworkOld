using InteractionFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractionFramework.Runtime.Demo
{
    public enum TestStates
    {
        StateOne,
        StateTwo,
        StateThree,
    }
    public class StateMachineDemo : MonoBehaviour
    {
        private StateMachine<TestStates> fsm;
        // Start is called before the first frame update
        void Start()
        {
            fsm = StateMachine<TestStates>.Initialize(this, TestStates.StateOne);

        }

        void StateOne_Enter()
        {
            Debug.Log("进入StateOne");
        }

        void StateOne_Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                fsm.ChangeState(TestStates.StateTwo, StateTransition.Safe);
            }
        }
        void StateOne_Exit()
        {
            Debug.Log("离开StateOne");
        }

        void StateTwo_Enter()
        {
            Debug.Log("进入StateTwo");
        }

        void StateTwo_Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                fsm.ChangeState(TestStates.StateThree, StateTransition.Safe);
            }
        }
        void StateTwo_Exit()
        {
            Debug.Log("离开StateTwo");
        }


        void StateThree_Enter()
        {
            Debug.Log("进入StateThree");
        }

        void StateThree_Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                fsm.ChangeState(TestStates.StateOne, StateTransition.Safe);
            }
        }
        void StateThree_Exit()
        {
            Debug.Log("离开StateThree");
        }


    }
}