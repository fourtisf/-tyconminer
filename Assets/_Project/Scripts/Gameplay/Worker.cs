using UnityEngine;
using GoldMineTycoon.Data;

namespace GoldMineTycoon.Gameplay
{
    /// <summary>
    /// Cosmetic worker that walks between two anchor points and plays Mixamo-driven
    /// animation states (see brief 6.1). Purely visual — production math lives in
    /// ProductionSystem, so workers can be culled on low-end devices without affecting
    /// earnings.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Worker : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int MiningHash = Animator.StringToHash("IsMining");

        [SerializeField] private WorkerData data;
        [SerializeField] private Transform stationPoint;
        [SerializeField] private Transform dropPoint;

        private Animator _animator;
        private Transform _target;
        private bool _carrying;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _target = stationPoint;
        }

        private void Update()
        {
            if (_target == null) return;

            var toTarget = _target.position - transform.position;
            var distance = toTarget.magnitude;

            if (distance > 0.05f)
            {
                var step = data.moveSpeed * Time.deltaTime;
                transform.position += toTarget.normalized * step;
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, Quaternion.LookRotation(toTarget), 10f * Time.deltaTime);
                _animator.SetFloat(SpeedHash, data.moveSpeed);
                _animator.SetBool(MiningHash, false);
            }
            else
            {
                _animator.SetFloat(SpeedHash, 0f);
                OnArrived();
            }
        }

        private void OnArrived()
        {
            if (_target == stationPoint)
            {
                _animator.SetBool(MiningHash, true);
                if (!_carrying) { _carrying = true; _target = dropPoint; }
            }
            else
            {
                _carrying = false;
                _target = stationPoint;
            }
        }
    }
}
