using UnityEngine;
using UnityEngine.Events;

namespace BalconyReady
{
    public class BRAction : MonoBehaviour
    {
        public enum ActionType { Review, ResetRound, NextRound, CancelSelection }
        public ActionType action;
        public BRManager manager;

        void Awake()
        {
            var interaction = GetComponent<InteractableGeneral>();
            if (interaction != null)
            {
                if (interaction.onPrimaryInteract == null) interaction.onPrimaryInteract = new UnityEvent();
                interaction.onPrimaryInteract.AddListener(Activate);
            }
        }

        public void Activate()
        {
            if (manager == null) return;
            switch (action)
            {
                case ActionType.Review: manager.Review(); break;
                case ActionType.ResetRound: manager.ResetRound(); break;
                case ActionType.NextRound: manager.NextRound(); break;
                case ActionType.CancelSelection: manager.CancelSelection(); break;
            }
        }
    }
}
