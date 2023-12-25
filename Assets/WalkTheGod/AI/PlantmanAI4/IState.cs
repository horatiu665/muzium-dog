using UnityEngine;
using System.Collections;

namespace PlantmanAI4
{
	/// <summary>
	/// interface for all AI states. all states must implement functions from here, and they may have their own functionalities and such.
	/// </summary>
	public interface IState
	{

		/// <summary>
		/// name of state, purely for display purposes.
		/// </summary>
		/// <returns>name of state, does not have to be unique, but it is recommended</returns>
		string GetName();

		/// <summary>
		/// when true and state is active, cannot exit state no matter what conditions.
		/// </summary>
		bool GetUninterruptible();

		/// <summary>
		/// when multiple states are active at the same time, state with highest priority will run first.
		/// </summary>
		float GetPriority();

		/// <summary>
		/// happens when entering state
		/// </summary>
		void OnEnter();

		/// <summary>
		/// happens every deltaTime (decided by AIController)
		/// </summary>
		void OnExecute(float deltaTime);

		/// <summary>
		/// happens when exiting/changing state.
		/// </summary>
		void OnExit();


		/// <summary>
		/// when true, state can be activated and switched to.
		/// </summary>
		/// <returns></returns>
		bool ConditionsMet();

	}


	/*
	Use this to init a state cleanly:
	
    public string GetName()
    {
        return "Idle";
    }

    public bool GetUninterruptible()
    {
        return false;
    }

    public float GetPriority()
    {
        return 0;
    }

    public void OnEnter()
    {
    }

    public void OnExecute(float deltaTime)
    {
    }

    public void OnExit()
    {
    }

    public bool ConditionsMet()
    {
        return true;
    }

	 */
}