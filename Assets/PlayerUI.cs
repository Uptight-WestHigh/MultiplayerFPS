using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform thrusterFuelFill;

    [SerializeField]
    RectTransform healthFill;

    private PlayerController controller;
    private Player player;

    public void SetController(PlayerController _controller)
    {
        controller = _controller;
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
    }

    void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());
        SetHealthAmount(player.GetHealthAmount());
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }

    void SetHealthAmount(float _amount)
    {
        healthFill.localScale = new Vector3(1f, _amount / 100, 1f);
    }

}
