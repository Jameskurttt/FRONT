using UnityEngine;

public class HealingOrb : Pickup
{
    private IEffect effect;

    private void Awake()
    {
        pickupName = "Healing Orb";
        effect = GetComponent<IEffect>();
    }

    protected override void PickUp(GameObject player)
    {
        if (effect != null)
        {
            effect.ApplyEffect(player);
        }

        base.PickUp(player);
    }
}