using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [SerializeField] private float holdThreshold = 0.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem _lowGlowPS;
    [SerializeField] private ParticleSystem _lowGlowPSHold;
    [SerializeField] private ParticleSystem _swordPS;
    [SerializeField] private ParticleSystem _shieldPS;

    public Action OnClick;
    public Action OnHold;

    private bool isHolding = false;
    private bool holdTriggered = false;
    private float holdTimer = 0f;

    private static readonly int IdleTrigger = Animator.StringToHash("Idle");
    private static readonly int ClickTrigger = Animator.StringToHash("Click");
    private static readonly int IsHoldingBool = Animator.StringToHash("IsHolding");

    public void OnPointerDown(PointerEventData eventData) {
        isHolding = true;
        holdTriggered = false;
        holdTimer = 0f;

        if (animator != null) {
            animator.ResetTrigger(IdleTrigger);
            animator.SetBool(IsHoldingBool, false); // сбрасываем на всякий
            var shieldMain = _shieldPS.main;
            shieldMain.loop = false;
            _shieldPS.Stop();

            var swordMain = _swordPS.main;
            swordMain.loop = false;
            _swordPS.Stop();
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (!holdTriggered) {
            OnClick?.Invoke();
            if (animator != null)
                animator.SetTrigger(ClickTrigger);
            _lowGlowPS.Play();
            _shieldPS.Play();
            _swordPS.Play();
        } else {
            if (animator != null)
                animator.SetTrigger(IdleTrigger);
            _lowGlowPSHold.Stop();
            var shieldMain = _shieldPS.main;
            shieldMain.loop = false;
            _shieldPS.Stop();

            var swordMain = _swordPS.main;
            swordMain.loop = false;
            _swordPS.Stop();
        }

        isHolding = false;

        if (animator != null)
            animator.SetBool(IsHoldingBool, false);
    }

    private void Update() {
        if (isHolding) {
            holdTimer += Time.deltaTime;

            if (!holdTriggered && holdTimer >= holdThreshold) {
                holdTriggered = true;
                OnHold?.Invoke();

                if (animator != null)
                    animator.SetBool(IsHoldingBool, true);
                _lowGlowPSHold.Play();

                var shieldMain = _shieldPS.main;
                shieldMain.loop = true;
                _shieldPS.Play();

                var swordMain = _swordPS.main;
                swordMain.loop = true;
                _swordPS.Play();
            }
        }
    }

    private void OnEnable() {
        if (animator != null) {
            animator.SetTrigger(IdleTrigger);
            animator.SetBool(IsHoldingBool, false);
            _lowGlowPSHold.Stop();
            var shieldMain = _shieldPS.main;
            shieldMain.loop = false;
            _shieldPS.Stop();

            var swordMain = _swordPS.main;
            swordMain.loop = false;
            _swordPS.Stop();
        }
    }
}
