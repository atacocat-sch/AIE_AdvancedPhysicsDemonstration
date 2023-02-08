using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public static class BetterRagdollWizard
{
    [MenuItem("Tools/Ragdollify")]
    public static void Apply ()
    {
        List<CharacterJoint> joints = new List<CharacterJoint>();
        foreach (GameObject selection in Selection.gameObjects)
        {
            selection.AddComponent<Rigidbody>();
            var joint = selection.AddComponent<CharacterJoint>();
            joints.Add(joint);
        }

        List<Rigidbody> bodies = new List<Rigidbody>();
        foreach (var joint in joints)
        {
            var rigidbody = joint.GetComponent<Rigidbody>();
            joint.connectedBody = joint.transform.parent.GetOrAddComponent<Rigidbody>();

            if (!bodies.Contains(rigidbody)) bodies.Add(rigidbody);
            if (!bodies.Contains(joint.connectedBody)) bodies.Add(joint.connectedBody);
        }

        float totalMass = 80.0f;
        float segmentMass = totalMass / bodies.Count;

        foreach (var body in bodies)
        {
            body.mass = segmentMass;
        }
    }
}
