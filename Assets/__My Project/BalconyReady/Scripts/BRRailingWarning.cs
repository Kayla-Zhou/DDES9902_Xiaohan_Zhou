using UnityEngine;

namespace BalconyReady
{
    // Distance-based proximity works with the existing CharacterController,
    // without adding a Rigidbody or changing the balcony's solid colliders.
    public class BRRailingWarning : MonoBehaviour
    {
        public Transform player;
        public Transform playerView;
        public Collider[] railings;
        public GameObject warningSign;
        [Min(0.1f)] public float warningDistance = 0.85f;
        [Min(0.1f)] public float clearDistance = 1.05f;
        public bool IsWarningActive { get; private set; }

        void Awake()
        {
            SetWarning(false);
        }

        void LateUpdate()
        {
            if (player == null || railings == null || warningSign == null) return;
            Vector3 sample = player.position + Vector3.up;
            Vector3 nearest = sample;
            float nearestDistance = float.PositiveInfinity;
            foreach (var railing in railings)
            {
                if (railing == null || !railing.enabled || !railing.gameObject.activeInHierarchy) continue;
                Vector3 point = railing.ClosestPoint(sample);
                Vector3 delta = sample - point;
                delta.y = 0;
                float distance = delta.magnitude;
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = point;
                }
            }

            // A small hysteresis avoids flicker when standing on the threshold.
            float threshold = IsWarningActive ? Mathf.Max(clearDistance, warningDistance) : warningDistance;
            SetWarning(nearestDistance <= threshold);
            if (!IsWarningActive) return;

            Vector3 inward = sample - nearest;
            inward.y = 0;
            warningSign.transform.position = new Vector3(nearest.x, 1.65f, nearest.z) + inward.normalized * -0.14f;
            if (playerView != null)
            {
                Vector3 direction = warningSign.transform.position - playerView.position;
                direction.y = 0;
                if (direction.sqrMagnitude > 0.001f)
                    warningSign.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        void SetWarning(bool active)
        {
            IsWarningActive = active;
            if (warningSign != null && warningSign.activeSelf != active)
                warningSign.SetActive(active);
        }

        void OnDisable()
        {
            SetWarning(false);
        }
    }
}
