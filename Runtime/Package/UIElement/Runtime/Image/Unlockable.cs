using UnityEngine;

namespace NIX.Packages
{
    public enum LockState
    {
        Lock,
        Unlock
    }

    public interface IUnlockable
    {
        LockState State { get; }
        void SetState(LockState state);
    }

    public class Unlockable : MonoBehaviour, IUnlockable
    {
        public LockState State { get; protected set; }

        public virtual void SetState(LockState state)
        {
            this.State = state;
        }
    }
}