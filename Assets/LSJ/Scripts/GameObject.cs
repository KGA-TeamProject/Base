using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameObject : IInteractable
{
    public abstract void Interact(Player player);
}
