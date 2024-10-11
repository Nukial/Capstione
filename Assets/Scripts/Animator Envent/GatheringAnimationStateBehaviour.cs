using UnityEngine;

public class GatheringAnimationStateBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ToolUsageController toolUsageController = animator.GetComponentInParent<ToolUsageController>();
        if (toolUsageController != null)
        {
            toolUsageController.OnGatheringAnimationStart();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ToolUsageController toolUsageController = animator.GetComponentInParent<ToolUsageController>();
        if (toolUsageController != null)
        {
            toolUsageController.OnGatheringAnimationEnd();
        }
    }
}
