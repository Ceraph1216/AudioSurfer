using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VisEmitParticlesTrigger))]
public class VisEmitParticlesTriggerEditor : VisBaseTriggerEditor 
{
    public VisEmitParticlesTriggerEditor()
    {
        showAutomaticInspectorGUI = false;
    }

    protected override void CustomInspectorGUI()
    {
#if !(UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_3 || UNITY_3_4)
        VisEmitParticlesTrigger trigger = target as VisEmitParticlesTrigger;
        if (trigger != null)
        {
            ParticleSystem particleSystemComp = trigger.GetComponent<ParticleSystem>();
            ParticleEmitter particleEmitterComp = trigger.GetComponent<ParticleEmitter>();
            
            base.CustomInspectorGUI();

            if (particleSystemComp != null)
            {
                trigger.emitCount = EditorGUILayout.IntField("  Emit Count", trigger.emitCount);
            }

            if (particleEmitterComp != null)
            {
                EditorGUILayout.HelpBox("Using Legacy Particle System", MessageType.Info);
            }

            if (particleSystemComp != null)
            {
                EditorGUILayout.HelpBox("Using Shruiken Particle System", MessageType.Info);
            }

            if (particleSystemComp == null && particleEmitterComp == null)
            {
                EditorGUILayout.HelpBox("No supported Particle Component found! Please add a ParticleSystem (Shruiken) or ParticleEmitter (Legacy) Component to this Game Object.", MessageType.Error);
            }
        }     
#else   
        base.CustomInspectorGUI();
#endif
    }
}