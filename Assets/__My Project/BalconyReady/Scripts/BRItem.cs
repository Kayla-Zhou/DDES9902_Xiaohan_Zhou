using UnityEngine;
using UnityEngine.Events;

namespace BalconyReady
{
    // The position data is a teaching scenario, not a wind/structural simulation.
    public class BRItem : MonoBehaviour
    {
        public string displayName;
        public bool largeItem;
        public bool roundTwoOnly;
        public Vector3 roundOnePosition;
        public Vector3 roundTwoPosition;
        public GameObject selectedMarker;
        public BRManager manager;
        [HideInInspector] public BRSlot currentSlot;

        void Awake()
        {
            var interaction = GetComponent<InteractableGeneral>();
            if (interaction != null)
            {
                if (interaction.onPrimaryInteract == null) interaction.onPrimaryInteract = new UnityEvent();
                interaction.onPrimaryInteract.AddListener(Select);
            }
        }

        public void Select() { if (manager != null) manager.Select(this); }
        public void SetSelected(bool value) { if (selectedMarker != null) selectedMarker.SetActive(value); }
    }
}
