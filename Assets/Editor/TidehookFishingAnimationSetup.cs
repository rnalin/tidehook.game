using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class TidehookFishingAnimationSetup
{
    const string ControllerPath = "Assets/_Project/Animations/char fishing sprite sheet_0.controller";
    const string IdleClipPath = "Assets/_Project/Animations/Idle.anim";
    const string HookedClipPath = "Assets/_Project/Animations/CharacterFishing.anim";
    const string HookTrigger = "hook";
    const string HookedBool = "isHooked";

    [MenuItem("Tidehook/Configure Fishing Animation")]
    public static void ConfigureFishingAnimation()
    {
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        var idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(IdleClipPath);
        var hookedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(HookedClipPath);

        if (controller == null)
        {
            Debug.LogError("Animator Controller not found at " + ControllerPath);
            return;
        }

        if (idleClip == null || hookedClip == null)
        {
            Debug.LogError("Idle or hook animation clip not found in Assets/_Project/Animations.");
            return;
        }

        SetClipLoop(idleClip, true);
        SetClipLoop(hookedClip, false);

        Undo.RecordObject(controller, "Configure Fishing Animation");
        ConfigureController(controller, idleClip, hookedClip);
        EditorUtility.SetDirty(controller);

        var character = GameObject.Find("FishingCharacter");
        if (character == null)
        {
            Debug.LogWarning("FishingCharacter not found in the open scene. Controller configured, but scene object was not updated.");
            AssetDatabase.SaveAssets();
            return;
        }

        Undo.RegisterCompleteObjectUndo(character, "Configure Fishing Character Animator");

        var animator = character.GetComponent<Animator>();
        if (animator == null)
            animator = Undo.AddComponent<Animator>(character);

        animator.runtimeAnimatorController = controller;

        var bridge = character.GetComponent<FishingCharacterAnimator>();
        if (bridge == null)
            bridge = Undo.AddComponent<FishingCharacterAnimator>(character);

        var serializedBridge = new SerializedObject(bridge);
        serializedBridge.FindProperty("animator").objectReferenceValue = animator;
        serializedBridge.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(character);
        AssetDatabase.SaveAssets();
        Debug.Log("Fishing animation configured: trigger 'hook' enters hooked pose, bool 'isHooked' holds it until catch/close.");
    }

    static void SetClipLoop(AnimationClip clip, bool loop)
    {
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        if (settings.loopTime == loop) return;

        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);
    }

    static void ConfigureController(AnimatorController controller, AnimationClip idleClip, AnimationClip hookedClip)
    {
        EnsureParameter(controller, HookTrigger, AnimatorControllerParameterType.Trigger);
        EnsureParameter(controller, HookedBool, AnimatorControllerParameterType.Bool);

        if (controller.layers.Length == 0)
        {
            controller.AddLayer("Base Layer");
        }

        var layer = controller.layers[0];
        var stateMachine = layer.stateMachine;

        var idleState = EnsureState(stateMachine, "Idle", idleClip, new Vector3(250f, 100f, 0f));
        var hookedState = EnsureState(stateMachine, "Hooked", hookedClip, new Vector3(520f, 100f, 0f));
        stateMachine.defaultState = idleState;

        RemoveTransitions(idleState, HookedBool, HookTrigger);
        RemoveTransitions(hookedState, HookedBool, HookTrigger);

        var idleToHooked = idleState.AddTransition(hookedState);
        idleToHooked.hasExitTime = false;
        idleToHooked.duration = 0.02f;
        idleToHooked.AddCondition(AnimatorConditionMode.If, 0f, HookTrigger);

        var hookedToIdle = hookedState.AddTransition(idleState);
        hookedToIdle.hasExitTime = false;
        hookedToIdle.duration = 0.05f;
        hookedToIdle.AddCondition(AnimatorConditionMode.IfNot, 0f, HookedBool);
    }

    static AnimatorState EnsureState(AnimatorStateMachine stateMachine, string stateName, Motion motion, Vector3 position)
    {
        var state = stateMachine.states.FirstOrDefault(s => s.state.name == stateName).state;
        if (state == null)
            state = stateMachine.AddState(stateName, position);

        state.motion = motion;
        return state;
    }

    static void EnsureParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
    {
        if (controller.parameters.Any(p => p.name == name))
            return;

        controller.AddParameter(name, type);
    }

    static void RemoveTransitions(AnimatorState state, string boolParameter, string triggerParameter)
    {
        foreach (var transition in state.transitions.ToArray())
        {
            bool usesFishingParameter = transition.conditions.Any(c =>
                c.parameter == boolParameter || c.parameter == triggerParameter);

            if (usesFishingParameter)
                state.RemoveTransition(transition);
        }
    }
}
