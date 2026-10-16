using System.Text;
using UnityEngine;
using TMPro;

namespace BalconyReady
{
    public class BRManager : MonoBehaviour
    {
        public BRItem[] items;
        public BRSlot[] slots;
        public TMP_Text taskText;
        public TMP_Text feedbackText;
        public TMP_Text selectionText;
        public Transform player;
        public Vector3 spawnPoint = new Vector3(0, 0.05f, -4.3f);
        public float indoorReviewBoundary = -0.6f;
        public int round = 1;
        [HideInInspector] public BRItem selected;
        [HideInInspector] public bool roundComplete;
        [HideInInspector] public int moves;
        [HideInInspector] public int reviews;

        void Start() { BeginRound(1); }

        public void BeginRound(int number)
        {
            round = Mathf.Clamp(number, 1, 2);
            roundComplete = false;
            moves = 0; reviews = 0;
            CancelSelection();
            foreach (var slot in slots) slot.occupant = null;
            foreach (var item in items)
            {
                item.currentSlot = null;
                item.SetSelected(false);
                item.gameObject.SetActive(round == 2 || !item.roundTwoOnly);
                item.transform.position = round == 1 ? item.roundOnePosition : item.roundTwoPosition;
                item.transform.rotation = Quaternion.identity;
            }
            ReturnToStart();
            taskText.text = round == 1
                ? "<b>01 / PREPARE</b>\nFind the pot, chair and basket.\nMove them to suitable indoor bays.\nThen walk inside and CHECK.\n\nSelect an item, then select a bay.\nClick the item again to cancel."
                : "<b>02 / CHECK AGAIN</b>\nThe layout has changed.\nPrepare all FOUR movable items.\nAlso inspect the indoor passage.\n\nDo not assume that indoors\nalways means well placed.";
            feedbackText.text = "<b>BEFORE THE STORM</b>\nThis is a calm-weather preparation exercise.\n\nExplore first. The CHECK button explains\nyour current arrangement. You can revise it.\n\nNo countdown. No speed bonus.";
        }

        void ReturnToStart()
        {
            if (player == null) return;
            var controller = player.GetComponent<CharacterController>();
            bool enabledBefore = controller != null && controller.enabled;
            if (controller != null) controller.enabled = false;
            player.SetPositionAndRotation(spawnPoint, Quaternion.identity);
            if (controller != null) controller.enabled = enabledBefore;
        }

        public void Select(BRItem item)
        {
            if (!item.gameObject.activeInHierarchy) return;
            if (selected == item) { CancelSelection(); return; }
            if (selected != null) selected.SetSelected(false);
            selected = item;
            selected.SetSelected(true);
            selectionText.text = "<b>SELECTED: " + item.displayName.ToUpperInvariant() + "</b>\nWalk to a bay and click its post.\nThe item stays here until you place it.";
        }

        public void CancelSelection()
        {
            if (selected != null) selected.SetSelected(false);
            selected = null;
            if (selectionText != null) selectionText.text = "<b>NOTHING SELECTED</b>\nLook at an item and left-click.\nWASD: move / Mouse: look / Esc: cursor";
        }

        public void Place(BRSlot slot)
        {
            if (selected == null)
            {
                selectionText.text = "<b>SELECT AN ITEM FIRST</b>\nClick a pot, chair, basket or toy,\nthen click a bay's placement post.";
                return;
            }
            if (slot.occupant != null && slot.occupant != selected)
            {
                selectionText.text = "<b>BAY OCCUPIED</b>\nMove its item to another bay first.\nStill selected: " + selected.displayName;
                return;
            }
            var item = selected;
            if (item.currentSlot != null) item.currentSlot.occupant = null;
            item.currentSlot = slot;
            slot.occupant = item;
            item.transform.SetPositionAndRotation(slot.placementAnchor.position, slot.placementAnchor.rotation);
            roundComplete = false;
            moves++;
            CancelSelection();
            feedbackText.text = "<b>ARRANGEMENT CHANGED</b>\n" + item.displayName + " placed at " + slot.displayName + ".\n\nWalk the space again.\nUse CHECK indoors for an explanation.\nPrevious checks no longer confirm this layout.";
        }

        public void Review()
        {
            if (player != null && player.position.z >= indoorReviewBoundary)
            {
                feedbackText.text = "<b>REVIEW FROM INDOORS</b>\nReturn to the review station inside.\nYou can still look around freely.";
                return;
            }
            reviews++;
            bool complete = true;
            var report = new StringBuilder("<b>LAYOUT REVIEW / ROUND " + round + "</b>\n");
            foreach (var item in items)
            {
                if (!item.gameObject.activeInHierarchy) continue;
                bool suitable = item.currentSlot != null && item.currentSlot.IsSuitable(item);
                complete &= suitable;
                report.Append(suitable ? "<color=#76D9BC>OK</color> " : "<color=#FFD291>RECHECK</color> ");
                report.Append(item.displayName).Append(": ");
                report.Append(item.currentSlot == null
                    ? (item.roundTwoOnly ? "In the indoor passage; relocate it." : "Not yet in a designated indoor bay.")
                    : item.currentSlot.Explain(item));
                report.Append("\n\n");
            }
            roundComplete = complete;
            report.Append(complete
                ? (round == 1 ? "<b>ROUND COMPLETE</b>\nTry ROUND 2 with a changed layout."
                             : "<b>BOTH ROUNDS COMPLETE</b>\nExplain why your chosen locations work.\nThis model does not certify a real home as safe.")
                : "<b>REVISE AND CHECK AGAIN</b>\nMoving an item can create another problem.");
            feedbackText.text = report.ToString();
            Debug.Log("[BalconyReady] Round " + round + " review=" + reviews + " moves=" + moves + " complete=" + complete);
        }

        public void ResetRound() { BeginRound(round); }

        public void NextRound()
        {
            if (!roundComplete)
            {
                feedbackText.text = "<b>CHECK THIS ROUND FIRST</b>\nComplete the arrangement and use CHECK.\nYou can reset this round at any time.";
                return;
            }
            if (round == 1) BeginRound(2);
            else feedbackText.text = "<b>TRAINING COMPLETE</b>\nWhat changed between the two layouts?\nWhy is moving everything indoors not enough?\n\nUse RESET to practise this round again.\nConsult official guidance for your real home.";
        }
    }
}
