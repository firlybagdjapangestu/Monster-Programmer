using UnityEngine;

namespace UI.Controll {
    public class UiController : MonoBehaviour
    {
        [Header("[Object Name Key]")]
        [SerializeField] private string NameKey;

        public bool IsName(string _name)
        {
            return _name == NameKey;
        }

        private Vector2 direction = Vector2.zero;
        public void OnMoveHorizontal(float direc)
        {
            direction.x = direc;

            OnPlayerMove(direction);
        }

        public void OnMoveVertical(float direc)
        {
            direction.y = direc;

            OnPlayerMove(direction);
        }

        public void OnPlayerMove(Vector2 dir)
        {
            PlayerController.Main?.OnPlayerMove(dir);
        }

        public void OnPlayerInteract()
        {
            PlayerController.Main?.OnPlayerInteract();
        }
    }

}


