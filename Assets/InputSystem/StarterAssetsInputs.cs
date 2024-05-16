using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool meleeAttack;
        public bool aim;
        public bool shoot;
        public bool reload;
        public bool dragFists;
        public bool dragMeleeWeapon;
        public bool dragPistol;
        public bool dragRifle;
        public bool grabItem;
        public bool throwItem;
        public bool pauseGame;
        [SerializeField] GameObject pauseMenu;
        [SerializeField] GameObject gameOverMenu;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
        public void OnMeleeAttack(InputValue value)
        {
            MeleeAttackInput(value.isPressed);
        }
        public void OnAim(InputValue value)
        {
            AimInput(value.isPressed);
        }
        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }
        public void OnReload(InputValue value)
        {
            ReloadInput(value.isPressed);
        }
        public void OnDragFists(InputValue value)
        {
            DragFistsInput(value.isPressed);
        }
        public void OnDragMeleeWeapon(InputValue value)
        {
            DragMeleeWeaponInput(value.isPressed);
        }
        public void OnDragPistol(InputValue value)
        {
            DragPistolInput(value.isPressed);
        }
        public void OnDragRifle(InputValue value)
        {
            DragRifleInput(value.isPressed);
        }
        public void OnGrabItem(InputValue value)
        {
            GrabItemInput(value.isPressed);
        }
        public void OnThrowItem(InputValue value)
        {
            ThrowItemInput(value.isPressed);
        }
        public void OnPauseItem(InputValue value)
        {
            PauseItemInput(value.isPressed);
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }
        public void MeleeAttackInput(bool newMeleeAttackState)
        {
            meleeAttack = newMeleeAttackState;
        }
        public void AimInput(bool newAimState)
        {
            aim = newAimState;
        }
        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }
        public void ReloadInput(bool newReloadState)
        {
            reload = newReloadState;
        }
        public void DragFistsInput(bool newDragFistsState)
        {
            dragFists = newDragFistsState;
        }
        public void DragMeleeWeaponInput(bool newDragMeleeWeaponState)
        {
            dragMeleeWeapon = newDragMeleeWeaponState;
        }
        public void DragPistolInput(bool newDragPistolState)
        {
            dragPistol = newDragPistolState;
        }
        public void DragRifleInput(bool newDragRifleState)
        {
            dragRifle = newDragRifleState;
        }
        public void GrabItemInput(bool newGrabItemState)
        {
            grabItem = newGrabItemState;
        }
        public void ThrowItemInput(bool newThrowItemState)
        {
            throwItem = newThrowItemState;
        }
        public void PauseItemInput(bool newPauseItemState)
        {
            pauseGame = newPauseItemState;
        }

        public bool fistsInputOnly()
        {
			return dragFists && !dragMeleeWeapon && !dragPistol && !dragRifle;
        }
        public bool meleeInputOnly()
        {
			return !dragFists && dragMeleeWeapon && !dragPistol && !dragRifle;
        }
        public bool pistolInputOnly()
        {
			return !dragFists && !dragMeleeWeapon && dragPistol && !dragRifle;
        }
        public bool rifleInputOnly()
        {
			return !dragFists && !dragMeleeWeapon && !dragPistol && dragRifle;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if(pauseMenu.activeSelf || gameOverMenu.activeSelf)
            {
            SetCursorState(!cursorLocked);
            }
            else
            {
            SetCursorState(cursorLocked);
            }
        }

        public void DisableActiveInputs()
        {
            jump = sprint = meleeAttack = aim = shoot = reload = dragFists = dragMeleeWeapon = dragPistol = dragRifle = grabItem = throwItem = pauseGame = false;
        }

        public void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

}