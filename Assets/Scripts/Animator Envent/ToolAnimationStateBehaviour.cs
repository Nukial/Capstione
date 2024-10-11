using UnityEngine;

public class ToolAnimationStateBehaviour : StateMachineBehaviour
{
    // Khi trạng thái animation bắt đầu
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ToolUsageController toolUsageController = animator.GetComponentInParent<ToolUsageController>();
        if (toolUsageController != null)
        {
            toolUsageController.OnToolAnimationStart();
        }
    }

    // Khi trạng thái animation kết thúc
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ToolUsageController toolUsageController = animator.GetComponentInParent<ToolUsageController>();
        if (toolUsageController != null)
        {
            toolUsageController.OnToolAnimationEnd();
        }
    }
}
