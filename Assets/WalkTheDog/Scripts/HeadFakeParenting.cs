namespace DogAI
{
    using UnityEngine;
    using System.Collections;
    using ToyBoxHHH;

    // copied from fake parenting in toyboxhhh
    public class HeadFakeParenting : MonoBehaviour
    {
        [Header("Move pos/rot to this target object")]
        public Transform fakeParent;

        public bool pos = true, rot = true;

        public bool useOffset = true;
        public Vector3 offsetPos;
        public Quaternion offsetRot;

        public bool update = true, fixedUpdate, lateUpdate;

        [DebugButton]
        public void SetOffset()
        {
            offsetPos = transform.position - fakeParent.position;
            offsetRot = Quaternion.Inverse(fakeParent.rotation) * transform.rotation;
        }

        private void DoIt()
        {
            if (fakeParent == null)
            {
                return;
            }
            if (pos)
            {
                transform.position = fakeParent.position;
                if (useOffset)
                {
                    transform.position += fakeParent.rotation * offsetPos;
                }
            }
            if (rot)
            {
                transform.rotation = fakeParent.rotation;
                if (useOffset)
                {
                    transform.rotation *= offsetRot;
                }
            }
        }

        void Update()
        {
            if (update)
            {
                DoIt();
            }
        }

        void FixedUpdate()
        {
            if (fixedUpdate)
            {
                DoIt();
            }
        }

        void LateUpdate()
        {
            if (lateUpdate)
            {
                DoIt();
            }
        }
    }
}