using UnityEngine;
using UnityEngine.Events;

namespace BalconyReady
{
    public class BRSlot : MonoBehaviour
    {
        public string displayName;
        public bool indoorStorage;
        public bool blocksRoute;
        public bool acceptsLargeItems = true;
        public Transform placementAnchor;
        public BRManager manager;
        [HideInInspector] public BRItem occupant;

        void Awake()
        {
            var interaction = GetComponent<InteractableGeneral>();
            if (interaction != null)
            {
                if (interaction.onPrimaryInteract == null) interaction.onPrimaryInteract = new UnityEvent();
                interaction.onPrimaryInteract.AddListener(Place);
            }
        }

        public bool IsSuitable(BRItem item)
        {
            return indoorStorage && !blocksRoute && (!item.largeItem || acceptsLargeItems);
        }

        public string Explain(BRItem item)
        {
            if (!indoorStorage) return "Still outside: relocating within the balcony has not completed this task.";
            if (blocksRoute) return "Indoors, but in the marked passage. Move it to a storage bay.";
            if (item.largeItem && !acceptsLargeItems) return "This marked shelf is for small items. Use a floor bay for the chair.";
            return "In a designated indoor bay, with the passage clear.";
        }

        public void Place() { if (manager != null) manager.Place(this); }
    }
}
